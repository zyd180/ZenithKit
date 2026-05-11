using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class DiskCleanupView : UserControl
{
    public DiskCleanupView(DiskCleanupViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
