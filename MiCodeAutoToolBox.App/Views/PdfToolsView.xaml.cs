using System.Windows;
using MiCodeAutoToolBox.App.ViewModels;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Views;

public partial class PdfToolsView : UserControl
{
    public PdfToolsView(PdfToolsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void AddPath_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is PdfToolsViewModel vm && !string.IsNullOrWhiteSpace(AddPathBox.Text))
        {
            vm.MergeList.Add(AddPathBox.Text);
            AddPathBox.Text = string.Empty;
        }
    }
}
