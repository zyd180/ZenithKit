using System.IO;
using System.Windows;
using ZenithKit.Core.Services;
using ZenithKit.Core.Modules;
using ZenithKit.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Application = System.Windows.Application;

namespace ZenithKit.App;

public partial class App : Application
{
    public IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Register UI command to show main window
        var commandBus = Services.GetRequiredService<ICommandBus>();
        commandBus.Register("ui.show", () =>
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var mw = Services.GetRequiredService<MainWindow>();
                if (mw.WindowState == WindowState.Minimized)
                {
                    mw.WindowState = WindowState.Normal;
                }
                mw.Show();
                mw.Activate();
            });
        });

        // Create tray
        Services.GetRequiredService<Tray.TrayIcon>();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICommandBus, CommandBus>();
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IModuleCatalog>(sp =>
        {
            var catalog = new ModuleCatalog();
            // Register initial high-frequency utilities
            catalog.Register(new ModuleMetadata("clipboard", "剪贴板历史", "记录并搜索剪贴板（文本/图片/RTF）"));
            catalog.Register(new ModuleMetadata("screenshot", "截图标注", "截图、涂鸦标注、快捷保存"));
            // catalog.Register(new ModuleMetadata("launcher", "启动器", "全局快捷命令/应用启动"));
            // catalog.Register(new ModuleMetadata("filesearch", "文件快速搜索", "文件名索引搜索与最近文件"));
            // catalog.Register(new ModuleMetadata("rename", "批量重命名", "模式/编号/大小写批量改名"));
            catalog.Register(new ModuleMetadata("archive", "压缩/解压", "zip 压缩/解压工具"));
            // catalog.Register(new ModuleMetadata("checksum", "校验和", "MD5/SHA1/SHA256 计算与验证"));
            catalog.Register(new ModuleMetadata("image", "图片转换/压缩", "格式转换、缩放、压缩"));
            catalog.Register(new ModuleMetadata("pdf", "PDF 合并/拆分", "离线 PDF 合并、拆分"));
            catalog.Register(new ModuleMetadata("diff", "文本对比", "双文件文本差异对比"));
            return catalog;
        });

        // Default storage: AppData
        services.AddSingleton<IStorageService>(sp =>
        {
            var appName = "ZenithKit";
            var initial = new StorageOptions
            {
                Location = StorageLocation.AppData,
                CurrentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName)
            };
            return new StorageService(appName, initial);
        });

        // Services
        services.AddSingleton<IClipboardHistoryService, ClipboardHistoryService>();
        services.AddSingleton<IScreenshotService, ScreenshotService>();
        services.AddSingleton<IWindowEnumerator, WindowEnumerator>();
        services.AddSingleton<IFileSearchService, FileSearchService>();
        services.AddSingleton<ILauncherService, LauncherService>();
        services.AddSingleton<IRenameService, RenameService>();
        services.AddSingleton<IArchiveService, ArchiveService>();
        services.AddSingleton<IChecksumService, ChecksumService>();
        services.AddSingleton<IImageToolsService, ImageToolsService>();
        services.AddSingleton<IPdfToolsService, PdfToolsService>();
        services.AddSingleton<IDiffService, DiffService>();
        services.AddSingleton<IRegionSelector, RegionSelector>();

        // ViewModels
        services.AddSingleton<ViewModels.MainViewModel>();
        services.AddTransient<ViewModels.ClipboardViewModel>();
        services.AddTransient<ViewModels.ScreenshotViewModel>();
        services.AddTransient<ViewModels.FileSearchViewModel>();
        services.AddTransient<ViewModels.LauncherViewModel>();
        services.AddTransient<ViewModels.RenameViewModel>();
        services.AddTransient<ViewModels.ArchiveViewModel>();
        services.AddTransient<ViewModels.ChecksumViewModel>();
        services.AddTransient<ViewModels.ImageToolsViewModel>();
        services.AddTransient<ViewModels.PdfToolsViewModel>();
        services.AddTransient<ViewModels.DiffViewModel>();
        services.AddSingleton<IModuleViewProvider, ModuleViewProvider>();

        // Views registered as transient
        services.AddTransient<Views.ClipboardView>();
        services.AddTransient<Views.ScreenshotView>();
        services.AddTransient<Views.LauncherView>();
        services.AddTransient<Views.FileSearchView>();
        services.AddTransient<Views.RenameView>();
        services.AddTransient<Views.ArchiveView>();
        services.AddTransient<Views.ChecksumView>();
        services.AddTransient<Views.ImageToolsView>();
        services.AddTransient<Views.PdfToolsView>();
        services.AddTransient<Views.DiffView>();

        // Tray
        services.AddSingleton<Tray.TrayIcon>();

        // Windows
        services.AddSingleton<MainWindow>();
    }
}
