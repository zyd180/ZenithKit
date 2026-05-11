using System.IO;
using ZenithKit.Core.Services;

namespace ZenithKit.App.Services;

public interface IFileSearchService
{
    Task<IReadOnlyList<string>> SearchAsync(string query, int maxResults = 100, string? rootPath = null, string? filter = null, CancellationToken cancellationToken = default);
}

public sealed class FileSearchService : IFileSearchService
{
    private readonly IStorageService _storageService;

    public FileSearchService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public Task<IReadOnlyList<string>> SearchAsync(string query, int maxResults = 100, string? rootPath = null, string? filter = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(query)) return Array.Empty<string>();
            var root = string.IsNullOrWhiteSpace(rootPath)
                ? _storageService.CurrentPath
                : rootPath;
            if (!Directory.Exists(root)) return Array.Empty<string>();

            var results = new List<string>();
            var pattern = string.IsNullOrWhiteSpace(filter) ? "*" + query + "*" : filter.Replace("*", "") + "*" + query + "*";
            try
            {
                foreach (var file in Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    results.Add(file);
                    if (results.Count >= maxResults) break;
                }
            }
            catch
            {
                // swallow access denied etc.
            }
            return (IReadOnlyList<string>)results;
        }, cancellationToken);
    }
}
