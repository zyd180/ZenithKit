namespace MiCodeAutoToolBox.App.Services;

public interface IRenameService
{
    /// <summary>
    /// Preview rename files in folder (non-recursive) using pattern with {0} placeholder for sequence.
    /// </summary>
    Task<IReadOnlyList<string>> PreviewAsync(string folderPath, string pattern, int start = 1, int step = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rename files in folder (non-recursive) using pattern with {0} placeholder for sequence.
    /// </summary>
    Task<IReadOnlyList<string>> RenameAsync(string folderPath, string pattern, int start = 1, int step = 1, CancellationToken cancellationToken = default);
}
