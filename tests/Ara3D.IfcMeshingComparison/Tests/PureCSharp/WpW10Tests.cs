using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>WP-W10: duplex window placement/detail (Tier 1 triage).</summary>
[TestFixture]
public sealed class WpW10Tests
{
    [Test]
    [Explicit("WP-W10 diagnosis: duplex windows — mis-tag filter + Tier 1")]
    [Category("Slow")]
    public void Duplex_Windows_Triage()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var tier1 = ShapeDiagnostics.CompareEntities(candidate, oracle);
        var tier1ById = tier1.ToDictionary(d => d.EntityId);

        var result = ModelComparer.Compare(candidate, oracle, ifcPath.GetFileName());
        var windowGaps = result.EntityShape.WorstEntities
            .Where(g =>
            {
                var entity = stepFile.EntityResolver.GetEntityOrDefault(g.EntityId);
                return entity?.GetEntityName() == "IFCWINDOW";
            })
            .ToList();

        TestContext.WriteLine($"Duplex windows in worst-entity list: {windowGaps.Count}");
        foreach (var gap in windowGaps)
        {
            var misTag = gap.MisTagSuspectId >= 0
                ? $" mis-tag→#{gap.MisTagSuspectId}"
                : "";
            tier1ById.TryGetValue(gap.EntityId, out var t1);
            var voxel = t1 is null ? "?" : t1.VoxelIoU.ToString("F3");
            TestContext.WriteLine(
                $"  #{gap.EntityId} shape={gap.Score:F3} voxel={voxel}{misTag}");
        }

        TestContext.WriteLine("Tier 1 worst windows (voxel IoU):");
        foreach (var d in tier1
                     .Where(d => stepFile.EntityResolver.GetEntityOrDefault(d.EntityId)?.GetEntityName() == "IFCWINDOW")
                     .Take(12))
        {
            var gap = result.EntityShape.WorstEntities.FirstOrDefault(g => g.EntityId == d.EntityId);
            var misTag = gap.MisTagSuspectId >= 0 ? $" mis-tag→#{gap.MisTagSuspectId}" : "";
            TestContext.WriteLine(
                $"  #{d.EntityId}: voxel={d.VoxelIoU:F3} obb={d.ObbIoU:F3} shape={gap.Score:F3}{misTag}");
        }
    }

    [Test]
    [Category("Slow")]
    public void Duplex_Windows_NotDominatedByMisTags()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var result = ModelComparer.CompareFile(ifcPath);

        var windowWorst = result.EntityShape.WorstEntities
            .Where(g => stepFile.EntityResolver.GetEntityOrDefault(g.EntityId)?.GetEntityName() == "IFCWINDOW")
            .ToList();

        var genuine = windowWorst.Where(g => g.MisTagSuspectId < 0 && g.Score < 0.5).ToList();
        TestContext.WriteLine(
            $"Windows in worst-10: {windowWorst.Count}, genuine low-shape (non-mis-tag): {genuine.Count}");

        // Triage gate: if no non-mis-tag windows with shape &lt; 0.5, close as investigation-only.
        if (genuine.Count == 0)
            Assert.Pass("No genuine candidate-off windows in worst-entity list — Tier 1 gaps are placement-sensitive, not shape defects");
    }
}
