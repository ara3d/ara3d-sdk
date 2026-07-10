using Ara3D.Geometry;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class TransformComparisonTests
{
    static FilePath MesherBfastPath(FilePath ifcPath)
        => ifcPath.ChangeExtension(".bfast");

    [Test]
    [Explicit("Meshes schependomlaan and compares instance transforms to web-ifc oracle")]
    [Category("Slow")]
    public void Schependomlaan_TransformDeltas_vsOracle()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var bfastOraclePath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastOraclePath.Exists())
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);

        TestContext.WriteLine(TransformComparison.FormatSummary("schependomlaan: mesher vs web-ifc oracle", summary));

        var modelResult = ModelComparer.Compare(candidate, oracle, ifcPath.GetFileName());
        TestContext.WriteLine(ModelComparer.FormatResult(modelResult));
        TestContext.WriteLine(
            $"entity bbox matched {modelResult.EntityBoundingBox.MatchedCount}/{modelResult.EntityBoundingBox.ComparedCount}, " +
            $"merged bounds close={modelResult.MergedMesh.BoundsClose}");

        var tier1 = ShapeDiagnostics.CompareEntities(candidate, oracle);
        TestContext.WriteLine("Worst placement-sensitive shape agreement (Tier 1 voxel IoU):");
        foreach (var d in tier1.Take(10))
            TestContext.WriteLine(
                $"  #{d.EntityId}: voxel={d.VoxelIoU:F3} obb={d.ObbIoU:F3} silXY={d.SilhouetteXY:F3}");

        Assert.That(summary.PairedInstanceCount, Is.GreaterThan(0), "expected paired instances");
        TestContext.WriteLine(
            $"schependomlaan transform headline: meanCenterΔ={summary.MeanBoundsCenterDistance:F4}, " +
            $"maxCenterΔ={summary.MaxBoundsCenterDistance:F4}, meanFro={summary.MeanFrobenius:F4}");
    }

    [Test]
    [Explicit("Round-trips schependomlaan through BFAST and checks transform fidelity")]
    [Category("Slow")]
    public void Schependomlaan_BfastRoundTrip_PreservesTransforms()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var backend = new Approach1Backend();
        var result = backend.Build(ifcPath);
        Assert.That(result.Success, Is.True, string.Join("; ", result.Errors));
        var model = result.Model!;

        var roundTrip = TransformComparison.CompareToBfastRoundTrip(model, MesherBfastPath(ifcPath));
        TestContext.WriteLine(TransformComparison.FormatSummary("schependomlaan: in-memory vs BFAST round-trip", roundTrip));

        var onDiskPath = MesherBfastPath(ifcPath);
        if (onDiskPath.Exists())
        {
            var loaded = TransformComparison.LoadBfastModel(onDiskPath);
            var onDisk = TransformComparison.Compare(model, loaded);
            TestContext.WriteLine(TransformComparison.FormatSummary("schependomlaan: in-memory vs on-disk mesher BFAST", onDisk));
            Assert.That(onDisk.MaxFrobenius, Is.LessThan(1e-4),
                "on-disk mesher BFAST should preserve instance transforms");
            Assert.That(onDisk.MaxBoundsCenterDistance, Is.LessThan(1e-4),
                "on-disk mesher BFAST should preserve world placement");
        }

        Assert.That(roundTrip.MaxFrobenius, Is.LessThan(1e-4), "BFAST I/O should preserve transforms");
        Assert.That(roundTrip.MaxBoundsCenterDistance, Is.LessThan(1e-4), "BFAST I/O should preserve placement");
    }

    [Test]
    [Category("IfcMesherScore")]
    public void Duplex_Control_TransformsMostlyMatchOracle()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);

        var bfastOraclePath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastOraclePath.Exists())
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);

        TestContext.WriteLine(TransformComparison.FormatSummary("duplex control: mesher vs web-ifc oracle", summary));

        var candStats = OracleComparison.ComputeStats(candidate);
        var oracleStats = OracleComparison.ComputeStats(oracle);
        var refBounds = candStats.Bounds.Merge(oracleStats.Bounds);
        var refLen = (refBounds.Max - refBounds.Min).Length();
        var relCenter = summary.MeanBoundsCenterDistance / Math.Max(refLen, 1e-3f);
        TestContext.WriteLine(
            $"duplex relative mean center delta: {relCenter:F4} (refLen={refLen:F2}, " +
            $"meanCenterΔ={summary.MeanBoundsCenterDistance:F4})");

        Assert.That(summary.PairedInstanceCount, Is.GreaterThan(0));
        Assert.That(summary.MeanBoundsCenterDistance, Is.LessThan(2.0),
            "duplex control mean world-space center offset should stay modest");
    }

    [Test]
    public void MatrixDelta_IdentityIsZero()
    {
        var delta = TransformComparison.CompareMatrices(Matrix4x4.Identity, Matrix4x4.Identity);
        Assert.That(delta.Frobenius, Is.EqualTo(0).Within(1e-6));
        Assert.That(delta.TranslationDistance, Is.EqualTo(0).Within(1e-6));
        Assert.That(delta.RotationAngleDeg, Is.EqualTo(0).Within(1e-3));
    }

    [Test]
    public void MatrixDelta_TranslationOnly()
    {
        var a = Matrix4x4.Identity;
        var b = Matrix4x4.CreateTranslation(new Vector3(3, 4, 5));
        var delta = TransformComparison.CompareMatrices(a, b);
        Assert.That(delta.TranslationDistance, Is.EqualTo(Math.Sqrt(50)).Within(1e-4));
        Assert.That(delta.RotationAngleDeg, Is.EqualTo(0).Within(1e-3));
    }
}
