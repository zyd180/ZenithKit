using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class ArchiveViewModel : ObservableObject
{
    private readonly IArchiveService _archiveService;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _resultPath = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    public ArchiveViewModel(IArchiveService archiveService)
    {
        _archiveService = archiveService;
    }

    [RelayCommand]
    private void BrowseSource()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.OpenFileDialog();
        dlg.Title = "选择文件或压缩包";
        dlg.Filter = "所有文件|*.*";
        dlg.CheckFileExists = true;
        dlg.Multiselect = false;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            SourcePath = dlg.FileName;
        }
#pragma warning restore CA1416
    }

    [RelayCommand]
    private async Task Zip()
    {
        try
        {
            Status = "压缩中...";
            ResultPath = await _archiveService.ZipAsync(SourcePath);
            Status = $"压缩完成: {ResultPath}";
        }
        catch (Exception ex)
        {
            Status = $"压缩失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Unzip()
    {
        try
        {
            Status = "解压中...";
            ResultPath = await _archiveService.UnzipAsync(SourcePath);
            Status = $"解压完成: {ResultPath}";
        }
        catch (Exception ex)
        {
            Status = $"解压失败: {ex.Message}";
        }
    }
}
