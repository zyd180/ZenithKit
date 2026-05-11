namespace ZenithKit.Core.Modules;

public sealed record ModuleMetadata(
    string Id,
    string Name,
    string Description,
    string? Version = null,
    string? Author = null
);
