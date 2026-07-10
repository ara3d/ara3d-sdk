using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.IfcTypes;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class WpPlacementTests
{
    static void DumpEntityTransforms(int entityId, Model3D candidate, Model3D oracle)
    {
        var cand = candidate.Instances.Where(i => i.EntityIndex == entityId).ToList();
        var orac = oracle.Instances.Where(i => i.EntityIndex == entityId).ToList();
        TestContext.WriteLine($"Entity #{entityId}: candidate={cand.Count} oracle={orac.Count}");
        for (var i = 0; i < Math.Min(cand.Count, orac.Count); i++)
        {
            var cd = TransformComparison.CompareMatrices(cand[i].Matrix4x4, orac[i].Matrix4x4);
            TestContext.WriteLine(
                $"  [{i}] candT={FormatT(cand[i].Matrix4x4)} oracleT={FormatT(orac[i].Matrix4x4)} " +
                $"tri={candidate.Meshes[cand[i].MeshIndex].FaceIndices.Count} " +
                $"fro={cd.Frobenius:F3} trans={cd.TranslationDistance:F3} rot={cd.RotationAngleDeg:F2}");
        }
    }

    static string FormatT(Matrix4x4 m) => $"({m.M41:F3},{m.M42:F3},{m.M43:F3})";

    [Test]
    [Explicit("Diagnose placement for worst schependomlaan entities")]
    public void Schependomlaan_WorstEntities_PlacementDiagnosis()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        foreach (var id in new[] { 44513, 771571, 27461, 452074 })
            DumpEntityTransforms(id, candidate, oracle);

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var ctx = new MeshingContext(file);
        var entity = ctx.GetEntity(44513);
        var placement = MeshHelpers.ResolveRequired(ctx, entity, IfcProduct.Instance.ObjectPlacement);
        var productFrame = Placements.ReadLocalPlacement(ctx, placement);
        TestContext.WriteLine($"#44513 product placement origin: {productFrame.Origin}");

        var rep = MeshHelpers.ResolveRequired(ctx, entity, IfcProduct.Instance.Representation);
        var parts = new List<CollectedPart>();
        GeometryPartCollector.CollectParts(ctx, rep, Matrix4x4.Identity, 44513, parts);
        foreach (var (part, idx) in parts.Select((p, i) => (p, i)))
            TestContext.WriteLine(
                $"  part[{idx}] T=({part.Transform.M41:F3},{part.Transform.M42:F3},{part.Transform.M43:F3}) tris={part.Mesh.FaceIndices.Count}");

        if (GeometryDispatcher.TryGetMappedItemTransform(ctx, ctx.GetEntity(44499), out var mapping))
        {
            var productM = productFrame.Matrix;
            TestContext.WriteLine(
                $"  part*product T=({(mapping * productM).M41:F3},{(mapping * productM).M42:F3},{(mapping * productM).M43:F3})");
        }

        var candInst = candidate.Instances.First(i => i.EntityIndex == 44513);
        var oracInst = oracle.Instances.First(i => i.EntityIndex == 44513);
        var candMesh = candidate.Meshes[candInst.MeshIndex];
        var oracMesh = oracle.Meshes[oracInst.MeshIndex];
        var candCenter = MeshHelpers.GetBounds(MeshHelpers.Transform(candMesh, candInst.Matrix4x4)).Center;
        var oracCenter = MeshHelpers.GetBounds(MeshHelpers.Transform(oracMesh, oracInst.Matrix4x4)).Center;
        TestContext.WriteLine($"  cand bounds center=({candCenter.X:F3},{candCenter.Y:F3},{candCenter.Z:F3})");
        TestContext.WriteLine($"  orac bounds center=({oracCenter.X:F3},{oracCenter.Y:F3},{oracCenter.Z:F3})");
    }

    [Test]
    public void InstanceMatrix_UsesPartTimesProductRowVectorOrder()
    {
        var part = Matrix4x4.CreateRotationZ((Angle)(MathF.PI / 4));
        var product = Matrix4x4.CreateTranslation(new Vector3(10, 20, 0))
            * Matrix4x4.CreateRotationZ((Angle)(MathF.PI / 3));
        var correct = part * product;
        var wrong = product * part;
        var productOrigin = Vector3.Zero.Transform(product);
        var correctWorld = Vector3.Zero.Transform(correct);
        var wrongWorld = Vector3.Zero.Transform(wrong);
        Assert.That((float)correctWorld.X, Is.EqualTo((float)productOrigin.X).Within(1e-3f));
        Assert.That((float)correctWorld.Y, Is.EqualTo((float)productOrigin.Y).Within(1e-3f));
        Assert.That((float)(wrongWorld - productOrigin).Length, Is.GreaterThan(1f),
            "product*part rotates the product placement away from oracle");
    }

    [Test]
    public void MappedItemTransform_IsTargetTimesOrigin()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCCARTESIANPOINT((1000.,0.,0.));
            #2=IFCDIRECTION((1.,0.,0.));
            #3=IFCDIRECTION((0.,0.,1.));
            #4=IFCAXIS2PLACEMENT3D(#1,#3,#2);
            #5=IFCCARTESIANPOINT((0.,0.,0.));
            #6=IFCDIRECTION((1.,0.,0.));
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCAXIS2PLACEMENT3D(#5,#7,#6);
            #9=IFCSHAPEREPRESENTATION($,'Body','Tessellation',());
            #10=IFCREPRESENTATIONMAP(#4,#9);
            #11=IFCCARTESIANPOINT((0.,0.,0.));
            #12=IFCDIRECTION((1.,0.,0.));
            #13=IFCDIRECTION((0.,0.,1.));
            #14=IFCCARTESIANTRANSFORMATIONOPERATOR3D(#12,$,#11,$,#13);
            #15=IFCMAPPEDITEM(#10,#14);
            """,
            lengthScaleOverride: 0.001);

        var ctx = model.Context;
        var mapped = ctx.GetEntity(15);
        Assert.That(GeometryDispatcher.TryGetMappedItemTransform(ctx, mapped, out var transform), Is.True);

        var origin = Placements.ReadAxis2Placement3D(ctx, ctx.GetEntity(4));
        var target = Placements.ReadCartesianTransformationOperator3D(ctx, ctx.GetEntity(14));
        var expected = target * origin.Matrix;
        var delta = TransformComparison.CompareMatrices(transform, expected);
        Assert.That(delta.Frobenius, Is.LessThan(1e-3));
    }

    [Test]
    public void LocalPlacement_ChainComposesParentThenRelative()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCDIRECTION((1.,0.,0.));
            #3=IFCDIRECTION((0.,0.,1.));
            #4=IFCAXIS2PLACEMENT3D(#1,#3,#2);
            #5=IFCLOCALPLACEMENT($,#4);
            #6=IFCCARTESIANPOINT((0.,0.,3000.));
            #7=IFCAXIS2PLACEMENT3D(#6,#3,#2);
            #8=IFCLOCALPLACEMENT(#5,#7);
            #9=IFCCARTESIANPOINT((1000.,2000.,0.));
            #10=IFCAXIS2PLACEMENT3D(#9,#3,#2);
            #11=IFCLOCALPLACEMENT(#8,#10);
            """,
            lengthScaleOverride: 0.001);

        var frame = Placements.ReadLocalPlacement(model.Context, model.Entity(11));
        Assert.That((float)frame.Origin.X, Is.EqualTo(1f).Within(1e-3f));
        Assert.That((float)frame.Origin.Y, Is.EqualTo(2f).Within(1e-3f));
        Assert.That((float)frame.Origin.Z, Is.EqualTo(3f).Within(1e-3f));
    }

    [Test]
    public void MappedItemTransform_RotatedMappingOrigin_IsTargetTimesOrigin()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCCARTESIANPOINT((100.,0.,0.));
            #6=IFCDIRECTION((0.,0.,1.));
            #7=IFCDIRECTION((0.,1.,0.));
            #8=IFCAXIS2PLACEMENT3D(#5,#6,#7);
            #9=IFCREPRESENTATIONMAP(#8,#4);
            #10=IFCCARTESIANPOINT((0.,0.,0.));
            #11=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#10,1.,$);
            #12=IFCMAPPEDITEM(#9,#11);
            """,
            lengthScaleOverride: 0.001);

        var ctx = model.Context;
        var mapped = ctx.GetEntity(12);
        Assert.That(GeometryDispatcher.TryGetMappedItemTransform(ctx, mapped, out var transform), Is.True);

        var origin = Placements.ReadAxis2Placement3D(ctx, ctx.GetEntity(8));
        var target = Placements.ReadCartesianTransformationOperator3D(ctx, ctx.GetEntity(11));
        var expected = target * origin.Matrix;
        var delta = TransformComparison.CompareMatrices(transform, expected);
        Assert.That(delta.Frobenius, Is.LessThan(1e-3));
    }

    [Test]
    [Explicit("Gate: schependomlaan placement should improve after fix")]
    [Category("Slow")]
    public void Schependomlaan_PlacementGate()
    {
        var ifcPath = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        TestFiles.RequireExists(ifcPath);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);
        var modelResult = ModelComparer.Compare(candidate, oracle, ifcPath.GetFileName());

        TestContext.WriteLine(
            $"bbox matched {modelResult.EntityBoundingBox.MatchedCount}/{modelResult.EntityBoundingBox.ComparedCount}, " +
            $"meanCenterΔ={summary.MeanBoundsCenterDistance:F4}, maxCenterΔ={summary.MaxBoundsCenterDistance:F4}");

        Assert.That(modelResult.EntityBoundingBox.MatchedCount, Is.GreaterThanOrEqualTo(520),
            "entity bbox match should improve materially on schependomlaan");
        Assert.That(summary.MeanBoundsCenterDistance, Is.LessThan(0.65),
            "mean world-space center offset should drop near duplex-control quality");
    }
}
