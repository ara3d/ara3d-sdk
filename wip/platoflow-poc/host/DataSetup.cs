using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Ara3D.BimOpenSchema.IO;
using Ara3D.Utils;

namespace PlatoFlow.Host;

/// <summary>Makes <c>data/</c> self-sufficient at startup: the BOS files the viewer loads, the
/// DuckDB databases SQL runs against, the carbon CSV, <c>models.json</c> and <c>kinds.json</c>.
/// Everything is idempotent — a second run copies and converts nothing.</summary>
public static class DataSetup
{
    public const string DuplexBos = "duplex.bos";
    public const string DuplexIfc = "duplex.ifc";
    public const string SnowdonBos = "snowdon.bos";
    public const string SnowdonIfc = "snowdon.ifc";
    public const string RacBos = "rac_basic.bos";
    public const string CarbonCsv = "carbon.csv";
    public const string FallbackCarbonCsv = "rac-carbon.csv";
    public const string UnitCostsCsv = "unit-costs.csv";

    public static ModelCatalog Prepare(string root)
    {
        var data = Path.Combine(root, "data");
        Directory.CreateDirectory(data);
        Directory.CreateDirectory(Path.Combine(data, "out"));

        var models = new List<ModelEntry>();

        // rac_basic first: it is a straight copy, and it is what the duplex fallback needs.
        if (CopyIfMissing(PocPaths.Source.RacBasicBos, Path.Combine(data, RacBos)))
            models.Add(new ModelEntry("rac_basic", "RAC Basic Sample Project 2025", RacBos, "rac_basic.duckdb", null));
        else
            Log($"MISSING source BOS {PocPaths.Source.RacBasicBos} — rac_basic will not be served");

        CopyIfMissing(PocPaths.Source.CarbonCsv, Path.Combine(data, CarbonCsv));

        var duplex = PrepareDuplex(data);
        if (duplex != null)
            models.Insert(0, duplex);

        // Snowdon goes FIRST in models.json (95MB IFC; first conversion takes several minutes).
        var snowdon = PrepareIfcModel(data,
            PocPaths.Source.SnowdonIfc, SnowdonIfc, SnowdonBos,
            "snowdon", "Snowdon Towers Architectural", "snowdon.duckdb");
        if (snowdon != null)
            models.Insert(0, snowdon);

        // Discipline models for the multi-model demo. Inserted right after snowdon, which
        // stays first in models.json (it is the load.model schema default).
        var insertAt = models.Count > 0 && models[0].Id == "snowdon" ? 1 : 0;
        var hvac = PrepareIfcModel(data,
            PocPaths.Source.SnowdonHvacIfc, "snowdon-hvac.ifc", "snowdon-hvac.bos",
            "snowdon-hvac", "Snowdon Towers HVAC", "snowdon-hvac.duckdb");
        if (hvac != null)
            models.Insert(insertAt++, hvac);
        var structural = PrepareIfcModel(data,
            PocPaths.Source.SnowdonStructIfc, "snowdon-struct.ifc", "snowdon-struct.bos",
            "snowdon-struct", "Snowdon Towers Structural", "snowdon-struct.duckdb");
        if (structural != null)
            models.Insert(insertAt, structural);

        var catalog = new ModelCatalog(root, models);
        foreach (var model in models)
            EnsureDatabase(catalog, model);

        if (duplex == null)
            WriteFallbackCarbon(catalog);

        WriteUnitCosts(catalog);

        WriteModelsJson(catalog);
        EnsureKindsJson(root, data);
        return catalog;
    }

    /// <summary>Copies the duplex IFC into <c>data/</c> (so pset writes never touch the source repo)
    /// and converts it to BOS. Conversion re-reads the file with geometry enabled through the native
    /// web-ifc DLL, so it is the one step that can genuinely fail; a failure is reported and the
    /// model is dropped rather than taking the server down.</summary>
    private static ModelEntry? PrepareDuplex(string data)
        => PrepareIfcModel(data,
            PocPaths.Source.DuplexIfc, DuplexIfc, DuplexBos,
            "duplex", "Duplex Apartment", "duplex.duckdb");

    /// <summary>Copies an external IFC into <c>data/</c> and converts it to BOS when the BOS is
    /// missing or stale, then applies the browser-loader compatibility patch. Returns null (and the
    /// model is dropped rather than taking the server down) if the source is missing or the
    /// conversion fails.</summary>
    private static ModelEntry? PrepareIfcModel(
        string data, string sourceIfc, string ifcFile, string bosFile, string id, string name, string dbFile)
    {
        var ifc = Path.Combine(data, ifcFile);
        if (!CopyIfMissing(sourceIfc, ifc))
        {
            Log($"MISSING source IFC {sourceIfc} — {id} unavailable");
            return null;
        }

        var bos = Path.Combine(data, bosFile);
        if (!File.Exists(bos) || File.GetLastWriteTimeUtc(bos) < File.GetLastWriteTimeUtc(ifc))
        {
            try
            {
                Log($"converting {ifcFile} to BOS (loads geometry; takes a while)...");
                var watch = Stopwatch.StartNew();
                Convert(new FilePath(ifc), new FilePath(bos));
                Log($"converted in {watch.Elapsed.TotalSeconds:F1}s, {new FileInfo(bos).Length / 1024} KB");
            }
            catch (Exception ex)
            {
                Log($"{id} IFC->BOS conversion FAILED: {ex.GetType().Name}: {ex.Message}");
                TryDelete(bos);
                return null;
            }
        }

        // The browser loader only reads the old split-parameter layout, and this converter writes the
        // new one. Runs before EnsureDatabase so the DuckDB database is built from the patched (and
        // type-normalised) archive.
        try
        {
            if (LegacyBosTables.Ensure(new FilePath(bos)))
                Log($"{bosFile} patched for the browser loader");
        }
        catch (Exception ex)
        {
            Log($"legacy BOS table patch FAILED: {ex.GetType().Name}: {ex.Message}");
        }

        return new ModelEntry(id, name, bosFile, dbFile, ifcFile);
    }

    /// <summary>Runs the converter directly instead of <c>IfcToBosConverter.Convert</c>, which never
    /// disposes the <c>IfcFile</c> it opens — a server cannot leak a pinned whole-file buffer and a
    /// native web-ifc model per conversion.</summary>
    private static void Convert(FilePath input, FilePath output)
    {
        var converter = new IfcToBosConverter(input);
        try
        {
            converter.SaveToBos(output);
        }
        finally
        {
            converter.IfcFile?.Dispose();
        }
    }

    private static void EnsureDatabase(ModelCatalog catalog, ModelEntry model)
    {
        var bos = catalog.PathOf(model.BosFile);
        var db = catalog.PathOf(model.DbFile);
        if (File.Exists(db) && File.GetLastWriteTimeUtc(db) >= File.GetLastWriteTimeUtc(bos))
            return;

        Log($"building DuckDB for {model.Id}...");
        var watch = Stopwatch.StartNew();
        Duck.BuildDatabase(new FilePath(bos), new FilePath(db));
        Log($"  {model.Id} database ready in {watch.Elapsed.TotalSeconds:F1}s");
    }

    /// <summary>Fallback when duplex is unavailable: a carbon table keyed by rac_basic GlobalIds so
    /// the CSV-join beat still has something to join. The numbers are synthetic but deterministic —
    /// derived from the GlobalId — so a demo looks the same on every machine.</summary>
    private static void WriteFallbackCarbon(ModelCatalog catalog)
    {
        var model = catalog.Find("rac_basic");
        if (model == null)
            return;

        try
        {
            var table = catalog.Query(
                "rac_basic",
                """
                SELECT GlobalId, Name, Category FROM EntityText
                WHERE GlobalId IS NOT NULL AND Category IS NOT NULL
                ORDER BY EntityIndex LIMIT 2000
                """);

            var sb = new StringBuilder("GlobalId,Name,Category,embodied_carbon\n");
            foreach (var row in table["rows"]!.AsArray())
            {
                var cells = row!.AsArray();
                var globalId = cells[0]!.GetValue<string>();
                sb.Append(globalId).Append(',')
                    .Append(Csv(cells[1]?.GetValue<string>() ?? "")).Append(',')
                    .Append(Csv(cells[2]?.GetValue<string>() ?? "")).Append(',')
                    .Append(SyntheticCarbon(globalId).ToString("F1")).Append('\n');
            }

            var path = catalog.PathOf(FallbackCarbonCsv);
            File.WriteAllText(path, sb.ToString());
            Log($"wrote fallback carbon CSV {FallbackCarbonCsv} ({table["rows"]!.AsArray().Count} rows)");
        }
        catch (Exception ex)
        {
            Log($"fallback carbon CSV failed: {ex.Message}");
        }
    }

    /// <summary>Rate table for the cost-estimate demo: synthetic per-category unit rates times a
    /// REAL quantity parameter (Volume/Area from the model's own psets) where one exists, else a
    /// flat per-instance rate. Deterministic, so the demo looks the same on every machine.</summary>
    private const string UnitCostSql =
        """
        WITH q AS (
          SELECT e.GlobalId, e.Category,
            MAX(CASE WHEN p.Name = 'Volume' AND p.ValueType = 'Number' THEN TRY_CAST(p.Value AS DOUBLE) END) AS vol,
            MAX(CASE WHEN p.Name = 'Area' AND p.ValueType = 'Number' THEN TRY_CAST(p.Value AS DOUBLE) END) AS area
          FROM EntityText e
          LEFT JOIN ParameterText p ON p.EntityIndex = e.EntityIndex
            AND p.Name IN ('Volume', 'Area') AND p.ValueType = 'Number'
          WHERE e.Category IN ('IFCWALL','IFCSLAB','IFCMEMBER','IFCPLATE','IFCCOLUMN','IFCDOOR','IFCWINDOW',
            'IFCCOVERING','IFCBUILDINGELEMENTPROXY','IFCFURNITURE','IFCLIGHTFIXTURE','IFCRAILING','IFCSTAIR',
            'IFCROOF','IFCCURTAINWALL')
            AND e.GlobalId IS NOT NULL
          GROUP BY e.GlobalId, e.Category)
        SELECT GlobalId, Category, ROUND(CASE Category
          WHEN 'IFCWALL' THEN COALESCE(vol * 4.5, area * 8.5, 900)
          WHEN 'IFCSLAB' THEN COALESCE(vol * 3.8, area * 9.5, 1200)
          WHEN 'IFCMEMBER' THEN COALESCE(vol * 210, 350)
          WHEN 'IFCPLATE' THEN COALESCE(vol * 160, 280)
          WHEN 'IFCCOLUMN' THEN COALESCE(vol * 24, 800)
          WHEN 'IFCDOOR' THEN COALESCE(area * 31, 950)
          WHEN 'IFCWINDOW' THEN COALESCE(area * 42, 700)
          WHEN 'IFCCOVERING' THEN COALESCE(area * 4.5, 300)
          WHEN 'IFCCURTAINWALL' THEN COALESCE(area * 65, 2000)
          WHEN 'IFCROOF' THEN COALESCE(area * 12, vol * 4, 2500)
          WHEN 'IFCSTAIR' THEN 4200
          WHEN 'IFCRAILING' THEN 620
          WHEN 'IFCFURNITURE' THEN 480
          WHEN 'IFCLIGHTFIXTURE' THEN 210
          ELSE COALESCE(vol * 3, area * 6, 400) END, 2) AS cost
        FROM q
        """;

    /// <summary>Self-provisions <c>data/unit-costs.csv</c> for the cost-estimate demo (same pattern
    /// as the rac_basic fallback carbon CSV: <c>data/*</c> is gitignored, so the file is derived
    /// from the model at startup when missing and a fresh clone just works).</summary>
    private static void WriteUnitCosts(ModelCatalog catalog)
    {
        if (catalog.Find("snowdon") == null)
            return;
        var path = catalog.PathOf(UnitCostsCsv);
        if (File.Exists(path))
            return;

        try
        {
            var table = catalog.Query("snowdon", UnitCostSql);
            var sb = new StringBuilder("GlobalId,Category,cost\n");
            var rows = table["rows"]!.AsArray();
            foreach (var row in rows)
            {
                var cells = row!.AsArray();
                sb.Append(cells[0]!.GetValue<string>()).Append(',')
                    .Append(Csv(cells[1]!.GetValue<string>())).Append(',')
                    .Append(cells[2]!.ToJsonString())   // ROUNDed number; JSON text is culture-safe
                    .Append('\n');
            }
            File.WriteAllText(path, sb.ToString());
            Log($"wrote {UnitCostsCsv} ({rows.Count} rows, costs from real Volume/Area quantities)");
        }
        catch (Exception ex)
        {
            Log($"unit-costs CSV failed: {ex.Message}");
        }
    }

    private static double SyntheticCarbon(string globalId)
    {
        var hash = 17;
        foreach (var c in globalId)
            hash = unchecked(hash * 31 + c);
        return 5.0 + Math.Abs(hash % 4000) / 10.0;
    }

    private static string Csv(string value)
        => value.Contains(',') || value.Contains('"')
            ? '"' + value.Replace("\"", "\"\"") + '"'
            : value;

    private static void WriteModelsJson(ModelCatalog catalog)
    {
        var array = new JsonArray();
        foreach (var model in catalog.Models)
            array.Add(model.ToJson(catalog.DataDir));

        File.WriteAllText(
            Path.Combine(catalog.DataDir, "models.json"),
            array.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Seeds <c>data/kinds.json</c> from the checked-in default. The web app may POST an
    /// authoritative dump of <c>kinds.ts</c> over it at startup; that copy then wins.</summary>
    private static void EnsureKindsJson(string root, string data)
    {
        var target = Path.Combine(data, "kinds.json");
        if (File.Exists(target))
            return;

        foreach (var candidate in new[]
                 {
                     Path.Combine(AppContext.BaseDirectory, "kinds.default.json"),
                     Path.Combine(root, "host", "kinds.default.json"),
                 })
        {
            if (!File.Exists(candidate))
                continue;
            File.Copy(candidate, target);
            return;
        }

        File.WriteAllText(target, "[]");
        Log("kinds.default.json not found — data/kinds.json seeded empty");
    }

    private static bool CopyIfMissing(string source, string target)
    {
        if (File.Exists(target))
            return true;
        if (!File.Exists(source))
            return false;

        File.Copy(source, target);
        Log($"copied {Path.GetFileName(target)} ({new FileInfo(target).Length / 1024} KB)");
        return true;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (IOException)
        {
        }
    }

    private static void Log(string message)
        => Console.WriteLine($"[data] {message}");
}
