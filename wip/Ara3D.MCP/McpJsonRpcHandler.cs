using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ara3D.MCP;

internal sealed class McpJsonRpcHandler
{
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
    }

    public McpHttpResult HandlePost(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return McpHttpResult.BadRequest();

        JsonNode? message;
        try
        {
            message = JsonNode.Parse(body);
        }
        catch (JsonException)
        {
            return McpHttpResult.BadRequest();
        }

        if (message is JsonArray batch)
            return HandleBatch(batch);

        if (message is JsonObject obj)
            return HandleMessage(obj);

        return McpHttpResult.BadRequest();
    }

    private McpHttpResult HandleBatch(JsonArray batch)
    {
        var requests = batch
            .OfType<JsonObject>()
            .Where(IsRequest)
            .ToList();

        if (requests.Count == 0)
            return McpHttpResult.Accepted();

        var responses = requests
            .Select(HandleRequestObject)
            .Where(response => response != null)
            .Cast<JsonObject>()
            .ToList();

        if (responses.Count == 0)
            return McpHttpResult.Accepted();

        return responses.Count == 1
            ? McpHttpResult.Json(responses[0])
            : McpHttpResult.Json(new JsonArray(responses.Select(item => (JsonNode?)item).ToArray()));
    }

    private McpHttpResult HandleMessage(JsonObject message)
    {
        if (!IsRequest(message))
            return McpHttpResult.Accepted();

        var response = HandleRequestObject(message);
        return response == null
            ? McpHttpResult.Accepted()
            : McpHttpResult.Json(response);
    }

    private JsonObject? HandleRequestObject(JsonObject request)
    {
        var id = request["id"];
        var method = request["method"]?.GetValue<string>() ?? "";

        try
        {
            var result = method switch
            {
                "initialize" => HandleInitialize(),
                "tools/list" => HandleToolsList(),
                "tools/call" => HandleToolsCall(request["params"] as JsonObject),
                "ping" => new JsonObject(),
                _ => throw new McpProtocolException(-32601, $"Method not found: {method}"),
            };

            if (id == null || id.GetValueKind() == JsonValueKind.Null)
                return null;

            return new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id.DeepClone(),
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
    {
        if (id == null || id.GetValueKind() == JsonValueKind.Null)
            return null;

        return new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id.DeepClone(),
            ["error"] = new JsonObject
            {
                ["code"] = code,
                ["message"] = message,
            },
        };
    }

    private static bool IsRequest(JsonObject message)
        => message["method"] != null && message["id"] != null;

    private JsonObject HandleInitialize()
        => new()
        {
            ["protocolVersion"] = _protocolVersion,
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

    private JsonObject HandleToolsList()
        => new()
        {
            ["tools"] = new JsonArray(_registry.ListTools().Select(tool => (JsonNode?)tool).ToArray()),
        };

    private JsonObject HandleToolsCall(JsonObject? parameters)
    {
        var name = parameters?["name"]?.GetValue<string>();
        var arguments = parameters?["arguments"] as JsonObject ?? new JsonObject();
        var text = _registry.CallAsync(name, arguments, CancellationToken.None).GetAwaiter().GetResult();
        return ToolResult(text);
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
