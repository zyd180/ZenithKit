using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MiCodeAutoToolBox.App.Services;
using MiCodeAutoToolBox.App.Models;

namespace MiCodeAutoToolBox.App.ViewModels;

public partial class ClipboardViewModel : ObservableObject
{
    public ReadOnlyObservableCollection<ClipboardEntry> Items { get; }

    public ClipboardViewModel(IClipboardHistoryService historyService)
    {
        Items = historyService.Items;
        historyService.Start();
    }
}
