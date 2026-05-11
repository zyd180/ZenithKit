namespace ZenithKit.Core.Services;

public enum StorageLocation
{
    AppData,
    Portable,
    Custom
}

public sealed record StorageOptions
{
    public required StorageLocation Location { get; init; }
    public required string CurrentPath { get; init; }
}
