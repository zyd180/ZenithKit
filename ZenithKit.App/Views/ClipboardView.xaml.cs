using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class ClipboardView : UserControl
{
    public ClipboardView(ClipboardViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
