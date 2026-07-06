using System.Text.Json.Nodes;

namespace Ara3D.MCP;

/// <summary>Programmatic MCP server with runtime-mutable tools over localhost HTTP.</summary>
public sealed class McpServer : IDisposable
{
    public const int DefaultPort = 8766;
    public const string McpPath = "/mcp";
    public const string ProtocolVersion = "2025-03-26";

    private readonly McpToolRegistry _registry = new();
    private readonly McpJsonRpcHandler _jsonRpc;
    private readonly int _port;
    private readonly string _host;
    private McpHttpListener? _http;

    public McpServer(
        int port = DefaultPort,
        string serverName = "mcp-server",
        string serverVersion = "1.0.0",
        string host = "127.0.0.1")
    {
        _port = port;
        _host = host;
        ServerName = serverName;
        ServerVersion = serverVersion;
        _jsonRpc = new McpJsonRpcHandler(_registry, ProtocolVersion, serverName, serverVersion);
    }

    public string ServerName { get; }

    public string ServerVersion { get; }

    public string Url => $"http://{_host}:{_port}{McpPath}";

    public bool Active => _http?.Active ?? false;

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

    public McpHttpResult HandlePost(string body)
        => _jsonRpc.HandlePost(body);

    public void Start()
    {
        if (_http is { Active: true })
            return;

        _http?.Dispose();
        _http = new McpHttpListener(_jsonRpc, _port, _host, McpPath);
        _http.Start();
    }

    public void Stop()
    {
        _http?.Dispose();
        _http = null;
    }

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
