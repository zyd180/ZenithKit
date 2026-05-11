using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class LauncherView : UserControl
{
    public LauncherView(LauncherViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
