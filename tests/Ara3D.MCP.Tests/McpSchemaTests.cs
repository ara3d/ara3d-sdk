using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.MCP.Tests;

[TestFixture]
public sealed class McpSchemaTests
{
    [Test]
    public void None_IsAnEmptyObjectSchema()
    {
        var schema = McpSchema.None();
        Assert.That(schema["type"]!.GetValue<string>(), Is.EqualTo("object"));
        Assert.That(schema["properties"]!.AsObject(), Is.Empty);
        Assert.That(schema["required"], Is.Null);
    }

    [Test]
    public void Primitives_KeepTypeDescriptionAndRequired()
    {
        var schema = McpSchema.Object()
            .String("s", "A string.", required: true)
            .Number("n", "A number.")
            .Integer("i", "An integer.")
            .Boolean("b", "A boolean.")
            .Build();

        var props = schema["properties"]!.AsObject();
        Assert.That(props["s"]!["type"]!.GetValue<string>(), Is.EqualTo("string"));
        Assert.That(props["s"]!["description"]!.GetValue<string>(), Is.EqualTo("A string."));
        Assert.That(props["n"]!["type"]!.GetValue<string>(), Is.EqualTo("number"));
        Assert.That(props["i"]!["type"]!.GetValue<string>(), Is.EqualTo("integer"));
        Assert.That(props["b"]!["type"]!.GetValue<string>(), Is.EqualTo("boolean"));
        Assert.That(RequiredNames(schema), Is.EquivalentTo(new[] { "s" }));
    }

    [Test]
    public void Defaults_AreEmittedOnlyWhenGiven()
    {
        var schema = McpSchema.Object()
            .String("s", "A string.", defaultValue: "abc")
            .Number("n", "A number.", defaultValue: 1.5)
            .Integer("i", "An integer.", defaultValue: 7)
            .Boolean("b", "A boolean.", defaultValue: true)
            .String("plain", "No default.")
            .Build();

        var props = schema["properties"]!.AsObject();
        Assert.That(props["s"]!["default"]!.GetValue<string>(), Is.EqualTo("abc"));
        Assert.That(props["n"]!["default"]!.GetValue<double>(), Is.EqualTo(1.5));
        Assert.That(props["i"]!["default"]!.GetValue<int>(), Is.EqualTo(7));
        Assert.That(props["b"]!["default"]!.GetValue<bool>(), Is.True);
        Assert.That(props["plain"]!["default"], Is.Null);
    }

    [Test]
    public void Enum_ListsValuesAndDefault()
    {
        var schema = McpSchema.Object()
            .Enum("kind", "Relation kind.", ["contains", "aggregates"], required: true, defaultValue: "contains")
            .Build();

        var property = schema["properties"]!["kind"]!;
        Assert.That(property["type"]!.GetValue<string>(), Is.EqualTo("string"));
        Assert.That(
            property["enum"]!.AsArray().Select(item => item!.GetValue<string>()),
            Is.EqualTo(new[] { "contains", "aggregates" }));
        Assert.That(property["default"]!.GetValue<string>(), Is.EqualTo("contains"));
        Assert.That(RequiredNames(schema), Is.EquivalentTo(new[] { "kind" }));
    }

    [Test]
    public void Enum_RejectsEmptyValueList()
        => Assert.That(
            () => McpSchema.Object().Enum("kind", "Relation kind.", []),
            Throws.ArgumentException);

    [Test]
    public void Array_OfPrimitives_UsesItemType()
    {
        var schema = McpSchema.Object()
            .Array("ids", "Entity ids.", "integer", required: true)
            .Build();

        var property = schema["properties"]!["ids"]!;
        Assert.That(property["type"]!.GetValue<string>(), Is.EqualTo("array"));
        Assert.That(property["items"]!["type"]!.GetValue<string>(), Is.EqualTo("integer"));
        Assert.That(RequiredNames(schema), Is.EquivalentTo(new[] { "ids" }));
    }

    [Test]
    public void Array_OfObjects_CarriesTheItemSchema()
    {
        var item = McpSchema.Object()
            .String("name", "Property name.", required: true)
            .Build();

        var schema = McpSchema.Object()
            .Array("properties", "Property list.", item)
            .Build();

        var property = schema["properties"]!["properties"]!;
        Assert.That(property["items"]!["type"]!.GetValue<string>(), Is.EqualTo("object"));
        Assert.That(property["items"]!["properties"]!["name"]!["type"]!.GetValue<string>(), Is.EqualTo("string"));
    }

    [Test]
    public void NestedObject_KeepsItsOwnPropertiesAndRequired()
    {
        var schema = McpSchema.Object()
            .Object(
                "page",
                "Paging options.",
                McpSchema.Object()
                    .Integer("offset", "Start index.", defaultValue: 0)
                    .Integer("limit", "Max rows.", required: true),
                required: true)
            .Build();

        var nested = schema["properties"]!["page"]!;
        Assert.That(nested["type"]!.GetValue<string>(), Is.EqualTo("object"));
        Assert.That(nested["description"]!.GetValue<string>(), Is.EqualTo("Paging options."));
        Assert.That(nested["properties"]!["offset"]!["default"]!.GetValue<int>(), Is.EqualTo(0));
        Assert.That(
            nested["required"]!.AsArray().Select(item => item!.GetValue<string>()),
            Is.EquivalentTo(new[] { "limit" }));
        Assert.That(RequiredNames(schema), Is.EquivalentTo(new[] { "page" }));
    }

    [Test]
    public void RichSchema_SurvivesToolsList()
    {
        using var mcp = new McpServer(18799);
        mcp.Tool(
            "query",
            "Runs a query.",
            McpSchema.Object()
                .Enum("kind", "Relation kind.", ["contains", "aggregates"], required: true)
                .Array("ids", "Entity ids.", "integer")
                .Object("page", "Paging.", McpSchema.Object().Integer("limit", "Max rows.", defaultValue: 50))
                .Build(),
            (_, _) => Task.FromResult("ok"));

        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
            """);

        var listed = JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]![0]!["inputSchema"]!;
        Assert.That(listed["properties"]!["kind"]!["enum"]!.AsArray(), Has.Count.EqualTo(2));
        Assert.That(listed["properties"]!["ids"]!["type"]!.GetValue<string>(), Is.EqualTo("array"));
        Assert.That(
            listed["properties"]!["page"]!["properties"]!["limit"]!["default"]!.GetValue<int>(),
            Is.EqualTo(50));
    }

    private static IReadOnlyList<string> RequiredNames(JsonObject schema)
        => schema["required"] is JsonArray required
            ? required.Select(item => item!.GetValue<string>()).ToList()
            : [];
}
