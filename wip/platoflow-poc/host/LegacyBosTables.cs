using System.IO.Compression;
using Ara3D.Utils;
using DuckDB.NET.Data;

namespace PlatoFlow.Host;

/// <summary>Adds the old split-parameter tables to a newly converted .bos so the browser loader can
/// read it, and normalises the descriptor type codes on the way through.
///
/// Two independent incompatibilities live here, both found by running the real files:
///
/// 1. <b>Layout.</b> The current <c>IfcToBosConverter</c> writes one <c>Parameters</c> table plus a
///    <c>Numbers</c> side table. <c>@ara3d/ara3d-webgl</c> 1.3.15 only reads the older split form —
///    <c>IntegerParameters</c>, <c>SingleParameters</c>, <c>StringParameters</c>,
///    <c>EntityParameters</c>, <c>PointParameters</c> — and throws
///    <c>Could not find "IntegerParameters.parquet" in zip archive</c> otherwise. The new tables are
///    kept (the host's own SQL views use them) and the five old ones are derived alongside.
///
/// 2. <b>Type codes.</b> <c>ParameterType</c> is <c>Int=0, Bool=0, Number=1, Entity=2, String=3,
///    Point=4</c>, and that is what the loader's <c>GetVal</c> assumes (3 indexes Strings, 2 is an
///    entity, anything else is the raw number). rac_basic, written by the old converter, matches it
///    exactly. duplex, written by the current one, is uniformly <b>one higher</b>: its Number
///    parameters are typed 2, Entity 3, String 4 — consistent with a writer whose enum gave
///    <c>Bool</c> its own value instead of aliasing it to <c>Int</c>. Left alone this mislabels every
///    parameter in the file and sends each value to the wrong lookup table, in the loader
///    <i>and</i> in <c>IfcDuck.ParameterText</c>. The shift is detected by checking each type code's
///    values against the row count of the table they would have to index, and folded out of
///    <c>Descriptors.parquet</c>, which fixes both readers at once.</summary>
public static class LegacyBosTables
{
    private const int TypeInt = 0;
    private const int TypeNumber = 1;
    private const int TypeEntity = 2;
    private const int TypeString = 3;
    private const int TypePoint = 4;

    private static readonly string[] LegacyTables =
        ["IntegerParameters", "SingleParameters", "StringParameters", "EntityParameters", "PointParameters"];

    /// <summary>Idempotent: a .bos that already carries the five tables is left untouched.
    /// Returns true when the file was rewritten.</summary>
    public static bool Ensure(FilePath bos)
    {
        if (AlreadyPatched(bos))
            return false;

        var work = Path.Combine(Path.GetTempPath(), "platoflow-bos", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(work);
        try
        {
            ZipFile.ExtractToDirectory(bos.FullPath, work);
            if (!File.Exists(Path.Combine(work, "Parameters.parquet")))
            {
                Console.WriteLine("[data]   no Parameters table; this .bos is already the split layout");
                return false;
            }

            using var conn = new DuckDBConnection("DataSource=:memory:");
            conn.Open();
            Load(conn, work, "Parameters", "Descriptors", "Numbers", "Strings", "Entities", "Points");

            var shift = DetectTypeShift(conn);
            Console.WriteLine($"[data]   descriptor type shift detected: {shift}");

            WriteLegacyTables(conn, work, shift);
            if (shift != 0)
                NormaliseDescriptors(conn, work, shift);

            Repack(bos, work, shift != 0);
            return true;
        }
        finally
        {
            TryDeleteDirectory(work);
        }
    }

    private static bool AlreadyPatched(FilePath bos)
    {
        using var zip = ZipFile.OpenRead(bos.FullPath);
        var names = zip.Entries.Select(e => e.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return LegacyTables.All(t => names.Contains(t + ".parquet"));
    }

    private static void Load(DuckDBConnection conn, string folder, params string[] tables)
    {
        foreach (var table in tables)
        {
            var file = Path.Combine(folder, table + ".parquet");
            if (!File.Exists(file))
                continue;

            // A real table, not a view over read_parquet: every join in BOS is on rowid, which only
            // exists once the rows have been materialised in parquet order.
            Duck.Execute(conn, $"CREATE TABLE {table} AS SELECT * FROM read_parquet('{Sql(file)}')");
        }
    }

    /// <summary>Finds how far the file's type codes are displaced from <c>ParameterType</c>, by
    /// testing the only thing that can be checked without trusting either side: a code that claims
    /// to be String must have values that are legal indices into Strings, Entity into Entities,
    /// Number into Numbers, Point into Points. The smallest shift under which every code is legal
    /// wins; Int/Bool holds its value inline and constrains nothing.</summary>
    private static int DetectTypeShift(DuckDBConnection conn)
    {
        var limits = new Dictionary<int, long>
        {
            [TypeNumber] = Count(conn, "Numbers"),
            [TypeEntity] = Count(conn, "Entities"),
            [TypeString] = Count(conn, "Strings"),
            [TypePoint] = Count(conn, "Points"),
        };

        var observed = MaxValuePerType(conn);

        for (var shift = 0; shift <= 1; shift++)
        {
            var ok = true;
            foreach (var (raw, max) in observed)
            {
                var canonical = raw - shift;
                if (canonical < TypeInt || canonical > TypePoint)
                {
                    ok = false;
                    break;
                }

                if (canonical != TypeInt && max >= limits[canonical])
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
                return shift;
        }

        Console.WriteLine("[data]   WARNING: no descriptor type shift validates; assuming the codes are canonical");
        return 0;
    }

    private static List<(int Type, long MaxValue)> MaxValuePerType(DuckDBConnection conn)
    {
        var result = new List<(int, long)>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT d.Type, max(p.Value)
            FROM Parameters p JOIN Descriptors d ON d.rowid = p.Descriptor
            GROUP BY 1
            """;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            result.Add((Convert.ToInt32(reader.GetValue(0)), Convert.ToInt64(reader.GetValue(1))));
        return result;
    }

    private static long Count(DuckDBConnection conn, string table)
    {
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT count(*) FROM {table}";
            return Convert.ToInt64(cmd.ExecuteScalar());
        }
        catch (DuckDBException)
        {
            return 0;
        }
    }

    /// <summary>One parquet file per legacy table. Only <c>SingleParameters</c> changes its values:
    /// the new layout stores a Number parameter as an index into <c>Numbers</c>, the old one stores
    /// the float itself. String, Entity and Point values are already indices into the same tables
    /// the old reader looks them up in, so they are copied straight across.</summary>
    private static void WriteLegacyTables(DuckDBConnection conn, string folder, int shift)
    {
        Export(conn, folder, "IntegerParameters", TypeInt + shift, "CAST(p.Value AS INTEGER)", null);
        Export(conn, folder, "SingleParameters", TypeNumber + shift, "CAST(nv.Numbers AS FLOAT)",
            "LEFT JOIN Numbers nv ON nv.rowid = p.Value");
        Export(conn, folder, "StringParameters", TypeString + shift, "CAST(p.Value AS INTEGER)", null);
        Export(conn, folder, "EntityParameters", TypeEntity + shift, "CAST(p.Value AS INTEGER)", null);
        Export(conn, folder, "PointParameters", TypePoint + shift, "CAST(p.Value AS INTEGER)", null);
    }

    private static void Export(
        DuckDBConnection conn,
        string folder,
        string table,
        int rawType,
        string valueExpression,
        string? extraJoin)
    {
        var select = $"""
            SELECT CAST(p.Entity AS INTEGER) AS Entity,
                   CAST(p.Descriptor AS INTEGER) AS Descriptor,
                   {valueExpression} AS Value
            FROM Parameters p
            JOIN Descriptors d ON d.rowid = p.Descriptor
            {extraJoin ?? ""}
            WHERE d.Type = {rawType}
            """;

        var output = Path.Combine(folder, table + ".parquet");
        // Uncompressed: hyparquet reads it without needing a codec registered, and these tables are
        // a few hundred KB at PoC scale.
        Duck.Execute(conn, $"COPY ({select}) TO '{Sql(output)}' (FORMAT PARQUET, COMPRESSION UNCOMPRESSED)");
        Console.WriteLine($"[data]   {table}: {Rows(conn, select)} rows");
    }

    private static long Rows(DuckDBConnection conn, string select)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT count(*) FROM ({select}) AS _q";
        return Convert.ToInt64(cmd.ExecuteScalar());
    }

    private static void NormaliseDescriptors(DuckDBConnection conn, string folder, int shift)
    {
        var output = Path.Combine(folder, "Descriptors.parquet");
        Duck.Execute(conn, $"""
            COPY (SELECT CAST(Name AS INTEGER) AS Name, CAST(Units AS INTEGER) AS Units,
                         CAST("Group" AS INTEGER) AS "Group", CAST(Type - {shift} AS INTEGER) AS Type
                  FROM Descriptors)
            TO '{Sql(output)}' (FORMAT PARQUET, COMPRESSION UNCOMPRESSED)
            """);
        Console.WriteLine($"[data]   Descriptors.Type normalised (-{shift}) to the ParameterType enum");
    }

    /// <summary>Rewrites the archive from the working folder. Rebuilding it wholesale rather than
    /// updating in place keeps entry order and naming uniform, which matters because the loader
    /// finds tables by matching the end of an entry name.</summary>
    private static void Repack(FilePath bos, string folder, bool descriptorsRewritten)
    {
        var temp = bos.FullPath + ".tmp";
        if (File.Exists(temp))
            File.Delete(temp);

        using (var archive = ZipFile.Open(temp, ZipArchiveMode.Create))
            foreach (var file in Directory.GetFiles(folder))
                archive.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);

        File.Delete(bos.FullPath);
        File.Move(temp, bos.FullPath);

        var note = descriptorsRewritten ? " (Descriptors rewritten)" : "";
        Console.WriteLine($"[data]   repacked {Path.GetFileName(bos.FullPath)} with legacy parameter tables{note}");
    }

    private static string Sql(string path)
        => path.Replace('\\', '/').Replace("'", "''");

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}
