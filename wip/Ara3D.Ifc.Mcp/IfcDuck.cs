using System.Text;
using Ara3D.BimOpenSchema;
using Ara3D.Utils;
using DuckDB.NET.Data;

namespace Ara3D.Ifc.Mcp;

public readonly record struct IfcColumn(string Name, string Type);

public readonly record struct IfcTableInfo(string Table, long RowCount, IReadOnlyList<IfcColumn> Columns);

/// <summary>A slice of a query result. <paramref name="Total"/> is the row count of the unpaged
/// query, so a caller can tell a complete answer from a truncated one without a second call.</summary>
public readonly record struct IfcQueryResult(
    long Total,
    int Skip,
    int Count,
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyList<object?>> Rows);

/// <summary>Read-only SQL over the DuckDB database derived from a model's BOS conversion.</summary>
public static class IfcDuck
{
    /// <summary>Adds the views that make the database answerable. BIM Open Schema interns every
    /// string and every enum, so the raw tables are almost entirely integer indexes: a query
    /// against <c>Entities</c> alone can see no names at all. Each view resolves those indexes by
    /// joining on <c>rowid</c>, which is the row order the Parquet tables were written in.</summary>
    public static void CreateViews(FilePath database)
    {
        using var conn = Connect(database);

        Execute(conn, """
            CREATE OR REPLACE VIEW EntityText AS
            SELECT e.rowid AS EntityIndex, e.LocalId AS StepId, sg.Strings AS GlobalId,
                   sn.Strings AS Name, sc.Strings AS Category, st.Strings AS Type
            FROM Entities e
            LEFT JOIN Strings sg ON sg.rowid = e.GlobalId
            LEFT JOIN Strings sn ON sn.rowid = e.Name
            LEFT JOIN Entities ec ON ec.rowid = e.Category
            LEFT JOIN Strings sc ON sc.rowid = ec.Name
            LEFT JOIN Entities et ON et.rowid = e.Type
            LEFT JOIN Strings st ON st.rowid = et.Name
            """);

        Execute(conn, $"""
            CREATE OR REPLACE VIEW ParameterText AS
            SELECT p.Entity AS EntityIndex, dn.Strings AS Name, dg.Strings AS ParameterGroup,
                   du.Strings AS Units, {EnumCase("d.Type", Enum.GetValues<ParameterType>())} AS ValueType,
                   CASE d.Type
                       WHEN {(int)ParameterType.String} THEN sv.Strings
                       WHEN {(int)ParameterType.Number} THEN CAST(nv.Numbers AS VARCHAR)
                       WHEN {(int)ParameterType.Entity} THEN ev.Strings
                       ELSE CAST(p.Value AS VARCHAR)
                   END AS Value
            FROM Parameters p
            JOIN Descriptors d ON d.rowid = p.Descriptor
            LEFT JOIN Strings dn ON dn.rowid = d.Name
            LEFT JOIN Strings dg ON dg.rowid = d."Group"
            LEFT JOIN Strings du ON du.rowid = d.Units
            LEFT JOIN Strings sv ON sv.rowid = p.Value
            LEFT JOIN Numbers nv ON nv.rowid = p.Value
            LEFT JOIN Entities ee ON ee.rowid = p.Value
            LEFT JOIN Strings ev ON ev.rowid = ee.Name
            """);

        Execute(conn, $"""
            CREATE OR REPLACE VIEW RelationText AS
            SELECT r.EntityA AS EntityIndexA, an.Strings AS NameA,
                   r.EntityB AS EntityIndexB, bn.Strings AS NameB,
                   {EnumCase("r.RelationType", Enum.GetValues<RelationType>())} AS RelationType
            FROM Relations r
            LEFT JOIN Entities ea ON ea.rowid = r.EntityA
            LEFT JOIN Strings an ON an.rowid = ea.Name
            LEFT JOIN Entities eb ON eb.rowid = r.EntityB
            LEFT JOIN Strings bn ON bn.rowid = eb.Name
            """);
    }

    private static string EnumCase<T>(string column, IReadOnlyList<T> values) where T : struct, Enum
    {
        var sb = new StringBuilder("CASE ").Append(column);
        var seen = new HashSet<int>();
        foreach (var value in values)
        {
            var number = System.Convert.ToInt32(value);
            if (seen.Add(number))
                sb.Append($" WHEN {number} THEN '{value}'");
        }

        return sb.Append(" END").ToString();
    }

    public static IfcQueryResult Query(FilePath database, string sql, int skip, int take)
    {
        if (skip < 0)
            throw new ArgumentOutOfRangeException(nameof(skip), "skip cannot be negative.");
        if (take < 1)
            throw new ArgumentOutOfRangeException(nameof(take), "take must be at least 1.");

        var inner = ReadOnlyQuery(sql);
        using var conn = Connect(database);
        var total = Scalar(conn, $"SELECT count(*) FROM ({inner}) AS _q");
        var rows = Read(conn, $"SELECT * FROM ({inner}) AS _q LIMIT {take} OFFSET {skip}");
        return new IfcQueryResult(total, skip, rows.Rows.Count, rows.Columns, rows.Rows);
    }

    /// <summary>Writes the full, unpaged result of a query to a file. The format follows the
    /// output extension: <c>.parquet</c>, <c>.json</c>, anything else CSV with a header row.</summary>
    public static long Export(FilePath database, string sql, FilePath output)
    {
        var inner = ReadOnlyQuery(sql);
        var directory = Path.GetDirectoryName(output.FullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using var conn = Connect(database);
        var total = Scalar(conn, $"SELECT count(*) FROM ({inner}) AS _q");
        Execute(conn, $"COPY ({inner}) TO '{Escape(output.FullPath.Replace('\\', '/'))}' ({CopyOptions(output)})");
        return total;
    }

    public static IReadOnlyList<IfcTableInfo> Tables(FilePath database, string? only = null)
    {
        using var conn = Connect(database);
        var columns = Read(
            conn,
            "SELECT table_name, column_name, data_type FROM information_schema.columns "
            + "WHERE table_schema = 'main' ORDER BY table_name, ordinal_position");

        var grouped = new Dictionary<string, List<IfcColumn>>(StringComparer.OrdinalIgnoreCase);
        var order = new List<string>();
        foreach (var row in columns.Rows)
        {
            var table = row[0]?.ToString() ?? "";
            if (only != null && !table.Equals(only, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!grouped.TryGetValue(table, out var list))
            {
                grouped[table] = list = [];
                order.Add(table);
            }

            list.Add(new IfcColumn(row[1]?.ToString() ?? "", row[2]?.ToString() ?? ""));
        }

        if (only != null && order.Count == 0)
            throw new ArgumentException($"No table named '{only}'. Call ifc_table with no 'table' argument to list them.");

        var result = new IfcTableInfo[order.Count];
        for (var i = 0; i < result.Length; i++)
        {
            var name = order[i];
            result[i] = new IfcTableInfo(name, Scalar(conn, $"SELECT count(*) FROM \"{name}\""), grouped[name]);
        }

        return result;
    }

    /// <summary>Accepts a single read-only statement and returns it without its trailing semicolon.
    /// The database is a throwaway copy, but a query tool that can silently rewrite it would make
    /// every later answer in the session unexplainable.</summary>
    public static string ReadOnlyQuery(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            throw new ArgumentException("A SQL query is required.", nameof(sql));

        var trimmed = sql.Trim().TrimEnd(';').Trim();
        if (trimmed.Contains(';'))
            throw new ArgumentException("Only one statement is allowed per query.", nameof(sql));

        if (!StartsWithWord(trimmed, "select") && !StartsWithWord(trimmed, "with"))
            throw new ArgumentException("Only SELECT and WITH queries are allowed.", nameof(sql));

        return trimmed;
    }

    private static bool StartsWithWord(string text, string word)
        => text.StartsWith(word, StringComparison.OrdinalIgnoreCase)
           && (text.Length == word.Length || !char.IsLetterOrDigit(text[word.Length]));

    private static string CopyOptions(FilePath output)
        => Path.GetExtension(output.FullPath).ToLowerInvariant() switch
        {
            ".parquet" => "FORMAT PARQUET",
            ".json" => "FORMAT JSON",
            _ => "FORMAT CSV, HEADER",
        };

    private static DuckDBConnection Connect(FilePath database)
    {
        var conn = new DuckDBConnection($"DataSource={database.FullPath}");
        conn.Open();
        return conn;
    }

    private static long Scalar(DuckDBConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        return System.Convert.ToInt64(cmd.ExecuteScalar());
    }

    private static void Execute(DuckDBConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private static (IReadOnlyList<string> Columns, IReadOnlyList<IReadOnlyList<object?>> Rows) Read(
        DuckDBConnection conn,
        string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        var names = new string[reader.FieldCount];
        for (var i = 0; i < names.Length; i++)
            names[i] = reader.GetName(i);

        var rows = new List<IReadOnlyList<object?>>();
        while (reader.Read())
        {
            var row = new object?[names.Length];
            for (var i = 0; i < row.Length; i++)
                row[i] = reader.IsDBNull(i) ? null : Jsonable(reader.GetValue(i));
            rows.Add(row);
        }

        return (names, rows);
    }

    /// <summary>DuckDB hands back provider-specific values for several logical types; anything the
    /// JSON writer does not model natively is reported as its text form rather than as an object
    /// with the provider's internal field names.</summary>
    private static object? Jsonable(object value)
        => value switch
        {
            bool or string or byte or sbyte or short or ushort or int or uint
                or long or ulong or float or double or decimal or DateTime or Guid => value,
            byte[] bytes => $"{bytes.Length} bytes",
            _ => value.ToString(),
        };

    private static string Escape(string path)
        => path.Replace("'", "''");
}
