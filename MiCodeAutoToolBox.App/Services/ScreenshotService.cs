using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MiCodeAutoToolBox.Core.Services;
using WpfApp = System.Windows.Application;
using WpfClipboard = System.Windows.Clipboard;
using WpfInterop = System.Windows.Interop.Imaging;
using WpfInt32Rect = System.Windows.Int32Rect;

namespace MiCodeAutoToolBox.App.Services;

#pragma warning disable CA1416 // Windows-only APIs (Screen, Graphics, Clipboard)
public sealed class ScreenshotService : IScreenshotService
{
    private readonly IStorageService _storage;

    public string SaveDirectory { get; set; }

    public ScreenshotService(IStorageService storage)
    {
        _storage = storage;
        SaveDirectory = Path.Combine(_storage.CurrentPath, "screenshots");
    }

    public Task<string> CaptureAsync(CancellationToken cancellationToken = default)
    {
        return CaptureInternalAsync(GetVirtualScreenBounds(), cancellationToken);
    }

    public Task<string> CaptureRegionAsync(Rectangle region, CancellationToken cancellationToken = default)
    {
        return CaptureInternalAsync(region, cancellationToken);
    }

    public Task<string> CaptureWindowAsync(IntPtr hWnd, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (hWnd == IntPtr.Zero) throw new ArgumentException("Invalid window handle", nameof(hWnd));
            NativeMethods.GetWindowRect(hWnd, out var rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            return CaptureInternalAsync(bounds, cancellationToken).Result;
        }, cancellationToken);
    }

    private Task<string> CaptureInternalAsync(Rectangle bounds, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var bmp = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            }

            var shotsDir = string.IsNullOrWhiteSpace(SaveDirectory)
                ? Path.Combine(_storage.CurrentPath, "screenshots")
                : SaveDirectory;
            Directory.CreateDirectory(shotsDir);
            var filePath = Path.Combine(shotsDir, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            bmp.Save(filePath, ImageFormat.Png);

            // Copy to clipboard (bitmap) on UI dispatcher to satisfy STA
            var hBitmap = bmp.GetHbitmap();
            try
            {
                var source = WpfInterop.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, WpfInt32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                source.Freeze();

                var dispatcher = WpfApp.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.Invoke(() => WpfClipboard.SetImage(source), DispatcherPriority.Background);
                }
                else
                {
                    // Fallback (may throw if no dispatcher present)
                    WpfClipboard.SetImage(source);
                }
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }

            return filePath;
        }, cancellationToken);
    }

    private static Rectangle GetVirtualScreenBounds()
    {
        int minX = Screen.AllScreens.Min(s => s.Bounds.X);
        int minY = Screen.AllScreens.Min(s => s.Bounds.Y);
        int maxX = Screen.AllScreens.Max(s => s.Bounds.Right);
        int maxY = Screen.AllScreens.Max(s => s.Bounds.Bottom);
        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }
}
#pragma warning restore CA1416
