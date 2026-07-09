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
    public void BooleanClippingResult_PolygonalBoundedHalfSpace_RetainsGeometryOutsideBoundary()
    {
        // Box spans X,Y in [-2,2], Z in [0,4]. Base plane at Z=2 (agreement .T. removes Z>2).
        // Polygonal boundary (identity Position frame) covers only X in [0.5,5]: the +X half of the box.
        // Correct gated clip: the -X half keeps its full height (Z up to 4); only the +X half is cut at Z=2.
        // A naive plane-only clip would wrongly flatten the whole top to Z=2.
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,4.,4.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCAXIS2PLACEMENT3D(#4,$,$);
            #6=IFCPLANE(#5);
            #7=IFCCARTESIANPOINT((0.,0.,0.));
            #8=IFCAXIS2PLACEMENT3D(#7,$,$);
            #9=IFCCARTESIANPOINT((0.5,-5.));
            #10=IFCCARTESIANPOINT((5.,-5.));
            #11=IFCCARTESIANPOINT((5.,5.));
            #12=IFCCARTESIANPOINT((0.5,5.));
            #13=IFCPOLYLINE((#9,#10,#11,#12,#9));
            #14=IFCPOLYGONALBOUNDEDHALFSPACE(#6,.T.,#8,#13);
            #15=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#14);
            """);

        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(15))!.Value;
        var bounds = MeshHelpers.GetBounds(mesh);

        // The part OUTSIDE the polygon boundary is retained at full height (would be 2 if over-clipped).
        Assert.That(bounds.Max.Z.Value, Is.EqualTo(4f).Within(1e-4f));
        Assert.That(bounds.Min.Z.Value, Is.EqualTo(0f).Within(1e-4f));

        // Retained full-height geometry must exist on the -X (outside-prism) side; clipping still occurred
        // inside the prism (a cut vertex at Z~2 on the +X side).
        var keptHighOutside = false;
        var clippedInside = false;
        foreach (var pt in mesh.Points)
        {
            var x = pt.Vector3.X.Value;
            var z = pt.Vector3.Z.Value;
            if (z > 2.5f && x < 0f) keptHighOutside = true;
            if (Math.Abs(z - 2f) < 1e-3f && x > 0.4f) clippedInside = true;
        }
        Assert.That(keptHighOutside, Is.True, "geometry outside the polygonal boundary should keep full height");
        Assert.That(clippedInside, Is.True, "geometry inside the polygonal boundary should be clipped at the base plane");
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
