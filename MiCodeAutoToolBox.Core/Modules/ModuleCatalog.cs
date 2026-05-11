namespace MiCodeAutoToolBox.Core.Modules;

public sealed class ModuleCatalog : IModuleCatalog
{
    private readonly List<ModuleMetadata> _modules = new();
    public IEnumerable<ModuleMetadata> Modules => _modules;

    public void Register(ModuleMetadata module)
    {
        if (module is null) throw new ArgumentNullException(nameof(module));
        if (_modules.Any(m => string.Equals(m.Id, module.Id, StringComparison.OrdinalIgnoreCase)))
            return;
        _modules.Add(module);
    }
}
