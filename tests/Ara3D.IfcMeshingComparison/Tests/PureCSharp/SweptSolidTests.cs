using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class SweptSolidTests
{
    [Test]
    public void SurfaceOfLinearExtrusion_OpenProfile_RibbonBounds()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((4.,0.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCARBITRARYOPENPROFILEDEF(.CURVE.,'Path',#3);
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCSURFACEOFLINEAREXTRUSION(#4,$,#5,2.);
            """);

        var ctx = model.Context;
        var mesh = SweptSolids.BuildSurfaceOfLinearExtrusion(ctx, model.Entity(6));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(2));
        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That((float)bounds.Min.Z.Value, Is.EqualTo(0f).Within(1e-5f));
        Assert.That((float)bounds.Max.Z.Value, Is.EqualTo(2f).Within(1e-5f));
        Assert.That((float)bounds.Max.X.Value, Is.EqualTo(4f).Within(1e-5f));
    }

    [Test]
    public void SurfaceOfLinearExtrusion_WithPlacement()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((3.,0.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCARBITRARYOPENPROFILEDEF(.CURVE.,'Path',#3);
            #5=IFCCARTESIANPOINT((10.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCSURFACEOFLINEAREXTRUSION(#4,#6,#7,1.);
            """);

        var ctx = model.Context;
        var mesh = SweptSolids.BuildSurfaceOfLinearExtrusion(ctx, model.Entity(8));
        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That((float)bounds.Min.X.Value, Is.EqualTo(10f).Within(1e-4f));
        Assert.That((float)bounds.Max.X.Value, Is.EqualTo(13f).Within(1e-4f));
        Assert.That((float)bounds.Max.Z.Value, Is.EqualTo(1f).Within(1e-4f));
    }

    [Test]
    [Category("Slow")]
    public void DentalClinic_SurfaceOfLinearExtrusion_EntityBuildsRibbon()
    {
        TestFiles.RequireExists(TestFiles.DentalClinic);
        using var file = TestFiles.LoadStep(TestFiles.DentalClinic);
        var ctx = new MeshingContext(file);
        var mesh = GeometryDispatcher.TryBuild(ctx, ctx.GetEntity(159483));
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
        Assert.That(mesh.Value.Points.Count, Is.GreaterThanOrEqualTo(4));
        TestContext.WriteLine($"Triangles: {mesh.Value.FaceIndices.Count}, Points: {mesh.Value.Points.Count}");
    }

    [Test]
    [Category("Slow")]
    public void OfficeA_SurfaceOfLinearExtrusion_EntityBuildsRibbon()
    {
        TestFiles.RequireExists(TestFiles.OfficeA);
        using var file = TestFiles.LoadStep(TestFiles.OfficeA);
        var ctx = new MeshingContext(file);
        var mesh = GeometryDispatcher.TryBuild(ctx, ctx.GetEntity(45220));
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
        TestContext.WriteLine($"Triangles: {mesh.Value.FaceIndices.Count}");
    }

    [Test]
    public void ExtrudedAreaSolid_Rectangle_Bounds()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,3.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            """);

        var ctx = model.Context;
        var mesh = SweptSolids.BuildExtrudedAreaSolid(ctx, model.Entity(3));
        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That((float)bounds.Min.Z.Value, Is.EqualTo(0f).Within(1e-5f));
        Assert.That((float)bounds.Max.Z.Value, Is.EqualTo(4f).Within(1e-5f));
        Assert.That((float)bounds.Min.X.Value, Is.EqualTo(-1f).Within(1e-5f));
        Assert.That((float)bounds.Max.X.Value, Is.EqualTo(1f).Within(1e-5f));
    }

    [Test]
    public void ExtrudedAreaSolid_WithPlacement()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCCARTESIANPOINT((10.,20.,30.));
            #3=IFCAXIS2PLACEMENT3D(#2,$,$);
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCEXTRUDEDAREASOLID(#1,#3,#4,4.);
            """);

        var ctx = model.Context;
        var mesh = SweptSolids.BuildExtrudedAreaSolid(ctx, model.Entity(5));
        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That((float)bounds.Min.X.Value, Is.EqualTo(9f).Within(1e-4f));
        Assert.That((float)bounds.Max.Z.Value, Is.EqualTo(34f).Within(1e-4f));
    }

    [Test]
    public void ExampleIfc_SteelSectionExtrusions_Build()
    {
        TestFiles.RequireExists(TestFiles.Example);
        using var file = TestFiles.LoadStep(TestFiles.Example);
        var ctx = new MeshingContext(file);

        foreach (var (solidId, profileId) in new (int solid, int profile)[]
        {
            (3409, 3405), (3580, 3576), (3282, 3278), (3168, 3164), (2679, 2677),
        })
        {
            var curveId = file.EntityResolver.GetEntity(profileId).GetId(2);
            var points = CurveEvaluator.Evaluate2D(ctx, ctx.GetEntity(curveId), dropClosure: true);
            var cleaned = PolygonWithHoles.CleanRing(points);
            var selfIntersect = PolygonTriangulator.HasSelfIntersection(cleaned);
            TestContext.WriteLine($"#{solidId} profile #{profileId} curve #{curveId}: {cleaned.Count} pts, selfIntersect={selfIntersect}");
        }

        var built = 0;
        foreach (var id in new[] { 3409, 3580, 3282, 3168, 2679 })
        {
            var mesh = GeometryDispatcher.TryBuild(ctx, ctx.GetEntity(id));
            if (mesh.HasValue && mesh.Value.FaceIndices.Count > 0)
                built++;
        }
        Assert.That(built, Is.EqualTo(5), "All steel section extrusions from example.ifc should mesh");
    }

    [Test]
    public void ExampleIfc_ShsProfileWithVoids_Builds()
    {
        TestFiles.RequireExists(TestFiles.Example);
        using var file = TestFiles.LoadStep(TestFiles.Example);
        var ctx = new MeshingContext(file);

        foreach (var (solidId, profileId) in new (int solid, int profile)[]
        {
            (7649, 7642), (7935, 7928),
        })
        {
            var profile = ProfileBuilder.Build(ctx, ctx.GetEntity(profileId));
            TestContext.WriteLine(
                $"#{solidId} profile #{profileId}: outer={profile.Outer.Count} hole={profile.Holes[0].Count} " +
                $"outerSelf={PolygonTriangulator.HasSelfIntersection(profile.Outer)} " +
                $"holeSelf={PolygonTriangulator.HasSelfIntersection(profile.Holes[0])}");
            Assert.That(profile.Triangulate(), Is.Not.Empty);
            var mesh = GeometryDispatcher.TryBuild(ctx, ctx.GetEntity(solidId));
            Assert.That(mesh.HasValue, Is.True, $"#{solidId} should mesh");
            Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
        }
    }

    [Test]
    [Category("Slow")]
    public void ExampleIfc_MeshBoundsWithinOracleTolerance()
    {
        TestFiles.RequireExists(TestFiles.Example);
        var (mine, oracle) = OracleComparison.CompareFile(TestFiles.Example);
        TestContext.WriteLine(OracleComparison.FormatComparison("example.ifc", mine, oracle));
        Assert.That(mine.TriangleCount, Is.GreaterThan(100));
    }
}
