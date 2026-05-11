using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class ChecksumView : UserControl
{
    public ChecksumView(ChecksumViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
