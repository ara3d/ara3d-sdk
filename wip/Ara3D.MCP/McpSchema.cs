using System.Text.Json.Nodes;

namespace Ara3D.MCP;

/// <summary>Builds JSON Schema objects for MCP tool input schemas.</summary>
public static class McpSchema
{
    public static JsonObject None()
        => Object().Build();

    public static McpSchemaBuilder Object()
        => new();

    /// <summary>An item schema for an array of primitives.</summary>
    public static JsonObject Items(string type)
        => new() { ["type"] = type };
}

public sealed class McpSchemaBuilder
{
    private readonly JsonObject _properties = new();
    private readonly List<string> _required = [];

    public McpSchemaBuilder String(string name, string description, bool required = false, string? defaultValue = null)
        => Add(name, Primitive("string", description, defaultValue), required);

    public McpSchemaBuilder Number(string name, string description, bool required = false, double? defaultValue = null)
        => Add(name, Primitive("number", description, defaultValue), required);

    public McpSchemaBuilder Integer(string name, string description, bool required = false, int? defaultValue = null)
        => Add(name, Primitive("integer", description, defaultValue), required);

    public McpSchemaBuilder Boolean(string name, string description, bool required = false, bool? defaultValue = null)
        => Add(name, Primitive("boolean", description, defaultValue), required);

    /// <summary>A string property a client can validate against a fixed set of values.</summary>
    public McpSchemaBuilder Enum(
        string name,
        string description,
        IReadOnlyList<string> values,
        bool required = false,
        string? defaultValue = null)
    {
        if (values == null || values.Count == 0)
            throw new ArgumentException("An enum needs at least one value.", nameof(values));

        var schema = Primitive("string", description, defaultValue);
        schema["enum"] = ToJsonArray(values);
        return Add(name, schema, required);
    }

    public McpSchemaBuilder Array(string name, string description, string itemType, bool required = false)
        => Array(name, description, McpSchema.Items(itemType), required);

    public McpSchemaBuilder Array(string name, string description, JsonObject itemSchema, bool required = false)
    {
        if (itemSchema == null)
            throw new ArgumentNullException(nameof(itemSchema));

        return Add(name, new JsonObject
        {
            ["type"] = "array",
            ["description"] = description,
            ["items"] = itemSchema,
        }, required);
    }

    /// <summary>A nested object property, described by its own builder.</summary>
    public McpSchemaBuilder Object(string name, string description, McpSchemaBuilder properties, bool required = false)
    {
        if (properties == null)
            throw new ArgumentNullException(nameof(properties));

        var schema = properties.Build();
        schema["description"] = description;
        return Add(name, schema, required);
    }

    public JsonObject Build()
    {
        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = _properties,
        };

        if (_required.Count > 0)
            schema["required"] = ToJsonArray(_required);

        return schema;
    }

    private McpSchemaBuilder Add(string name, JsonObject schema, bool required)
    {
        _properties[name] = schema;

        if (required && !_required.Contains(name))
            _required.Add(name);

        return this;
    }

    private static JsonObject Primitive(string type, string description, JsonNode? defaultValue)
    {
        var schema = new JsonObject
        {
            ["type"] = type,
            ["description"] = description,
        };

        if (defaultValue != null)
            schema["default"] = defaultValue;

        return schema;
    }

    private static JsonArray ToJsonArray(IReadOnlyList<string> values)
        => new(values.Select(item => (JsonNode?)item).ToArray());
}
