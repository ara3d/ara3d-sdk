using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class BooleanTests
{
    [Test]
    public void BooleanClippingResult_HalfSpace_KeepsComplementForAgreementTrue()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,.T.);
            #9=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#8);
            """);

        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(9))!.Value;
        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That(bounds.Max.Z.Value, Is.EqualTo(2f).Within(1e-4f));
        Assert.That(bounds.Min.Z.Value, Is.EqualTo(0f).Within(1e-4f));
    }

    [Test]
    public void BooleanClippingResult_HalfSpace_ReducesVolume()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,.T.);
            #9=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#8);
            """);

        var ctx = model.Context;
        var solid = SweptSolids.BuildExtrudedAreaSolid(ctx, model.Entity(3));
        var clipped = Booleans.BuildBooleanClippingResult(ctx, model.Entity(9))!.Value;
        var v0 = Math.Abs(MeshHelpers.SignedVolume(solid));
        var v1 = Math.Abs(MeshHelpers.SignedVolume(clipped));
        Assert.That(v1, Is.LessThan(v0));
    }

    [Test]
    public void UnsupportedBoolean_Union_RecordsDiagnostic()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCBOOLEANRESULT(.UNION.,#3,#3);
            """);

        var ctx = model.Context;
        GeometryDispatcher.TryBuild(ctx, model.Entity(4));
        Assert.That(ctx.Diagnostics.EntityCounts.ContainsKey("IFCBOOLEANRESULT"), Is.True);
    }
}
