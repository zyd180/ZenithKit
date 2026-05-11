using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

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
        try
        {
            Results.Clear();
            Info = "搜索中...";
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var items = await _searchService.SearchAsync(Query, MaxResults, RootPath, Filter);
            foreach (var item in items)
            {
                Results.Add(item);
            }
            sw.Stop();
            Info = $"结果: {Results.Count} 个，耗时 {sw.ElapsedMilliseconds} ms";
        }
        catch (Exception ex)
        {
            Info = $"搜索失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private void BrowseRoot()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.FolderBrowserDialog();
        dlg.Description = "选择搜索根目录";
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            RootPath = dlg.SelectedPath;
        }
#pragma warning restore CA1416
    }
}
