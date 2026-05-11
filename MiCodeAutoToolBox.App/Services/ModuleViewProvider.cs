using Microsoft.Extensions.DependencyInjection;
using MiCodeAutoToolBox.Core.Modules;
using MiCodeAutoToolBox.App.Views;
using UserControl = System.Windows.Controls.UserControl;

namespace MiCodeAutoToolBox.App.Services;

public sealed class ModuleViewProvider : IModuleViewProvider
{
    private readonly IServiceProvider _services;

    public ModuleViewProvider(IServiceProvider services)
    {
        _services = services;
    }

    public UserControl GetView(ModuleMetadata module)
    {
        return module.Id switch
        {
            "clipboard" => _services.GetRequiredService<ClipboardView>(),
            "screenshot" => _services.GetRequiredService<ScreenshotView>(),
            // "launcher" => _services.GetRequiredService<LauncherView>(),
            // "filesearch" => _services.GetRequiredService<FileSearchView>(),
            // "rename" => _services.GetRequiredService<RenameView>(),
            "archive" => _services.GetRequiredService<ArchiveView>(),
            // "checksum" => _services.GetRequiredService<ChecksumView>(),
            "image" => _services.GetRequiredService<ImageToolsView>(),
            "pdf" => _services.GetRequiredService<PdfToolsView>(),
            "diff" => _services.GetRequiredService<DiffView>(),
            _ => new PlaceholderView(module)
        };
    }
}
