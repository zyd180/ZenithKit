using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class ChecksumViewModel : ObservableObject
{
    private readonly IChecksumService _checksumService;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _algorithm = "SHA256";

    [ObservableProperty]
    private string _result = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    public ChecksumViewModel(IChecksumService checksumService)
    {
        _checksumService = checksumService;
    }

    [RelayCommand]
    private async Task Compute()
    {
        try
        {
            Status = "计算中...";
            Result = await _checksumService.ComputeAsync(FilePath, Algorithm);
            Status = "计算完成";
        }
        catch (Exception ex)
        {
            Status = $"计算失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private void BrowseFile()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.OpenFileDialog();
        dlg.Title = "选择文件";
        dlg.Filter = "所有文件|*.*";
        dlg.CheckFileExists = true;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            FilePath = dlg.FileName;
        }
#pragma warning restore CA1416
    }
}
