using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class ScreenshotView : UserControl
{
    public ScreenshotView(ScreenshotViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
