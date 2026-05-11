using System.IO;

namespace ZenithKit.App.Services;

public sealed class RenameService : IRenameService
{
    public Task<IReadOnlyList<string>> PreviewAsync(string folderPath, string pattern, int start = 1, int step = 1, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteInternal(folderPath, pattern, start, step, apply: false, cancellationToken), cancellationToken);
    }

    public Task<IReadOnlyList<string>> RenameAsync(string folderPath, string pattern, int start = 1, int step = 1, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ExecuteInternal(folderPath, pattern, start, step, apply: true, cancellationToken), cancellationToken);
    }

    private IReadOnlyList<string> ExecuteInternal(string folderPath, string pattern, int start, int step, bool apply, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(pattern))
            return Array.Empty<string>();

        var dir = new DirectoryInfo(folderPath);
        if (!dir.Exists) return Array.Empty<string>();

        var files = dir.GetFiles().OrderBy(f => f.Name).ToArray();
        var renamed = new List<string>();
        int index = start;
        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var newName = string.Format(pattern, index);
            var newPath = Path.Combine(dir.FullName, newName + file.Extension);
            if (File.Exists(newPath))
            {
                renamed.Add($"冲突跳过: {newPath}");
                continue;
            }
            if (string.Equals(file.FullName, newPath, StringComparison.OrdinalIgnoreCase))
            {
                renamed.Add($"保持: {newPath}");
            }
            else
            {
                renamed.Add(apply
                    ? $"重命名: {file.FullName} -> {newPath}"
                    : $"预览: {file.FullName} -> {newPath}");
                if (apply)
                {
                    file.MoveTo(newPath);
                }
            }
            index += step;
        }
        return renamed;
    }
}
