using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using ZenithKit.App.Views;

namespace ZenithKit.App.Services;

#pragma warning disable CA1416 // Windows only
public sealed class RegionSelector : IRegionSelector
{
    public Task<Rectangle?> PickAsync()
    {
        var tcs = new TaskCompletionSource<Rectangle?>();
        var app = System.Windows.Application.Current;
        app?.Dispatcher.Invoke(() =>
        {
            var picker = new RegionSelectWindow { Owner = app?.MainWindow };
            if (picker.ShowDialog() == true)
            {
                var rect = new Rectangle((int)picker.SelectedRect.X, (int)picker.SelectedRect.Y, (int)picker.SelectedRect.Width, (int)picker.SelectedRect.Height);
                tcs.TrySetResult(rect);
            }
            else
            {
                tcs.TrySetResult(null);
            }
        });
        return tcs.Task;
    }
}
#pragma warning restore CA1416
