using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.MCP.Tests;

[TestFixture]
public sealed class McpServerTests
{
    private const int TestPort = 18766;

    [Test]
    public void Tool_RemoveTool_UpdatesToolsList()
    {
        using var mcp = new McpServer(TestPort + 1);
        mcp.Tool("alpha", "First tool.", () => "alpha");
        mcp.Tool("beta", "Second tool.", () => "beta");

        var listed = ListToolNames(mcp);
        Assert.That(listed, Is.EquivalentTo(new[] { "alpha", "beta" }));

        Assert.That(mcp.RemoveTool("alpha"), Is.True);
        listed = ListToolNames(mcp);
        Assert.That(listed, Is.EquivalentTo(new[] { "beta" }));
    }

    [Test]
    public void RegisterAfterStart_AppearsInNextToolsList()
    {
        using var mcp = new McpServer(TestPort + 2);
        mcp.Tool("before", "Registered before start.", () => "before");
        mcp.Start();

        try
        {
            var before = ListToolNames(mcp);
            Assert.That(before, Does.Contain("before"));

            mcp.Tool("after", "Registered after start.", () => "after");
            var after = ListToolNames(mcp);
            Assert.That(after, Does.Contain("after"));
        }
        finally
        {
            mcp.Stop();
        }
    }

    [Test]
    public void RemoveToolAfterStart_AbsentFromListAndCallFails()
    {
        using var mcp = new McpServer(TestPort + 3);
        mcp.Tool("temp", "Temporary tool.", () => "temp");
        mcp.Start();

        try
        {
            Assert.That(ListToolNames(mcp), Does.Contain("temp"));
            Assert.That(mcp.RemoveTool("temp"), Is.True);
            Assert.That(ListToolNames(mcp), Does.Not.Contain("temp"));

            var result = mcp.HandlePost("""
                {"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"temp","arguments":{}}}
                """);

            var error = JsonNode.Parse(result.JsonBody!)!["error"]!["message"]!.GetValue<string>();
            Assert.That(error, Does.Contain("Unknown tool"));
        }
        finally
        {
            mcp.Stop();
        }
    }

    [Test]
    public async Task HttpPost_ToolsListAndCall_RoundTrip()
    {
        using var mcp = new McpServer(TestPort + 4);
        mcp.Tool(
            "echo",
            "Echoes a message.",
            McpSchema.Object().String("message", "Text to echo.", required: true).Build(),
            (args, _) => Task.FromResult(args.GetRequiredString("message")));
        mcp.Start();

        try
        {
            using var client = new HttpClient();
            var listResponse = await client.PostAsync(
                mcp.Url,
                new StringContent(
                    """{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""",
                    Encoding.UTF8,
                    "application/json"));

            Assert.That(listResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            var listBody = await listResponse.Content.ReadAsStringAsync();
            Assert.That(listBody, Does.Contain("echo"));

            var callResponse = await client.PostAsync(
                mcp.Url,
                new StringContent(
                    """{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"echo","arguments":{"message":"hello"}}}""",
                    Encoding.UTF8,
                    "application/json"));

            Assert.That(callResponse.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            var callBody = await callResponse.Content.ReadAsStringAsync();
            var text = JsonNode.Parse(callBody)!["result"]!["content"]![0]!["text"]!.GetValue<string>();
            Assert.That(text, Is.EqualTo("hello"));
        }
        finally
        {
            mcp.Stop();
        }
    }

    [Test]
    public void Url_UsesConfiguredPortAndPath()
    {
        using var mcp = new McpServer(8766);
        Assert.That(mcp.Url, Is.EqualTo("http://127.0.0.1:8766/mcp"));
    }

    private static IReadOnlyList<string> ListToolNames(McpServer mcp)
    {
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
            """);

        var tools = JsonNode.Parse(result.JsonBody!)!["result"]!["tools"]!.AsArray();
        return tools
            .Select(tool => tool!["name"]!.GetValue<string>())
            .ToList();
    }
}
