using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class ImageToolsView : UserControl
{
    public ImageToolsView(ImageToolsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
