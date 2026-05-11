using ZenithKit.Core.Modules;
using UserControl = System.Windows.Controls.UserControl;

namespace ZenithKit.App.Views;

public partial class PlaceholderView : UserControl
{
    public PlaceholderView(ModuleMetadata module)
    {
        InitializeComponent();
        TitleText.Text = module.Name;
        DescText.Text = module.Description;
    }
}
