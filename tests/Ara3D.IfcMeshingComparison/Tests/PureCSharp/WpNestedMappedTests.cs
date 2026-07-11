using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.IfcTypes;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>Nested IFCMAPPEDITEM row-vector composition (inner * outer * parent).</summary>
[TestFixture]
public sealed class WpNestedMappedTests
{
    [Test]
    public void NestedMappedItem_ComposesInnerThenOuter_RowVector()
    {
        // Inner map: 90° about Z. Outer map: translate (10,0,0) m (IFC mm → scale 0.001).
        // Local point (1,0,0): correct → rotate to (0,1,0) then translate → (10,1,0).
        // Wrong parent*mapping order → translate then rotate → (0,11,0).
        using var model = MicroIfc.Parse(
            """
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,$,$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCCARTESIANPOINT((0.,0.,0.));
            #6=IFCDIRECTION((0.,0.,1.));
            #7=IFCDIRECTION((1.,0.,0.));
            #8=IFCAXIS2PLACEMENT3D(#5,#6,#7);
            #9=IFCREPRESENTATIONMAP(#8,#4);
            #10=IFCCARTESIANPOINT((0.,0.,0.));
            #11=IFCDIRECTION((0.,1.,0.));
            #12=IFCDIRECTION((0.,0.,1.));
            #13=IFCCARTESIANTRANSFORMATIONOPERATOR3D(#11,$,#10,1.,#12);
            #14=IFCMAPPEDITEM(#9,#13);
            #15=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#14));
            #16=IFCCARTESIANPOINT((0.,0.,0.));
            #17=IFCAXIS2PLACEMENT3D(#16,#6,#7);
            #18=IFCREPRESENTATIONMAP(#17,#15);
            #19=IFCCARTESIANPOINT((10000.,0.,0.));
            #20=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#19,1.,$);
            #21=IFCMAPPEDITEM(#18,#20);
            """,
            lengthScaleOverride: 0.001);

        var ctx = model.Context;
        var parts = new List<CollectedPart>();
        GeometryPartCollector.CollectParts(ctx, ctx.GetEntity(21), Matrix4x4.Identity, productEntityId: 1, parts);
        Assert.That(parts, Has.Count.EqualTo(1));

        var world = Vector3.Zero.Transform(parts[0].Transform);
        Assert.That((float)world.X, Is.EqualTo(10f).Within(1e-3f),
            "outer translation should apply after inner rotation");
        Assert.That((float)world.Y, Is.EqualTo(0f).Within(1e-3f));
        Assert.That((float)world.Z, Is.EqualTo(0f).Within(1e-3f));

        // A local +X unit offset must land at world (10,1,0) after inner 90° Z then outer +X.
        var localOffset = new Vector3(1, 0, 0).Transform(parts[0].Transform);
        Assert.That((float)localOffset.X, Is.EqualTo(10f).Within(1e-3f));
        Assert.That((float)localOffset.Y, Is.EqualTo(1f).Within(1e-3f),
            "inner rotation must run before outer translation (mapping*parent, not parent*mapping)");
    }

    [Test]
    public void NestedMappedItem_WrongParentTimesMapping_WouldRotateTranslation()
    {
        var inner = Matrix4x4.CreateRotationZ((Angle)(MathF.PI / 2));
        var outer = Matrix4x4.CreateTranslation(new Vector3(10, 0, 0));
        var correct = inner * outer;
        var wrong = outer * inner;
        var p = new Vector3(1, 0, 0);
        var correctWorld = p.Transform(correct);
        var wrongWorld = p.Transform(wrong);
        Assert.That((float)correctWorld.X, Is.EqualTo(10f).Within(1e-4f));
        Assert.That((float)correctWorld.Y, Is.EqualTo(1f).Within(1e-4f));
        Assert.That((float)(wrongWorld - correctWorld).Length, Is.GreaterThan(1f));
    }
}
