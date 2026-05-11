using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class FileSearchViewModel : ObservableObject
{
    private readonly IFileSearchService _searchService;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private string _rootPath = string.Empty;

    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private int _maxResults = 200;

    [ObservableProperty]
    private string _info = string.Empty;

    public ObservableCollection<string> Results { get; } = new();

    public FileSearchViewModel(IFileSearchService searchService)
    {
        _searchService = searchService;
    }

    [RelayCommand]
    private async Task Search()
    {
        Results.Clear();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var items = await _searchService.SearchAsync(Query, MaxResults, RootPath, Filter);
        foreach (var item in items)
        {
            Results.Add(item);
        }
        sw.Stop();
        Info = $"结果: {Results.Count} 个，耗时 {sw.ElapsedMilliseconds} ms";
    }
}
