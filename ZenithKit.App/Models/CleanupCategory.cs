using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZenithKit.App.Models;

public sealed class CleanupCategory : INotifyPropertyChanged
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Path { get; init; }

    private long _size;
    public long Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SizeText));
            }
        }
    }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public string SizeText => FormatBytes(Size);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        double size = bytes;
        int unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return $"{size:F2} {units[unit]}";
    }
}

public sealed record CleanupResult(int FilesDeleted, long SpaceFreed);

public sealed record CleanupProgress(string CategoryName, int FilesDeleted);
