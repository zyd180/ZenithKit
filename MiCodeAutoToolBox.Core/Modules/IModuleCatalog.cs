namespace MiCodeAutoToolBox.Core.Modules;

public interface IModuleCatalog
{
    IEnumerable<ModuleMetadata> Modules { get; }
    void Register(ModuleMetadata module);
}
