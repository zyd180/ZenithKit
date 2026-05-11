using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ZenithKit.App.Services;
using ZenithKit.App.Models;

namespace ZenithKit.App.ViewModels;

public partial class ClipboardViewModel : ObservableObject
{
    public ReadOnlyObservableCollection<ClipboardEntry> Items { get; }

    public ClipboardViewModel(IClipboardHistoryService historyService)
    {
        Items = historyService.Items;
        historyService.Start();
    }
}
