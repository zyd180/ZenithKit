using System.Collections.ObjectModel;
using System.Windows.Threading;
using MiCodeAutoToolBox.App.Models;
using MiCodeAutoToolBox.Core.Services;
using WpfClipboard = System.Windows.Clipboard;

namespace MiCodeAutoToolBox.App.Services;

public interface IClipboardHistoryService
{
    ReadOnlyObservableCollection<ClipboardEntry> Items { get; }
    void Start();
}

public sealed class ClipboardHistoryService : IClipboardHistoryService, IDisposable
{
    private readonly ObservableCollection<ClipboardEntry> _items = new();
    private readonly DispatcherTimer _timer;
    private readonly int _maxItems = 200;
    private string? _lastText;

    public ReadOnlyObservableCollection<ClipboardEntry> Items { get; }

    public ClipboardHistoryService()
    {
        Items = new ReadOnlyObservableCollection<ClipboardEntry>(_items);
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (_, _) => PollClipboard();
    }

    public void Start() => _timer.Start();

    private void PollClipboard()
    {
        try
        {
            if (!WpfClipboard.ContainsText()) return;
            var text = WpfClipboard.GetText();
            if (string.IsNullOrWhiteSpace(text)) return;
            if (text == _lastText) return;
            _lastText = text;
            _items.Insert(0, new ClipboardEntry { Text = text, Timestamp = DateTime.Now });
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
        _timer.Tick -= (_, _) => PollClipboard();
    }
}
