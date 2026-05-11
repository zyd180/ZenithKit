using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

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

    [ObservableProperty]
    private string _status = string.Empty;

    public PdfToolsViewModel(IPdfToolsService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task Merge()
    {
        try
        {
            Status = "合并中...";
            var result = await _service.MergeAsync(MergeList);
            MergeResult = result;
            Status = $"合并完成: {result}";
        }
        catch (Exception ex)
        {
            Status = $"合并失败: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task Split()
    {
        try
        {
            Status = "拆分中...";
            var output = string.IsNullOrWhiteSpace(SplitOutput)
                ? Path.Combine(Path.GetDirectoryName(SplitSource) ?? ".", "pdf_split")
                : SplitOutput;
            await _service.SplitAsync(SplitSource, output);
            Status = $"拆分完成: {output}";
        }
        catch (Exception ex)
        {
            Status = $"拆分失败: {ex.Message}";
        }
    }
}
