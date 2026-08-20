using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace PlatoFlow.Host;

/// <summary>The single HTTP surface: files, SQL, pset writes, the agent bridge and MCP. Everything
/// answers JSON with permissive CORS, because the only client is a vite dev server on another port
/// and a PoC that argues with the browser about origins is a PoC that does not get demoed.</summary>
public sealed class HostApi
{
    private readonly AskService _ask;
    private readonly McpEndpoint _mcp;

    public HostApi(ModelCatalog catalog, AgentBridge bridge)
    {
        Catalog = catalog;
        Bridge = bridge;
        _ask = new AskService(catalog);
        _mcp = new McpEndpoint(catalog, bridge, _ask);
    }

    public ModelCatalog Catalog { get; }
    public AgentBridge Bridge { get; }

    public void Handle(HttpListenerContext ctx)
    {
        Cors(ctx.Response);

        if (ctx.Request.HttpMethod == "OPTIONS")
        {
            ctx.Response.StatusCode = 204;
            return;
        }

        var path = ctx.Request.Url?.AbsolutePath ?? "/";
        try
        {
            Route(ctx, path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[api] {path} failed: {ex.GetType().Name}: {ex.Message}");
            WriteJson(ctx, new JsonObject { ["error"] = $"{ex.GetType().Name}: {ex.Message}" });
        }
    }

    /// <summary>Errors come back as HTTP 200 with an <c>error</c> field rather than a status code.
    /// The browser then has one code path for "the host answered" and never has to special-case a
    /// failed fetch to show the user what went wrong.</summary>
    private void Route(HttpListenerContext ctx, string path)
    {
        switch (path)
        {
            case "/api/models":
                WriteJson(ctx, Models());
                return;

            case "/api/file":
                ServeFile(ctx, ctx.Request.QueryString["path"]);
                return;

            case "/api/kinds":
                Kinds(ctx);
                return;

            case "/api/sql":
                var query = ReadJson(ctx);
                WriteJson(ctx, Catalog.Query(
                    query?["model"]?.GetValue<string>(),
                    query?["sql"]?.GetValue<string>()));
                return;

            case "/api/ask":
                var ask = ReadJson(ctx);
                WriteJson(ctx, _ask.Ask(
                    ask?["model"]?.GetValue<string>(),
                    ask?["question"]?.GetValue<string>()));
                return;

            case "/api/append-psets":
                WriteJson(ctx, PsetWriter.Append(Catalog, ReadJson(ctx)));
                return;

            case "/api/graphs":
                if (ctx.Request.HttpMethod == "POST")
                    WriteJson(ctx, GraphStore.Save(Catalog, ReadJson(ctx)));
                else
                    WriteJson(ctx, GraphStore.List(Catalog));
                return;

            case "/api/graph":
                WriteJson(ctx, GraphStore.Load(Catalog, ctx.Request.QueryString["name"]));
                return;

            case "/api/export-csv":
                WriteJson(ctx, GraphStore.ExportCsv(Catalog, ReadJson(ctx)));
                return;

            case "/api/intents":
                WriteJson(ctx, Bridge.Since(ParseInt(ctx.Request.QueryString["since"])));
                return;

            case "/api/state":
                var state = ReadJson(ctx);
                Bridge.SetState(state?["doc"], state?["results"]);
                WriteJson(ctx, new JsonObject { ["ok"] = true });
                return;

            case "/mcp":
                _mcp.Handle(ctx, ReadBody(ctx));
                return;

            case "/":
            case "/api/health":
                WriteJson(ctx, new JsonObject
                {
                    ["ok"] = true,
                    ["service"] = "platoflow-poc host",
                    ["root"] = Catalog.Root,
                    ["models"] = new JsonArray(Catalog.Models.Select(m => (JsonNode?)m.Id).ToArray()),
                });
                return;

            default:
                ctx.Response.StatusCode = 404;
                WriteJson(ctx, new JsonObject { ["error"] = $"No route {path}" }, 404);
                return;
        }
    }

    private JsonArray Models()
    {
        var file = Path.Combine(Catalog.DataDir, "models.json");
        return (File.Exists(file) ? JsonNode.Parse(File.ReadAllText(file)) as JsonArray : null) ?? [];
    }

    /// <summary>GET returns the node vocabulary; POST replaces it with the web app's own dump of
    /// <c>kinds.ts</c>, which keeps the MCP tool from drifting from what the editor actually renders.</summary>
    private void Kinds(HttpListenerContext ctx)
    {
        var file = Path.Combine(Catalog.DataDir, "kinds.json");
        if (ctx.Request.HttpMethod == "POST")
        {
            var body = ReadBody(ctx);
            if (JsonNode.Parse(body) is not JsonArray kinds)
                throw new ArgumentException("POST /api/kinds expects a JSON array of node kinds.");
            File.WriteAllText(file, kinds.ToJsonString());
            WriteJson(ctx, new JsonObject { ["ok"] = true, ["count"] = kinds.Count });
            return;
        }

        WriteJson(ctx, File.Exists(file) ? JsonNode.Parse(File.ReadAllText(file))! : new JsonArray());
    }

    /// <summary>Serves one file from <c>data/</c>. The sandbox is a full-path containment check
    /// after resolution, which rejects <c>..</c>, absolute paths and symlink-ish tricks in one test
    /// instead of trying to enumerate the bad spellings.</summary>
    private void ServeFile(HttpListenerContext ctx, string? relative)
    {
        if (string.IsNullOrWhiteSpace(relative))
        {
            WriteJson(ctx, new JsonObject { ["error"] = "path is required" });
            return;
        }

        var root = Path.GetFullPath(Catalog.DataDir) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(Catalog.DataDir, relative));
        if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            WriteJson(ctx, new JsonObject { ["error"] = $"path escapes the data root: {relative}" }, 403);
            return;
        }

        if (!File.Exists(full))
        {
            WriteJson(ctx, new JsonObject { ["error"] = $"not found: {relative}" }, 404);
            return;
        }

        var info = new FileInfo(full);
        ctx.Response.StatusCode = 200;
        ctx.Response.ContentType = ContentType(full);
        ctx.Response.ContentLength64 = info.Length;
        using var stream = File.OpenRead(full);
        stream.CopyTo(ctx.Response.OutputStream, 128 * 1024);
    }

    private static string ContentType(string path)
        => Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".json" => "application/json; charset=utf-8",
            ".csv" => "text/csv; charset=utf-8",
            ".ifc" => "text/plain; charset=utf-8",
            ".txt" or ".md" => "text/plain; charset=utf-8",
            _ => "application/octet-stream",
        };

    // ---- plumbing ----

    private static JsonNode? ReadJson(HttpListenerContext ctx)
    {
        var body = ReadBody(ctx);
        return string.IsNullOrWhiteSpace(body) ? null : JsonNode.Parse(body);
    }

    private static string ReadBody(HttpListenerContext ctx)
    {
        using var reader = new StreamReader(ctx.Request.InputStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static int ParseInt(string? text)
        => int.TryParse(text, out var value) ? value : 0;

    public static void Cors(HttpListenerResponse response)
    {
        response.AddHeader("Access-Control-Allow-Origin", "*");
        response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.AddHeader("Access-Control-Allow-Headers", "*");
        response.AddHeader("Access-Control-Max-Age", "86400");
    }

    public static void WriteJson(HttpListenerContext ctx, JsonNode node, int status = 200)
    {
        var bytes = Encoding.UTF8.GetBytes(node.ToJsonString());
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/json; charset=utf-8";
        ctx.Response.ContentLength64 = bytes.Length;
        ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
    }
}
