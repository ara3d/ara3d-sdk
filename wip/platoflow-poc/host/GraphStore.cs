using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PlatoFlow.Host;

/// <summary>Graph persistence plus CSV export (wave 9, design §7: "the graph JSON is the
/// product" — a folder of graph files IS the workflow library). Demo graphs ship in the repo's
/// <c>demo/</c>; user saves land in <c>data/graphs/</c> (gitignored, like everything in data).
/// Names are sanitized to <c>[A-Za-z0-9 _-]</c> so a request can never spell a path.</summary>
public static class GraphStore
{
    private static readonly JsonSerializerOptions Pretty = new() { WriteIndented = true };

    public static string GraphsDir(ModelCatalog catalog) => Path.Combine(catalog.DataDir, "graphs");
    public static string DemoDir(ModelCatalog catalog) => Path.Combine(catalog.Root, "demo");

    /// <summary>GET /api/graphs — every loadable graph by basename, demos and saves apart
    /// (the menu renders them as separate sections).</summary>
    public static JsonObject List(ModelCatalog catalog)
        => new()
        {
            ["demos"] = Names(DemoDir(catalog)),
            ["saved"] = Names(GraphsDir(catalog)),
        };

    /// <summary>GET /api/graph?name= — the stored graph JSON. A user save shadows a demo of
    /// the same name, which is what "save over the example I started from" should do.</summary>
    public static JsonNode Load(ModelCatalog catalog, string? name)
    {
        var clean = Sanitize(TrimExtension(name, ".json"));
        if (clean.Length == 0)
            return Error("name is required");

        foreach (var dir in new[] { GraphsDir(catalog), DemoDir(catalog) })
        {
            var file = Path.Combine(dir, clean + ".json");
            if (File.Exists(file))
                return JsonNode.Parse(File.ReadAllText(file))!;
        }
        return Error($"no graph \"{clean}\"");
    }

    /// <summary>POST /api/graphs {name, doc} — pretty-printed so a saved graph diffs and reads
    /// like the checked-in demos. Overwrite is deliberate: this is a save button, not an archive.</summary>
    public static JsonObject Save(ModelCatalog catalog, JsonNode? body)
    {
        var doc = body?["doc"];
        if (doc == null)
            return Error("doc is required");

        var clean = Sanitize(TrimExtension(body?["name"]?.GetValue<string>(), ".json"));
        if (clean.Length == 0)
            return Error("name is empty after sanitizing (allowed: letters, digits, space, - and _)");

        Directory.CreateDirectory(GraphsDir(catalog));
        File.WriteAllText(Path.Combine(GraphsDir(catalog), clean + ".json"), doc.ToJsonString(Pretty));
        return new JsonObject { ["ok"] = true, ["name"] = clean };
    }

    /// <summary>POST /api/export-csv {name, table:{columns, rows}} — the effectful half of
    /// sink.exportCsv (design §6: the node's evaluate stays pure; the Run button posts here).
    /// Always lands in <c>data/out/</c> with a forced <c>.csv</c> extension.</summary>
    public static JsonObject ExportCsv(ModelCatalog catalog, JsonNode? body)
    {
        if (body?["table"]?["columns"] is not JsonArray columns || body["table"]?["rows"] is not JsonArray rows)
            return Error("table with columns and rows is required");

        var clean = Sanitize(TrimExtension(body["name"]?.GetValue<string>(), ".csv"));
        if (clean.Length == 0)
            return Error("name is empty after sanitizing (allowed: letters, digits, space, - and _)");

        Directory.CreateDirectory(catalog.OutDir);
        var file = Path.Combine(catalog.OutDir, clean + ".csv");

        var sb = new StringBuilder();
        AppendRow(sb, columns);
        foreach (var row in rows)
            AppendRow(sb, row as JsonArray ?? []);
        File.WriteAllText(file, sb.ToString(), new UTF8Encoding(false));

        return new JsonObject { ["outPath"] = file, ["rows"] = rows.Count };
    }

    private static JsonArray Names(string dir)
    {
        var names = new JsonArray();
        if (!Directory.Exists(dir))
            return names;
        foreach (var file in Directory.GetFiles(dir, "*.json").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            names.Add(Path.GetFileNameWithoutExtension(file));
        return names;
    }

    private static void AppendRow(StringBuilder sb, JsonArray cells)
    {
        for (var i = 0; i < cells.Count; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append(Cell(cells[i]));
        }
        sb.Append('\n');
    }

    /// <summary>CSV cell rules: null is empty, numbers and booleans are their bare JSON text
    /// (culture-safe), strings are quoted — with doubled quotes — only when they contain a
    /// comma, quote or newline.</summary>
    private static string Cell(JsonNode? cell)
    {
        if (cell == null)
            return "";
        if (cell is JsonValue v && v.TryGetValue<string>(out var s))
            return s.IndexOfAny([',', '"', '\n', '\r']) >= 0
                ? '"' + s.Replace("\"", "\"\"") + '"'
                : s;
        return cell.ToJsonString();
    }

    /// <summary>Keeps only <c>[A-Za-z0-9 _-]</c>. Separators and dots simply vanish, so
    /// <c>../evil</c> collapses to the harmless <c>evil</c> and an all-junk name becomes
    /// empty — which callers report as an error.</summary>
    public static string Sanitize(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "";
        var sb = new StringBuilder(name.Length);
        foreach (var c in name)
            if (char.IsAsciiLetterOrDigit(c) || c is ' ' or '_' or '-')
                sb.Append(c);
        return sb.ToString().Trim();
    }

    /// <summary>A caller-supplied extension is stripped before sanitizing (dots would be eaten
    /// anyway: "export.csv" must become "export", not "exportcsv").</summary>
    private static string TrimExtension(string? name, string ext)
        => name != null && name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)
            ? name[..^ext.Length]
            : name ?? "";

    private static JsonObject Error(string message) => new() { ["error"] = message };
}
