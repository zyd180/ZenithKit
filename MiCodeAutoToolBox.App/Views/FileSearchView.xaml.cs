using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class FileSearchView : UserControl
{
    public FileSearchView(FileSearchViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
