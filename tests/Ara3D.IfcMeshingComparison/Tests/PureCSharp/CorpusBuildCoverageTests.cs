using System.Text.Json;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class CorpusBuildCoverageTests
{
    [Test]
    [Category("Slow")]
    public void Corpus_QuickFiles_NoUnsupportedForSupportedBacklogTypes()
    {
        var rows = ScanFiles(TestFiles.QuickComparisonFiles().ToList());
        WriteReport(rows, "geometry_coverage_quick.json");

        var supported = GeometryCreationBacklog.KnownItems
            .Where(i => i.Support == GeometryCreationSupport.Supported)
            .Select(i => i.EntityName)
            .ToHashSet(StringComparer.Ordinal);

        var violations = rows
            .Where(r => supported.Contains(r.Entity) && r.UnsupportedCount > 0)
            .Select(r => $"{r.Entity}: unsupported={r.UnsupportedCount}")
            .ToList();
        Assert.That(violations, Is.Empty,
            "Supported backlog types recorded Unsupported on quick files:\n" + string.Join("\n", violations));
    }

    [Test]
    [Category("Slow")]
    [Explicit("Full studio/data corpus geometry coverage scan")]
    public void Corpus_StudioData_WriteCoverageReport()
    {
        var files = TestFiles.StudioDataFiles().Concat(TestFiles.QuickComparisonFiles()).Distinct().ToList();
        Assume.That(files, Is.Not.Empty, "No IFC corpus files found");
        var rows = ScanFiles(files);
        WriteReport(rows, "geometry_coverage.json");
        Assert.That(rows, Is.Not.Empty);
    }

    static List<CoverageRow> ScanFiles(IReadOnlyList<FilePath> files)
    {
        var byEntity = new Dictionary<string, CoverageRow>(StringComparer.Ordinal);
        foreach (var path in files)
        {
            if (!path.Exists())
            {
                TestContext.WriteLine($"skip missing {path}");
                continue;
            }
            TestFiles.RequireExists(path);
            using var file = new IfcFile(path, includeGeometry: false);
            var (model, diagnostics) = ModelAssembler.BuildModel(file);
            TestContext.WriteLine($"{path.GetFileName()}: meshes={model.Meshes.Count} instances={model.Instances.Count}");

            foreach (var (entity, status) in diagnostics.EntityStatus)
            {
                if (!byEntity.TryGetValue(entity, out var row))
                {
                    row = new CoverageRow(entity, 0, 0, 0, []);
                    byEntity[entity] = row;
                }
                var count = diagnostics.EntityCounts.GetValueOrDefault(entity);
                row = row with
                {
                    CorpusCount = row.CorpusCount + count,
                    BuiltCount = row.BuiltCount + (status == GeometrySupportStatus.Unsupported ? 0 : count),
                    UnsupportedCount = row.UnsupportedCount + (status == GeometrySupportStatus.Unsupported ? count : 0),
                };
                if (status == GeometrySupportStatus.Unsupported && row.FailingSamples.Count < 5)
                    row = row with { FailingSamples = row.FailingSamples.Append(path.GetFileName()).Distinct().ToList() };
                byEntity[entity] = row;
            }
        }
        return byEntity.Values
            .OrderByDescending(r => r.CorpusCount)
            .ThenBy(r => r.Entity, StringComparer.Ordinal)
            .ToList();
    }

    static void WriteReport(IReadOnlyList<CoverageRow> rows, string fileName)
    {
        TestFiles.ReportsDir.Create();
        var path = TestFiles.ReportsDir.RelativeFile(fileName);
        var json = JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
        TestContext.WriteLine($"Wrote {path}");
    }

    public sealed record CoverageRow(
        string Entity,
        int CorpusCount,
        int BuiltCount,
        int UnsupportedCount,
        IReadOnlyList<string> FailingSamples);
}
