using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZenithKit.App.Services;

namespace ZenithKit.App.ViewModels;

public partial class LauncherViewModel : ObservableObject
{
    private readonly ILauncherService _launcherService;

    [ObservableProperty]
    private string _target = string.Empty;

    [ObservableProperty]
    private string _status = string.Empty;

    public LauncherViewModel(ILauncherService launcherService)
    {
        _launcherService = launcherService;
    }

    [RelayCommand]
    private async Task Launch()
    {
        try
        {
            Status = "启动中...";
            await _launcherService.LaunchAsync(Target);
            Status = $"已启动: {Target}";
        }
        catch (Exception ex)
        {
            Status = $"启动失败: {ex.Message}";
        }
    }
}
