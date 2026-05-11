using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Models;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class ScreenshotViewModel : ObservableObject
{
    private readonly IScreenshotService _screenshotService;
    private readonly IWindowEnumerator _windowEnumerator;
    private readonly IRegionSelector _regionSelector;

    [ObservableProperty]
    private string? _lastSavedPath;

    [ObservableProperty]
    private string _saveDirectory;

    [ObservableProperty]
    private string? _status;

    [ObservableProperty]
    private ObservableCollection<WindowEntry> _windows = new();

    [ObservableProperty]
    private WindowEntry? _selectedWindow;

    public ScreenshotViewModel(IScreenshotService screenshotService, IWindowEnumerator windowEnumerator, IRegionSelector regionSelector)
    {
        _screenshotService = screenshotService;
        _windowEnumerator = windowEnumerator;
        _regionSelector = regionSelector;
        SaveDirectory = _screenshotService.SaveDirectory;
        RefreshWindows();
    }

    [RelayCommand]
    private async Task Capture()
    {
        try
        {
            Status = "全屏截图中...";
            string path = await _screenshotService.CaptureAsync();
            LastSavedPath = path;
            Status = $"已保存: {path}";
        }
        catch (Exception ex)
        {
            Status = $"全屏截图失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CaptureRegion()
    {
        try
        {
            Status = "请选择区域...";
            var picked = await _regionSelector.PickAsync();
            if (picked is null || picked.Value.Width <= 0 || picked.Value.Height <= 0)
            {
                Status = "已取消或选择区域无效";
                return;
            }

            Status = "区域截图中...";
            var rect = new System.Drawing.Rectangle(picked.Value.X, picked.Value.Y, picked.Value.Width, picked.Value.Height);
            string path = await _screenshotService.CaptureRegionAsync(rect);
            LastSavedPath = path;
            Status = $"已保存: {path}";
        }
        catch (Exception ex)
        {
            Status = $"区域截图失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CaptureWindow()
    {
        if (SelectedWindow is null)
        {
            Status = "请选择一个窗口再截图";
            return;
        }

        try
        {
            Status = "窗口截图中...";
            string path = await _screenshotService.CaptureWindowAsync(SelectedWindow.Handle);
            LastSavedPath = path;
            Status = $"已保存: {path}";
        }
        catch (Exception ex)
        {
            Status = $"窗口截图失败: {ex.Message}";
        }
    }

    partial void OnSaveDirectoryChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            _screenshotService.SaveDirectory = value;
        }
    }

    [RelayCommand]
    private void RefreshWindows()
    {
        var list = _windowEnumerator.Enumerate();
        Windows = new ObservableCollection<WindowEntry>(list);
        SelectedWindow = Windows.FirstOrDefault();
        Status = Windows.Count == 0 ? "未找到可见窗口" : $"已加载 {Windows.Count} 个窗口";
    }

    [RelayCommand]
    private void BrowseSaveDirectory()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.FolderBrowserDialog();
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            SaveDirectory = dlg.SelectedPath;
            Status = $"保存路径: {SaveDirectory}";
        }
#pragma warning restore CA1416
    }
}
