using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>WP-W5: PRIMARK oracle-only products (403 IFCDISCRETEACCESSORY).</summary>
[TestFixture]
public sealed class WpW5Tests
{
    static FilePath PrimarkIfc => new(@"c:\Users\cdigg\git\studio\data\20210221PRIMARK.ifc");

    [Test]
    [Explicit("WP-W5 diagnosis: PRIMARK oracle-only products")]
    [Category("Slow")]
    public void ScorePrimarkStretch_Diagnosis()
    {
        var ifcPath = PrimarkIfc;
        TestFiles.RequireExists(ifcPath);

        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath))
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        OracleEntityMap.Write(ifcPath);
        var map = OracleEntityMap.Build(ifcPath);
        TestContext.WriteLine($"Oracle map: {map.OracleInstanceCount} instances, {map.OracleMeshCount} meshes");

        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var (model, diagnostics) = ModelAssembler.BuildModel(stepFile);
        TestContext.WriteLine(
            $"Candidate: {model.Instances.Count} instances, {model.Meshes.Count} meshes, " +
            $"{model.Meshes.Sum(m => m.FaceIndices.Count)} tris");

        var result = ModelComparer.Compare(model, ModelComparer.LoadOracle(ifcPath), ifcPath.GetFileName());
        TestContext.WriteLine(ModelComparer.FormatResult(result));

        var candidateEntities = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();
        var oracleInstByEntity = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());

        var oracleOnlyProducts = map.ProductRepresentationTrees
            .Where(t => !candidateEntities.Contains(t.EntityId) && oracleInstByEntity.ContainsKey(t.EntityId))
            .ToList();
        TestContext.WriteLine($"Oracle-only products: {oracleOnlyProducts.Count}");

        TestContext.WriteLine("Oracle-only products by type:");
        foreach (var group in oracleOnlyProducts
                     .GroupBy(t => t.EntityName)
                     .OrderByDescending(g => g.Count()))
            TestContext.WriteLine($"  {group.Key}: {group.Count()}");

        TestContext.WriteLine("Diagnostics unsupported:");
        foreach (var (name, count) in diagnostics.EntityCounts
                     .Where(kv => diagnostics.EntityStatus.GetValueOrDefault(kv.Key) == GeometrySupportStatus.Unsupported)
                     .OrderByDescending(kv => kv.Value)
                     .Take(20))
            TestContext.WriteLine($"  {name}: {count}");
    }

    /// <summary>
    /// Catalog baseline (studio_catalog_evaluation.json): #92859 is top oracle-only IFCDISCRETEACCESSORY (644 tris).
    /// Profile #92854 is a multi-segment IFCCOMPOSITECURVE with trimmed arcs + DISCONTINUOUS closure.
    /// Fails today; gate for WP-W4 CurveEvaluator composite-curve fix.
    /// </summary>
    [Test]
    [Category("Slow")]
    public void Primark_DiscreteAccessory_CompositePlateProfile_Builds()
    {
        TestFiles.RequireExists(PrimarkIfc);
        using var file = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(file);
        const int accessoryId = 92859;
        var accessory = ctx.GetEntity(accessoryId);
        Assert.That(accessory.GetEntityName(), Is.EqualTo("IFCDISCRETEACCESSORY"));

        var mesh = ModelAssembler.BuildEntityMesh(ctx, accessory);
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0),
            $"#{accessoryId} PL15*220 composite-profile plate should mesh (catalog: 644 oracle tris)");
    }

    /// <summary>
    /// Clipping rep with simple polyline profile — smaller slice of the 108 boolean-clipping oracle-only set.
    /// </summary>
    [Test]
    [Category("Slow")]
    public void Primark_DiscreteAccessory_ClippedPlate_Builds()
    {
        TestFiles.RequireExists(PrimarkIfc);
        using var file = new IfcFile(PrimarkIfc, includeGeometry: false);
        var ctx = new MeshingContext(file);
        const int accessoryId = 40020;
        var mesh = ModelAssembler.BuildEntityMesh(ctx, ctx.GetEntity(accessoryId));
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0),
            $"#{accessoryId} clipped PL15*180 plate should mesh");
    }

    [Test]
    [Explicit("Diagnostic: composite profile point dump")]
    public void PrimarkStyle_CompositePlateProfile_Micro_Diagnosis()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((140.,0.));
            #3=IFCCARTESIANPOINT((140.,100.));
            #4=IFCCARTESIANPOINT((0.,50.));
            #5=IFCPOLYLINE((#1,#2,#3,#4));
            #6=IFCCARTESIANPOINT((-10.,50.));
            #7=IFCDIRECTION((0.,1.));
            #8=IFCAXIS2PLACEMENT2D(#6,#7);
            #9=IFCCIRCLE(#8,10.);
            #10=IFCTRIMMEDCURVE(#9,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(1.5707963267949)),.F.,.PARAMETER.);
            #11=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#5);
            #12=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#10);
            #13=IFCCARTESIANPOINT((0.,40.));
            #14=IFCPOLYLINE((#4,#13,#1));
            #15=IFCCOMPOSITECURVESEGMENT(.DISCONTINUOUS.,.T.,#14);
            #16=IFCCOMPOSITECURVE((#11,#12,#15),.F.);
            #17=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'plate',#16);
            #18=IFCDIRECTION((0.,0.,1.));
            #19=IFCEXTRUDEDAREASOLID(#17,$,#18,0.015);
            """);

        var curvePts = CurveEvaluator.Evaluate2D(model.Context, model.Entity(16), dropClosure: true);
        TestContext.WriteLine($"curve points ({curvePts.Count}):");
        foreach (var p in curvePts)
            TestContext.WriteLine($"  ({p.X:F4},{p.Y:F4})");
        TestContext.WriteLine($"self-intersect={PolygonTriangulator.HasSelfIntersection(curvePts)}");

        try
        {
            var profile = ProfileBuilder.Build(model.Context, model.Entity(17));
            TestContext.WriteLine($"profile outer={profile.Outer.Count} area={profile.Area:F6}");
            TestContext.WriteLine($"tris={profile.Triangulate().Count}");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"profile build: {ex.GetType().Name}: {ex.Message}");
        }

        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(19));
        TestContext.WriteLine($"extrusion mesh null={mesh is null}");
        foreach (var note in model.Context.Diagnostics.Messages.Take(5))
            TestContext.WriteLine($"diag: {note}");
    }

    [Test]
    public void PrimarkStyle_CompositePlateProfile_Micro_Builds()
    {
        // Minimal excerpt of PRIMARK #92854-style composite: polyline + quarter arc + DISCONTINUOUS close.
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.));
            #2=IFCCARTESIANPOINT((140.,0.));
            #3=IFCCARTESIANPOINT((140.,100.));
            #4=IFCCARTESIANPOINT((0.,50.));
            #5=IFCPOLYLINE((#1,#2,#3,#4));
            #6=IFCCARTESIANPOINT((-10.,50.));
            #7=IFCDIRECTION((0.,1.));
            #8=IFCAXIS2PLACEMENT2D(#6,#7);
            #9=IFCCIRCLE(#8,10.);
            #10=IFCTRIMMEDCURVE(#9,(IFCPARAMETERVALUE(0.)),(IFCPARAMETERVALUE(1.5707963267949)),.F.,.PARAMETER.);
            #11=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#5);
            #12=IFCCOMPOSITECURVESEGMENT(.CONTINUOUS.,.T.,#10);
            #13=IFCCARTESIANPOINT((0.,40.));
            #14=IFCPOLYLINE((#4,#13,#1));
            #15=IFCCOMPOSITECURVESEGMENT(.DISCONTINUOUS.,.T.,#14);
            #16=IFCCOMPOSITECURVE((#11,#12,#15),.F.);
            #17=IFCARBITRARYCLOSEDPROFILEDEF(.AREA.,'plate',#16);
            #18=IFCDIRECTION((0.,0.,1.));
            #19=IFCEXTRUDEDAREASOLID(#17,$,#18,0.015);
            """);

        var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(19));
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
    }
}
