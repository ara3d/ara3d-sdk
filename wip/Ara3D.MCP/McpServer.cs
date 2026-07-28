using System.Text;
using System.Text.Json.Nodes;

namespace Ara3D.MCP;

/// <summary>Programmatic MCP server with runtime-mutable tools, over either localhost HTTP or
/// line-delimited JSON on stdin/stdout.</summary>
public sealed class McpServer : IDisposable
{
    public const int DefaultPort = 8766;
    public const string McpPath = "/mcp";
    public const string ProtocolVersion = "2025-03-26";

    /// <summary>Versions the server can speak, newest first. A client asking for one of these gets
    /// it back from initialize; anything else is answered with <see cref="ProtocolVersion"/>.</summary>
    public static readonly IReadOnlyList<string> SupportedProtocolVersions =
        ["2025-06-18", "2025-03-26", "2024-11-05"];

    private readonly McpToolRegistry _registry = new();
    private readonly McpJsonRpcHandler _jsonRpc;
    private readonly int _port;
    private readonly string _host;
    private readonly McpTransport _transport;
    private McpHttpListener? _http;
    private McpStdioTransport? _stdio;

    public McpServer(
        int port = DefaultPort,
        string serverName = "mcp-server",
        string serverVersion = "1.0.0",
        string host = "127.0.0.1",
        McpTransport transport = McpTransport.Http)
    {
        _port = port;
        _host = host;
        _transport = transport;
        ServerName = serverName;
        ServerVersion = serverVersion;
        _jsonRpc = new McpJsonRpcHandler(_registry, ProtocolVersion, serverName, serverVersion);
    }

    public string ServerName { get; }

    public string ServerVersion { get; }

    public McpTransport Transport => _transport;

    /// <summary>Null under the stdio transport, which has no address.</summary>
    public string? Url
        => _transport == McpTransport.Http ? $"http://{_host}:{_port}{McpPath}" : null;

    public bool Active => _http?.Active ?? _stdio?.Active ?? false;

    public McpServer Tool(string name, string description, Func<CancellationToken, Task<string>> handler)
        => Tool(name, description, McpSchema.None(), (_, ct) => handler(ct));

    public McpServer Tool(string name, string description, Func<string> handler)
        => Tool(name, description, (_, _) => Task.FromResult(handler()));

    public McpServer Tool(string name, string description, Func<McpToolArgs, CancellationToken, Task<string>> handler)
        => Tool(name, description, McpSchema.None(), handler);

    public McpServer Tool(
        string name,
        string description,
        JsonObject schema,
        Func<McpToolArgs, CancellationToken, Task<string>> handler)
    {
        _registry.Register(name, description, schema, handler);
        return this;
    }

    public bool RemoveTool(string name)
        => _registry.Remove(name);

    /// <summary>True once a client has sent notifications/initialized.</summary>
    public bool ClientInitialized => _jsonRpc.ClientInitialized;

    /// <summary>The protocol version agreed with the client during initialize.</summary>
    public string NegotiatedProtocolVersion => _jsonRpc.NegotiatedProtocolVersion;

    public Task<McpHttpResult> HandlePostAsync(string body)
        => _jsonRpc.HandlePostAsync(body);

    /// <summary>Blocking convenience for callers that are not async; the transports use
    /// <see cref="HandlePostAsync"/> so no listener thread waits on a tool.</summary>
    public McpHttpResult HandlePost(string body)
        => _jsonRpc.HandlePostAsync(body).GetAwaiter().GetResult();

    public void Start()
    {
        if (_transport == McpTransport.Stdio)
        {
            StartStdio(StandardInput(), StandardOutput());
            return;
        }

        if (_http is { Active: true })
            return;

        _http?.Dispose();
        _http = new McpHttpListener(_jsonRpc, _port, _host, McpPath);
        _http.Start();
    }

    /// <summary>Serves line-delimited JSON-RPC over the given streams. Exposed separately from
    /// <see cref="Start"/> so a host can drive the transport over pipes it owns.</summary>
    public void StartStdio(TextReader input, TextWriter output)
    {
        if (_stdio is { Active: true })
            return;

        _stdio?.Dispose();
        _stdio = new McpStdioTransport(HandlePostAsync, input, output);
        _stdio.Start();
    }

    /// <summary>Blocks until the stdio pump ends (its client closed stdin). Returns immediately
    /// under HTTP, which has no such end-of-stream signal.</summary>
    public void WaitForShutdown()
        => _stdio?.WaitForShutdown();

    public void Stop()
    {
        _http?.Dispose();
        _http = null;
        _stdio?.Dispose();
        _stdio = null;
    }

    private static TextReader StandardInput()
        => new StreamReader(Console.OpenStandardInput(), new UTF8Encoding(false));

    private static TextWriter StandardOutput()
        => new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)) { AutoFlush = false };

    public void Dispose()
        => Stop();

    public static string CursorConfigJson(string serverKey, int port = DefaultPort)
        => $$"""
{
  "mcpServers": {
    "{{serverKey}}": {
      "url": "http://127.0.0.1:{{port}}/mcp"
    }
  }
}
""";
}
