using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class RenameView : UserControl
{
    public RenameView(RenameViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
