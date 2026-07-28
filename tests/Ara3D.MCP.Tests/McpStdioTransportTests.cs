using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.MCP.Tests;

[TestFixture]
public sealed class McpStdioTransportTests
{
    [Test]
    public void Stdio_ToolsListAndCall_RoundTrip()
    {
        var lines = Exchange(
            """{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""",
            """{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"echo","arguments":{"message":"hello"}}}""");

        Assert.That(lines, Has.Count.EqualTo(2));
        Assert.That(lines[0], Does.Contain("echo"));

        var text = JsonNode.Parse(lines[1])!["result"]!["content"]![0]!["text"]!.GetValue<string>();
        Assert.That(text, Is.EqualTo("hello"));
    }

    [Test]
    public void Stdio_Notification_WritesNothing()
    {
        var lines = Exchange(
            """{"jsonrpc":"2.0","method":"notifications/initialized"}""",
            """{"jsonrpc":"2.0","id":1,"method":"ping","params":{}}""");

        Assert.That(lines, Has.Count.EqualTo(1));
        Assert.That(JsonNode.Parse(lines[0])!["id"]!.GetValue<int>(), Is.EqualTo(1));
    }

    [Test]
    public void Stdio_UnparseableAndBlankLines_AreSkipped()
    {
        var lines = Exchange(
            "",
            "not json at all",
            """{"jsonrpc":"2.0","id":7,"method":"ping","params":{}}""");

        Assert.That(lines, Has.Count.EqualTo(1));
        Assert.That(JsonNode.Parse(lines[0])!["id"]!.GetValue<int>(), Is.EqualTo(7));
    }

    [Test]
    public void Stdio_UrlIsNullAndActiveTracksPump()
    {
        using var mcp = new McpServer(transport: McpTransport.Stdio);
        Assert.That(mcp.Url, Is.Null);
        Assert.That(mcp.Active, Is.False);
    }

    /// <summary>Feeds the given lines through a stdio server and returns the response lines,
    /// which arrive once the reader hits end of input and the pump exits.</summary>
    private static IReadOnlyList<string> Exchange(params string[] inputLines)
    {
        using var mcp = new McpServer(transport: McpTransport.Stdio);
        mcp.Tool(
            "echo",
            "Echoes a message.",
            McpSchema.Object().String("message", "Text to echo.", required: true).Build(),
            (args, _) => Task.FromResult(args.GetRequiredString("message")));

        var output = new StringWriter();
        using (var input = new StringReader(string.Join("\n", inputLines)))
        {
            mcp.StartStdio(input, output);
            mcp.WaitForShutdown();
        }

        return output
            .ToString()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }
}
