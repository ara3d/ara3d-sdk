using System.Text.Json.Nodes;

namespace Ara3D.MCP;

/// <summary>Typed accessors for MCP tool call arguments.</summary>
public sealed class McpToolArgs
{
    private readonly JsonObject _arguments;

    public McpToolArgs(JsonObject? arguments)
        => _arguments = arguments ?? new JsonObject();

    public string? GetString(string name)
    {
        if (_arguments[name] == null)
            return null;

        return _arguments[name]!.GetValue<string>();
    }

    public string GetRequiredString(string name)
    {
        var value = GetString(name);
        if (string.IsNullOrWhiteSpace(value))
            throw new McpProtocolException(-32602, $"Missing required argument: {name}");

        return value;
    }

    public int? GetInt(string name)
    {
        if (_arguments[name] == null)
            return null;

        return _arguments[name]!.GetValue<int>();
    }

    public int GetRequiredInt(string name)
    {
        if (_arguments[name] == null)
            throw new McpProtocolException(-32602, $"Missing required argument: {name}");

        return _arguments[name]!.GetValue<int>();
    }

    public double? GetNumber(string name)
    {
        if (_arguments[name] == null)
            return null;

        return _arguments[name]!.GetValue<double>();
    }

    public bool? GetBool(string name)
    {
        if (_arguments[name] == null)
            return null;

        return _arguments[name]!.GetValue<bool>();
    }
}
