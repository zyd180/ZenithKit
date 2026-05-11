using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ZenithKit.Core.Modules;

namespace ZenithKit.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ModuleMetadata? _selectedModule;

    public ObservableCollection<ModuleMetadata> Modules { get; } = new();

    public MainViewModel(IModuleCatalog catalog)
    {
        foreach (var module in catalog.Modules)
        {
            Modules.Add(module);
        }
        SelectedModule = Modules.FirstOrDefault();
    }
}
