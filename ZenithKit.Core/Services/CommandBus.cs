using System.Collections.Concurrent;

namespace ZenithKit.Core.Services;

public sealed class CommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<string, Action> _handlers = new();
    private readonly ConcurrentDictionary<string, string?> _descriptions = new();

    public IEnumerable<string> Commands => _handlers.Keys;

    public void Register(string commandId, Action handler, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(commandId))
            throw new ArgumentException("Command id is required", nameof(commandId));
        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        _handlers[commandId] = handler;
        if (description is not null)
        {
            _descriptions[commandId] = description;
        }
    }

    public bool TryExecute(string commandId)
    {
        if (_handlers.TryGetValue(commandId, out var handler))
        {
            handler();
            return true;
        }
        return false;
    }
}
