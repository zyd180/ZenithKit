using MiCodeAutoToolBox.App.Models;

namespace MiCodeAutoToolBox.App.Services;

public interface IWindowEnumerator
{
    IReadOnlyList<WindowEntry> Enumerate();
}
