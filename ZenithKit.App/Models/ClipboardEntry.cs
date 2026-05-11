namespace ZenithKit.App.Models;

public enum ClipboardContentType { Text, Image, Rtf }

public sealed class ClipboardEntry
{
    public required string Text { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public ClipboardContentType ContentType { get; init; } = ClipboardContentType.Text;
    public string TypeLabel => ContentType switch
    {
        ClipboardContentType.Image => "图片",
        ClipboardContentType.Rtf => "RTF",
        _ => "文本"
    };
    public override string ToString() => Text;
}
