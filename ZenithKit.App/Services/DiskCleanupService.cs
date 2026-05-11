using System.IO;
using System.Runtime.InteropServices;
using ZenithKit.App.Models;

namespace ZenithKit.App.Services;

public interface IDiskCleanupService
{
    List<CleanupCategory> GetCategories();
    Task ScanAsync(IEnumerable<CleanupCategory> categories, CancellationToken cancellationToken = default);
    Task<CleanupResult> CleanAsync(IEnumerable<CleanupCategory> categories, IProgress<CleanupProgress>? progress = null, CancellationToken cancellationToken = default);
}

public sealed class DiskCleanupService : IDiskCleanupService
{
    public List<CleanupCategory> GetCategories()
    {
        return
        [
            new CleanupCategory
            {
                Id = "wintemp",
                Name = "Windows 临时文件",
                Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")
            },
            new CleanupCategory
            {
                Id = "usertemp",
                Name = "用户临时文件",
                Path = Environment.GetEnvironmentVariable("TEMP") ?? Path.GetTempPath()
            },
            new CleanupCategory
            {
                Id = "thumbcache",
                Name = "缩略图缓存",
                Path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    @"Microsoft\Windows\Explorer")
            },
            new CleanupCategory
            {
                Id = "wucache",
                Name = "Windows 更新缓存",
                Path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    @"SoftwareDistribution\Download")
            },
            new CleanupCategory
            {
                Id = "recycle",
                Name = "回收站",
                Path = @"C:\$Recycle.Bin"
            },
            new CleanupCategory
            {
                Id = "dnscache",
                Name = "DNS 缓存",
                Path = string.Empty
            }
        ];
    }

    public Task ScanAsync(IEnumerable<CleanupCategory> categories, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            foreach (var cat in categories)
            {
                cancellationToken.ThrowIfCancellationRequested();
                cat.Size = cat.Id == "dnscache" ? 0 : CalculateDirectorySize(cat.Path, cancellationToken);
            }
        }, cancellationToken);
    }

    public Task<CleanupResult> CleanAsync(
        IEnumerable<CleanupCategory> categories,
        IProgress<CleanupProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            int totalFiles = 0;
            long totalFreed = 0;

            foreach (var cat in categories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (cat.Id == "dnscache")
                {
                    FlushDns();
                    progress?.Report(new CleanupProgress(cat.Name, 0));
                    continue;
                }

                if (cat.Id == "recycle")
                {
                    long freed = EmptyRecycleBin();
                    totalFreed += freed;
                    cat.Size = 0;
                    progress?.Report(new CleanupProgress(cat.Name, 0));
                    continue;
                }

                int deleted = CleanDirectory(cat.Path, out long dirFreed, cancellationToken);
                totalFiles += deleted;
                totalFreed += dirFreed;
                cat.Size = 0;
                progress?.Report(new CleanupProgress(cat.Name, deleted));
            }

            return new CleanupResult(totalFiles, totalFreed);
        }, cancellationToken);
    }

    private static long CalculateDirectorySize(string path, CancellationToken ct)
    {
        if (!Directory.Exists(path)) return 0;

        long size = 0;
        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            }))
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    size += new FileInfo(file).Length;
                }
                catch
                {
                    // skip locked files
                }
            }
        }
        catch
        {
            // skip inaccessible directories
        }
        return size;
    }

    private static int CleanDirectory(string path, out long freed, CancellationToken ct)
    {
        freed = 0;
        if (!Directory.Exists(path)) return 0;

        int count = 0;
        try
        {
            foreach (var file in Directory.EnumerateFiles(path, "*", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            }))
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var fi = new FileInfo(file);
                    long len = fi.Length;
                    fi.Delete();
                    freed += len;
                    count++;
                }
                catch
                {
                    // skip locked/in-use files
                }
            }

            // Remove empty subdirectories
            foreach (var dir in Directory.EnumerateDirectories(path, "*", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true
            }))
            {
                try
                {
                    if (!Directory.EnumerateFileSystemEntries(dir).Any())
                        Directory.Delete(dir, false);
                }
                catch
                {
                    // skip
                }
            }
        }
        catch
        {
            // skip inaccessible directories
        }
        return count;
    }

    private static void FlushDns()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo("ipconfig", "/flushdns")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process.Start(psi)?.WaitForExit(5000);
        }
        catch
        {
            // best effort
        }
    }

    private static long EmptyRecycleBin()
    {
        try
        {
            // Estimate size before emptying
            long size = CalculateDirectorySize(@"C:\$Recycle.Bin", CancellationToken.None);
            SHEmptyRecycleBin(IntPtr.Zero, null,
                SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
            return size;
        }
        catch
        {
            return 0;
        }
    }

    private const uint SHERB_NOCONFIRMATION = 0x00000001;
    private const uint SHERB_NOPROGRESSUI = 0x00000002;
    private const uint SHERB_NOSOUND = 0x00000004;

    [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
    private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);
}
