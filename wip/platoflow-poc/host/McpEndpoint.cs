using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PlatoFlow.Host;

/// <summary>A hand-rolled MCP server over JSON-RPC 2.0: <c>initialize</c>, <c>tools/list</c>,
/// <c>tools/call</c>, <c>ping</c>. The full <c>wip/Ara3D.MCP</c> stack owns its own HttpListener and
/// keeps its JSON-RPC handler internal, so borrowing it would mean a second port; three methods on
/// the port we already have is less machinery than adapting it.
///
/// Every mutating tool enqueues an intent rather than editing anything: the browser's reducer stays
/// the only thing that changes a graph, which is what keeps agent edits and mouse edits identical.</summary>
public sealed class McpEndpoint(ModelCatalog catalog, AgentBridge bridge, AskService ask)
{
    private const string ProtocolVersion = "2025-06-18";

    public void Handle(HttpListenerContext ctx, string body)
    {
        if (!OriginAllowed(ctx.Request.Headers["Origin"]))
        {
            HostApi.WriteJson(ctx, Error(null, -32600, "Origin not allowed"), 403);
            return;
        }

        JsonNode? message;
        try
        {
            message = JsonNode.Parse(body);
        }
        catch (JsonException ex)
        {
            HostApi.WriteJson(ctx, Error(null, -32700, $"Parse error: {ex.Message}"));
            return;
        }

        if (message is JsonArray batch)
        {
            var responses = new JsonArray();
            foreach (var item in batch.OfType<JsonObject>())
                if (Dispatch(item) is { } response)
                    responses.Add(response);

            HostApi.WriteJson(ctx, responses);
            return;
        }

        if (message is not JsonObject request)
        {
            HostApi.WriteJson(ctx, Error(null, -32600, "Expected a JSON-RPC object or batch."));
            return;
        }

        var single = Dispatch(request);
        if (single == null)
        {
            // Notification: nothing to answer.
            ctx.Response.StatusCode = 202;
            HostApi.WriteJson(ctx, new JsonObject { ["ok"] = true }, 202);
            return;
        }

        HostApi.WriteJson(ctx, single);
    }

    /// <summary>Only local callers. An absent Origin is a non-browser client (an MCP CLI, curl) and
    /// is allowed; a browser-supplied Origin has to be loopback.</summary>
    private static bool OriginAllowed(string? origin)
        => string.IsNullOrEmpty(origin)
           || (Uri.TryCreate(origin, UriKind.Absolute, out var uri)
               && uri.Host is "localhost" or "127.0.0.1" or "::1" or "[::1]");

    private JsonObject? Dispatch(JsonObject request)
    {
        var id = request["id"];
        var method = request["method"]?.GetValue<string>() ?? "";
        if (id == null || id.GetValueKind() == JsonValueKind.Null)
            return null;

        try
        {
            JsonNode result = method switch
            {
                "initialize" => Initialize(),
                "tools/list" => ToolList(),
                "tools/call" => CallTool(request["params"] as JsonObject),
                "ping" => new JsonObject(),
                "notifications/initialized" => new JsonObject(),
                _ => throw new InvalidOperationException($"Method not found: {method}"),
            };

            return new JsonObject { ["jsonrpc"] = "2.0", ["id"] = id.DeepClone(), ["result"] = result };
        }
        catch (Exception ex)
        {
            return Error(id, -32603, $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static JsonObject Initialize()
        => new()
        {
            ["protocolVersion"] = ProtocolVersion,
            ["capabilities"] = new JsonObject { ["tools"] = new JsonObject() },
            ["serverInfo"] = new JsonObject { ["name"] = "platoflow-poc", ["version"] = "0.1.0" },
        };

    // ---- tools ----

    private JsonObject ToolList()
    {
        var tools = new JsonArray
        {
            Tool("list_node_kinds",
                "Lists every node kind the editor knows: id, category, input/output slots and parameter schemas. "
                + "Read this before calling add_node so the kind and parameter names are real.",
                Schema()),
            Tool("get_graph",
                "Returns the graph document the browser last pushed: nodes, edges and the display node.",
                Schema()),
            Tool("add_node",
                "Adds a node to the graph and returns its generated id. The node appears in the editor "
                + "within about 250ms, when the browser next polls for intents.",
                Schema(
                    ("kind", "string", "Node kind id from list_node_kinds, e.g. 'table.sql'.", true),
                    ("x", "number", "Canvas x position. Defaults to a free-ish spot.", false),
                    ("y", "number", "Canvas y position.", false),
                    ("params", "object", "Initial parameter values, keyed by parameter name.", false))),
            Tool("connect",
                "Wires one node's output slot to another node's input slot. Slot names and types come from list_node_kinds.",
                Schema(
                    ("fromNode", "string", "Source node id.", true),
                    ("fromSlot", "string", "Source output slot name, e.g. 'out'.", true),
                    ("toNode", "string", "Target node id.", true),
                    ("toSlot", "string", "Target input slot name, e.g. 'in'.", true))),
            Tool("set_param",
                "Sets one parameter on one node.",
                Schema(
                    ("node", "string", "Node id.", true),
                    ("name", "string", "Parameter name from that node's kind.", true),
                    ("value", null, "New value; type follows the parameter schema.", true))),
            Tool("set_display",
                "Flags which node the 3D viewer and panes show. Pass no node to clear the flag.",
                Schema(("node", "string", "Node id to display, or omit to clear.", false))),
            Tool("load_graph",
                "Replaces the whole graph with a document ({name, nodes, edges, display}).",
                Schema(("doc", "object", "A complete GraphDoc.", true))),
            Tool("read_node",
                "Reads the last evaluation result for one node: status, summary, and the first 50 rows if it produced a table.",
                Schema(("node", "string", "Node id.", true))),
            Tool("sql",
                "Runs one read-only SELECT/WITH statement against a model's BIM data and returns {columns, rows}. "
                + "Useful views: EntityText, ParameterText, RelationText.",
                Schema(
                    ("model", "string", "Model id from /api/models, e.g. 'duplex'.", true),
                    ("sql", "string", "A single SELECT or WITH statement.", true))),
            Tool("ask",
                "Turns a natural-language question about a model into one read-only DuckDB SELECT "
                + "statement via an LLM. Returns {sql} (run it with the sql tool) or {error}.",
                Schema(
                    ("model", "string", "Model id from /api/models, e.g. 'duplex'.", true),
                    ("question", "string", "The question to translate into SQL.", true))),
        };

        return new JsonObject { ["tools"] = tools };
    }

    private JsonObject CallTool(JsonObject? parameters)
    {
        var name = parameters?["name"]?.GetValue<string>() ?? "";
        var args = parameters?["arguments"] as JsonObject ?? [];

        try
        {
            return TextResult(Invoke(name, args), isError: false);
        }
        catch (Exception ex)
        {
            return TextResult(
                new JsonObject { ["error"] = $"{ex.GetType().Name}: {ex.Message}" },
                isError: true);
        }
    }

    private JsonNode Invoke(string name, JsonObject args) => name switch
    {
        "list_node_kinds" => Kinds(),
        "get_graph" => bridge.Doc?.DeepClone() ?? new JsonObject { ["message"] = "No graph pushed yet." },
        "add_node" => AddNode(args),
        "connect" => Enqueue(new JsonObject
        {
            ["t"] = "connect",
            ["from"] = Slot(Required(args, "fromNode"), Required(args, "fromSlot")),
            ["to"] = Slot(Required(args, "toNode"), Required(args, "toSlot")),
        }),
        "set_param" => Enqueue(new JsonObject
        {
            ["t"] = "setParam",
            ["node"] = Required(args, "node"),
            ["name"] = Required(args, "name"),
            ["value"] = args["value"]?.DeepClone(),
        }),
        "set_display" => Enqueue(new JsonObject
        {
            ["t"] = "setDisplay",
            ["node"] = args["node"]?.DeepClone(),
        }),
        "load_graph" => Enqueue(new JsonObject
        {
            ["t"] = "load",
            ["doc"] = (args["doc"] ?? throw new ArgumentException("'doc' is required")).DeepClone(),
        }),
        "read_node" => bridge.ReadNode(Required(args, "node")),
        "sql" => catalog.Query(args["model"]?.GetValue<string>(), args["sql"]?.GetValue<string>()),
        "ask" => ask.Ask(args["model"]?.GetValue<string>(), args["question"]?.GetValue<string>()),
        _ => throw new ArgumentException($"Unknown tool '{name}'"),
    };

    private JsonNode Kinds()
    {
        var file = Path.Combine(catalog.DataDir, "kinds.json");
        return File.Exists(file) ? JsonNode.Parse(File.ReadAllText(file))! : new JsonArray();
    }

    private JsonObject AddNode(JsonObject args)
    {
        var id = bridge.NextNodeId();
        var seq = bridge.Enqueue(new JsonObject
        {
            ["t"] = "addNode",
            ["id"] = id,
            ["kind"] = Required(args, "kind"),
            ["x"] = args["x"]?.GetValue<double>() ?? 80,
            ["y"] = args["y"]?.GetValue<double>() ?? 80,
            ["params"] = (args["params"] as JsonObject)?.DeepClone(),
        });

        return new JsonObject { ["id"] = id, ["seq"] = seq };
    }

    private JsonObject Enqueue(JsonObject intent)
        => new() { ["ok"] = true, ["seq"] = bridge.Enqueue(intent), ["intent"] = intent.DeepClone() };

    private static JsonObject Slot(string node, string slot)
        => new() { ["node"] = node, ["slot"] = slot };

    private static string Required(JsonObject args, string name)
        => args[name]?.GetValue<string>() ?? throw new ArgumentException($"'{name}' is required");

    // ---- schema helpers ----

    private static JsonObject Tool(string name, string description, JsonObject schema)
        => new()
        {
            ["name"] = name,
            ["description"] = description,
            ["inputSchema"] = schema,
        };

    /// <summary>A minimal but valid JSON Schema object. A null type means "any JSON value", which is
    /// what <c>set_param</c> needs.</summary>
    private static JsonObject Schema(params (string Name, string? Type, string Description, bool Required)[] fields)
    {
        var properties = new JsonObject();
        var required = new JsonArray();
        foreach (var (name, type, description, isRequired) in fields)
        {
            var property = new JsonObject { ["description"] = description };
            if (type != null)
                property["type"] = type;
            properties[name] = property;
            if (isRequired)
                required.Add(name);
        }

        var schema = new JsonObject { ["type"] = "object", ["properties"] = properties };
        if (required.Count > 0)
            schema["required"] = required;
        return schema;
    }

    private static JsonObject TextResult(JsonNode payload, bool isError)
        => new()
        {
            ["content"] = new JsonArray
            {
                new JsonObject { ["type"] = "text", ["text"] = payload.ToJsonString() },
            },
            ["isError"] = isError,
        };

    private static JsonObject Error(JsonNode? id, int code, string message)
        => new()
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id?.DeepClone(),
            ["error"] = new JsonObject { ["code"] = code, ["message"] = message },
        };
}
