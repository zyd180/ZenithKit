using System.Drawing;
using System.Threading.Tasks;

namespace MiCodeAutoToolBox.App.Services;

/// <summary>
/// Provides a mouse-drag region selection surface that returns a screen rectangle.
/// </summary>
public interface IRegionSelector
{
    Task<Rectangle?> PickAsync();
}
