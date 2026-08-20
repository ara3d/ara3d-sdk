using Ara3D.Utils;
using DuckDB.NET.Data;

namespace PlatoFlow.Host;

/// <summary>Views for BOS files that <c>IfcDuck.CreateViews</c> cannot read.
///
/// The two demo models turn out to use different BIM Open Schema layouts. What the current SDK
/// converter writes — and what <c>IfcDuck</c> expects — is one <c>Parameters</c> table with a
/// <c>Descriptors.Type</c> discriminator plus a <c>Numbers</c> side table. The rac_basic BOS in
/// ara3d-webgl instead splits parameters by value type into <c>SingleParameters</c>,
/// <c>IntegerParameters</c>, <c>StringParameters</c>, <c>EntityParameters</c> and
/// <c>PointParameters</c>, and has no <c>Numbers</c> table at all. Asking the first for the second's
/// tables is the "Table with name Parameters does not exist" error.
///
/// So this rebuilds the same three views over whatever the file actually contains, and drops any
/// piece that will not compile rather than failing the model. The joins mirror IfcDuck's: BOS
/// interns every string, so a raw table is nearly all integer indexes and only a <c>rowid</c> join
/// makes it readable.</summary>
public static class BosCompat
{
    public static void CreateViews(FilePath database)
    {
        using var conn = new DuckDBConnection($"DataSource={database.FullPath}");
        conn.Open();

        var tables = Tables(conn);

        TryCreate(conn, "EntityText", """
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

        TryCreate(conn, "RelationText", """
            SELECT r.EntityA AS EntityIndexA, an.Strings AS NameA,
                   r.EntityB AS EntityIndexB, bn.Strings AS NameB,
                   CAST(r.RelationType AS VARCHAR) AS RelationType
            FROM Relations r
            LEFT JOIN Entities ea ON ea.rowid = r.EntityA
            LEFT JOIN Strings an ON an.rowid = ea.Name
            LEFT JOIN Entities eb ON eb.rowid = r.EntityB
            LEFT JOIN Strings bn ON bn.rowid = eb.Name
            """);

        var branches = new List<string>();
        foreach (var (table, valueType, valueExpression, extraJoin) in ParameterSources())
        {
            if (!tables.Contains(table))
                continue;

            var branch = ParameterBranch(table, valueType, valueExpression, extraJoin);
            if (Compiles(conn, branch))
                branches.Add(branch);
            else
                Console.WriteLine($"[data]   skipping {table} (columns do not match the expected shape)");
        }

        if (branches.Count > 0)
            TryCreate(conn, "ParameterText", string.Join("\nUNION ALL\n", branches));
        else
            Console.WriteLine("[data]   no parameter tables recognised; ParameterText not created");
    }

    /// <summary>Each split parameter table, its reported ValueType, and how its Value column turns
    /// into text. PointParameters is deliberately absent: a 3D point has no useful text form and no
    /// node in the PoC asks for one.</summary>
    private static IEnumerable<(string Table, string ValueType, string ValueExpression, string ExtraJoin)> ParameterSources()
    {
        yield return ("SingleParameters", "Number", "CAST(p.Value AS VARCHAR)", "");
        yield return ("DoubleParameters", "Number", "CAST(p.Value AS VARCHAR)", "");
        yield return ("IntegerParameters", "Integer", "CAST(p.Value AS VARCHAR)", "");
        yield return ("StringParameters", "String", "sv.Strings",
            "LEFT JOIN Strings sv ON sv.rowid = p.Value");
        yield return ("EntityParameters", "Entity", "ev.Strings",
            "LEFT JOIN Entities ee ON ee.rowid = p.Value LEFT JOIN Strings ev ON ev.rowid = ee.Name");
    }

    private static string ParameterBranch(string table, string valueType, string valueExpression, string extraJoin)
        => $"""
            SELECT p.Entity AS EntityIndex, dn.Strings AS Name, dg.Strings AS ParameterGroup,
                   du.Strings AS Units, '{valueType}' AS ValueType, {valueExpression} AS Value
            FROM {table} p
            JOIN Descriptors d ON d.rowid = p.Descriptor
            LEFT JOIN Strings dn ON dn.rowid = d.Name
            LEFT JOIN Strings dg ON dg.rowid = d."Group"
            LEFT JOIN Strings du ON du.rowid = d.Units
            {extraJoin}
            """;

    private static HashSet<string> Tables(DuckDBConnection conn)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'main'";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            names.Add(reader.GetString(0));
        return names;
    }

    private static bool Compiles(DuckDBConnection conn, string select)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM ({select}) AS _probe LIMIT 0";
            using var reader = cmd.ExecuteReader();
            return true;
        }
        catch (DuckDBException)
        {
            return false;
        }
    }

    private static void TryCreate(DuckDBConnection conn, string name, string select)
    {
        try
        {
            Duck.Execute(conn, $"CREATE OR REPLACE VIEW {name} AS {select}");
            Console.WriteLine($"[data]   compatibility view {name} created");
        }
        catch (DuckDBException ex)
        {
            Console.WriteLine($"[data]   view {name} skipped: {ex.Message.Split('\n')[0]}");
        }
    }
}
