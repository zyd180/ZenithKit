using System.IO;
using System.IO.Compression;

namespace MiCodeAutoToolBox.App.Services;

public interface IArchiveService
{
    Task<string> ZipAsync(string sourcePath, string? zipPath = null, CancellationToken cancellationToken = default);
    Task<string> UnzipAsync(string zipPath, string? destinationFolder = null, CancellationToken cancellationToken = default);
}

public sealed class ArchiveService : IArchiveService
{
    public Task<string> ZipAsync(string sourcePath, string? zipPath = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new ArgumentException("sourcePath required", nameof(sourcePath));
            if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath)) throw new FileNotFoundException("Source not found", sourcePath);

            if (zipPath is null)
            {
                var baseDir = Path.GetDirectoryName(sourcePath) ?? Directory.GetCurrentDirectory();
                var name = Path.GetFileName(sourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                zipPath = Path.Combine(baseDir, name + ".zip");
            }

            if (Directory.Exists(sourcePath))
            {
                if (File.Exists(zipPath)) File.Delete(zipPath);
                ZipFile.CreateFromDirectory(sourcePath, zipPath, CompressionLevel.Optimal, includeBaseDirectory: true);
            }
            else
            {
                using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
                archive.CreateEntryFromFile(sourcePath, Path.GetFileName(sourcePath), CompressionLevel.Optimal);
            }

            return zipPath;
        }, cancellationToken);
    }

    public Task<string> UnzipAsync(string zipPath, string? destinationFolder = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(zipPath)) throw new ArgumentException("zipPath required", nameof(zipPath));
            if (!File.Exists(zipPath)) throw new FileNotFoundException("Zip not found", zipPath);

            if (destinationFolder is null)
            {
                var baseDir = Path.GetDirectoryName(zipPath) ?? Directory.GetCurrentDirectory();
                var name = Path.GetFileNameWithoutExtension(zipPath);
                destinationFolder = Path.Combine(baseDir, name + "_unzipped");
            }

            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            ZipFile.ExtractToDirectory(zipPath, destinationFolder, overwriteFiles: true);

            return destinationFolder;
        }, cancellationToken);
    }
}
