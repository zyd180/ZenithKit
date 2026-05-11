using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ZenithKit.Core.Services;
using UserControl = System.Windows.Controls.UserControl;
using WpfApp = System.Windows.Application;
using WpfMessageBox = System.Windows.MessageBox;
using WinForms = System.Windows.Forms;

namespace ZenithKit.App.Controls;

public partial class StorageSwitcher : UserControl
{
    private readonly IStorageService _storageService;

    public StorageSwitcher()
    {
        InitializeComponent();
        _storageService = ((App)WpfApp.Current).Services.GetRequiredService<IStorageService>();
        UpdateCurrentPath();

        LocationCombo.SelectionChanged += LocationCombo_SelectionChanged;
        ApplyButton.Click += ApplyButton_Click;
        ChoosePathButton.Click += ChoosePathButton_Click;
    }

    private void LocationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = (LocationCombo.SelectedItem as ComboBoxItem)?.Tag as string;
        ChoosePathButton.Visibility = selected == "Custom" ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        var selected = (LocationCombo.SelectedItem as ComboBoxItem)?.Tag as string;
        if (string.IsNullOrEmpty(selected)) return;

        string? customPath = null;
        if (selected == "Custom")
        {
            customPath = (string?)ChoosePathButton.Tag;
            if (string.IsNullOrWhiteSpace(customPath))
            {
                WpfMessageBox.Show("请先选择自定义路径", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        try
        {
            Progress.Visibility = Visibility.Visible;
            ApplyButton.IsEnabled = false;
            await _storageService.SwitchAsync(Enum.Parse<StorageLocation>(selected), customPath);
            UpdateCurrentPath();
            WpfMessageBox.Show("存储位置已切换", "完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            WpfMessageBox.Show($"切换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Progress.Visibility = Visibility.Collapsed;
            ApplyButton.IsEnabled = true;
        }
    }

    private void ChoosePathButton_Click(object sender, RoutedEventArgs e)
    {
#pragma warning disable CA1416 // Windows-only FolderBrowserDialog
        var dlg = new WinForms.FolderBrowserDialog();
        if (dlg.ShowDialog() == WinForms.DialogResult.OK)
        {
            ChoosePathButton.Tag = dlg.SelectedPath;
            ChoosePathButton.Content = dlg.SelectedPath;
        }
#pragma warning restore CA1416
    }

    private void UpdateCurrentPath()
    {
        var options = _storageService.GetOptions();
        CurrentPathText.Text = $"当前: {options.CurrentPath} ({options.Location})";
    }
}
