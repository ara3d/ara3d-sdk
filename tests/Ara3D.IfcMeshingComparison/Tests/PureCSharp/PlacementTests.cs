using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class PlacementTests
{
    [Test]
    public void Axis2Placement3D_DefaultAxes_IsIdentityAtOrigin()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCAXIS2PLACEMENT3D(#1,$,$);
            """);

        var ctx = model.Context;
        var frame = Placements.ReadAxis2Placement3D(ctx, model.Entity(2));
        var matrix = frame.Matrix;

        var p0 = frame.ToWorld(Vector3.Zero);
        var px = frame.ToWorld(Vector3.UnitX);
        var pz = frame.ToWorld(Vector3.UnitZ);
        AssertPoint(p0, 0, 0, 0);
        AssertPoint(px, 1, 0, 0);
        AssertPoint(pz, 0, 0, 1);
        Assert.That((double)matrix.M44, Is.EqualTo(1.0).Within(1e-6));
    }

    [Test]
    public void Axis2Placement3D_TranslatedOrigin()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((10.,20.,30.));
            #2=IFCAXIS2PLACEMENT3D(#1,$,$);
            """);

        var ctx = model.Context;
        var frame = Placements.ReadAxis2Placement3D(ctx, model.Entity(2));
        AssertPoint(frame.Origin, 10, 20, 30);
    }

    [Test]
    public void LocalPlacement_ChainsParentAndChild()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((100.,0.,0.));
            #2=IFCAXIS2PLACEMENT3D(#1,$,$);
            #3=IFCLOCALPLACEMENT($,#2);
            #4=IFCCARTESIANPOINT((0.,50.,0.));
            #5=IFCAXIS2PLACEMENT3D(#4,$,$);
            #6=IFCLOCALPLACEMENT(#3,#5);
            """);

        var ctx = model.Context;
        var frame = Placements.ReadLocalPlacement(ctx, model.Entity(6));
        AssertPoint(frame.Origin, 100, 50, 0);
    }

    [Test]
    public void SiUnit_Millimeter_ScaleIs0_001()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCSIUNIT(*,.LENGTHUNIT.,.MILLI.,.METRE.);
            #2=IFCUNITASSIGNMENT((#1));
            #3=IFCPROJECT('id',$,'P',$,$,$,$,(#2));
            """);

        var ctx = model.Context;
        Assert.That(ctx.LengthScale, Is.EqualTo(0.001).Within(1e-9));
    }

    static void AssertPoint(Point3D p, double x, double y, double z, float tol = 1e-5f)
    {
        Assert.Multiple(() =>
        {
            Assert.That(p.X.Value, Is.EqualTo(x).Within(tol));
            Assert.That(p.Y.Value, Is.EqualTo(y).Within(tol));
            Assert.That(p.Z.Value, Is.EqualTo(z).Within(tol));
        });
    }

    static void AssertPoint(Vector3 p, double x, double y, double z, float tol = 1e-5f)
    {
        Assert.Multiple(() =>
        {
            Assert.That((float)p.X, Is.EqualTo((float)x).Within(tol));
            Assert.That((float)p.Y, Is.EqualTo((float)y).Within(tol));
            Assert.That((float)p.Z, Is.EqualTo((float)z).Within(tol));
        });
    }
}
