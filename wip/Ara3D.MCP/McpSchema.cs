using System.Text.Json.Nodes;

namespace Ara3D.MCP;

/// <summary>Builds JSON Schema objects for MCP tool input schemas.</summary>
public static class McpSchema
{
    public static JsonObject None()
        => Object().Build();

    public static McpSchemaBuilder Object()
        => new();
}

public sealed class McpSchemaBuilder
{
    private readonly JsonObject _properties = new();
    private readonly List<string> _required = [];

    public McpSchemaBuilder String(string name, string description, bool required = false)
        => AddProperty(name, "string", description, required);

    public McpSchemaBuilder Number(string name, string description, bool required = false)
        => AddProperty(name, "number", description, required);

    public McpSchemaBuilder Integer(string name, string description, bool required = false)
        => AddProperty(name, "integer", description, required);

    public McpSchemaBuilder Boolean(string name, string description, bool required = false)
        => AddProperty(name, "boolean", description, required);

    public JsonObject Build()
    {
        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = _properties,
        };

        if (_required.Count > 0)
            schema["required"] = new JsonArray(_required.Select(item => (JsonNode?)item).ToArray());

        return schema;
    }

    private McpSchemaBuilder AddProperty(string name, string type, string description, bool required)
    {
        _properties[name] = new JsonObject
        {
            ["type"] = type,
            ["description"] = description,
        };

        if (required)
            _required.Add(name);

        return this;
    }
}
