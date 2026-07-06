using System.Text.Json.Nodes;

namespace Ara3D.MCP.Tests;

[TestFixture]
public class McpJsonTests
{
    sealed record SampleDto(string UserName, string? OptionalValue);

    [Test]
    public void Serialize_UsesCamelCase()
    {
        var json = McpJson.Serialize(new SampleDto("alice", null));
        var node = JsonNode.Parse(json)!.AsObject();

        Assert.That(node.ContainsKey("userName"), Is.True);
        Assert.That(node.ContainsKey("UserName"), Is.False);
        Assert.That(node["userName"]!.GetValue<string>(), Is.EqualTo("alice"));
    }

    [Test]
    public void Serialize_OmitsNulls()
    {
        var json = McpJson.Serialize(new SampleDto("alice", null));
        var node = JsonNode.Parse(json)!.AsObject();

        Assert.That(node.ContainsKey("optionalValue"), Is.False);
    }

    [Test]
    public void Deserialize_RoundTrips()
    {
        var original = new SampleDto("bob", "value");
        var json = McpJson.Serialize(original);
        var restored = McpJson.Deserialize<SampleDto>(json);

        Assert.That(restored.UserName, Is.EqualTo("bob"));
        Assert.That(restored.OptionalValue, Is.EqualTo("value"));
    }
}
