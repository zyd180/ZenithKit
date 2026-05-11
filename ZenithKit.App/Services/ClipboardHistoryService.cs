using System.Collections.ObjectModel;
using System.Windows.Threading;
using ZenithKit.App.Models;
using ZenithKit.Core.Services;
using WpfClipboard = System.Windows.Clipboard;

namespace ZenithKit.App.Services;

public interface IClipboardHistoryService
{
    ReadOnlyObservableCollection<ClipboardEntry> Items { get; }
    void Start();
}

public sealed class ClipboardHistoryService : IClipboardHistoryService, IDisposable
{
    private readonly ObservableCollection<ClipboardEntry> _items = new();
    private readonly DispatcherTimer _timer;
    private readonly EventHandler _tickHandler;
    private readonly int _maxItems = 200;
    private string? _lastContent;

    public ReadOnlyObservableCollection<ClipboardEntry> Items { get; }

    public ClipboardHistoryService()
    {
        Items = new ReadOnlyObservableCollection<ClipboardEntry>(_items);
        _tickHandler = (_, _) => PollClipboard();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += _tickHandler;
    }

    public void Start() => _timer.Start();

    private void PollClipboard()
    {
        try
        {
            if (WpfClipboard.ContainsImage())
            {
                var key = $"[Image:{DateTime.Now:yyyyMMddHHmmss}]";
                if (key == _lastContent) return;
                _lastContent = key;
                _items.Insert(0, new ClipboardEntry { Text = "[图片]", Timestamp = DateTime.Now, ContentType = ClipboardContentType.Image });
            }
            else if (WpfClipboard.ContainsText())
            {
                var text = WpfClipboard.GetText();
                if (string.IsNullOrWhiteSpace(text)) return;
                if (text == _lastContent) return;
                _lastContent = text;
                var isRtf = WpfClipboard.ContainsText(System.Windows.TextDataFormat.Rtf);
                _items.Insert(0, new ClipboardEntry
                {
                    Text = text,
                    Timestamp = DateTime.Now,
                    ContentType = isRtf ? ClipboardContentType.Rtf : ClipboardContentType.Text
                });
            }
            else
            {
                return;
            }

            while (_items.Count > _maxItems) _items.RemoveAt(_items.Count - 1);
        }
        catch
        {
            // ignore clipboard race conditions
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= _tickHandler;
    }
}
