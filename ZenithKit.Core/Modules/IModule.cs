namespace ZenithKit.Core.Modules;

public interface IModule
{
    string Id { get; }
    string Name { get; }
    string Description { get; }

    /// <summary>
    /// Called on startup to allow the module to register commands, menu entries, hotkeys, etc.
    /// </summary>
    void Initialize();
}
