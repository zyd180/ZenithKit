namespace MiCodeAutoToolBox.App.Services;

using System.Drawing;

public interface IScreenshotService
{
    string SaveDirectory { get; set; }

    Task<string> CaptureAsync(CancellationToken cancellationToken = default);
    Task<string> CaptureRegionAsync(Rectangle region, CancellationToken cancellationToken = default);
    Task<string> CaptureWindowAsync(IntPtr hWnd, CancellationToken cancellationToken = default);
}
