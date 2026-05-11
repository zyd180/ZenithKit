using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class ArchiveView : UserControl
{
    public ArchiveView(ArchiveViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
