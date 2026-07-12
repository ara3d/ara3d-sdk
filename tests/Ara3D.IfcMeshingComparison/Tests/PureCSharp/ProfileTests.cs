using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
[Category("IfcMesherCorrectness")]
public sealed class ProfileTests
{
    [Test]
    public void RectangleProfile_AreaAndBounds()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'R',$,4.,2.);
            """);

        var ctx = model.Context;
        var profile = ProfileBuilder.Build(ctx, model.Entity(1));

        Assert.That(profile.Outer, Has.Count.EqualTo(4));
        Assert.That(profile.Area, Is.EqualTo(8.0).Within(1e-4));
        Assert.That((float)profile.Bounds.Min.X.Value, Is.EqualTo(-2f).Within(1e-5f));
        Assert.That((float)profile.Bounds.Max.X.Value, Is.EqualTo(2f).Within(1e-5f));
        Assert.That((float)profile.Bounds.Min.Y.Value, Is.EqualTo(-1f).Within(1e-5f));
        Assert.That((float)profile.Bounds.Max.Y.Value, Is.EqualTo(1f).Within(1e-5f));
    }

    [Test]
    public void CircleProfile_ApproximateArea()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCIRCLEPROFILEDEF(.AREA.,'C',$,2.);
            """);

        var ctx = model.Context;
        var profile = ProfileBuilder.Build(ctx, model.Entity(1));
        var expected = Math.PI * 4;
        Assert.That(profile.Area, Is.EqualTo(expected).Within(0.2));
    }

    [Test]
    public void ArbitraryClosedProfile_WithPolyline()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.));
            #3=IFCCARTESIANPOINT((0.,1.));
            #4=IFCPOLYLINE((#1,#2,#3,#1));
            #5=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'Tri',#4);
            """);

        var ctx = model.Context;
        var profile = ProfileBuilder.Build(ctx, model.Entity(5));
        Assert.That(profile.Area, Is.EqualTo(1.0).Within(1e-4));
    }

    [Test]
    public void CircleHollowProfile_InnerRadiusFromWallThickness()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCIRCLEHOLLOWPROFILEDEF(.AREA.,'Pipe',$,10.,2.);
            """);

        var ctx = model.Context;
        var profile = ProfileBuilder.Build(ctx, model.Entity(1));
        var outerArea = Math.PI * 100;
        var innerArea = Math.PI * 64;
        var expected = outerArea - innerArea;
        Assert.That(profile.Area, Is.EqualTo(expected).Within(2.0));
    }

    [Test]
    public void ArbitraryOpenProfile_ReturnsOpenPath()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((3.,0.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCARBITRARYOPENPROFILEDEF(.CURVE.,'Path',#3);
            """);

        var ctx = model.Context;
        var path = ProfileBuilder.BuildOpen(ctx, model.Entity(4));
        Assert.That(path, Has.Count.EqualTo(2));
        Assert.That((float)path[1].X, Is.EqualTo(3f).Within(1e-5f));
    }

    [Test]
    public void ArbitraryProfileDef_ClosedCurve_BuildsArea()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.));
            #3=IFCCARTESIANPOINT((0.,2.));
            #4=IFCPOLYLINE((#1,#2,#3,#1));
            #5=IFCARBITRARYPROFILEDEF(.AREA.,'Tri',#4);
            """);

        var ctx = model.Context;
        var profile = ProfileBuilder.Build(ctx, model.Entity(5));
        Assert.That(profile.Area, Is.EqualTo(2.0).Within(1e-4));
    }

    [Test]
    public void SteelProfiles_BuildWithoutError()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCLSHAPEPROFILEDEF(.AREA.,'L',$,2.5,2.5,0.375,$,$,$,$);
            #2=IFCTSHAPEPROFILEDEF(.AREA.,'T',$,6.,8.,0.37,0.64,$,$,$,$,$,$);
            #3=IFCUSHAPEPROFILEDEF(.AREA.,'C',$,10.,2.6,0.24,0.436,$,$,$,$);
            #4=IFCRECTANGLEHOLLOWPROFILEDEF(.AREA.,'RHS',$,4.,6.,0.25,$,$);
            """);

        var ctx = model.Context;
        Assert.That(ProfileBuilder.Build(ctx, model.Entity(1)).Area, Is.GreaterThan(0));
        Assert.That(ProfileBuilder.Build(ctx, model.Entity(2)).Area, Is.GreaterThan(0));
        Assert.That(ProfileBuilder.Build(ctx, model.Entity(3)).Area, Is.GreaterThan(0));
        Assert.That(ProfileBuilder.Build(ctx, model.Entity(4)).Area, Is.GreaterThan(0));
    }

    [Test]
    [Category("Slow")]
    public void AiscSculpture_SteelProfiles_ProduceExtrusions()
    {
        TestFiles.RequireExists(TestFiles.AiscSculptureBrep);
        using var file = TestFiles.LoadStep(TestFiles.AiscSculptureBrep);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
        Assert.That(diagnostics.EntityStatus.GetValueOrDefault("IFCLSHAPEPROFILEDEF"),
            Is.EqualTo(GeometrySupportStatus.Supported));
    }

    [Test]
    public void Profile_TriangulationProducesTriangles()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'R',$,2.,2.);
            """);

        var ctx = model.Context;
        var tris = ProfileBuilder.Build(ctx, model.Entity(1)).Triangulate();
        Assert.That(tris, Has.Count.EqualTo(2));
    }

    [Test]
    [Category("Slow")]
    public void SteelPlates_BrepFile_ProducesGeometry()
    {
        TestFiles.RequireExists(TestFiles.SteelPlates);
        using var file = TestFiles.LoadStep(TestFiles.SteelPlates);
        var (model, _) = ModelAssembler.BuildModel(file);
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
    }
}
