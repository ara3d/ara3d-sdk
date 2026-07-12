using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcTypes;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
[Category("IfcMesherCorrectness")]
public sealed class CurveTests
{
    [Test]
    public void Polyline2D_ReturnsAllPoints()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.));
            #3=IFCCARTESIANPOINT((1.,1.));
            #4=IFCPOLYLINE((#1,#2,#3));
            """);

        var ctx = model.Context;
        var pts = CurveEvaluator.Evaluate2D(ctx, model.Entity(4));
        Assert.That(pts, Has.Count.EqualTo(3));
    }

    [Test]
    public void ExampleLShapeProfile_IsSimplePolygon()
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
            """, lengthScaleOverride: 0.001);

        var points = CurveEvaluator.Evaluate2D(model.Context, model.Entity(7), dropClosure: true);
        var cleaned = PolygonWithHoles.CleanRing(points);

        Assert.That(PolygonTriangulator.HasSelfIntersection(cleaned), Is.False);
    }

    [Test]
    public void CompositeCurve_HonorsSameSense()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.F.,#3);
            #5=IFCCOMPOSITECURVE((#4),.F.);
            """);

        var ctx = model.Context;
        var pts = CurveEvaluator.Evaluate2D(ctx, model.Entity(5));
        Assert.That(pts, Has.Count.EqualTo(2));
        Assert.That((float)pts[0].X, Is.EqualTo(1f).Within(1e-5f));
        Assert.That((float)pts[^1].X, Is.EqualTo(0f).Within(1e-5f));
    }

    [Test]
    public void TrimmedCircle_UsesParameterRadians()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCAXIS2PLACEMENT2D(#1,$);
            #3=IFCCIRCLE(#2,1.);
            #4=IFCTRIMMEDCURVE(#3,(0.),(1.5707963267948966),.T.);
            """);

        var ctx = model.Context;
        var pts = CurveEvaluator.Evaluate2D(ctx, model.Entity(4));
        Assert.That(pts, Has.Count.GreaterThan(1));
        Assert.That((float)pts[0].X, Is.EqualTo(1f).Within(0.05f));
        Assert.That((float)pts[^1].Y, Is.EqualTo(1f).Within(0.05f));
    }

    [Test]
    public void ExampleMirroredLAngleColumnProfile_IsSimplePolygon()
    {
        using var model = MicroIfc.Parse("""
            #23=IFCDIRECTION((1.,0.));
            #27=IFCDIRECTION((0.,1.));
            #29=IFCDIRECTION((0.,-1.));
            #1=IFCCARTESIANPOINT((-20.8642857142861,-20.8642857142859));
            #2=IFCCARTESIANPOINT((42.6357142857139,-20.8642857142859));
            #3=IFCPOLYLINE((#1,#2));
            #4=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#3);
            #5=IFCCARTESIANPOINT((36.2857142857139,-20.8642857142859));
            #6=IFCAXIS2PLACEMENT2D(#5,#23);
            #7=IFCCIRCLE(#6,6.35);
            #8=IFCTRIMMEDCURVE(#7,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(90.0000000000003)),.T.,.PARAMETER.);
            #9=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#8);
            #10=IFCCARTESIANPOINT((36.2857142857139,-14.5142857142859));
            #11=IFCCARTESIANPOINT((-8.16428571428556,-14.5142857142855));
            #12=IFCPOLYLINE((#10,#11));
            #13=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#12);
            #14=IFCCARTESIANPOINT((-8.16428571428549,-8.16428571428547));
            #15=IFCAXIS2PLACEMENT2D(#14,#29);
            #16=IFCCIRCLE(#15,6.35000000000001);
            #17=IFCTRIMMEDCURVE(#16,(IFCPARAMETERVALUE(270.)),(IFCPARAMETERVALUE(0.)),.T.,.PARAMETER.);
            #18=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.F.,#17);
            #19=IFCCARTESIANPOINT((-14.5142857142855,-8.16428571428541));
            #20=IFCCARTESIANPOINT((-14.5142857142851,36.2857142857143));
            #21=IFCPOLYLINE((#19,#20));
            #22=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#21);
            #30=IFCCARTESIANPOINT((-20.8642857142851,36.2857142857144));
            #31=IFCAXIS2PLACEMENT2D(#30,#23);
            #32=IFCCIRCLE(#31,6.35000000000001);
            #33=IFCTRIMMEDCURVE(#32,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(90.000000000005)),.T.,.PARAMETER.);
            #34=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#33);
            #35=IFCCARTESIANPOINT((-20.8642857142856,42.6357142857144));
            #36=IFCCARTESIANPOINT((-20.8642857142861,-20.8642857142859));
            #37=IFCPOLYLINE((#35,#36));
            #38=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#37);
            #39=IFCCOMPOSITECURVE((#4,#9,#13,#18,#22,#34,#38),.F.);
            """, lengthScaleOverride: 0.001);

        var points = CurveEvaluator.Evaluate2D(model.Context, model.Entity(39), dropClosure: true);
        var cleaned = PolygonWithHoles.CleanRing(points);

        Assert.That(PolygonTriangulator.HasSelfIntersection(cleaned), Is.False,
            () => $"Self-intersection in mirrored L-angle profile with {cleaned.Count} points");
    }

    [Test]
    public void ExampleLAngleColumnProfile_IsSimplePolygon()
    {
        using var model = MicroIfc.Parse("""
            #23=IFCDIRECTION((1.,0.));
            #27=IFCDIRECTION((0.,1.));
            #29=IFCDIRECTION((0.,-1.));
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
            """, lengthScaleOverride: 0.001);

        var points = CurveEvaluator.Evaluate2D(model.Context, model.Entity(39), dropClosure: true);
        var cleaned = PolygonWithHoles.CleanRing(points);

        Assert.That(PolygonTriangulator.HasSelfIntersection(cleaned), Is.False,
            () => $"Self-intersection in L-angle profile with {cleaned.Count} points");
        var profile = ProfileBuilder.Build(model.Context, model.Entity(40));
        Assert.That(profile.Triangulate(), Is.Not.Empty);
    }

    [Test]
    public void ExampleIfc_LAngleProfiles_FromFullFile_AreSimple()
    {
        TestFiles.RequireExists(TestFiles.Example);
        using var file = TestFiles.LoadStep(TestFiles.Example);
        var ctx = new MeshingContext(file);

        foreach (var (profileId, label) in new (int id, string name)[]
        {
            (3405, "L2-col"), (3576, "L2-col-mirror"), (3278, "L3-beam"),
        })
        {
            var profile = ctx.GetEntity(profileId);
            var curveId = profile.GetId(IfcArbitraryClosedProfileDef.Instance.OuterCurve.Index);
            var points = CurveEvaluator.Evaluate2D(ctx, ctx.GetEntity(curveId), dropClosure: true);
            var cleaned = PolygonWithHoles.CleanRing(points);
            Assert.That(PolygonTriangulator.HasSelfIntersection(cleaned), Is.False,
                () => $"{label} #{profileId} curve #{curveId}: {cleaned.Count} pts, scale={ctx.LengthScale}");
        }
    }

    [Test]
    public void CompositeCurve_JoinsSegments()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((1.,0.));
            #3=IFCCARTESIANPOINT((1.,1.));
            #4=IFCPOLYLINE((#1,#2));
            #5=IFCPOLYLINE((#2,#3));
            #6=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#4);
            #7=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#5);
            #8=IFCCOMPOSITECURVE((#6,#7),.F.);
            """);

        var ctx = model.Context;
        var pts = CurveEvaluator.Evaluate2D(ctx, model.Entity(8));
        Assert.That(pts, Has.Count.EqualTo(3));
        Assert.That((float)pts[0].X, Is.EqualTo(0f).Within(1e-5f));
        Assert.That((float)pts[^1].Y, Is.EqualTo(1f).Within(1e-5f));
    }
}
