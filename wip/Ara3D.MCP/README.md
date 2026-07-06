# Ara3D.MCP

Programmatic MCP server for .NET — runtime-mutable tools over localhost HTTP. Zero NuGet deps (`HttpListener` + `System.Text.Json`).

**Types:** `McpServer`, `McpSchema`, `McpToolArgs`, `ToolRunner`, `McpToolResult`, `McpJson`, `IUiThreadInvoker`

---

## Quick start — host your own server

```csharp
using Ara3D.MCP;

var mcp = new McpServer(
    port: McpServer.DefaultPort,          // 8766
    serverName: "my-app",
    serverVersion: "1.0.0",
    host: "127.0.0.1");

mcp.Tool("ping_app", "Returns app status.", () => "ok");
mcp.Start();                              // listens at mcp.Url → http://127.0.0.1:8766/mcp
// mcp.Stop(); mcp.Dispose();
```

Test without HTTP:

```csharp
var result = mcp.HandlePost("""{"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}""");
// result.StatusCode == 200, result.JsonBody has tools array
```

---

## Registering tools

`McpServer.Tool(...)` is fluent — each call returns `this`. Handlers return `Task<string>` (plain text or JSON).

```csharp
// No arguments
mcp.Tool("hello", "Says hello.", (_, ct) => Task.FromResult("hello"));

// Sync sugar
mcp.Tool("version", "App version.", () => "1.0.0");

// With schema + typed args
mcp.Tool(
    "export",
    "Export geometry to a file.",
    McpSchema.Object()
        .String("filePath", "Absolute output path.", required: true)
        .String("format", "glb | bfast | bos")
        .Integer("quality", "0–100 compression level")
        .Build(),
    async (args, ct) =>
    {
        var path = args.GetRequiredString("filePath");
        var format = args.GetString("format") ?? "glb";
        return await ToolRunner.RunAsync(() => DoExport(path, format));
    });
```

### `McpSchema`

```csharp
McpSchema.None()                              // empty object schema
McpSchema.Object()
    .String(name, description, required: false)
    .Number(name, description, required: false)
    .Integer(name, description, required: false)
    .Boolean(name, description, required: false)
    .Build()                                  // JsonObject → pass to Tool(...)
```

### `McpToolArgs`

| Method | Behavior |
|--------|----------|
| `GetString(name)` | `null` if missing |
| `GetRequiredString(name)` | throws `McpProtocolException(-32602)` if missing/blank |
| `GetInt(name)` / `GetRequiredInt(name)` | nullable / required |
| `GetNumber(name)` | nullable `double` |
| `GetBool(name)` | nullable `bool` |

Missing required args surface as JSON-RPC errors on `tools/call` (not tool text).

---

## `ToolRunner` result envelope

Use inside tool handlers to return structured JSON (camelCase via `McpJson`):

```csharp
// Success
await ToolRunner.RunAsync(() => new { count = 3, names = new[] { "a", "b" } });
// → {"ok":true,"data":{"count":3,"names":["a","b"]}}

// Success + suggested follow-up tools
await ToolRunner.RunAsync(
    () => new { added = "Cube" },
    nextRecommendedTools: ["list_scene", "fit_view"]);
// → {"ok":true,"data":{"added":"Cube"},"nextRecommendedTools":["list_scene","fit_view"]}

// Failure (exceptions caught)
await ToolRunner.RunAsync(() => throw new InvalidOperationException("boom"));
// → {"ok":false,"error":"boom","type":"InvalidOperationException"}
```

`McpToolResult` fields: `ok`, `data`, `error`, `type` (exception name, failures only), `nextRecommendedTools` (omitted when empty).

Sync overloads: `RunAsync(Func<object>)`, `RunAsync(Func<object>, nextRecommendedTools)`.

---

## Runtime add / remove

```csharp
mcp.Tool("temp_tool", "Temporary.", () => "x");
mcp.RemoveTool("temp_tool");   // bool — false if name not registered
```

`tools/list` reflects the current registry immediately (no restart).

---

## HTTP endpoint

| | |
|---|---|
| URL | `http://127.0.0.1:8766/mcp` (`McpServer.DefaultPort`, `McpServer.McpPath`) |
| Method | **POST** only (GET → 405) |
| Body | JSON-RPC 2.0 object or batch array |
| Response | `200` + JSON body, `202` for notifications, `400` bad body, `403` bad Origin, `404` wrong path |

Origin header (if present) must be `localhost` or `127.0.0.1`.

Helper for Cursor config:

```csharp
McpServer.CursorConfigJson("my-server", port: 8766);
```

---

## JSON-RPC methods

All requests need `"jsonrpc":"2.0"` and `"id"` (notifications without `id` → HTTP 202, no body).

### `initialize`

```json
{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}
```

```json
{"jsonrpc":"2.0","id":1,"result":{
  "protocolVersion":"2025-03-26",
  "capabilities":{"tools":{}},
  "serverInfo":{"name":"my-app","version":"1.0.0"}
}}
```

### `tools/list`

```json
{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}
```

```json
{"jsonrpc":"2.0","id":2,"result":{"tools":[
  {"name":"hello","description":"Says hello.","inputSchema":{"type":"object","properties":{}}}
]}}
```

### `tools/call`

```json
{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{
  "name":"export",
  "arguments":{"filePath":"C:/out.glb"}
}}
```

```json
{"jsonrpc":"2.0","id":3,"result":{
  "content":[{"type":"text","text":"{\"ok\":true,\"data\":{...}}"}],
  "isError":false
}}
```

Handler return value → `result.content[0].text`. Unknown tool / bad args → JSON-RPC `error` object (not `isError`).

### `ping`

```json
{"jsonrpc":"2.0","id":4,"method":"ping","params":{}}
```

```json
{"jsonrpc":"2.0","id":4,"result":{}}
```

---

## Studio integration (reference consumer)

Ara3D Studio hosts MCP at `http://127.0.0.1:8766/mcp` via `StudioMcpService`:

```csharp
_mcp = new McpServer(Port, StudioMcpTools.ServerName, StudioMcpTools.ServerVersion);
StudioMcpTools.Register(_mcp, adapter);           // built-in scene/script meta-tools
ScriptMcpTools.Refresh(_mcp, pluginService);        // per-script invoke tools (script_{snake_case})
_mcp.Start();
```

- **`StudioMcpTools.Register`** — registers `studio_document_info`, `list_scene`, `fit_view`, `export_geometry`, `list_scripts`, `list_generators`, `list_modifiers`, `describe_script`, `add_generator`, `add_modifier_to_selection`, `set_selected_node_property`, `rebuild_scene`.
- **`ScriptMcpTools.Refresh`** — removes all prior `script_*` tools, re-registers from loaded plugin scripts. Called on plugin load / recompilation.

Pattern for dynamic tools: `RemoveTool` stale names → `Tool(...)` new entries (see `ScriptMcpTools.Refresh`).

---

## Cursor `mcp.json`

```json
{
  "mcpServers": {
    "ara3d-studio": {
      "url": "http://127.0.0.1:8766/mcp"
    }
  }
}
```

Or generate: `StudioMcpHttpServer.CursorConfigJson()` / `McpServer.CursorConfigJson("ara3d-studio")`.

After Studio starts, check logs for `Studio MCP server started at http://127.0.0.1:8766/mcp`.
