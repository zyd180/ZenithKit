using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Models;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class DiffViewModel : ObservableObject
{
    private readonly IDiffService _diffService;
    private readonly ICollectionView _filteredPairs;

    [ObservableProperty]
    private string _leftPath = string.Empty;

    [ObservableProperty]
    private string _rightPath = string.Empty;

    [ObservableProperty]
    private string _diffResult = string.Empty;

    [ObservableProperty]
    private bool _onlyDiffs;

    public ObservableCollection<DiffPair> Pairs { get; } = new();

    public ICollectionView FilteredPairs => _filteredPairs;

    public DiffViewModel(IDiffService diffService)
    {
        _diffService = diffService;
        _filteredPairs = CollectionViewSource.GetDefaultView(Pairs);
        _filteredPairs.Filter = FilterPair;
    }

    private bool FilterPair(object? obj)
    {
        if (obj is not DiffPair pair) return false;
        if (!OnlyDiffs) return true;
        return pair.IsDifferent;
    }

    partial void OnOnlyDiffsChanged(bool value)
    {
        _filteredPairs.Refresh();
    }

    [RelayCommand]
    private void BrowseLeft()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.OpenFileDialog();
        dlg.Title = "选择左侧文件";
        dlg.Filter = "文本文件|*.txt;*.md;*.json;*.xml;*.cs;*.cpp;*.h;*.js;*.ts;*.config;*.*|所有文件|*.*";
        dlg.CheckFileExists = true;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            LeftPath = dlg.FileName;
        }
#pragma warning restore CA1416
    }

    [RelayCommand]
    private void BrowseRight()
    {
#pragma warning disable CA1416
        using var dlg = new System.Windows.Forms.OpenFileDialog();
        dlg.Title = "选择右侧文件";
        dlg.Filter = "文本文件|*.txt;*.md;*.json;*.xml;*.cs;*.cpp;*.h;*.js;*.ts;*.config;*.*|所有文件|*.*";
        dlg.CheckFileExists = true;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            RightPath = dlg.FileName;
        }
#pragma warning restore CA1416
    }

    [RelayCommand]
    private async Task Diff()
    {
        var list = await _diffService.DiffAsync(LeftPath, RightPath);
        Pairs.Clear();
        foreach (var item in list)
        {
            Pairs.Add(item);
        }
        _filteredPairs.Refresh();
        DiffResult = $"对比完成：{list.Count} 行";
    }

    [RelayCommand]
    private void Clear()
    {
        DiffResult = string.Empty;
        Pairs.Clear();
        _filteredPairs.Refresh();
    }
}
