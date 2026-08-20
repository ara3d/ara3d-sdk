using System.Text.Json.Nodes;
using Ara3D.BimOpenSchema.IO;
using Ara3D.Ifc.Mcp;
using Ara3D.Utils;
using DuckDB.NET.Data;

namespace PlatoFlow.Host;

/// <summary>One model the host can serve: a BOS file for the viewer, a DuckDB database for SQL,
/// and (when we have one) the source IFC that pset writes patch.</summary>
public sealed class ModelEntry(string id, string name, string bosFile, string dbFile, string? ifcFile)
{
    public string Id { get; } = id;
    public string Name { get; } = name;

    /// <summary>File name inside <c>data/</c>, not a path: everything the browser reads goes
    /// through <c>/api/file?path=</c>, which only accepts names under the data root.</summary>
    public string BosFile { get; } = bosFile;

    public string DbFile { get; } = dbFile;
    public string? IfcFile { get; } = ifcFile;

    public JsonObject ToJson(string dataDir)
        => new()
        {
            ["id"] = Id,
            ["name"] = Name,
            ["bosUrl"] = $"/api/file?path={Uri.EscapeDataString(BosFile)}",
            ["ifcPath"] = IfcFile == null ? null : Path.Combine(dataDir, IfcFile),
        };
}

/// <summary>The models plus their open DuckDB connections. Connections are opened on first query
/// and kept: opening costs a file handle and a catalog read, and the server answers one request at
/// a time, so a single long-lived connection per model is both cheapest and safe.</summary>
public sealed class ModelCatalog(string root, IReadOnlyList<ModelEntry> models) : IDisposable
{
    private readonly Dictionary<string, DuckDBConnection> _connections = new(StringComparer.OrdinalIgnoreCase);

    public string Root { get; } = root;
    public string DataDir { get; } = Path.Combine(root, "data");
    public string OutDir { get; } = Path.Combine(root, "data", "out");
    public IReadOnlyList<ModelEntry> Models { get; } = models;

    public ModelEntry? Find(string? id)
        => id == null ? null : Models.FirstOrDefault(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase));

    public ModelEntry Require(string? id)
        => Find(id) ?? throw new ArgumentException(
            $"No model '{id}'. Known models: {string.Join(", ", Models.Select(m => m.Id))}");

    public string PathOf(string fileName)
        => Path.Combine(DataDir, fileName);

    public DuckDBConnection Connection(ModelEntry model)
    {
        if (_connections.TryGetValue(model.Id, out var existing))
            return existing;

        var conn = new DuckDBConnection($"DataSource={PathOf(model.DbFile)}");
        conn.Open();
        _connections[model.Id] = conn;
        return conn;
    }

    /// <summary>A single read-only statement against one model, as <c>{columns, rows}</c>.
    /// <paramref name="cap"/> exists only to keep a runaway SELECT from filling memory; the PoC's
    /// tables are small enough that hitting it means the query was a mistake.</summary>
    public JsonObject Query(string? modelId, string? sql, int cap = 20000)
    {
        var model = Require(modelId);
        var statement = IfcDuck.ReadOnlyQuery(sql ?? "");
        return Duck.Read(Connection(model), statement, cap);
    }

    public void Dispose()
    {
        foreach (var conn in _connections.Values)
            conn.Dispose();
        _connections.Clear();
    }
}

/// <summary>Reads a DuckDB result straight into the wire shape the browser wants:
/// <c>{columns: string[], rows: Cell[][]}</c> with numbers as JSON numbers and NULL as null.</summary>
public static class Duck
{
    public static JsonObject Read(DuckDBConnection conn, string sql, int cap)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        var columns = new JsonArray();
        for (var i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i));

        var rows = new JsonArray();
        var truncated = false;
        while (reader.Read())
        {
            if (rows.Count >= cap)
            {
                truncated = true;
                break;
            }

            var row = new JsonArray();
            for (var i = 0; i < reader.FieldCount; i++)
                row.Add(reader.IsDBNull(i) ? null : Cell(reader.GetValue(i)));
            rows.Add(row);
        }

        var result = new JsonObject { ["columns"] = columns, ["rows"] = rows };
        if (truncated)
            result["truncatedAt"] = cap;
        return result;
    }

    /// <summary>DuckDB hands back provider types for several logical types. Anything JSON does not
    /// model natively becomes its text form rather than an object of provider-internal fields.</summary>
    private static JsonNode? Cell(object value)
        => value switch
        {
            bool b => JsonValue.Create(b),
            byte or sbyte or short or ushort or int or uint or long => JsonValue.Create(Convert.ToInt64(value)),
            ulong u => JsonValue.Create((double)u),
            float or double or decimal => JsonValue.Create(Convert.ToDouble(value)),
            string s => JsonValue.Create(s),
            byte[] bytes => JsonValue.Create($"{bytes.Length} bytes"),
            _ => JsonValue.Create(value.ToString()),
        };

    public static void Execute(DuckDBConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public static void BuildDatabase(FilePath bos, FilePath database)
    {
        if (File.Exists(database.FullPath))
            File.Delete(database.FullPath);
        bos.BosToDuckDB(database);

        try
        {
            IfcDuck.CreateViews(database);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[data] IfcDuck.CreateViews failed ({ex.Message.Split('\n')[0]}); using compatibility views");
            BosCompat.CreateViews(database);
        }
    }
}
