using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class LauncherViewModel : ObservableObject
{
    private readonly ILauncherService _launcherService;

    [ObservableProperty]
    private string _target = string.Empty;

    public LauncherViewModel(ILauncherService launcherService)
    {
        _launcherService = launcherService;
    }

    [RelayCommand]
    private async Task Launch()
    {
        await _launcherService.LaunchAsync(Target);
    }
}
