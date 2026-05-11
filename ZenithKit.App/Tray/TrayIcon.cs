using System;
using System.Windows;
using System.Windows.Forms;
using ZenithKit.Core.Services;
using WpfApp = System.Windows.Application;

namespace ZenithKit.App.Tray;

#pragma warning disable CA1416 // Windows-only APIs (NotifyIcon, WinForms menu)
public sealed class TrayIcon : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ICommandBus _commandBus;

    public TrayIcon(ICommandBus commandBus)
    {
        _commandBus = commandBus;
        _notifyIcon = new NotifyIcon
        {
            Text = "ZenithKit",
            Visible = true,
            Icon = System.Drawing.SystemIcons.Application
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("打开主界面", null, (_, _) => _commandBus.TryExecute("ui.show"));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("退出", null, (_, _) => WpfApp.Current?.Shutdown());
        _notifyIcon.ContextMenuStrip = contextMenu;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
#pragma warning restore CA1416
