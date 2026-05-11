using System.Diagnostics;

namespace MiCodeAutoToolBox.App.Services;

public interface ILauncherService
{
    Task LaunchAsync(string target, CancellationToken cancellationToken = default);
}

public sealed class LauncherService : ILauncherService
{
    public Task LaunchAsync(string target, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(target))
            return Task.CompletedTask;

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var psi = new ProcessStartInfo
            {
                FileName = target,
                UseShellExecute = true
            };
            Process.Start(psi);
        }, cancellationToken);
    }
}
