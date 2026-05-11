namespace MiCodeAutoToolBox.Core.Services;

public interface IStorageService
{
    /// <summary>
    /// Current resolved storage path.
    /// </summary>
    string CurrentPath { get; }

    /// <summary>
    /// Get current storage options.
    /// </summary>
    StorageOptions GetOptions();

    /// <summary>
    /// Switch storage location and migrate data atomically (copy -> verify -> swap -> backup).
    /// </summary>
    /// <param name="location">Target location</param>
    /// <param name="customPath">Required when location is Custom</param>
    Task SwitchAsync(StorageLocation location, string? customPath = null, CancellationToken cancellationToken = default);
}
