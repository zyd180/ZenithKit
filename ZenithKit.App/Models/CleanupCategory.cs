namespace ZenithKit.App.Models;

public sealed class CleanupCategory
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Path { get; init; }
    public long Size { get; set; }
    public bool IsSelected { get; set; } = true;

    public string SizeText => FormatBytes(Size);

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double size = bytes;
        int unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return $"{size:F2} {units[unit]}";
    }
}

public sealed record CleanupResult(int FilesDeleted, long SpaceFreed);

public sealed record CleanupProgress(string CategoryName, int FilesDeleted);
