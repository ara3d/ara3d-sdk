using System.Text.Json.Nodes;

namespace Ara3D.MCP.Tests;

[TestFixture]
public class ToolRunnerTests
{
    static JsonObject Parse(string json)
        => JsonNode.Parse(json)!.AsObject();

    [Test]
    public async Task RunAsync_SyncSuccess()
    {
        var json = await ToolRunner.RunAsync(() => new { value = 42 });
        var node = Parse(json);

        Assert.That(node["ok"]!.GetValue<bool>(), Is.True);
        Assert.That(node["data"]!["value"]!.GetValue<int>(), Is.EqualTo(42));
    }

    [Test]
    public async Task RunAsync_AsyncSuccess()
    {
        var json = await ToolRunner.RunAsync(async () =>
        {
            await Task.Yield();
            return (object)new { name = "test" };
        });
        var node = Parse(json);

        Assert.That(node["ok"]!.GetValue<bool>(), Is.True);
        Assert.That(node["data"]!["name"]!.GetValue<string>(), Is.EqualTo("test"));
    }

    [Test]
    public async Task RunAsync_CatchesException()
    {
        var json = await ToolRunner.RunAsync(() => throw new InvalidOperationException("boom"));
        var node = Parse(json);

        Assert.That(node["ok"]!.GetValue<bool>(), Is.False);
        Assert.That(node["error"]!.GetValue<string>(), Is.EqualTo("boom"));
        Assert.That(node["type"]!.GetValue<string>(), Is.EqualTo(nameof(InvalidOperationException)));
    }

    [Test]
    public async Task RunAsync_IncludesNextRecommendedTools()
    {
        var json = await ToolRunner.RunAsync(
            () => new { id = 1 },
            ["list_scene", "fit_view"]);
        var node = Parse(json);

        Assert.That(node["ok"]!.GetValue<bool>(), Is.True);
        var next = node["nextRecommendedTools"]!.AsArray();
        Assert.That(next.Count, Is.EqualTo(2));
        Assert.That(next[0]!.GetValue<string>(), Is.EqualTo("list_scene"));
        Assert.That(next[1]!.GetValue<string>(), Is.EqualTo("fit_view"));
    }

    [Test]
    public async Task RunAsync_OmitsNextRecommendedToolsWhenEmpty()
    {
        var json = await ToolRunner.RunAsync(() => new { id = 1 }, []);
        var node = Parse(json);

        Assert.That(node.ContainsKey("nextRecommendedTools"), Is.False);
    }
}
