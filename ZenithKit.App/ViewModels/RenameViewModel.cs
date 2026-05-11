using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class RenameViewModel : ObservableObject
{
    private readonly IRenameService _renameService;

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private string _pattern = "newname_{0}";

    [ObservableProperty]
    private int _start = 1;

    [ObservableProperty]
    private int _step = 1;

    [ObservableProperty]
    private string _status = string.Empty;

    public ObservableCollection<string> Renamed { get; } = new();

    public RenameViewModel(IRenameService renameService)
    {
        _renameService = renameService;
    }

    [RelayCommand]
    private async Task Preview()
    {
        try
        {
            Renamed.Clear();
            Status = "预览中...";
            var results = await _renameService.PreviewAsync(FolderPath, Pattern, Start, Step);
            foreach (var path in results)
            {
                Renamed.Add(path);
            }
            Status = $"预览完成: {results.Count} 个文件";
        }
        catch (Exception ex)
        {
            Status = $"预览失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Rename()
    {
        try
        {
            Renamed.Clear();
            Status = "重命名中...";
            var results = await _renameService.RenameAsync(FolderPath, Pattern, Start, Step);
            foreach (var path in results)
            {
                Renamed.Add(path);
            }
            Status = $"重命名完成: {results.Count} 个文件";
        }
        catch (Exception ex)
        {
            Status = $"重命名失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private void BrowseFolder()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.FolderBrowserDialog();
        dlg.Description = "选择要重命名的文件夹";
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            FolderPath = dlg.SelectedPath;
        }
#pragma warning restore CA1416
    }
}
