using System.Text;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Reporting;

public static class MarkdownReportWriter
{
    public static FilePath WriteComparisonReport(ComparisonReport report)
    {
        TestFiles.ReportsDir.Create();
        var path = TestFiles.ReportsDir.RelativeFile($"comparison_{report.GeneratedUtc:yyyyMMdd_HHmmss}.md");
        System.IO.File.WriteAllText(path, FormatComparisonReport(report));
        return path;
    }

    public static FilePath WriteCapabilitiesReport(ComparisonReport report)
    {
        TestFiles.ReportsDir.Create();
        var path = TestFiles.ReportsDir.RelativeFile("capabilities.md");
        System.IO.File.WriteAllText(path, FormatCapabilitiesReport(report));
        return path;
    }

    public static string FormatComparisonReport(ComparisonReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# IFC Meshing Comparison Report");
        sb.AppendLine();
        sb.AppendLine($"Generated: {report.GeneratedUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();

        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine("Backends compared:");
        foreach (var backend in report.Backends)
            sb.AppendLine($"- **{backend.Name}**: {backend.Description}");
        sb.AppendLine();
        sb.AppendLine(
            "Note: `Tests/Native/IfcMeshingTests` exercises the same native web-ifc path as **WebIfcDll**. " +
            "**Approach1** is the pure C# modular mesher under evaluation.");
        sb.AppendLine();

        sb.AppendLine("## Per-File Results");
        sb.AppendLine();
        foreach (var file in report.IfcFiles)
        {
            var fileResults = report.Results.Where(r => r.IfcPath == file).ToList();
            if (fileResults.Count == 0)
                continue;

            sb.AppendLine($"### {file.GetFileName()}");
            sb.AppendLine();
            sb.AppendLine("| Backend | Success | ms | Instances | Meshes | Triangles | Volume | Errors |");
            sb.AppendLine("|---|---|---:|---:|---:|---:|---:|---|");
            foreach (var r in fileResults)
            {
                var errors = r.Errors.Count == 0 ? "" : string.Join("; ", r.Errors);
                sb.AppendLine(
                    $"| {r.BackendName} | {(r.Success ? "yes" : "no")} | {r.ElapsedMs} | " +
                    $"{r.InstanceCount} | {r.MeshCount} | {r.TriangleCount} | {r.SignedVolume:F1} | {errors} |");
            }
            sb.AppendLine();
        }

        sb.AppendLine("## Performance");
        sb.AppendLine();
        sb.AppendLine("| IFC file | " + string.Join(" | ", report.Backends.Select(b => $"{b.Name} (ms)")) + " |");
        sb.AppendLine("|---|" + string.Join("|", report.Backends.Select(_ => "---:")) + "|");
        foreach (var file in report.IfcFiles)
        {
            var cells = report.Backends.Select(backend =>
            {
                var r = report.Results.FirstOrDefault(x => x.IfcPath == file && x.BackendName == backend.Name);
                return r is null ? "-" : r.ElapsedMs.ToString();
            });
            sb.AppendLine($"| {file.GetFileName()} | {string.Join(" | ", cells)} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Capability Gaps");
        sb.AppendLine();
        sb.AppendLine(FormatCapabilityTable(report));
        return sb.ToString();
    }

    public static string FormatCapabilitiesReport(ComparisonReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# IFC Meshing Capability Matrix");
        sb.AppendLine();
        sb.AppendLine($"Generated: {report.GeneratedUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        sb.AppendLine(FormatCapabilityTable(report));
        return sb.ToString();
    }

    static string FormatCapabilityTable(ComparisonReport report)
    {
        var sb = new StringBuilder();
        var backendNames = report.Backends.Select(b => b.Name).ToList();
        sb.AppendLine("| Entity | Catalog count | " + string.Join(" | ", backendNames) + " |");
        sb.AppendLine("|---|---:|" + string.Join("|", backendNames.Select(_ => "---")) + "|");

        foreach (var row in report.Capabilities)
        {
            var cells = backendNames.Select(name =>
            {
                if (!row.BackendStatus.TryGetValue(name, out var status))
                    return "-";
                return status switch
                {
                    GeometrySupportStatus.Supported => "supported",
                    GeometrySupportStatus.Approximate => "approx",
                    GeometrySupportStatus.Unsupported => "unsupported",
                    _ => "-",
                };
            });
            sb.AppendLine($"| {row.EntityName} | {row.CatalogCount} | {string.Join(" | ", cells)} |");
        }
        return sb.ToString();
    }
}
