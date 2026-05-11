using System.Security.Cryptography;
using System.Text;

namespace MiCodeAutoToolBox.Core.Services;

public sealed class StorageService : IStorageService
{
    private readonly object _gate = new();
    private StorageOptions _options;
    private readonly string _appName;

    public StorageService(string appName, StorageOptions initialOptions)
    {
        _appName = appName;
        _options = initialOptions;
        Directory.CreateDirectory(_options.CurrentPath);
    }

    public string CurrentPath => _options.CurrentPath;

    public StorageOptions GetOptions() => _options;

    public async Task SwitchAsync(StorageLocation location, string? customPath = null, CancellationToken cancellationToken = default)
    {
        string targetPath = ResolvePath(location, customPath);
        string tempPath = targetPath + "_tmp";
        string backupPath = _options.CurrentPath + "_backup";

        lock (_gate)
        {
            Directory.CreateDirectory(targetPath);
        }

        // Copy current -> temp
        await CopyDirectoryAsync(_options.CurrentPath, tempPath, cancellationToken).ConfigureAwait(false);

        // Basic integrity check: ensure file counts match
        if (!await HasSameStructure(_options.CurrentPath, tempPath, cancellationToken).ConfigureAwait(false))
        {
            Directory.Delete(tempPath, true);
            throw new InvalidOperationException("Storage migration integrity check failed");
        }

        // Move current -> backup, temp -> target
        lock (_gate)
        {
            if (Directory.Exists(backupPath)) Directory.Delete(backupPath, true);
            Directory.Move(_options.CurrentPath, backupPath);
            Directory.Move(tempPath, targetPath);
            _options = new StorageOptions { Location = location, CurrentPath = targetPath };
        }

        // Cleanup backup on success (could keep for rollback; here remove to save space)
        Directory.Delete(backupPath, true);
    }

    private string ResolvePath(StorageLocation location, string? customPath)
    {
        return location switch
        {
            StorageLocation.AppData => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName),
            StorageLocation.Portable => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "data")),
            StorageLocation.Custom => string.IsNullOrWhiteSpace(customPath)
                ? throw new ArgumentException("Custom path required", nameof(customPath))
                : Path.GetFullPath(customPath),
            _ => throw new ArgumentOutOfRangeException(nameof(location))
        };
    }

    private static async Task CopyDirectoryAsync(string sourceDir, string destDir, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(destDir);
        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var relative = Path.GetRelativePath(sourceDir, file);
            var dest = Path.Combine(destDir, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            using var sourceStream = File.OpenRead(file);
            await using var destStream = File.Create(dest);
            await sourceStream.CopyToAsync(destStream, cancellationToken).ConfigureAwait(false);
        }
    }

    private static Task<bool> HasSameStructure(string a, string b, CancellationToken cancellationToken)
    {
        var filesA = Directory.GetFiles(a, "*", SearchOption.AllDirectories).Select(f => Path.GetRelativePath(a, f)).OrderBy(x => x).ToArray();
        var filesB = Directory.GetFiles(b, "*", SearchOption.AllDirectories).Select(f => Path.GetRelativePath(b, f)).OrderBy(x => x).ToArray();
        cancellationToken.ThrowIfCancellationRequested();
        if (filesA.Length != filesB.Length) return Task.FromResult(false);
        for (int i = 0; i < filesA.Length; i++)
        {
            if (!filesA[i].Equals(filesB[i], StringComparison.OrdinalIgnoreCase)) return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }
}
