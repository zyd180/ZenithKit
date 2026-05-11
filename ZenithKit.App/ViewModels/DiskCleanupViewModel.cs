using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Models;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class DiskCleanupViewModel : ObservableObject
{
    private readonly IDiskCleanupService _cleanupService;

    public ObservableCollection<CleanupCategory> Categories { get; } = [];

    [ObservableProperty]
    private string _statusText = "点击「扫描」检测可清理的文件";

    [ObservableProperty]
    private string _totalSizeText = string.Empty;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _isCleaning;

    [ObservableProperty]
    private bool _hasResults;

    [ObservableProperty]
    private double _progressValue;

    public DiskCleanupViewModel(IDiskCleanupService cleanupService)
    {
        _cleanupService = cleanupService;
        foreach (var cat in cleanupService.GetCategories())
            Categories.Add(cat);
    }

    [RelayCommand]
    private async Task Scan()
    {
        IsScanning = true;
        HasResults = false;
        StatusText = "正在扫描...";

        try
        {
            await _cleanupService.ScanAsync(Categories);
            OnPropertyChanged(nameof(Categories));

            long total = Categories.Sum(c => c.Size);
            TotalSizeText = $"可释放空间: {CleanupCategory.FormatBytes(total)}";
            StatusText = $"扫描完成，共发现 {Categories.Count(c => c.Size > 0)} 个可清理类别";
            HasResults = total > 0;
        }
        catch (Exception ex)
        {
            StatusText = $"扫描失败: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }

    [RelayCommand]
    private async Task Clean()
    {
        var selected = Categories.Where(c => c.IsSelected && (c.Size > 0 || c.Id == "dnscache")).ToList();
        if (selected.Count == 0)
        {
            StatusText = "请勾选要清理的类别";
            return;
        }

        IsCleaning = true;
        StatusText = "正在清理...";
        ProgressValue = 0;

        try
        {
            var progress = new Progress<CleanupProgress>(p =>
            {
                StatusText = $"已清理: {p.CategoryName} ({p.FilesDeleted} 个文件)";
                ProgressValue += 100.0 / selected.Count;
            });

            var result = await _cleanupService.CleanAsync(selected, progress);

            TotalSizeText = $"已释放: {CleanupCategory.FormatBytes(result.SpaceFreed)}";
            StatusText = $"清理完成，删除了 {result.FilesDeleted} 个文件，释放 {CleanupCategory.FormatBytes(result.SpaceFreed)}";
            ProgressValue = 100;

            // refresh sizes
            await _cleanupService.ScanAsync(Categories);
            OnPropertyChanged(nameof(Categories));
        }
        catch (Exception ex)
        {
            StatusText = $"清理失败: {ex.Message}";
        }
        finally
        {
            IsCleaning = false;
        }
    }
}
