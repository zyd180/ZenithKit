using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class ClipboardView : UserControl
{
    public ClipboardView(ClipboardViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
