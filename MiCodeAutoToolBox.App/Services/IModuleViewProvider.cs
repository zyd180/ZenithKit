using MiCodeAutoToolBox.Core.Modules;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Services;

public interface IModuleViewProvider
{
    UserControl GetView(ModuleMetadata module);
}
