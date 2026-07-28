using System.Diagnostics;
using System.Text.Json.Nodes;
using Ara3D.MCP;

namespace Ara3D.MCP.Tests;

[TestFixture]
public sealed class McpProtocolTests
{
    [Test]
    public void Initialize_EchoesASupportedClientVersion()
    {
        using var mcp = new McpServer(18780);
        var version = Initialize(mcp, "2025-06-18");

        Assert.That(version, Is.EqualTo("2025-06-18"));
        Assert.That(mcp.NegotiatedProtocolVersion, Is.EqualTo("2025-06-18"));
    }

    [Test]
    public void Initialize_FallsBackToTheServerVersionForAnUnknownRequest()
    {
        using var mcp = new McpServer(18781);
        Assert.That(Initialize(mcp, "1999-01-01"), Is.EqualTo(McpServer.ProtocolVersion));
    }

    [Test]
    public void Initialize_WithoutAVersion_AnswersWithTheServerVersion()
    {
        using var mcp = new McpServer(18782);
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}
            """);

        var version = JsonNode.Parse(result.JsonBody!)!["result"]!["protocolVersion"]!.GetValue<string>();
        Assert.That(version, Is.EqualTo(McpServer.ProtocolVersion));
    }

    [Test]
    public void Initialize_WithANonStringVersion_DoesNotFault()
    {
        using var mcp = new McpServer(18783);
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":7}}
            """);

        var payload = JsonNode.Parse(result.JsonBody!)!;
        Assert.That(payload["error"], Is.Null);
        Assert.That(payload["result"]!["protocolVersion"]!.GetValue<string>(), Is.EqualTo(McpServer.ProtocolVersion));
    }

    [Test]
    public void SupportedProtocolVersions_IncludeTheServerDefault()
        => Assert.That(McpServer.SupportedProtocolVersions, Does.Contain(McpServer.ProtocolVersion));

    [Test]
    public void InitializedNotification_IsAcceptedWithNoResponse()
    {
        using var mcp = new McpServer(18784);
        Assert.That(mcp.ClientInitialized, Is.False);

        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","method":"notifications/initialized"}
            """);

        Assert.That(result.StatusCode, Is.EqualTo(202));
        Assert.That(result.JsonBody, Is.Null);
        Assert.That(mcp.ClientInitialized, Is.True);
    }

    [Test]
    public void InitializedNotification_InABatch_IsHandledNotRejected()
    {
        using var mcp = new McpServer(18785);
        var result = mcp.HandlePost("""
            [{"jsonrpc":"2.0","method":"notifications/initialized"},
             {"jsonrpc":"2.0","id":1,"method":"ping","params":{}}]
            """);

        var payload = JsonNode.Parse(result.JsonBody!)!;
        Assert.That(payload["error"], Is.Null);
        Assert.That(payload["id"]!.GetValue<int>(), Is.EqualTo(1));
        Assert.That(mcp.ClientInitialized, Is.True);
    }

    [Test]
    public void InitializedSentAsARequest_GetsAnEmptyResultNotMethodNotFound()
    {
        using var mcp = new McpServer(18786);
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":3,"method":"notifications/initialized"}
            """);

        var payload = JsonNode.Parse(result.JsonBody!)!;
        Assert.That(payload["error"], Is.Null);
        Assert.That(payload["result"]!.AsObject(), Is.Empty);
        Assert.That(mcp.ClientInitialized, Is.True);
    }

    [Test]
    public void UnknownNotification_IsAcceptedSilently()
    {
        using var mcp = new McpServer(18787);
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","method":"notifications/cancelled","params":{}}
            """);

        Assert.That(result.StatusCode, Is.EqualTo(202));
        Assert.That(result.JsonBody, Is.Null);
        Assert.That(mcp.ClientInitialized, Is.False);
    }

    [Test]
    public void UnparseableBody_ReturnsParseErrorNotHttp400()
    {
        using var mcp = new McpServer(18788);
        var result = mcp.HandlePost("not json at all");

        Assert.That(result.StatusCode, Is.EqualTo(200));
        var payload = JsonNode.Parse(result.JsonBody!)!;
        Assert.That(payload["error"]!["code"]!.GetValue<int>(), Is.EqualTo(-32700));
        Assert.That(payload["id"], Is.Null);
        Assert.That(payload["jsonrpc"]!.GetValue<string>(), Is.EqualTo("2.0"));
    }

    [Test]
    public void EmptyBody_ReturnsParseError()
    {
        using var mcp = new McpServer(18789);
        var result = mcp.HandlePost("   ");

        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(
            JsonNode.Parse(result.JsonBody!)!["error"]!["code"]!.GetValue<int>(),
            Is.EqualTo(-32700));
    }

    [Test]
    public void NonObjectJson_ReturnsInvalidRequest()
    {
        using var mcp = new McpServer(18790);
        var result = mcp.HandlePost("42");

        Assert.That(
            JsonNode.Parse(result.JsonBody!)!["error"]!["code"]!.GetValue<int>(),
            Is.EqualTo(-32600));
    }

    [Test]
    public void UnknownMethod_StillReturnsMethodNotFound()
    {
        using var mcp = new McpServer(18791);
        var result = mcp.HandlePost("""
            {"jsonrpc":"2.0","id":9,"method":"resources/list","params":{}}
            """);

        Assert.That(
            JsonNode.Parse(result.JsonBody!)!["error"]!["code"]!.GetValue<int>(),
            Is.EqualTo(-32601));
    }

    /// <summary>A tool that blocks must not stop the listener accepting the next request; the
    /// second call only completes because the first one is not holding the accept loop.</summary>
    [Test]
    public async Task SlowToolCall_DoesNotBlockTheListenerThread()
    {
        using var gate = new ManualResetEventSlim(false);
        using var mcp = new McpServer(18792);
        mcp.Tool("slow", "Blocks until released.", async _ =>
        {
            await Task.Run(() => gate.Wait(TimeSpan.FromSeconds(10)));
            return "slow";
        });
        mcp.Tool("fast", "Returns at once.", () => "fast");
        mcp.Start();

        try
        {
            using var client = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var slow = Post(client, mcp.Url!, """
                {"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"slow","arguments":{}}}
                """);

            var watch = Stopwatch.StartNew();
            var fast = await Post(client, mcp.Url!, """
                {"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"fast","arguments":{}}}
                """);
            watch.Stop();

            Assert.That(TextOf(fast), Is.EqualTo("fast"));
            Assert.That(watch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(5)));

            gate.Set();
            Assert.That(TextOf(await slow), Is.EqualTo("slow"));
        }
        finally
        {
            gate.Set();
            mcp.Stop();
        }
    }

    [Test]
    public async Task HandlePostAsync_RunsToolsCallWithoutBlocking()
    {
        using var mcp = new McpServer(18793);
        mcp.Tool("delayed", "Waits a moment.", async _ =>
        {
            await Task.Delay(20);
            return "done";
        });

        var result = await mcp.HandlePostAsync("""
            {"jsonrpc":"2.0","id":1,"method":"tools/call","params":{"name":"delayed","arguments":{}}}
            """);

        Assert.That(TextOf(result.JsonBody!), Is.EqualTo("done"));
    }

    private static string Initialize(McpServer mcp, string requestedVersion)
    {
        var result = mcp.HandlePost($$$"""
            {"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"{{{requestedVersion}}}"}}
            """);

        return JsonNode.Parse(result.JsonBody!)!["result"]!["protocolVersion"]!.GetValue<string>();
    }

    private static async Task<string> Post(System.Net.Http.HttpClient client, string url, string body)
    {
        var response = await client.PostAsync(
            url,
            new System.Net.Http.StringContent(body, System.Text.Encoding.UTF8, "application/json"));
        return await response.Content.ReadAsStringAsync();
    }

    private static string TextOf(string json)
        => JsonNode.Parse(json)!["result"]!["content"]![0]!["text"]!.GetValue<string>();
}
