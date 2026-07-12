using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Harness.GeometryOracles;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
[Category("IfcMesherCorrectness")]
public sealed class CutOracleTests
{
    [Test]
    [GeometryCoverage("IFCBOOLEANCLIPPINGRESULT")]
    [GeometryCoverage("IFCBOOLEANRESULT")]
    [GeometryCoverage("IFCHALFSPACESOLID")]
    [GeometryCoverage("IFCPLANE")]
    public void CutOracle_HalfSpace_AgreementMatrix_2x2(
        [Values(true, false)] bool agreement,
        [Values(true, false)] bool planeNormalUp)
    {
        var agreementFlag = agreement ? ".T." : ".F.";
        var nz = planeNormalUp ? "1." : "-1.";
        using var model = MicroIfc.Parse($"""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,{nz}));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,{agreementFlag});
            #9=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#8);
            """);

        var solid = SweptSolids.BuildExtrudedAreaSolid(model.Context, model.Entity(3));
        var clipped = GeometryDispatcher.TryBuild(model.Context, model.Entity(9))!.Value;
        MeshTestAssert.MeshValid(clipped);
        Assert.That(clipped.FaceIndices.Count, Is.GreaterThan(0));

        // Mid-plane clip keeps half the extrusion height. Signed volume is unreliable on open shells
        // (no cut-plane caps), so gate on bounds + outward faces instead.
        var bounds = MeshHelpers.GetBounds(clipped);
        var height = bounds.Max.Z.Value - bounds.Min.Z.Value;
        Assert.That(height, Is.EqualTo(2f).Within(0.05f), "mid-plane clip should keep half the extrusion height");
        Assert.That(
            MathF.Abs(bounds.Max.Z.Value - 2f) < 0.05f || MathF.Abs(bounds.Min.Z.Value - 2f) < 0.05f,
            Is.True,
            "one Z extremum should lie on the cut plane at Z=2");
        Assert.That(ClipOracle.PostClipFacesOutward(clipped, minFraction: 0.8f), Is.True);
        Assert.That(AnalyticalOracle.AbsVolume(clipped), Is.LessThan(AnalyticalOracle.AbsVolume(solid)));
    }

    [Test]
    [GeometryCoverage("IFCPOLYGONALBOUNDEDHALFSPACE")]
    public void CutOracle_PolygonalBounded_NoOverClip_NoUnderClip()
    {
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
        MeshTestAssert.MeshValid(mesh);
        MeshTestAssert.ClipKeptBounds(mesh, maxZ: 4f, minZ: 0f);

        var bounds = MeshHelpers.GetBounds(mesh);
        Assert.That(bounds.Max.Z.Value, Is.EqualTo(4f).Within(1e-4f), "outside prism must keep full height");

        var (keptHighOutside, clippedInside) = ClipOracle.SamplePolygonalZClipRegions(
            mesh, planeZ: 2f, prismMinX: 0.5f, highZThreshold: 2.5f);
        Assert.That(keptHighOutside, Is.True, "outside prism must keep full height vertices");
        Assert.That(clippedInside, Is.True, "inside prism must show cut-plane vertices");
    }

    [Test]
    public void CutOracle_NestedGableClip_VolumeBracket()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((0.36,0.));
            #3=IFCCARTESIANPOINT((0.36,5.));
            #4=IFCCARTESIANPOINT((0.,5.));
            #5=IFCPOLYLINE((#1,#2,#3,#4,#1));
            #6=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'Wall',#5);
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCEXTRUDEDAREASOLID(#6,$,#7,6.);
            #9=IFCCARTESIANPOINT((-2.5,0.,3.));
            #10=IFCDIRECTION((-1.,0.,1.));
            #11=IFCDIRECTION((1.,0.,0.));
            #12=IFCAXIS2PLACEMENT3D(#9,#10,#11);
            #13=IFCPLANE(#12);
            #14=IFCHALFSPACESOLID(#13,.F.);
            #15=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#8,#14);
            #16=IFCCARTESIANPOINT((2.5,0.,3.));
            #17=IFCDIRECTION((1.,0.,1.));
            #18=IFCDIRECTION((1.,0.,0.));
            #19=IFCAXIS2PLACEMENT3D(#16,#17,#18);
            #20=IFCPLANE(#19);
            #21=IFCHALFSPACESOLID(#20,.F.);
            #22=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#15,#21);
            """);

        var solid = GeometryDispatcher.TryBuild(model.Context, model.Entity(8))!.Value;
        var clipped = GeometryDispatcher.TryBuild(model.Context, model.Entity(22))!.Value;
        var v0 = AnalyticalOracle.AbsVolume(solid);
        var v1 = AnalyticalOracle.AbsVolume(clipped);
        Assert.That(v1, Is.GreaterThan(0));
        Assert.That(v1, Is.LessThan(v0));
        // Residual should be a large fraction of the prism (gable cuts only the top corners).
        Assert.That(v1, Is.GreaterThan(v0 * 0.5));
        Assert.That(v1, Is.LessThan(v0 * 0.99));
        MeshTestAssert.ClipKeptBounds(clipped, maxZ: 5.55f, minZ: 0f, tol: 0.05f);
    }

    [Test]
    public void CutOracle_ExtrusionEndClip_ExactDepth()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,0.2,0.2);
            #2=IFCDIRECTION((0.,0.,-1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((1.,0.,0.));
            #5=IFCDIRECTION((1.,0.,0.));
            #6=IFCDIRECTION((0.,0.,-1.));
            #7=IFCAXIS2PLACEMENT3D(#4,#5,#6);
            #8=IFCPLANE(#7);
            #9=IFCHALFSPACESOLID(#8,.F.);
            #10=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#9);
            """);

        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(10))!.Value;
        MeshTestAssert.MeshValid(mesh);
        MeshTestAssert.ClipKeptBounds(mesh, maxZ: 0f, minZ: -1f, tol: 1e-3f);
    }

    [Test]
    public void CutOracle_ClipTriangle_QuadSplit_PreservesWinding()
    {
        // Unit triangle straddling Z=0: two verts above, one below → quad after clip.
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,-1.),(1.,0.,1.),(0.,1.,1.)));
            #2=IFCTRIANGULATEDFACESET(#1,$,.T.,((1,2,3)),$);
            #3=IFCCARTESIANPOINT((0.,0.,0.));
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCAXIS2PLACEMENT3D(#3,$,#4);
            #6=IFCPLANE(#5);
            #7=IFCHALFSPACESOLID(#6,.T.);
            """);

        var tri = Tessellated.BuildTriangulatedFaceSet(model.Context, model.Entity(2));
        var inputNormal = Vector3.Cross(
            tri.Points[1].Vector3 - tri.Points[0].Vector3,
            tri.Points[2].Vector3 - tri.Points[0].Vector3);
        var clipped = Booleans.ClipByHalfSpace(model.Context, tri, model.Entity(7));
        Assert.That(clipped.FaceIndices.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(ClipOracle.ClippedFacesMatchReferenceNormal(clipped, inputNormal), Is.True,
            "quad-split faces must stay in the input triangle's normal hemisphere");
    }

    [Test]
    [GeometryCoverage("IFCPRODUCTDEFINITIONSHAPE")]
    public void CutOracle_OpeningCarve_VoidAbsent_RevealPresent()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'W',$,0.3,4.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,3.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('wall-guid',$,'Wall',$,$,#8,#5,$);
            #10=IFCRECTANGLEPROFILEDEF(.AREA.,'O',$,0.5,1.);
            #11=IFCCARTESIANPOINT((0.,0.,1.));
            #12=IFCAXIS2PLACEMENT3D(#11,$,$);
            #13=IFCEXTRUDEDAREASOLID(#10,#12,#2,1.);
            #14=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#13));
            #15=IFCPRODUCTDEFINITIONSHAPE($,$,(#14));
            #16=IFCOPENINGELEMENT('open-guid',$,'Opening',$,$,#8,#15,$);
            #17=IFCRELVOIDSELEMENT('void-guid',$,$,$,#9,#16);
            """);

        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        var wall = built.Meshes[built.Instances[0].MeshIndex];
        MeshTestAssert.MeshValid(wall);

        // Point in the void (through the wall thickness at opening center) should be outside.
        MeshTestAssert.PointOutside(wall, 0f, 0f, 1.5f);
        // Point in solid wall material away from the opening should be inside.
        MeshTestAssert.PointInside(wall, 0f, 1.5f, 0.5f);
    }

    [Test]
    [Category("Slow")]
    public void CutOracle_SteelPlates_ClippedBeams_FirstPrinciples()
    {
        var path = TestFiles.ResolveOrIgnore("steelplates.ifc");
        using var file = new IfcFile(path, includeGeometry: false);
        var ctx = new MeshingContext(file);
        foreach (var id in new[] { 1193, 633, 1385 })
        {
            var entity = ctx.GetEntityOrDefault(id);
            if (entity is null)
            {
                Assert.Ignore($"#{id} missing in steelplates.ifc");
                return;
            }
            var mesh = ModelAssembler.BuildEntityMesh(ctx, entity);
            Assert.That(mesh, Is.Not.Null, $"#{id} should produce a mesh");
            var m = mesh!.Value;
            MeshTestAssert.MeshValid(m);
            Assert.That(Math.Abs(MeshHelpers.SignedVolume(m)), Is.GreaterThan(0), $"#{id} abs signed volume");
            Assert.That(TopologyOracle.CountOpenEdges(m), Is.LessThan(64),
                $"#{id} open-edge budget for clipped beam");
        }
    }
}
