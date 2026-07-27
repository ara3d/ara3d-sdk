using System.Text.Json.Nodes;

namespace Ara3D.MCP.Tests;

/// <summary>A failing tool call must say so at the protocol level. Reporting isError:false on a
/// failure makes a generic MCP client treat a failed call as a successful one.</summary>
[TestFixture]
public sealed class ToolErrorEnvelopeTests
{
    [Test]
    public void ThrownException_ReportsIsError()
    {
        var result = Call(mcp => mcp.Tool("boom", "Always throws.",
            _ => ToolRunner.RunAsync(object () => throw new InvalidOperationException("no good"))));

        Assert.That(result!["isError"]!.GetValue<bool>(), Is.True);
        Assert.That(Payload(result)["error"]!.GetValue<string>(), Is.EqualTo("no good"));
    }

    [Test]
    public void ProtocolException_ReportsIsError()
    {
        var result = Call(mcp => mcp.Tool("bad-args", "Always rejects.",
            _ => ToolRunner.RunAsync(object () => throw new McpProtocolException(-32602, "Missing argument"))));

        Assert.That(result!["isError"]!.GetValue<bool>(), Is.True);
    }

    [Test]
    public void SuccessfulCall_DoesNotReportIsError()
    {
        var result = Call(mcp => mcp.Tool("fine", "Always works.",
            _ => ToolRunner.RunAsync(() => (object)new { value = 42 })));

        Assert.That(result!["isError"]!.GetValue<bool>(), Is.False);
        Assert.That(Payload(result)["ok"]!.GetValue<bool>(), Is.True);
    }

    /// <summary>Handlers that return plain text never went through the standard envelope, so there
    /// is no "ok" flag to read and they must keep reporting success.</summary>
    [Test]
    public void PlainTextHandler_DoesNotReportIsError()
    {
        var result = Call(mcp => mcp.Tool("plain", "Returns raw text.", () => "just a string"));

        Assert.That(result!["isError"]!.GetValue<bool>(), Is.False);
    }

    private static JsonNode? Call(Func<McpServer, McpServer> register)
    {
        using var mcp = new McpServer(0, "test", "1.0.0");
        register(mcp);

        var toolName = JsonNode.Parse(mcp.HandlePost("""
            {"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
            """).JsonBody!)!["result"]!["tools"]![0]!["name"]!.GetValue<string>();

        var request = new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = 2,
            ["method"] = "tools/call",
            ["params"] = new JsonObject
            {
                ["name"] = toolName,
                ["arguments"] = new JsonObject()
            }
        };

        return JsonNode.Parse(mcp.HandlePost(request.ToJsonString()).JsonBody!)!["result"];
    }

    private static JsonNode Payload(JsonNode result)
        => JsonNode.Parse(result["content"]![0]!["text"]!.GetValue<string>())!;
}
