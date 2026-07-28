using System.Diagnostics;
using System.Text.Json;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>The regression guard for the stdio outage class of bug: anything the server does that
/// consumes or blocks its own stdin (a spawned child inheriting the handle, most famously) is
/// invisible to in-process tests, because in-process tests never make stdin the protocol stream.
/// These tests launch the published server the way an MCP client does and require an answer.</summary>
[TestFixture]
public class StdioEndToEndTests
{
    private static readonly TimeSpan HandshakeTimeout = TimeSpan.FromSeconds(20);
    private static readonly TimeSpan CallTimeout = TimeSpan.FromSeconds(10);

    [Test]
    public void Stdio_Initialize_Answers()
    {
        using var server = new StdioServerProcess(Array.Empty<string>());
        var response = Initialize(server);
        AssertEnvelope(response, 1, server);
        Assert.That(response!.Value.TryGetProperty("result", out _), Is.True,
            $"initialize returned no result. stderr:\n{server.Stderr}");
    }

    [Test]
    public void Stdio_ToolsCall_AnswersWhileStdinStaysOpen()
    {
        using var server = new StdioServerProcess(Array.Empty<string>());
        AssertEnvelope(Initialize(server), 1, server);

        var watch = Stopwatch.StartNew();
        server.Send(new
        {
            jsonrpc = "2.0",
            id = 2,
            method = "tools/call",
            @params = new { name = "ifc_models", arguments = new { } }
        });
        var response = server.ReadResponse(2, CallTimeout);
        watch.Stop();

        AssertEnvelope(response, 2, server);
        Assert.That(watch.Elapsed, Is.LessThan(CallTimeout));
    }

    /// <summary>A second call proves the pump survives the first one; the original outage left the
    /// server able to answer nothing at all, but a half-consumed stdin would show up here.</summary>
    [Test]
    public void Stdio_TwoCalls_BothAnswer()
    {
        using var server = new StdioServerProcess(Array.Empty<string>());
        AssertEnvelope(Initialize(server), 1, server);

        for (var id = 2; id <= 3; id++)
        {
            server.Send(new { jsonrpc = "2.0", id, method = "tools/list" });
            AssertEnvelope(server.ReadResponse(id, CallTimeout), id, server);
        }
    }

    private static JsonElement? Initialize(StdioServerProcess server)
    {
        server.Send(new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "initialize",
            @params = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new { },
                clientInfo = new { name = "Ara3D.Ifc.Mcp.Tests", version = "1.0" }
            }
        });
        return server.ReadResponse(1, HandshakeTimeout);
    }

    /// <summary>Asserts only what every protocol revision must keep: a reply arrived, it is
    /// JSON-RPC 2.0, it carries the request id, and it holds exactly one of result or error.</summary>
    private static void AssertEnvelope(JsonElement? response, int id, StdioServerProcess server)
    {
        Assert.That(response, Is.Not.Null,
            $"No JSON-RPC response with id {id} arrived before the timeout. stderr:\n{server.Stderr}");

        var element = response!.Value;
        Assert.That(element.GetProperty("jsonrpc").GetString(), Is.EqualTo("2.0"));
        Assert.That(element.GetProperty("id").GetInt32(), Is.EqualTo(id));

        var hasResult = element.TryGetProperty("result", out _);
        var hasError = element.TryGetProperty("error", out _);
        Assert.That(hasResult ^ hasError, Is.True, $"Malformed envelope: {element}");
    }
}
