namespace MiCodeAutoToolBox.Core.Services;

/// <summary>
/// Simple command bus to dispatch actions (menu, hotkey, tray) to modules.
/// </summary>
public interface ICommandBus
{
    void Register(string commandId, Action handler, string? description = null);
    bool TryExecute(string commandId);
    IEnumerable<string> Commands { get; }
}
