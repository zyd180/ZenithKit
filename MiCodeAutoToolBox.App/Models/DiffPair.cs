namespace MiCodeAutoToolBox.App.Models;

public sealed class DiffPair
{
    public int LineNumber { get; init; }
    public string Left { get; init; } = string.Empty;
    public string Right { get; init; } = string.Empty;
    public bool IsDifferent { get; init; }
}
