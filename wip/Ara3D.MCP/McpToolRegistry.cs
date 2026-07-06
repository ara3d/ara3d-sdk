using System.Collections.Concurrent;
using System.Text.Json.Nodes;

namespace Ara3D.MCP;

internal sealed record McpToolEntry(
    string Name,
    string Description,
    JsonObject InputSchema,
    Func<McpToolArgs, CancellationToken, Task<string>> Handler);

internal sealed class McpToolRegistry
{
    private readonly ConcurrentDictionary<string, McpToolEntry> _tools = new(StringComparer.Ordinal);

    public void Register(
        string name,
        string description,
        JsonObject inputSchema,
        Func<McpToolArgs, CancellationToken, Task<string>> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(handler);

        _tools[name] = new McpToolEntry(name, description, inputSchema, handler);
    }

    public bool Remove(string name)
        => _tools.TryRemove(name, out _);

    public IReadOnlyList<JsonObject> ListTools()
        => _tools.Values
            .OrderBy(tool => tool.Name, StringComparer.Ordinal)
            .Select(ToListItem)
            .ToList();

    public async Task<string> CallAsync(string? name, JsonObject arguments, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name) || !_tools.TryGetValue(name, out var entry))
            throw new McpProtocolException(-32602, $"Unknown tool: {name}");

        return await entry.Handler(new McpToolArgs(arguments), cancellationToken).ConfigureAwait(false);
    }

    private static JsonObject ToListItem(McpToolEntry entry)
        => new()
        {
            ["name"] = entry.Name,
            ["description"] = entry.Description,
            ["inputSchema"] = entry.InputSchema.DeepClone(),
        };
}
