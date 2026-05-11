using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class ChecksumViewModel : ObservableObject
{
    private readonly IChecksumService _checksumService;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _algorithm = "SHA256";

    [ObservableProperty]
    private string _result = string.Empty;

    public ChecksumViewModel(IChecksumService checksumService)
    {
        _checksumService = checksumService;
    }

    [RelayCommand]
    private async Task Compute()
    {
        Result = await _checksumService.ComputeAsync(FilePath, Algorithm);
    }
}
