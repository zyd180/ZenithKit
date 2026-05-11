using ZenithKit.Core.Modules;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Services;

public interface IModuleViewProvider
{
    UserControl GetView(ModuleMetadata module);
}
