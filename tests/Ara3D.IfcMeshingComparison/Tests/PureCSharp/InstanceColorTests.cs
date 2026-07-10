using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class InstanceColorTests
{
    [Test]
    public void StyledItem_ViaPresentationStyleAssignment_SetsInstanceColorAndAlpha()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #10=IFCCOLOURRGB($,1.,0.25,0.);
            #11=IFCSURFACESTYLERENDERING(#10,0.4,$,$,$,$,$,$,.PHONG.);
            #12=IFCSURFACESTYLE($,.BOTH.,(#11));
            #13=IFCPRESENTATIONSTYLEASSIGNMENT((#12));
            #14=IFCSTYLEDITEM(#3,(#13),$);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#14));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('g',$,'Wall',$,$,#8,#5,$);
            """);

        Assert.That(model.Context.File, Is.Not.Null);
        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(1));

        var color = built.Instances[0].Color;
        Assert.That((float)color.R.Value, Is.EqualTo(1f).Within(1f / 255f));
        Assert.That((float)color.G.Value, Is.EqualTo(0.25f).Within(1f / 255f));
        Assert.That((float)color.B.Value, Is.EqualTo(0f).Within(1f / 255f));
        Assert.That((float)color.A.Value, Is.EqualTo(0.6f).Within(1f / 255f));
    }

    [Test]
    public void StyledItem_DirectSurfaceStyle_SetsInstanceColor()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #10=IFCCOLOURRGB($,0.,0.5,1.);
            #11=IFCSURFACESTYLESHADING(#10,$);
            #12=IFCSURFACESTYLE($,.BOTH.,(#11));
            #14=IFCSTYLEDITEM(#3,(#12),$);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#14));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('g',$,'Wall',$,$,#8,#5,$);
            """);

        Assert.That(model.Context.File, Is.Not.Null);
        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(1));

        var color = built.Instances[0].Color;
        Assert.That((float)color.R.Value, Is.EqualTo(0f).Within(1f / 255f));
        Assert.That((float)color.G.Value, Is.EqualTo(0.5f).Within(1f / 255f));
        Assert.That((float)color.B.Value, Is.EqualTo(1f).Within(1f / 255f));
        Assert.That((float)color.A.Value, Is.EqualTo(1f).Within(1f / 255f));
    }

    [Test]
    public void UnstyledProduct_KeepsDefaultMaterial()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('g',$,'Wall',$,$,#8,#5,$);
            """);

        Assert.That(model.Context.File, Is.Not.Null);
        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(1));

        var expected = Material.Default.Color;
        var color = built.Instances[0].Color;
        Assert.That((float)color.R.Value, Is.EqualTo((float)expected.R.Value).Within(1f / 255f));
        Assert.That((float)color.G.Value, Is.EqualTo((float)expected.G.Value).Within(1f / 255f));
        Assert.That((float)color.B.Value, Is.EqualTo((float)expected.B.Value).Within(1f / 255f));
        Assert.That((float)color.A.Value, Is.EqualTo((float)expected.A.Value).Within(1f / 255f));
    }

    [Test]
    public void StyledByItem_InverseOnBodySolid_SetsInstanceColor()
    {
        // Body lists the solid; StyledItem references it (common IFC authoring pattern).
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #10=IFCCOLOURRGB($,0.75,0.75,0.75);
            #11=IFCSURFACESTYLERENDERING(#10,0.,$,$,$,$,$,$,.PHONG.);
            #12=IFCSURFACESTYLE($,.BOTH.,(#11));
            #13=IFCPRESENTATIONSTYLEASSIGNMENT((#12));
            #14=IFCSTYLEDITEM(#3,(#13),$);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('g',$,'Wall',$,$,#8,#5,$);
            """);

        Assert.That(model.Context.File, Is.Not.Null);
        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(1));

        var color = built.Instances[0].Color;
        Assert.That((float)color.R.Value, Is.EqualTo(0.75f).Within(1f / 255f));
        Assert.That((float)color.G.Value, Is.EqualTo(0.75f).Within(1f / 255f));
        Assert.That((float)color.B.Value, Is.EqualTo(0.75f).Within(1f / 255f));
    }

    [Test]
    [Category("Slow")]
    public void ExampleIfc_HasNonDefaultInstanceColors()
    {
        TestFiles.RequireExists(TestFiles.Example);
        using var file = new IfcFile(TestFiles.Example, includeGeometry: false);
        var (built, _) = ModelAssembler.BuildModel(file);

        Assert.That(built.Instances, Has.Count.GreaterThan(0));
        var defaultPacked = new InstanceStruct(-1, Matrix4x4.Identity, 0, Material.Default).PackedColor;
        var nonDefault = built.Instances.Count(i => i.PackedColor != defaultPacked);
        Assert.That(nonDefault, Is.GreaterThan(0),
            () => $"Expected some styled instances on example.ifc; total={built.Instances.Count}, nonDefault={nonDefault}");
    }
}
