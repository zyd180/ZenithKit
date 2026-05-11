using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class ArchiveView : UserControl
{
    public ArchiveView(ArchiveViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
