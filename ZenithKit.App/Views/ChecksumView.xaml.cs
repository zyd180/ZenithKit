using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class ChecksumView : UserControl
{
    public ChecksumView(ChecksumViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
