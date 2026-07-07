using Ara3D.BimOpenSchema.IO;
using Ara3D.DataTable;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema.Harmonizer.Tests;

public static class LegacyFormatProbe
{
    [Test, Explicit, Category("Diagnostic")]
    public static async Task DumpLegacyParameterTables()
    {
        var dataSet = await HarmonizerTests.RevitBosFile.ReadParquetFromZipAsync();
        foreach (var t in dataSet.Tables)
        {
            if (!t.Name.Contains("Parameters"))
                continue;
            Console.WriteLine($"=== {t.Name}: {t.Rows.Count} rows ===");
            Console.WriteLine("Columns: " + string.Join(", ",
                t.Columns.Select(c => $"{c.Descriptor.Name}:{c.Descriptor.Type.Name}")));
            for (var i = 0; i < Math.Min(3, t.Rows.Count); ++i)
                Console.WriteLine("  " + string.Join(" | ",
                    Enumerable.Range(0, t.Columns.Count).Select(c => t[c, i]?.ToString() ?? "null")));
        }
    }
}
