namespace ZenithKit.App.Models;

public sealed class ClipboardEntry
{
    public required string Text { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public override string ToString() => Text;
}
