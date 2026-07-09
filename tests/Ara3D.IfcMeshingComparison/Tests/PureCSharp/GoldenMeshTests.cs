using Ara3D.Geometry;
using Ara3D.IfcTypes;
using Ara3D.IfcTypes.Ifc4;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class GoldenMeshTests
{
    [Test]
    public void HarnessUsesIfcTypeSchemaAttributeLayouts()
    {
        Assert.Multiple(() =>
        {
            Assert.That(IfcExtrudedAreaSolid.Instance.SweptArea.Index, Is.EqualTo(0));
            Assert.That(IfcExtrudedAreaSolid.Instance.Position.Index, Is.EqualTo(1));
            Assert.That(IfcExtrudedAreaSolid.Instance.ExtrudedDirection.Index, Is.EqualTo(2));
            Assert.That(IfcExtrudedAreaSolid.Instance.Depth.Index, Is.EqualTo(3));
            Assert.That(IfcRectangleProfileDef.Instance.XDim.Index, Is.EqualTo(3));
            Assert.That(IfcRectangleProfileDef.Instance.YDim.Index, Is.EqualTo(4));
            Assert.That(IfcRectangleHollowProfileDef.Instance.WallThickness.Index, Is.EqualTo(5));
            Assert.That(IfcCircleHollowProfileDef.Instance.WallThickness.Index, Is.EqualTo(4));
            Assert.That(IfcEllipseProfileDef.Instance.SemiAxis1.Index, Is.EqualTo(3));
            Assert.That(IfcIShapeProfileDef.Instance.OverallWidth.Index, Is.EqualTo(3));
            Assert.That(IfcLShapeProfileDef.Instance.Thickness.Index, Is.EqualTo(5));
            Assert.That(IfcArbitraryProfileDefWithVoids.Instance.InnerCurves.Index, Is.EqualTo(3));
            Assert.That(IfcMappedItem.Instance.MappingSource.Index, Is.EqualTo(0));
            Assert.That(IfcMappedItem.Instance.MappingTarget.Index, Is.EqualTo(1));
            Assert.That(IfcRepresentationMap.Instance.MappedRepresentation.Index, Is.EqualTo(1));
            Assert.That(IfcCartesianTransformationOperator.Instance.LocalOrigin.Index, Is.EqualTo(2));
            Assert.That(IfcCartesianTransformationOperator.Instance.Scale.Index, Is.EqualTo(3));
            Assert.That(IfcCartesianTransformationOperator3DnonUniform.Instance.Scale2.Index, Is.EqualTo(5));
            Assert.That(IfcCartesianTransformationOperator3DnonUniform.Instance.Scale3.Index, Is.EqualTo(6));
            Assert.That(IfcManifoldSolidBrep.Instance.Outer.Index, Is.EqualTo(0));
            Assert.That(IfcConnectedFaceSet.Instance.CfsFaces.Index, Is.EqualTo(0));
            Assert.That(IfcFace.Instance.Bounds.Index, Is.EqualTo(0));
            Assert.That(IfcFaceBound.Instance.Bound.Index, Is.EqualTo(0));
            Assert.That(IfcPolyLoop.Instance.Polygon.Index, Is.EqualTo(0));
            Assert.That(IfcFaceBasedSurfaceModel.Instance.FbsmFaces.Index, Is.EqualTo(0));
            Assert.That(IfcShellBasedSurfaceModel.Instance.SbsmBoundary.Index, Is.EqualTo(0));
            Assert.That(IfcCartesianPointList3D.Instance.CoordList.Index, Is.EqualTo(0));
            Assert.That(IfcTessellatedFaceSet.Instance.Coordinates.Index, Is.EqualTo(0));
            Assert.That(IfcTriangulatedFaceSet.Instance.CoordIndex.Index, Is.EqualTo(3));
            Assert.That(IfcPolygonalFaceSet.Instance.Faces.Index, Is.EqualTo(2));
            Assert.That(IfcIndexedPolygonalFace.Instance.CoordIndex.Index, Is.EqualTo(0));
            Assert.That(IfcIndexedPolygonalFaceWithVoids.Instance.InnerCoordIndices.Index, Is.EqualTo(1));
            Assert.That(IfcCompositeCurve.Instance.Segments.Index, Is.EqualTo(0));
            Assert.That(IfcCompositeCurveSegment.Instance.SameSense.Index, Is.EqualTo(1));
            Assert.That(IfcCompositeCurveSegment.Instance.ParentCurve.Index, Is.EqualTo(2));
            Assert.That(IfcConic.Instance.Position.Index, Is.EqualTo(0));
            Assert.That(IfcCircle.Instance.Radius.Index, Is.EqualTo(1));
            Assert.That(IfcTrimmedCurve.Instance.BasisCurve.Index, Is.EqualTo(0));
            Assert.That(IfcTrimmedCurve.Instance.Trim1.Index, Is.EqualTo(1));
            Assert.That(IfcTrimmedCurve.Instance.Trim2.Index, Is.EqualTo(2));
            Assert.That(IfcBooleanResult.Instance.FirstOperand.Index, Is.EqualTo(1));
            Assert.That(IfcBooleanResult.Instance.SecondOperand.Index, Is.EqualTo(2));
            Assert.That(IfcSweptDiskSolid.Instance.Directrix.Index, Is.EqualTo(0));
            Assert.That(IfcSweptDiskSolid.Instance.Radius.Index, Is.EqualTo(1));
            Assert.That(IfcSweptDiskSolid.Instance.InnerRadius.Index, Is.EqualTo(2));
            Assert.That(IfcAxis1Placement.Instance.Axis.Index, Is.EqualTo(1));
            Assert.That(IfcRevolvedAreaSolid.Instance.Axis.Index, Is.EqualTo(2));
            Assert.That(IfcRevolvedAreaSolid.Instance.Angle.Index, Is.EqualTo(3));
            Assert.That(IfcHalfSpaceSolid.Instance.BaseSurface.Index, Is.EqualTo(0));
            Assert.That(IfcHalfSpaceSolid.Instance.AgreementFlag.Index, Is.EqualTo(1));
            Assert.That(IfcElementarySurface.Instance.Position.Index, Is.EqualTo(0));
        });
    }

    [Test]
    public void MeshesRectangleExtrudedAreaSolid()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,3.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (-1f, -1.5f, 0f), (1f, 1.5f, 4f));
    }

    [Test]
    public void MeshesArbitraryClosedPolylineExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.));
            #3=IFCCARTESIANPOINT((0.,1.));
            #4=IFCCARTESIANPOINT((0.,0.));
            #5=IFCPOLYLINE((#1,#2,#3,#4));
            #6=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'Triangle',#5);
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCEXTRUDEDAREASOLID(#6,$,#7,5.);
            """);

        var mesh = MeshRequired(model, 8);

        AssertMeshCounts(mesh, expectedPoints: 6, expectedFaces: 8);
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 1f, 5f));
    }

    [Test]
    public void MeshesFaceBasedSurfaceModelWithOpenShellBoundary()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCPOLYLOOP((#1,#2,#3,#4));
            #6=IFCFACEOUTERBOUND(#5,.T.);
            #7=IFCFACE((#6));
            #8=IFCOPENSHELL((#7));
            #9=IFCFACEBASEDSURFACEMODEL((#8));
            """);

        var mesh = MeshRequired(model, 9);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 0f));
    }

    [Test]
    public void MeshesCircleProfileExtrusionWithConfiguredSegments()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCIRCLEPROFILEDEF(.AREA.,'Column',$,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,3.);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 24, expectedFaces: 44);
        AssertBounds(mesh, (-2f, -2f, 0f), (2f, 2f, 3f), tolerance: 1e-5f);
    }

    [Test]
    public void MeshesRectangleHollowProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEHOLLOWPROFILEDEF(.AREA.,'Tube',$,4.,3.,0.5,$,$);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 16, expectedFaces: 32);
        AssertBounds(mesh, (-2f, -1.5f, 0f), (2f, 1.5f, 2f));
    }

    [Test]
    public void MeshesCircleHollowProfileExtrusionWithConfiguredSegments()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCIRCLEHOLLOWPROFILEDEF(.AREA.,'Pipe',$,2.,0.25);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,3.);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 48, expectedFaces: 96);
        AssertBounds(mesh, (-2f, -2f, 0f), (2f, 2f, 3f), tolerance: 1e-5f);
    }

    [Test]
    public void MeshesSmallInchBoltShankCircleExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCIRCLEPROFILEDEF(.AREA.,'Bolt shank',$,0.375);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            """, lengthScaleOverride: 0.0254, circleSegments: 32);

        var mesh = MeshRequired(model, 3);

        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(124));
        var r = (float)(0.375 * 0.0254);
        AssertBounds(mesh, (-r, -r, 0f), (r, r, 2f * 0.0254f), tolerance: 1e-6f);
    }

    [Test]
    public void MeshesEllipseProfileExtrusionWithConfiguredSegments()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCELLIPSEPROFILEDEF(.AREA.,'Oval',$,3.,1.5);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 24, expectedFaces: 44);
        AssertBounds(mesh, (-3f, -1.5f, 0f), (3f, 1.5f, 2f), tolerance: 1e-5f);
    }

    [Test]
    public void MeshesIShapeProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCISHAPEPROFILEDEF(.AREA.,'Beam',$,4.,6.,1.,1.,$,$,$);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,5.);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 24, expectedFaces: 44);
        AssertBounds(mesh, (-2f, -3f, 0f), (2f, 3f, 5f));
    }

    [Test]
    public void MeshesCompositeProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Outer',$,4.,3.);
            #2=IFCRECTANGLEPROFILEDEF(.AREA.,'Inner',$,2.,1.);
            #3=IFCCOMPOSITEPROFILEDEF(.AREA.,'Composite',(#1,#2));
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCEXTRUDEDAREASOLID(#3,$,#4,2.);
            """);

        var mesh = MeshRequired(model, 5);

        AssertMeshCounts(mesh, expectedPoints: 16, expectedFaces: 32);
        AssertBounds(mesh, (-2f, -1.5f, 0f), (2f, 1.5f, 2f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCCOMPOSITEPROFILEDEF"));
    }

    [Test]
    public void MeshesDisjointCompositeProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.));
            #3=IFCCARTESIANPOINT((1.,1.));
            #4=IFCCARTESIANPOINT((0.,1.));
            #5=IFCPOLYLINE((#1,#2,#3,#4,#1));
            #6=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'Left',#5);
            #7=IFCCARTESIANPOINT((2.,0.));
            #8=IFCCARTESIANPOINT((3.,0.));
            #9=IFCCARTESIANPOINT((3.,1.));
            #10=IFCCARTESIANPOINT((2.,1.));
            #11=IFCPOLYLINE((#7,#8,#9,#10,#7));
            #12=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'Right',#11);
            #13=IFCCOMPOSITEPROFILEDEF(.AREA.,'Disjoint',(#6,#12));
            #14=IFCDIRECTION((0.,0.,1.));
            #15=IFCEXTRUDEDAREASOLID(#13,$,#14,2.);
            """);

        var mesh = MeshRequired(model, 15);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (0f, 0f, 0f), (3f, 1f, 2f));
    }

    [Test]
    public void MeshesDerivedProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Parent',$,2.,2.);
            #2=IFCCARTESIANPOINT((1.,0.));
            #3=IFCCARTESIANTRANSFORMATIONOPERATOR2D($,$,#2,2.,$);
            #4=IFCDERIVEDPROFILEDEF(.AREA.,'Derived',#1,#3);
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCEXTRUDEDAREASOLID(#4,$,#5,3.);
            """);

        var mesh = MeshRequired(model, 6);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (0f, -2f, 0f), (4f, 2f, 3f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCDERIVEDPROFILEDEF"));
    }

    [Test]
    public void MeshesTrapeziumProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCTRAPEZIUMPROFILEDEF(.AREA.,'Trap',$,4.,2.,3.,1.,$);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            """);

        var mesh = MeshRequired(model, 3);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (-2f, -1.5f, 0f), (2f, 1.5f, 2f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCTRAPEZIUMPROFILEDEF"));
    }

    [Test]
    public void MeshesLShapeProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCLSHAPEPROFILEDEF(.AREA.,'Angle',$,5.,4.,1.,$,$,$);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 12, expectedFaces: 20);
        AssertBounds(mesh, (-2f, -2.5f, 0f), (2f, 2.5f, 2f));
    }

    [Test]
    public void MeshesSteelPlateLAngleColumnPlacement()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCDIRECTION((1.,0.));
            #3=IFCAXIS2PLACEMENT2D(#1,#2);
            #4=IFCLSHAPEPROFILEDEF(.AREA.,$,#3,101.6,88.900002,9.525,$,$,$,$);
            #5=IFCDIRECTION((0.,0.,-1.));
            #6=IFCDIRECTION((-1.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#8,#6,#9);
            #8=IFCCARTESIANPOINT((0.,0.,0.));
            #9=IFCDIRECTION((0.,0.,1.));
            #10=IFCEXTRUDEDAREASOLID(#4,#7,#5,254.);
            #11=IFCSHAPEREPRESENTATION(#12,'Body','SweptSolid',(#10));
            #12=IFCGEOMETRICREPRESENTATIONCONTEXT('Plan','Model',3,1.E-5,#13,$);
            #13=IFCAXIS2PLACEMENT3D(#8,#9,#14);
            #14=IFCDIRECTION((1.,0.,0.));
            #15=IFCPRODUCTDEFINITIONSHAPE($,$,(#11));
            #16=IFCCARTESIANPOINT((4794.758,7562.85,-298.45));
            #17=IFCDIRECTION((1.,0.,0.));
            #18=IFCDIRECTION((0.,0.,1.));
            #19=IFCAXIS2PLACEMENT3D(#16,#17,#18);
            #20=IFCLOCALPLACEMENT($,#19);
            #21=IFCCOLUMN('id',$,'ANGLE',$,$,#20,#15,$);
            """, lengthScaleOverride: 0.001);

        var mesh = MeshRequired(model, 21);
        AssertBounds(mesh, (4.750308f, 7.51205f, -0.29845f), (4.8392076f, 7.61365f, -0.04445f), tolerance: 0.002f);
    }

    [Test]
    public void MeshesArbitraryProfileWithVoidsUsingPolylineRings()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((4.,0.));
            #3=IFCCARTESIANPOINT((4.,3.));
            #4=IFCCARTESIANPOINT((0.,3.));
            #5=IFCPOLYLINE((#1,#2,#3,#4,#1));
            #6=IFCCARTESIANPOINT((1.,1.));
            #7=IFCCARTESIANPOINT((3.,1.));
            #8=IFCCARTESIANPOINT((3.,2.));
            #9=IFCCARTESIANPOINT((1.,2.));
            #10=IFCPOLYLINE((#6,#7,#8,#9,#6));
            #11=IFCARBITRARYPROFILEDEFWITHVOIDS(.AREA.,'PlateWithHole',#5,(#10));
            #12=IFCDIRECTION((0.,0.,1.));
            #13=IFCEXTRUDEDAREASOLID(#11,$,#12,1.);
            """);

        var mesh = MeshRequired(model, 13);

        AssertMeshCounts(mesh, expectedPoints: 16, expectedFaces: 32);
        AssertBounds(mesh, (0f, 0f, 0f), (4f, 3f, 1f));
    }

    [Test]
    public void MeshesArbitraryClosedProfileWithCompositePolylineCurve()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.));
            #3=IFCCARTESIANPOINT((2.,1.));
            #4=IFCCARTESIANPOINT((0.,1.));
            #5=IFCPOLYLINE((#1,#2));
            #6=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#5);
            #7=IFCPOLYLINE((#2,#3));
            #8=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#7);
            #9=IFCPOLYLINE((#3,#4));
            #10=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#9);
            #11=IFCPOLYLINE((#4,#1));
            #12=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#11);
            #13=IFCCOMPOSITECURVE((#6,#8,#10,#12),.F.);
            #14=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'CompositeSquare',#13);
            #15=IFCDIRECTION((0.,0.,1.));
            #16=IFCEXTRUDEDAREASOLID(#14,$,#15,3.);
            """);

        var mesh = MeshRequired(model, 16);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 1f, 3f));
    }

    [Test]
    public void MeshesArbitraryClosedProfileWithBSplineControlPolygon()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.));
            #3=IFCCARTESIANPOINT((2.,1.));
            #4=IFCCARTESIANPOINT((0.,1.));
            #5=IFCBSPLINECURVE(1,(#1,#2,#3,#4),.POLYLINE_FORM.,.T.,.F.);
            #6=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'SplineSquare',#5);
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCEXTRUDEDAREASOLID(#6,$,#7,3.);
            """);
        var mesh = MeshRequired(model, 8);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 1f, 3f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCBSPLINECURVE"));
    }

    [Test]
    public void MeshesArbitraryClosedProfileWithIndexedPolyCurve()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST2D(((0.,0.),(2.,0.),(2.,1.),(0.,1.)));
            #2=IFCINDEXEDPOLYCURVE(#1,(IFCLINEINDEX((1,2)),IFCLINEINDEX((2,3)),IFCLINEINDEX((3,4)),IFCLINEINDEX((4,1))),.F.);
            #3=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'IndexedSquare',#2);
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCEXTRUDEDAREASOLID(#3,$,#4,3.);
            """);
        var mesh = MeshRequired(model, 5);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 1f, 3f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCINDEXEDPOLYCURVE"));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCLINEINDEX"));
    }

    [Test]
    public void MeshesExampleConcreteLShapeColumnProfile()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((-151.6,-191.933333333333));
            #2=IFCCARTESIANPOINT((153.2,-191.933333333333));
            #3=IFCCARTESIANPOINT((153.2,112.866666666667));
            #4=IFCCARTESIANPOINT((-1.59999999998231,112.866666666667));
            #5=IFCCARTESIANPOINT((-1.59999999998231,79.066666666666));
            #6=IFCCARTESIANPOINT((-151.6,79.066666666666));
            #7=IFCPOLYLINE((#1,#2,#3,#4,#5,#6,#1));
            #8=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'300x300',#7);
            #9=IFCDIRECTION((0.,0.,1.));
            #10=IFCEXTRUDEDAREASOLID(#8,$,#9,3.);
            """, lengthScaleOverride: 0.001);

        var mesh = MeshRequired(model, 10);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        Assert.That(mesh.Points, Has.Count.GreaterThanOrEqualTo(8));
    }

    [Test]
    public void MeshesSteelSectionProfileWithTrimmedFilletArc()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((138.6,-152.5));
            #2=IFCCARTESIANPOINT((154.,-152.5));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#3);
            #5=IFCCARTESIANPOINT((154.,152.5));
            #6=IFCPOLYLINE((#2,#5));
            #7=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#6);
            #8=IFCCARTESIANPOINT((138.6,21.4500000000001));
            #9=IFCPOLYLINE((#5,#8));
            #10=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#9);
            #11=IFCCARTESIANPOINT((122.1,21.45));
            #12=IFCAXIS2PLACEMENT2D(#11,#13);
            #13=IFCDIRECTION((1.,0.));
            #14=IFCCIRCLE(#12,16.5);
            #15=IFCTRIMMEDCURVE(#14,(IFCPARAMETERVALUE(270.)),(IFCPARAMETERVALUE(0.)),.T.,.PARAMETER.);
            #16=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.F.,#15);
            #17=IFCCARTESIANPOINT((122.1,4.95000000000002));
            #18=IFCCARTESIANPOINT((-122.1,4.95000000000002));
            #19=IFCPOLYLINE((#17,#18));
            #20=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#19);
            #21=IFCCOMPOSITECURVE((#4,#7,#10,#16,#20),.F.);
            #22=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'PartialUC',#21);
            #23=IFCDIRECTION((0.,0.,1.));
            #24=IFCEXTRUDEDAREASOLID(#22,$,#23,3.);
            """, lengthScaleOverride: 0.001, circleSegments: 16);

        var mesh = MeshRequired(model, 24);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
    }

    [Test]
    public void MeshesExampleLAngleColumnProfileExtrusion()
    {
        using var model = MicroIfc.Parse("""
            #23=IFCDIRECTION((1.,0.));
            #27=IFCDIRECTION((0.,1.));
            #1=IFCCARTESIANPOINT((-42.6357142857139,-20.8642857142859));
            #2=IFCCARTESIANPOINT((20.8642857142861,-20.8642857142859));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#3);
            #5=IFCCARTESIANPOINT((20.8642857142861,-20.8642857142859));
            #6=IFCCARTESIANPOINT((20.8642857142856,42.6357142857144));
            #7=IFCPOLYLINE((#5,#6));
            #8=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#7);
            #9=IFCCARTESIANPOINT((20.8642857142851,36.2857142857144));
            #10=IFCAXIS2PLACEMENT2D(#9,#27);
            #11=IFCCIRCLE(#10,6.35);
            #12=IFCTRIMMEDCURVE(#11,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(90.0000000000052)),.T.,.PARAMETER.);
            #13=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#12);
            #14=IFCCARTESIANPOINT((14.5142857142851,36.2857142857143));
            #15=IFCCARTESIANPOINT((14.5142857142855,-8.16428571428543));
            #16=IFCPOLYLINE((#14,#15));
            #17=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#16);
            #18=IFCCARTESIANPOINT((8.1642857142855,-8.16428571428549));
            #19=IFCAXIS2PLACEMENT2D(#18,#23);
            #20=IFCCIRCLE(#19,6.35);
            #21=IFCTRIMMEDCURVE(#20,(IFCPARAMETERVALUE(270.)),(IFCPARAMETERVALUE(0.)),.T.,.PARAMETER.);
            #22=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.F.,#21);
            #30=IFCCARTESIANPOINT((8.16428571428557,-14.5142857142855));
            #31=IFCCARTESIANPOINT((-36.2857142857139,-14.5142857142859));
            #32=IFCPOLYLINE((#30,#31));
            #33=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#32);
            #34=IFCCARTESIANPOINT((-36.2857142857139,-20.8642857142859));
            #35=IFCAXIS2PLACEMENT2D(#34,#27);
            #36=IFCCIRCLE(#35,6.35000000000001);
            #37=IFCTRIMMEDCURVE(#36,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(90.0000000000002)),.T.,.PARAMETER.);
            #38=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#37);
            #39=IFCCOMPOSITECURVE((#4,#8,#13,#17,#22,#33,#38),.F.);
            #40=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'L2-1/2X2-1/2X1/4',#39);
            #41=IFCDIRECTION((0.,0.,1.));
            #42=IFCEXTRUDEDAREASOLID(#40,$,#41,1.29);
            """, lengthScaleOverride: 0.001);

        var mesh = MeshRequired(model, 42);
        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
    }

    [Test]
    public void ReadsTrimmedCircleCurveAsSampledArc()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCAXIS2PLACEMENT2D(#1,$);
            #3=IFCCIRCLE(#2,2.);
            #4=IFCTRIMMEDCURVE(#3,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(90.)),.T.,.PARAMETER.);
            """, circleSegments: 8);

        var points = CurveEvaluator.Evaluate2D(model.Context, model.Entity(4));

        Assert.Multiple(() =>
        {
            Assert.That(points, Has.Count.EqualTo(3));
            Assert.That(points[0].X.Value, Is.EqualTo(2f).Within(1e-5f));
            Assert.That(points[0].Y.Value, Is.EqualTo(0f).Within(1e-5f));
            Assert.That(points[^1].X.Value, Is.EqualTo(0f).Within(1e-5f));
            Assert.That(points[^1].Y.Value, Is.EqualTo(2f).Within(1e-5f));
        });
    }

    [Test]
    public void AppliesAxis2Placement3DToExtrudedSolid()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCCARTESIANPOINT((10.,20.,30.));
            #3=IFCAXIS2PLACEMENT3D(#2,$,$);
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCEXTRUDEDAREASOLID(#1,#3,#4,4.);
            """);

        var mesh = MeshRequired(model, 5);

        AssertBounds(mesh, (9f, 19f, 30f), (11f, 21f, 34f));
    }

    [Test]
    public void MeshesProductShapeThroughLocalPlacement()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((100.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('wall-guid',$,'Wall',$,$,#8,#5,$);
            """);

        var mesh = MeshRequired(model, 9);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (99f, -1f, 0f), (101f, 1f, 4f));
    }

    [Test]
    public void MeshesMappedItemWithCartesianTransformationOperator3D()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,3.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCCARTESIANPOINT((0.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCREPRESENTATIONMAP(#6,#4);
            #8=IFCCARTESIANPOINT((10.,20.,30.));
            #9=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#8,2.,$);
            #10=IFCMAPPEDITEM(#7,#9);
            #11=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#10));
            """);

        var mesh = MeshRequired(model, 11);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (8f, 17f, 30f), (12f, 23f, 38f));
    }

    [Test]
    public void MeshesMappedItemWithNonUniformCartesianTransformationOperator3D()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,3.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCCARTESIANPOINT((0.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCREPRESENTATIONMAP(#6,#4);
            #8=IFCCARTESIANPOINT((10.,20.,30.));
            #9=IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM($,$,#8,2.,$,3.,4.);
            #10=IFCMAPPEDITEM(#7,#9);
            #11=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#10));
            """);

        var mesh = MeshRequired(model, 11);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (8f, 15.5f, 30f), (12f, 24.5f, 46f));
    }

    [Test]
    public void MeshesMappedItemWithNonIdentityMappingOrigin()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCCARTESIANPOINT((100.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCREPRESENTATIONMAP(#6,#4);
            #8=IFCCARTESIANPOINT((0.,0.,0.));
            #9=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#8,1.,$);
            #10=IFCMAPPEDITEM(#7,#9);
            #11=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#10));
            """);

        var mesh = MeshRequired(model, 11);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (99f, -1f, 0f), (101f, 1f, 4f));
    }

    [Test]
    public void MappedItemWithMultipleBodyItems_InstancesPerPart()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'A',$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            #4=IFCRECTANGLEPROFILEDEF(.AREA.,'B',$,1.,1.);
            #5=IFCEXTRUDEDAREASOLID(#4,$,#2,1.);
            #6=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3,#5));
            #7=IFCCARTESIANPOINT((0.,0.,0.));
            #8=IFCAXIS2PLACEMENT3D(#7,$,$);
            #9=IFCREPRESENTATIONMAP(#8,#6);
            #10=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#7,$,$);
            #11=IFCMAPPEDITEM(#9,#10);
            #12=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#11));
            #13=IFCPRODUCTDEFINITIONSHAPE($,$,(#12));
            #14=IFCLOCALPLACEMENT($,#8);
            #15=IFCMEMBER('g',$,'M',$,$,#14,#13,$);
            """);

        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(2));
        Assert.That(built.Instances.Select(i => i.EntityIndex).Distinct(), Is.EqualTo(new[] { 15 }));
    }

    [Test]
    public void FaceBasedSurfaceModel_EmitsInstancePerConnectedFaceSet()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCPOLYLOOP((#1,#2,#3,#4));
            #6=IFCFACEOUTERBOUND(#5,.T.);
            #7=IFCFACE((#6));
            #8=IFCCONNECTEDFACESET((#7));
            #9=IFCCARTESIANPOINT((2.,0.,0.));
            #10=IFCCARTESIANPOINT((3.,0.,0.));
            #11=IFCCARTESIANPOINT((3.,1.,0.));
            #12=IFCCARTESIANPOINT((2.,1.,0.));
            #13=IFCPOLYLOOP((#9,#10,#11,#12));
            #14=IFCFACEOUTERBOUND(#13,.T.);
            #15=IFCFACE((#14));
            #16=IFCCONNECTEDFACESET((#15));
            #17=IFCFACEBASEDSURFACEMODEL((#8,#16));
            #18=IFCSHAPEREPRESENTATION($,'Body','SurfaceModel',(#17));
            #19=IFCPRODUCTDEFINITIONSHAPE($,$,(#18));
            #20=IFCCARTESIANPOINT((0.,0.,0.));
            #21=IFCAXIS2PLACEMENT3D(#20,$,$);
            #22=IFCLOCALPLACEMENT($,#21);
            #23=IFCMEMBER('g',$,'M',$,$,#22,#19,$);
            """);

        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(2));
        Assert.That(built.Instances.Select(i => i.EntityIndex).Distinct(), Is.EqualTo(new[] { 23 }));
    }

    [Test]
    public void MeshesProductThroughNestedLocalPlacementChain()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCCARTESIANPOINT((10.,0.,0.));
            #10=IFCAXIS2PLACEMENT3D(#9,$,$);
            #11=IFCLOCALPLACEMENT(#8,#10);
            #12=IFCCARTESIANPOINT((5.,0.,0.));
            #13=IFCAXIS2PLACEMENT3D(#12,$,$);
            #14=IFCLOCALPLACEMENT(#11,#13);
            #15=IFCWALL('g',$,'Wall',$,$,#14,#5,$);
            """);

        var mesh = ModelAssembler.BuildEntityMesh(model.Context, model.Entity(15));
        Assert.That(mesh, Is.Not.Null);
        AssertBounds(mesh!.Value, (14f, -1f, 0f), (16f, 1f, 4f));
    }

    [Test]
    public void MeshesFacetedBrepClosedShellCube()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCCARTESIANPOINT((0.,0.,1.));
            #6=IFCCARTESIANPOINT((1.,0.,1.));
            #7=IFCCARTESIANPOINT((1.,1.,1.));
            #8=IFCCARTESIANPOINT((0.,1.,1.));
            #10=IFCPOLYLOOP((#1,#4,#3,#2));
            #11=IFCFACEOUTERBOUND(#10,.T.);
            #12=IFCFACE((#11));
            #13=IFCPOLYLOOP((#5,#6,#7,#8));
            #14=IFCFACEOUTERBOUND(#13,.T.);
            #15=IFCFACE((#14));
            #16=IFCPOLYLOOP((#1,#2,#6,#5));
            #17=IFCFACEOUTERBOUND(#16,.T.);
            #18=IFCFACE((#17));
            #19=IFCPOLYLOOP((#2,#3,#7,#6));
            #20=IFCFACEOUTERBOUND(#19,.T.);
            #21=IFCFACE((#20));
            #22=IFCPOLYLOOP((#3,#4,#8,#7));
            #23=IFCFACEOUTERBOUND(#22,.T.);
            #24=IFCFACE((#23));
            #25=IFCPOLYLOOP((#4,#1,#5,#8));
            #26=IFCFACEOUTERBOUND(#25,.T.);
            #27=IFCFACE((#26));
            #28=IFCCLOSEDSHELL((#12,#15,#18,#21,#24,#27));
            #29=IFCFACETEDBREP(#28);
            """);

        var mesh = MeshRequired(model, 29);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 1f));
    }

    [Test]
    public void MeshesFacetedBrepConcaveFace()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((2.,1.,0.));
            #4=IFCCARTESIANPOINT((1.,1.,0.));
            #5=IFCCARTESIANPOINT((1.,2.,0.));
            #6=IFCCARTESIANPOINT((0.,2.,0.));
            #7=IFCPOLYLOOP((#1,#2,#3,#4,#5,#6));
            #8=IFCFACEOUTERBOUND(#7,.T.);
            #9=IFCFACE((#8));
            #10=IFCOPENEDSHELL((#9));
            #11=IFCSHELLBASEDSURFACEMODEL((#10));
            """);

        var mesh = MeshRequired(model, 11);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 2f, 0f));
    }

    [Test]
    public void MeshesAdvancedBrepWithPlanarAdvancedFaces()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCCARTESIANPOINT((0.,0.,1.));
            #6=IFCCARTESIANPOINT((1.,0.,1.));
            #7=IFCCARTESIANPOINT((1.,1.,1.));
            #8=IFCCARTESIANPOINT((0.,1.,1.));
            #9=IFCCARTESIANPOINT((0.,0.,0.));
            #10=IFCAXIS2PLACEMENT3D(#9,$,$);
            #11=IFCPLANE(#10);
            #12=IFCPOLYLOOP((#1,#4,#3,#2));
            #13=IFCFACEOUTERBOUND(#12,.T.);
            #14=IFCADVANCEDFACE((#13),#11,.T.);
            #15=IFCPOLYLOOP((#5,#6,#7,#8));
            #16=IFCFACEOUTERBOUND(#15,.T.);
            #17=IFCADVANCEDFACE((#16),#11,.T.);
            #18=IFCPOLYLOOP((#1,#2,#6,#5));
            #19=IFCFACEOUTERBOUND(#18,.T.);
            #20=IFCADVANCEDFACE((#19),#11,.T.);
            #21=IFCPOLYLOOP((#2,#3,#7,#6));
            #22=IFCFACEOUTERBOUND(#21,.T.);
            #23=IFCADVANCEDFACE((#22),#11,.T.);
            #24=IFCPOLYLOOP((#3,#4,#8,#7));
            #25=IFCFACEOUTERBOUND(#24,.T.);
            #26=IFCADVANCEDFACE((#25),#11,.T.);
            #27=IFCPOLYLOOP((#4,#1,#5,#8));
            #28=IFCFACEOUTERBOUND(#27,.T.);
            #29=IFCADVANCEDFACE((#28),#11,.T.);
            #30=IFCCLOSEDSHELL((#14,#17,#20,#23,#26,#29));
            #31=IFCADVANCEDBREP(#30);
            """);
        var mesh = MeshRequired(model, 31);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 12);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 1f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCADVANCEDFACE"));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCPLANE"));
    }

    [Test]
    public void MeshesAdvancedBrepWithEdgeLoopBounds()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #10=IFCAXIS2PLACEMENT3D(#1,$,$);
            #11=IFCPLANE(#10);
            #12=IFCVERTEXPOINT(#1);
            #13=IFCVERTEXPOINT(#2);
            #14=IFCVERTEXPOINT(#3);
            #15=IFCVERTEXPOINT(#4);
            #20=IFCPOLYLINE((#1,#2));
            #21=IFCPOLYLINE((#2,#3));
            #22=IFCPOLYLINE((#3,#4));
            #23=IFCPOLYLINE((#4,#1));
            #30=IFCEDGECURVE(#12,#13,#20,.T.);
            #31=IFCEDGECURVE(#13,#14,#21,.T.);
            #32=IFCEDGECURVE(#14,#15,#22,.T.);
            #33=IFCEDGECURVE(#15,#12,#23,.T.);
            #40=IFCORIENTEDEDGE(*,*,#30,.T.);
            #41=IFCORIENTEDEDGE(*,*,#31,.T.);
            #42=IFCORIENTEDEDGE(*,*,#32,.T.);
            #43=IFCORIENTEDEDGE(*,*,#33,.T.);
            #50=IFCEDGELOOP((#40,#41,#42,#43));
            #51=IFCFACEOUTERBOUND(#50,.T.);
            #52=IFCADVANCEDFACE((#51),#11,.T.);
            #53=IFCCLOSEDSHELL((#52));
            #54=IFCADVANCEDBREP(#53);
            """);

        var mesh = MeshRequired(model, 54);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 0f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCEDGELOOP"));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCEDGECURVE"));
    }

    [Test]
    public void MeshesCurveBoundedPlaneAdvancedFace()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((2.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #10=IFCAXIS2PLACEMENT3D(#1,$,$);
            #11=IFCPLANE(#10);
            #12=IFCPOLYLINE((#1,#2,#3,#4,#1));
            #13=IFCCURVEBOUNDEDPLANE(#11,#12,$);
            #20=IFCVERTEXPOINT(#1);
            #21=IFCVERTEXPOINT(#2);
            #22=IFCVERTEXPOINT(#3);
            #23=IFCVERTEXPOINT(#4);
            #30=IFCPOLYLINE((#1,#2));
            #31=IFCPOLYLINE((#2,#3));
            #32=IFCPOLYLINE((#3,#4));
            #33=IFCPOLYLINE((#4,#1));
            #40=IFCEDGECURVE(#20,#21,#30,.T.);
            #41=IFCEDGECURVE(#21,#22,#31,.T.);
            #42=IFCEDGECURVE(#22,#23,#32,.T.);
            #43=IFCEDGECURVE(#23,#20,#33,.T.);
            #50=IFCORIENTEDEDGE(*,*,#40,.T.);
            #51=IFCORIENTEDEDGE(*,*,#41,.T.);
            #52=IFCORIENTEDEDGE(*,*,#42,.T.);
            #53=IFCORIENTEDEDGE(*,*,#43,.T.);
            #60=IFCEDGELOOP((#50,#51,#52,#53));
            #61=IFCFACEOUTERBOUND(#60,.T.);
            #62=IFCADVANCEDFACE((#61),#13,.T.);
            #63=IFCCLOSEDSHELL((#62));
            #64=IFCADVANCEDBREP(#63);
            """);

        var mesh = MeshRequired(model, 64);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (0f, 0f, 0f), (2f, 1f, 0f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCCURVEBOUNDEDPLANE"));
    }

    [Test]
    [Category("Slow")]
    public void DigitalHub_BicycleProxyAdvancedBrep_Builds()
    {
        var ifcPath = new FilePath(@"c:\Users\cdigg\git\studio\data\FM_ARC_DigitalHub.ifc");
        if (!ifcPath.Exists())
            Assert.Ignore($"Missing IFC test file: {ifcPath}");

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var ctx = new MeshingContext(file);
        var brep = ctx.GetEntity(135371);
        var mesh = Brep.BuildAdvancedBrep(ctx, brep);

        TestContext.WriteLine($"Bicycle brep #135371: {mesh.Points.Count} pts, {mesh.FaceIndices.Count} tris");
        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(100));
        Assert.That(ctx.Diagnostics.EntityStatus.Keys, Does.Contain("IFCEDGELOOP"));
    }

    [Test]
    public void MeshesFaceBasedSurfaceModel()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCPOLYLOOP((#1,#2,#3,#4));
            #6=IFCFACEOUTERBOUND(#5,.T.);
            #7=IFCFACE((#6));
            #8=IFCCONNECTEDFACESET((#7));
            #9=IFCFACEBASEDSURFACEMODEL((#8));
            """);

        var mesh = MeshRequired(model, 9);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 0f));
    }

    [Test]
    public void MeshesFaceWithInnerBoundHole()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((4.,0.,0.));
            #3=IFCCARTESIANPOINT((4.,4.,0.));
            #4=IFCCARTESIANPOINT((0.,4.,0.));
            #5=IFCCARTESIANPOINT((1.,1.,0.));
            #6=IFCCARTESIANPOINT((3.,1.,0.));
            #7=IFCCARTESIANPOINT((3.,3.,0.));
            #8=IFCCARTESIANPOINT((1.,3.,0.));
            #9=IFCPOLYLOOP((#1,#2,#3,#4));
            #10=IFCFACEOUTERBOUND(#9,.T.);
            #11=IFCPOLYLOOP((#5,#6,#7,#8));
            #12=IFCFACEBOUND(#11,.T.);
            #13=IFCFACE((#10,#12));
            """);

        var mesh = MeshRequired(model, 13);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 8);
        AssertBounds(mesh, (0f, 0f, 0f), (4f, 4f, 0f));
    }

    [Test]
    public void MeshesShellBasedSurfaceModel()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.,0.));
            #3=IFCCARTESIANPOINT((1.,1.,0.));
            #4=IFCCARTESIANPOINT((0.,1.,0.));
            #5=IFCPOLYLOOP((#1,#2,#3,#4));
            #6=IFCFACEOUTERBOUND(#5,.T.);
            #7=IFCFACE((#6));
            #8=IFCOPENEDSHELL((#7));
            #9=IFCSHELLBASEDSURFACEMODEL((#8));
            """);

        var mesh = MeshRequired(model, 9);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 0f));
    }

    [Test]
    public void MeshesTriangulatedFaceSet()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,0.),(1.,0.,0.),(0.,1.,0.),(0.,0.,1.)));
            #2=IFCTRIANGULATEDFACESET(#1,$,.T.,((1,2,3),(1,4,2),(2,4,3),(3,4,1)),$);
            """);

        var mesh = MeshRequired(model, 2);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 4);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 1f));
    }

    [Test]
    public void MeshesPolygonalFaceSet()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,0.),(1.,0.,0.),(1.,1.,0.),(0.,1.,0.)));
            #2=IFCINDEXEDPOLYGONALFACE((1,2,3,4));
            #3=IFCPOLYGONALFACESET(#1,.F.,(#2),$);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (1f, 1f, 0f));
    }

    [Test]
    public void MeshesPolygonalFaceSetWithVoids()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,0.),(4.,0.,0.),(4.,4.,0.),(0.,4.,0.),(1.,1.,0.),(3.,1.,0.),(3.,3.,0.),(1.,3.,0.)));
            #2=IFCINDEXEDPOLYGONALFACEWITHVOIDS((1,2,3,4),((5,6,7,8)));
            #3=IFCPOLYGONALFACESET(#1,.F.,(#2),$);
            """);

        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 8, expectedFaces: 8);
        AssertBounds(mesh, (0f, 0f, 0f), (4f, 4f, 0f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCINDEXEDPOLYGONALFACEWITHVOIDS"));
    }

    [Test]
    public void MeshesDuplexStyleOpenProfileRibbon()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.2084999999999992,-17.59149999999997));
            #2=IFCCARTESIANPOINT((6.261999999999995,-17.59149999999999));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCARBITRARYOPENPROFILEDEF(.CURVE.,$,#3);
            #5=IFCCARTESIANPOINT((0.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCSURFACEOFLINEAREXTRUSION(#4,#6,#7,2.6);
            """);

        var mesh = MeshRequired(model, 8);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0.2085f, -17.5915f, 0f), (6.262f, -17.5915f, 2.6f), tolerance: 1e-3f);
    }

    [Test]
    public void MeshesSurfaceOfLinearExtrusionOpenProfile()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((4.,0.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCARBITRARYOPENPROFILEDEF(.CURVE.,'Path',#3);
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCSURFACEOFLINEAREXTRUSION(#4,$,#5,2.);
            """);

        var mesh = MeshRequired(model, 6);

        AssertMeshCounts(mesh, expectedPoints: 4, expectedFaces: 2);
        AssertBounds(mesh, (0f, 0f, 0f), (4f, 0f, 2f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCSURFACEOFLINEAREXTRUSION"));
    }

    [Test]
    public void MeshesSweptDiskSolidAlongStraightPolyline()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((0.,0.,5.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCSWEPTDISKSOLID(#3,1.,$,$,$);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 4);

        AssertMeshCounts(mesh, expectedPoints: 24, expectedFaces: 48);
        AssertBounds(mesh, (-1f, -1f, 0f), (1f, 1f, 5f), tolerance: 1e-5f);
    }

    [Test]
    public void MeshesSweptDiskSolidWithInnerRadius()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((0.,0.,5.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCSWEPTDISKSOLID(#3,2.,0.5,$,$);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 4);

        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(96));
        AssertBounds(mesh, (-2f, -2f, 0f), (2f, 2f, 5f), tolerance: 1e-5f);
    }

    [Test]
    public void MeshesSweptDiskSolidAlongBSplineControlPolygon()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((0.,0.,5.));
            #3=IFCBSPLINECURVE(1,(#1,#2),.POLYLINE_FORM.,.F.,.F.);
            #4=IFCSWEPTDISKSOLID(#3,1.,$,$,$);
            """, circleSegments: 12);
        var mesh = MeshRequired(model, 4);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        AssertBounds(mesh, (-1f, -1f, 0f), (1f, 1f, 5f), tolerance: 1e-5f);
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCBSPLINECURVE"));
    }

    [Test]
    public void MeshesSweptDiskSolidAlongIndexedPolyCurve()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,0.),(0.,0.,5.)));
            #2=IFCINDEXEDPOLYCURVE(#1,$,.F.);
            #3=IFCSWEPTDISKSOLID(#2,1.,$,$,$);
            """, circleSegments: 12);
        var mesh = MeshRequired(model, 3);

        AssertMeshCounts(mesh, expectedPoints: 24, expectedFaces: 48);
        AssertBounds(mesh, (-1f, -1f, 0f), (1f, 1f, 5f), tolerance: 1e-5f);
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCINDEXEDPOLYCURVE"));
    }

    [Test]
    public void MeshesRevolvedAreaSolidPartialAngleWithEndCap()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((2.,0.));
            #2=IFCAXIS2PLACEMENT2D(#1,$);
            #3=IFCRECTANGLEPROFILEDEF(.AREA.,'Arm',#2,1.,2.);
            #4=IFCCARTESIANPOINT((0.,0.,0.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS1PLACEMENT(#4,#5);
            #7=IFCREVOLVEDAREASOLID(#3,$,#6,1.5707963267948966);
            """, circleSegments: 8);

        var mesh = MeshRequired(model, 7);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(8));
        var maxExtent = mesh.Points.Max(p => MathF.Sqrt(p.X.Value * p.X.Value + p.Y.Value * p.Y.Value));
        Assert.That(maxExtent, Is.EqualTo(MathF.Sqrt(2.5f * 2.5f + 1f * 1f)).Within(1e-3f));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCREVOLVEDAREASOLID"));
    }

    [Test]
    public void MeshesRevolvedAreaSolidAroundAxis()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((2.,0.));
            #2=IFCAXIS2PLACEMENT2D(#1,$);
            #3=IFCRECTANGLEPROFILEDEF(.AREA.,'RingProfile',#2,1.,2.);
            #4=IFCCARTESIANPOINT((0.,0.,0.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS1PLACEMENT(#4,#5);
            #7=IFCREVOLVEDAREASOLID(#3,$,#6,6.283185307179586);
            """, circleSegments: 12);

        var mesh = MeshRequired(model, 7);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        Assert.That(mesh.Points, Has.Count.GreaterThan(8));
        Assert.That(mesh.Points.Count(p => float.IsNaN(p.X.Value)), Is.EqualTo(0));
    }

    [Test]
    public void UnsupportedRepresentationItemsAreReportedWithoutFailingWholeRepresentation()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCBOOLEANRESULT(.UNION.,#3,#3);
            #5=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3,#4));
            """);
        var mesh = MeshRequired(model, 5);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        Assert.That(GetUnsupported(model), Does.Contain("IFCBOOLEANRESULT"));
    }

    [Test]
    public void MeshesBooleanClippingResultWithPlanarHalfSpace()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,.T.);
            #9=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#8);
            """);

        var solid = MeshRequired(model, 3);
        var mesh = MeshRequired(model, 9);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        Assert.That(Math.Abs(MeshHelpers.SignedVolume(mesh)), Is.LessThan(Math.Abs(MeshHelpers.SignedVolume(solid))));
        AssertBounds(mesh, (-1f, -1f, 0f), (1f, 1f, 2f), tolerance: 1e-4f);
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCHALFSPACESOLID"));
    }

    [Test]
    public void MeshesBooleanClippingResultKeepsOppositeHalfSpaceWhenAgreementFalse()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,.F.);
            #9=IFCBOOLEANCLIPPINGRESULT(.DIFFERENCE.,#3,#8);
            """);

        var mesh = MeshRequired(model, 9);
        AssertBounds(mesh, (-1f, -1f, 2f), (1f, 1f, 4f), tolerance: 1e-4f);
    }

    [Test]
    public void MeshesExtrusionEndBooleanClipAlongDepth()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,0.2,0.2);
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

        var mesh = MeshRequired(model, 10);
        AssertBounds(mesh, (-0.1f, -0.1f, -1f), (0.1f, 0.1f, 0f), tolerance: 1e-4f);
    }

    [Test]
    public void MeshesNestedObliqueGableBooleanClips()
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

        var mesh = MeshRequired(model, 22);
        AssertBounds(mesh, (0f, 0f, 0f), (0.36f, 5f, 5.5f), tolerance: 0.05f);
    }

    [Test]
    public void MeshesBooleanResultDifferenceWithHalfSpace()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCCARTESIANPOINT((0.,0.,2.));
            #5=IFCDIRECTION((0.,0.,1.));
            #6=IFCAXIS2PLACEMENT3D(#4,$,#5);
            #7=IFCPLANE(#6);
            #8=IFCHALFSPACESOLID(#7,.T.);
            #9=IFCBOOLEANRESULT(.DIFFERENCE.,#3,#8);
            """);

        var solid = MeshRequired(model, 3);
        var mesh = MeshRequired(model, 9);

        Assert.That(mesh.FaceIndices, Has.Count.GreaterThan(0));
        Assert.That(Math.Abs(MeshHelpers.SignedVolume(mesh)), Is.LessThan(Math.Abs(MeshHelpers.SignedVolume(solid))));
        Assert.That(model.Context.Diagnostics.EntityStatus.Keys, Does.Contain("IFCBOOLEANRESULT"));
    }

    [Test]
    public void BacklogScannerListsEncounteredGeometryCreationItems()
    {
        using var temp = new TemporaryIfcFile("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'BoxProfile',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCBOOLEANRESULT(.UNION.,#3,#3);
            """);

        var encountered = GeometryCreationBacklog.Scan(temp.Path);

        Assert.Multiple(() =>
        {
            Assert.That(encountered.Select(e => e.EntityName), Does.Contain("IFCEXTRUDEDAREASOLID"));
            Assert.That(encountered.Select(e => e.EntityName), Does.Contain("IFCBOOLEANRESULT"));
            Assert.That(encountered.Single(e => e.EntityName == "IFCBOOLEANRESULT").Support, Is.EqualTo(GeometryCreationSupport.Partial));
        });
    }

    [Test]
    [Explicit("Exploratory scanner for building the geometry creation backlog from local IFC corpora.")]
    public void ScanExternalIfcSamplesForGeometryCreationBacklog()
    {
        var files = new[]
        {
            new FilePath(@"C:\Users\cdigg\git\ifc-sharp\engine_web-ifc\tests\ifcfiles\public\example.ifc"),
            new FilePath(@"C:\Users\cdigg\git\3d-format-shootout\data\git-repo-copies\speckle\ifcs\steelplates.ifc"),
        };

        var encountered = GeometryCreationBacklog.Scan(files);

        TestContext.WriteLine("Encountered IFC geometry creation items:");
        foreach (var item in encountered)
            TestContext.WriteLine($"{item.EntityName}: {item.Count} [{item.Support}] {item.Notes}");

        Assert.That(encountered, Is.Not.Empty);
    }

    static void AssertMeshCounts(TriangleMesh3D mesh, int expectedPoints, int expectedFaces)
    {
        Assert.Multiple(() =>
        {
            Assert.That(mesh.Points, Has.Count.EqualTo(expectedPoints));
            Assert.That(mesh.FaceIndices, Has.Count.EqualTo(expectedFaces));
        });
    }

    static void AssertBounds(
        TriangleMesh3D mesh,
        (float X, float Y, float Z) expectedMin,
        (float X, float Y, float Z) expectedMax,
        float tolerance = 1e-6f)
    {
        var min = (
            X: mesh.Points.Min(p => p.X.Value),
            Y: mesh.Points.Min(p => p.Y.Value),
            Z: mesh.Points.Min(p => p.Z.Value));
        var max = (
            X: mesh.Points.Max(p => p.X.Value),
            Y: mesh.Points.Max(p => p.Y.Value),
            Z: mesh.Points.Max(p => p.Z.Value));

        Assert.Multiple(() =>
        {
            Assert.That(min.X, Is.EqualTo(expectedMin.X).Within(tolerance));
            Assert.That(min.Y, Is.EqualTo(expectedMin.Y).Within(tolerance));
            Assert.That(min.Z, Is.EqualTo(expectedMin.Z).Within(tolerance));
            Assert.That(max.X, Is.EqualTo(expectedMax.X).Within(tolerance));
            Assert.That(max.Y, Is.EqualTo(expectedMax.Y).Within(tolerance));
            Assert.That(max.Z, Is.EqualTo(expectedMax.Z).Within(tolerance));
        });
    }

    static TriangleMesh3D MeshRequired(MicroIfcModel model, int entityId)
    {
        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(entityId));
        if (mesh is null)
        {
            var notes = model.Context.Diagnostics.Messages.TakeLast(3);
            var detail = notes.Any() ? string.Join("; ", notes) : "no diagnostics";
            throw new InvalidOperationException($"Could not mesh entity #{entityId}: {detail}");
        }
        return mesh.Value;
    }

    static HashSet<string> GetUnsupported(MicroIfcModel model)
        => model.Context.Diagnostics.EntityStatus
            .Where(kv => kv.Value == GeometrySupportStatus.Unsupported)
            .Select(kv => kv.Key)
            .ToHashSet(StringComparer.Ordinal);

    sealed class TemporaryIfcFile : IDisposable
    {
        public TemporaryIfcFile(string content)
        {
            Path = new FilePath(System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ara3d-golden-ifc-{Guid.NewGuid():N}.ifc"));
            File.WriteAllText(Path, MicroIfc.WrapData(content));
        }

        public FilePath Path { get; }

        public void Dispose()
        {
            try { File.Delete(Path); }
            catch (IOException) { }
        }
    }
}