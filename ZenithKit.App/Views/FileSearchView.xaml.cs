using ZenithKit.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class FileSearchView : UserControl
{
    public FileSearchView(FileSearchViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
