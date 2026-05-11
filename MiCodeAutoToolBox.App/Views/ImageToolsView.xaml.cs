using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class ImageToolsView : UserControl
{
    public ImageToolsView(ImageToolsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
