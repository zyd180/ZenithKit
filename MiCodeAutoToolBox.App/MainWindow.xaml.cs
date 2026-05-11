using System.Windows;
using MiCodeAutoToolBox.App.ViewModels;
using MiCodeAutoToolBox.App.Services;
using MiCodeAutoToolBox.Core.Modules;

namespace MiCodeAutoToolBox.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly IModuleViewProvider _viewProvider;

    public MainWindow(MainViewModel viewModel, IModuleViewProvider viewProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewProvider = viewProvider;
        DataContext = viewModel;

        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        UpdateContent();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedModule))
        {
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (_viewModel.SelectedModule is ModuleMetadata module)
        {
            ModuleContent.Content = _viewProvider.GetView(module);
        }
        else
        {
            ModuleContent.Content = null;
        }
    }
}
