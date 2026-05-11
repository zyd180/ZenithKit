using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiCodeAutoToolBox.App.Services;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class RenameViewModel : ObservableObject
{
    private readonly IRenameService _renameService;

    [ObservableProperty]
    private string _folderPath = string.Empty;

    [ObservableProperty]
    private string _pattern = "newname_{0}";

    [ObservableProperty]
    private int _start = 1;

    [ObservableProperty]
    private int _step = 1;

    public ObservableCollection<string> Renamed { get; } = new();

    public RenameViewModel(IRenameService renameService)
    {
        _renameService = renameService;
    }

    [RelayCommand]
    private async Task Preview()
    {
        Renamed.Clear();
        var results = await _renameService.PreviewAsync(FolderPath, Pattern, Start, Step);
        foreach (var path in results)
        {
            Renamed.Add(path);
        }
    }

    [RelayCommand]
    private async Task Rename()
    {
        Renamed.Clear();
        var results = await _renameService.RenameAsync(FolderPath, Pattern, Start, Step);
        foreach (var path in results)
        {
            Renamed.Add(path);
        }
    }
}
