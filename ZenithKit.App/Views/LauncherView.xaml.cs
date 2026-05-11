using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class LauncherView : UserControl
{
    public LauncherView(LauncherViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
