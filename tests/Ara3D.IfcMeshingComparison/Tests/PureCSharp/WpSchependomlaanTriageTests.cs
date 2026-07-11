using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>
/// Schependomlaan max-Δ triage: classify placement-matrix bugs vs shape-inflated center Δ,
/// and split entity-bbox % so the ~15% match rate is not read as pure transform failure.
/// </summary>
[TestFixture]
public sealed class WpSchependomlaanTriageTests
{
    [Test]
    public void ClassifyDelta_MatrixDisagree_IsPlacementMatrix()
    {
        var mesh = UnitBox();
        var matrix = new MatrixDelta(Frobenius: 1.0, TranslationDistance: 2.0, RotationAngleDeg: 0, ScaleRatioMax: 1);
        Assert.That(
            TransformComparison.ClassifyDelta(matrix, boundsCenterDistance: 2.0, mesh, mesh),
            Is.EqualTo(PlacementDeltaClass.PlacementMatrix));
    }

    [Test]
    public void ClassifyDelta_MatchingMatrix_LargeCenter_IsShapeInflated()
    {
        var mesh = UnitBox();
        // Rotation agrees and matrix translation is small — center Δ comes from shape/extent.
        var matrix = new MatrixDelta(Frobenius: 0.001, TranslationDistance: 0.01, RotationAngleDeg: 0.01, ScaleRatioMax: 1);
        Assert.That(
            TransformComparison.ClassifyDelta(matrix, boundsCenterDistance: 5.0, mesh, mesh),
            Is.EqualTo(PlacementDeltaClass.ShapeInflatedCenter));
    }

    [Test]
    public void ClassifyDelta_HighFrobenius_SmallCenter_IsMatched()
    {
        var mesh = UnitBox();
        // Baked vs unbaked frames: large Frobenius/translation in matrix, but world centers agree.
        var matrix = new MatrixDelta(Frobenius: 5.0, TranslationDistance: 3.0, RotationAngleDeg: 0.01, ScaleRatioMax: 1);
        Assert.That(
            TransformComparison.ClassifyDelta(matrix, boundsCenterDistance: 0.05, mesh, mesh),
            Is.EqualTo(PlacementDeltaClass.Matched));
    }

    [Test]
    [Explicit("Dump worst schependomlaan instances with placement vs shape classification")]
    [Category("Slow")]
    public void Schependomlaan_MaxDelta_TriageDump()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);
        var modelResult = ModelComparer.Compare(candidate, oracle, ifcPath.GetFileName());

        TestContext.WriteLine(TransformComparison.FormatSummary("schependomlaan triage", summary));
        TestContext.WriteLine(
            $"entity bbox matched {modelResult.EntityBoundingBox.MatchedCount}/{modelResult.EntityBoundingBox.ComparedCount} " +
            $"({100.0 * modelResult.EntityBoundingBox.MatchedCount / Math.Max(1, modelResult.EntityBoundingBox.ComparedCount):F1}%)");

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        TestContext.WriteLine("Worst-20 with IFC type:");
        foreach (var d in summary.WorstInstances)
        {
            var name = file.EntityResolver.GetEntityOrDefault(d.EntityId)?.GetEntityName() ?? "?";
            TestContext.WriteLine(
                $"  #{d.EntityId} {name}: class={d.Classification} centerΔ={d.BoundsCenterDistance:F3} " +
                $"fro={d.MatrixDelta.Frobenius:F3} trans={d.MatrixDelta.TranslationDistance:F3} " +
                $"rot={d.MatrixDelta.RotationAngleDeg:F2} tri={d.CandidateTriCount}/{d.OracleTriCount}");
        }
    }

    [Test]
    [Explicit("Gate: schependomlaan placement split — bbox % is not pure transform")]
    [Category("Slow")]
    public void Schependomlaan_PlacementVsShape_BboxSplit()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);
        var modelResult = ModelComparer.Compare(candidate, oracle, ifcPath.GetFileName());

        var bboxPct = 100.0 * modelResult.EntityBoundingBox.MatchedCount /
                      Math.Max(1, modelResult.EntityBoundingBox.ComparedCount);
        var placementShare = summary.PairedInstanceCount == 0
            ? 0
            : 100.0 * summary.PlacementMatrixCount / summary.PairedInstanceCount;
        var shapeShare = summary.PairedInstanceCount == 0
            ? 0
            : 100.0 * summary.ShapeInflatedCenterCount / summary.PairedInstanceCount;

        TestContext.WriteLine(
            $"bbox matched {bboxPct:F1}%; paired={summary.PairedInstanceCount}; " +
            $"placement-matrix={summary.PlacementMatrixCount} ({placementShare:F1}%); " +
            $"shape-inflated={summary.ShapeInflatedCenterCount} ({shapeShare:F1}%); " +
            $"matched={summary.MatchedPlacementCount}; " +
            $"meanCenterΔ={summary.MeanBoundsCenterDistance:F4} max={summary.MaxBoundsCenterDistance:F4}; " +
            $"placement-only max centerΔ={summary.MaxCenterDistancePlacementOnly:F4}; " +
            $"shape-inflated max centerΔ={summary.MaxCenterDistanceShapeInflated:F4}");

        Assert.That(modelResult.EntityBoundingBox.MatchedCount, Is.GreaterThanOrEqualTo(520));
        Assert.That(summary.MeanBoundsCenterDistance, Is.LessThan(0.65));
        // With baking-aware classification, pure matrix bugs should be a minority of paired instances.
        Assert.That(summary.MatchedPlacementCount + summary.ShapeInflatedCenterCount,
            Is.GreaterThan(summary.PairedInstanceCount / 3),
            "bbox % is not pure transform: many pairs should classify as matched or shape-inflated");
        Assert.That(summary.PlacementMatrixCount + summary.ShapeInflatedCenterCount + summary.MatchedPlacementCount,
            Is.EqualTo(summary.PairedInstanceCount));
    }

    static Ara3D.Geometry.TriangleMesh3D UnitBox()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,$,$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            """);
        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(3));
        Assert.That(mesh, Is.Not.Null);
        return mesh!.Value;
    }
}
