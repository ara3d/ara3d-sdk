using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ara3D.MCP;

internal sealed class McpJsonRpcHandler
{
    public const string InitializedNotification = "notifications/initialized";

    private readonly McpToolRegistry _registry;
    private readonly string _protocolVersion;
    private readonly string _serverName;
    private readonly string _serverVersion;

    public McpJsonRpcHandler(
        McpToolRegistry registry,
        string protocolVersion,
        string serverName,
        string serverVersion)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _protocolVersion = protocolVersion;
        _serverName = serverName;
        _serverVersion = serverVersion;
        NegotiatedProtocolVersion = protocolVersion;
    }

    /// <summary>True once the client has completed the handshake with notifications/initialized.</summary>
    public bool ClientInitialized { get; private set; }

    /// <summary>The version agreed during initialize; the server's own until a client asks.</summary>
    public string NegotiatedProtocolVersion { get; private set; }

    public async Task<McpHttpResult> HandlePostAsync(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return ParseError("empty request body");

        JsonNode? message;
        try
        {
            message = JsonNode.Parse(body);
        }
        catch (JsonException ex)
        {
            return ParseError(ex.Message);
        }

        if (message is JsonArray batch)
            return await HandleBatchAsync(batch).ConfigureAwait(false);

        if (message is JsonObject obj)
            return await HandleMessageAsync(obj).ConfigureAwait(false);

        return McpHttpResult.Json(ErrorObject(null, -32600, "Invalid request: expected a JSON-RPC object or batch."));
    }

    private async Task<McpHttpResult> HandleBatchAsync(JsonArray batch)
    {
        var responses = new List<JsonObject>();

        foreach (var item in batch.OfType<JsonObject>())
        {
            if (!IsRequest(item))
            {
                HandleNotification(MethodOf(item));
                continue;
            }

            var response = await HandleRequestObjectAsync(item).ConfigureAwait(false);
            if (response != null)
                responses.Add(response);
        }

        if (responses.Count == 0)
            return McpHttpResult.Accepted();

        return responses.Count == 1
            ? McpHttpResult.Json(responses[0])
            : McpHttpResult.Json(new JsonArray(responses.Select(item => (JsonNode?)item).ToArray()));
    }

    private async Task<McpHttpResult> HandleMessageAsync(JsonObject message)
    {
        if (!IsRequest(message))
        {
            HandleNotification(MethodOf(message));
            return McpHttpResult.Accepted();
        }

        var response = await HandleRequestObjectAsync(message).ConfigureAwait(false);
        return response == null
            ? McpHttpResult.Accepted()
            : McpHttpResult.Json(response);
    }

    /// <summary>Notifications never get a response. The initialized notification completes the
    /// handshake; any other notification is accepted and ignored, as JSON-RPC requires.</summary>
    private void HandleNotification(string method)
    {
        if (method == InitializedNotification)
            ClientInitialized = true;
    }

    private async Task<JsonObject?> HandleRequestObjectAsync(JsonObject request)
    {
        var id = request["id"];
        var method = MethodOf(request);

        try
        {
            var result = method switch
            {
                "initialize" => HandleInitialize(request["params"] as JsonObject),
                "tools/list" => HandleToolsList(),
                "tools/call" => await HandleToolsCallAsync(request["params"] as JsonObject).ConfigureAwait(false),
                "ping" => new JsonObject(),
                InitializedNotification => HandleInitializedAsRequest(),
                _ => throw new McpProtocolException(-32601, $"Method not found: {method}"),
            };

            if (IsNull(id))
                return null;

            return new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id!.DeepClone(),
                ["result"] = result,
            };
        }
        catch (McpProtocolException ex)
        {
            return CreateError(id, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            return CreateError(id, -32603, ex.Message);
        }
    }

    private static JsonObject? CreateError(JsonNode? id, int code, string message)
        => IsNull(id) ? null : ErrorObject(id, code, message);

    private static JsonObject ErrorObject(JsonNode? id, int code, string message)
        => new()
        {
            ["jsonrpc"] = "2.0",
            ["id"] = IsNull(id) ? null : id!.DeepClone(),
            ["error"] = new JsonObject
            {
                ["code"] = code,
                ["message"] = message,
            },
        };

    private static McpHttpResult ParseError(string detail)
        => McpHttpResult.Json(ErrorObject(null, -32700, $"Parse error: {detail}"));

    private static bool IsNull(JsonNode? node)
        => node == null || node.GetValueKind() == JsonValueKind.Null;

    private static bool IsRequest(JsonObject message)
        => message["method"] != null && message["id"] != null;

    private static string MethodOf(JsonObject message)
        => AsString(message["method"]) ?? "";

    private static string? AsString(JsonNode? node)
        => node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;

    private JsonObject HandleInitializedAsRequest()
    {
        ClientInitialized = true;
        return new JsonObject();
    }

    private JsonObject HandleInitialize(JsonObject? parameters)
    {
        NegotiatedProtocolVersion = Negotiate(AsString(parameters?["protocolVersion"]));

        return new JsonObject
        {
            ["protocolVersion"] = NegotiatedProtocolVersion,
            ["capabilities"] = new JsonObject
            {
                ["tools"] = new JsonObject(),
            },
            ["serverInfo"] = new JsonObject
            {
                ["name"] = _serverName,
                ["version"] = _serverVersion,
            },
        };
    }

    /// <summary>Echoes the client's requested version when the server speaks it, otherwise answers
    /// with the server's own version and leaves the client to decide whether to continue.</summary>
    private string Negotiate(string? requested)
        => requested != null && McpServer.SupportedProtocolVersions.Contains(requested)
            ? requested
            : _protocolVersion;

    private JsonObject HandleToolsList()
        => new()
        {
            ["tools"] = new JsonArray(_registry.ListTools().Select(tool => (JsonNode?)tool).ToArray()),
        };

    private async Task<JsonObject> HandleToolsCallAsync(JsonObject? parameters)
    {
        var name = AsString(parameters?["name"]);
        var arguments = parameters?["arguments"] as JsonObject ?? new JsonObject();
        var text = await _registry.CallAsync(name, arguments, CancellationToken.None).ConfigureAwait(false);
        return ToolResult(text, ReportsFailure(text));
    }

    /// <summary>A handler that went through <see cref="ToolRunner"/> reports failure inside its
    /// payload, via the <see cref="McpToolResult"/> "ok" flag. That has to be lifted onto the
    /// protocol's isError, or a client sees a failed tool call as a successful one. Handlers that
    /// return arbitrary text carry no "ok" field and are unaffected.</summary>
    private static bool ReportsFailure(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            return JsonNode.Parse(text) is JsonObject envelope
                   && envelope["ok"] is JsonValue value
                   && value.TryGetValue<bool>(out var ok)
                   && !ok;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static JsonObject ToolResult(string text, bool isError = false)
        => new()
        {
            ["content"] = new JsonArray
            {
                new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = text,
                },
            },
            ["isError"] = isError,
        };
}
