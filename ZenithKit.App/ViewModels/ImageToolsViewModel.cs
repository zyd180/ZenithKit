using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class ImageToolsViewModel : ObservableObject
{
    private readonly IImageToolsService _service;

    [ObservableProperty]
    private string _sourcePath = string.Empty;

    [ObservableProperty]
    private string _format = "png";

    [ObservableProperty]
    private int? _maxWidth = null;

    [ObservableProperty]
    private int? _maxHeight = null;

    [ObservableProperty]
    private long? _quality = null;

    [ObservableProperty]
    private string _resultPath = string.Empty;

    [ObservableProperty]
    private string _saveDirectory;

    public ImageToolsViewModel(IImageToolsService service)
    {
        _service = service;
        _saveDirectory = _service.SaveDirectory;
    }

    [RelayCommand]
    private void Browse()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.OpenFileDialog();
        dlg.Title = "选择图片文件";
        dlg.Filter = "图片|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.webp|所有文件|*.*";
        dlg.CheckFileExists = true;
        dlg.Multiselect = false;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            SourcePath = dlg.FileName;
        }
#pragma warning restore CA1416
    }

    [RelayCommand]
    private void BrowseSaveDirectory()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.FolderBrowserDialog();
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            SaveDirectory = dlg.SelectedPath;
            _service.SaveDirectory = dlg.SelectedPath;
        }
#pragma warning restore CA1416
    }

    [RelayCommand]
    private async Task Convert()
    {
        _service.SaveDirectory = SaveDirectory;
        ResultPath = await _service.ConvertAsync(SourcePath, Format, MaxWidth, MaxHeight, Quality);
    }
}
