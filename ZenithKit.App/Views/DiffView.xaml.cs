using WpfUserControl = System.Windows.Controls.UserControl;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ZenithKit.App.ViewModels;

namespace ZenithKit.App.Views;

public partial class DiffView : WpfUserControl
{
    private bool _syncing;

    public DiffView(DiffViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void OnLeftScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (_syncing) return;
        _syncing = true;
        RightScroll.ScrollToVerticalOffset(e.VerticalOffset);
        _syncing = false;
    }

    private void OnRightScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (_syncing) return;
        _syncing = true;
        LeftScroll.ScrollToVerticalOffset(e.VerticalOffset);
        _syncing = false;
    }
}
