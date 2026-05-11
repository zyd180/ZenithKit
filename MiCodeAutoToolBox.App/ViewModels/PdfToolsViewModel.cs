using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class PdfToolsViewModel : ObservableObject
{
    private readonly IPdfToolsService _service;

    public ObservableCollection<string> MergeList { get; } = new();

    [ObservableProperty]
    private string _splitSource = string.Empty;

    [ObservableProperty]
    private string _splitOutput = string.Empty;

    [ObservableProperty]
    private string _mergeResult = string.Empty;

    public PdfToolsViewModel(IPdfToolsService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task Merge()
    {
        var result = await _service.MergeAsync(MergeList);
        MergeResult = result;
    }

    [RelayCommand]
    private async Task Split()
    {
        await _service.SplitAsync(SplitSource, string.IsNullOrWhiteSpace(SplitOutput) ? Path.Combine(Path.GetDirectoryName(SplitSource) ?? ".", "pdf_split") : SplitOutput);
    }
}
