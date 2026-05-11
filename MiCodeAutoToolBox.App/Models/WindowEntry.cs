using System;

namespace MiCodeAutoToolBox.App.Models;

public sealed record WindowEntry(int ProcessId, string ProcessName, string Title, IntPtr Handle)
{
    public override string ToString() => string.IsNullOrWhiteSpace(Title) ? ProcessName : Title;
}
