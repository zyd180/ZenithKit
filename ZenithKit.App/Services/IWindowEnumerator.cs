using ZenithKit.App.Models;

namespace ZenithKit.App.Services;

public interface IWindowEnumerator
{
    IReadOnlyList<WindowEntry> Enumerate();
}
