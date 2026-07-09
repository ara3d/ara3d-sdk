using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.IfcTypes;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class ModelBuildTests
{
    [Test]
    public void BuildModel_InstanceEntityIndexMatchesProductExpressId()
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
        Assert.That(built.Instances[0].EntityIndex, Is.EqualTo(9));
    }

    [Test]
    public void SteelPlates_InstanceEntityIdsUseOracleProductIdSpace()
    {
        TestFiles.RequireExists(TestFiles.SteelPlates);
        var candidate = ModelComparer.LoadCandidate(TestFiles.SteelPlates);
        var oracle = ModelComparer.LoadOracle(TestFiles.SteelPlates);
        var oracleMap = OracleEntityMap.Build(TestFiles.SteelPlates, oracle);

        var productIds = oracleMap.ProductRepresentationTrees
            .Select(t => t.EntityId)
            .ToHashSet();
        var oracleIds = oracle.Instances.Select(i => i.EntityIndex).ToHashSet();
        var candidateIds = candidate.Instances.Select(i => i.EntityIndex).ToHashSet();
        var shared = oracleIds.Intersect(candidateIds).Count();

        Assert.That(candidateIds, Is.SubsetOf(productIds),
            () => $"Candidate entity ids must be IFC product express ids, not representation items: " +
                  string.Join(", ", candidateIds.Except(productIds)));
        Assert.That(shared, Is.GreaterThanOrEqualTo(11),
            () => $"Expected strong product-id overlap with oracle; shared={shared}");
    }

    [Test]
    public void ProductShape_ExtrudedWall_MeshesViaDispatcher()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'Box',$,2.,2.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,4.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((100.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCWALL('g',$,'Wall',$,$,#8,#5,$);
            """);

        var ctx = model.Context;
        var mesh = ModelAssembler.BuildEntityMesh(ctx, model.Entity(9));
        Assert.That(mesh, Is.Not.Null);
        var bounds = MeshHelpers.GetBounds(mesh!.Value);
        Assert.That((float)bounds.Min.X.Value, Is.EqualTo(99f).Within(1e-3f));
    }

    [Test]
    [Category("Slow")]
    public void OpenHouse_BuildsMultipleProductInstances()
    {
        TestFiles.RequireExists(TestFiles.IfcOpenHouse);
        using var file = new IfcFile(TestFiles.IfcOpenHouse, includeGeometry: false);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);

        Assert.That(model.Instances, Has.Count.GreaterThan(10),
            () => $"Expected broad OpenHouse coverage; instances={model.Instances.Count}; " +
                  $"unsupported={string.Join(", ", diagnostics.EntityStatus.Where(kv => kv.Value == GeometrySupportStatus.Unsupported).Select(kv => kv.Key))}");
        Assert.That(model.Meshes.Sum(m => m.FaceIndices.Count), Is.GreaterThan(100));
    }

    [Test]
    [Category("Slow")]
    public void Small_AttDrivenExtrudedSolid_ProducesGeometry()
    {
        TestFiles.RequireExists(TestFiles.Small);
        using var file = TestFiles.LoadStep(TestFiles.Small);
        var ctx = new MeshingContext(file);
        var mesh = GeometryDispatcher.TryBuild(ctx, ctx.GetEntity(42));
        Assert.That(mesh, Is.Not.Null);
        Assert.That(mesh!.Value.FaceIndices.Count, Is.GreaterThan(0));
    }

    [Test]
    [Category("Slow")]
    public void DentalClinic_OpenProfiles_DoNotAbortModelBuild()
    {
        TestFiles.RequireExists(TestFiles.DentalClinic);
        using var file = TestFiles.LoadStep(TestFiles.DentalClinic);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
        var triangleCount = model.Meshes.Sum(m => m.FaceIndices.Count);
        Assert.That(triangleCount, Is.GreaterThan(0));
        TestContext.WriteLine($"Meshes: {model.Meshes.Count}, Triangles: {triangleCount}");
        TestContext.WriteLine($"Open profiles seen: {diagnostics.EntityCounts.GetValueOrDefault("IFCARBITRARYOPENPROFILEDEF")}");
        TestContext.WriteLine($"Surface extrusions seen: {diagnostics.EntityCounts.GetValueOrDefault("IFCSURFACEOFLINEAREXTRUSION")}");
    }

    [Test]
    [Category("Slow")]
    public void IfcOpenHouse_BuildsModelWithInstances()
    {
        TestFiles.RequireExists(TestFiles.IfcOpenHouse);
        using var file = TestFiles.LoadStep(TestFiles.IfcOpenHouse);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);
        Assert.That(model.Instances.Count, Is.GreaterThan(0));
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
        TestContext.WriteLine($"Instances: {model.Instances.Count}, Meshes: {model.Meshes.Count}");
        TestContext.WriteLine($"Diagnostics: {diagnostics.EntityCounts.Count} entity types");
    }

    [Test]
    public void SteelPlates_SharedEntityBoundingBoxesMatchOracle()
    {
        TestFiles.RequireExists(TestFiles.SteelPlates);
        var candidate = ModelComparer.LoadCandidate(TestFiles.SteelPlates);
        var oracle = ModelComparer.LoadOracle(TestFiles.SteelPlates);
        var candBounds = EntityBounds(candidate);
        var oracleBounds = EntityBounds(oracle);
        var shared = candBounds.Keys.Intersect(oracleBounds.Keys).ToList();
        var matched = shared.Count(id => OracleComparison.BoundsClose(candBounds[id], oracleBounds[id], 0.05));
        var mismatched = shared.Where(id => !OracleComparison.BoundsClose(candBounds[id], oracleBounds[id], 0.05)).ToList();
        if (mismatched.Count > 0)
            TestContext.WriteLine("Mismatched: " + string.Join(", ", mismatched.Select(id => $"#{id}")));

        Assert.That(shared, Has.Count.GreaterThanOrEqualTo(10));
        Assert.That(matched, Is.GreaterThanOrEqualTo(11),
            () => $"Expected ≥11 shared entity bboxes within tolerance; matched={matched}/{shared.Count}; mismatched=[{string.Join(",", mismatched)}]");
    }

    [Test]
    public void IfcOpenHouse_SharedEntityBoundingBoxesMatchOracle()
    {
        TestFiles.RequireExists(TestFiles.IfcOpenHouse);
        var candidate = ModelComparer.LoadCandidate(TestFiles.IfcOpenHouse);
        var oracle = ModelComparer.LoadOracle(TestFiles.IfcOpenHouse);
        var candBounds = EntityBounds(candidate);
        var oracleBounds = EntityBounds(oracle);
        var shared = candBounds.Keys.Intersect(oracleBounds.Keys).ToList();
        var matched = shared.Count(id => OracleComparison.BoundsClose(candBounds[id], oracleBounds[id], 0.05));
        var mismatched = shared.Where(id => !OracleComparison.BoundsClose(candBounds[id], oracleBounds[id], 0.05)).ToList();
        if (mismatched.Count > 0)
            TestContext.WriteLine("Mismatched: " + string.Join(", ", mismatched.Select(id => $"#{id}")));

        Assert.That(shared, Has.Count.GreaterThanOrEqualTo(33));
        Assert.That(matched, Is.GreaterThanOrEqualTo(33),
            () => $"Expected ≥33 shared entity bboxes within tolerance; matched={matched}/{shared.Count}; mismatched=[{string.Join(",", mismatched)}]");
    }

    [Test]
    public void MappedItem_MultiSolidRepresentation_EmitsMultipleInstances()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'A',$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            #4=IFCRECTANGLEPROFILEDEF(.AREA.,'B',$,1.,1.);
            #5=IFCCARTESIANPOINT((2.,0.,0.));
            #6=IFCAXIS2PLACEMENT3D(#5,$,$);
            #7=IFCEXTRUDEDAREASOLID(#4,#6,#2,1.);
            #8=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3,#7));
            #9=IFCCARTESIANPOINT((0.,0.,0.));
            #10=IFCAXIS2PLACEMENT3D(#9,$,$);
            #11=IFCREPRESENTATIONMAP(#10,#8);
            #12=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#9,1.,$);
            #13=IFCMAPPEDITEM(#11,#12);
            #14=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#13));
            #15=IFCPRODUCTDEFINITIONSHAPE($,$,(#14));
            """);

        var parts = new List<CollectedPart>();
        GeometryPartCollector.CollectParts(model.Context, model.Entity(15), Matrix4x4.Identity, 99, parts);
        Assert.That(parts, Has.Count.EqualTo(2));
        Assert.That(parts.Select(p => p.EntityIndex).Distinct().Single(), Is.EqualTo(99));
    }

    [Test]
    [Category("Slow")]
    public void Example_PrintOracleOnlyGaps()
    {
        TestFiles.RequireExists(TestFiles.Example);
        var oracle = ModelComparer.LoadOracle(TestFiles.Example);
        var candidate = ModelComparer.LoadCandidate(TestFiles.Example);
        var oracleIds = oracle.Instances.Select(i => i.EntityIndex).ToHashSet();
        var candidateIds = candidate.Instances.Select(i => i.EntityIndex).ToHashSet();
        var oracleOnly = oracleIds.Except(candidateIds).OrderBy(x => x).ToList();
        TestContext.WriteLine($"oracleOnly={oracleOnly.Count} candidate={candidate.Instances.Count} oracle={oracle.Instances.Count}");

        using var file = new IfcFile(TestFiles.Example, includeGeometry: false);
        var byType = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        foreach (var id in oracleOnly)
        {
            var name = file.EntityResolver.GetEntity(id).GetEntityName();
            if (!byType.TryGetValue(name, out var list))
            {
                list = [];
                byType[name] = list;
            }
            list.Add(id);
        }
        foreach (var kv in byType.OrderByDescending(kv => kv.Value.Count))
            TestContext.WriteLine($"{kv.Key}: {kv.Value.Count} [{string.Join(", ", kv.Value.Take(8))}]");

        foreach (var id in oracleOnly.Take(20))
        {
            var tree = OracleEntityMap.Build(TestFiles.Example).ProductRepresentationTrees
                .FirstOrDefault(t => t.EntityId == id);
            var geom = tree is null ? "?" : SummarizeTree(tree.Children.FirstOrDefault());
            TestContext.WriteLine($"  #{id} {file.EntityResolver.GetEntity(id).GetEntityName()}: {geom}");
        }

        var (_, diag) = ModelAssembler.BuildModel(file);
        TestContext.WriteLine("--- unsupported ---");
        foreach (var kv in diag.EntityStatus.Where(kv => kv.Value == GeometrySupportStatus.Unsupported)
                     .OrderByDescending(kv => diag.EntityCounts.GetValueOrDefault(kv.Key)).Take(12))
            TestContext.WriteLine($"{kv.Key}: {diag.EntityCounts.GetValueOrDefault(kv.Key)}");
        Assert.Pass();
    }

    [Test]
    public void Example_SharedEntityBoundingBoxesMatchOracle()
    {
        TestFiles.RequireExists(TestFiles.Example);
        var candidate = ModelComparer.LoadCandidate(TestFiles.Example);
        var oracle = ModelComparer.LoadOracle(TestFiles.Example);
        var candBounds = EntityBounds(candidate);
        var oracleBounds = EntityBounds(oracle);
        var shared = candBounds.Keys.Intersect(oracleBounds.Keys).ToList();
        var matched = shared.Count(id => OracleComparison.BoundsClose(candBounds[id], oracleBounds[id], 0.05));

        Assert.That(shared, Has.Count.GreaterThanOrEqualTo(20));
        Assert.That(matched, Is.EqualTo(shared.Count),
            () => $"Expected all shared entity bboxes to match oracle; matched={matched}/{shared.Count}");
    }

    static string SummarizeTree(RepresentationTreeNode? node)
    {
        if (node is null) return "no-rep";
        if (node.Children.Count == 0)
            return node.Notes ?? node.EntityName;
        return $"{node.EntityName}({string.Join(",", node.Children.Select(SummarizeTree))})";
    }

    static Dictionary<int, Bounds3D> EntityBounds(Model3D model)
    {
        var bounds = new Dictionary<int, Bounds3D>();
        foreach (var inst in model.Instances)
        {
            if (inst.EntityIndex < 0 || inst.MeshIndex < 0 || inst.MeshIndex >= model.Meshes.Count)
                continue;
            var transformed = MeshHelpers.Transform(model.Meshes[inst.MeshIndex], inst.Matrix4x4);
            var meshBounds = MeshHelpers.GetBounds(transformed);
            if (bounds.TryGetValue(inst.EntityIndex, out var existing))
                bounds[inst.EntityIndex] = existing.Merge(meshBounds);
            else
                bounds[inst.EntityIndex] = meshBounds;
        }
        return bounds;
    }

    [Test]
    [Category("Slow")]
    public void IfcOpenHouse_BoundsRoughlyMatchOracle()
    {
        TestFiles.RequireExists(TestFiles.IfcOpenHouse);
        var (mine, oracle) = OracleComparison.CompareFile(TestFiles.IfcOpenHouse);
        TestContext.WriteLine(OracleComparison.FormatComparison("IfcOpenHouse", mine, oracle));
        Assert.That(mine.TriangleCount, Is.GreaterThan(0));
        Assert.That(OracleComparison.BoundsOverlap(mine.Bounds, oracle.Bounds));
    }

    [Test]
    public void InchConversionUnit_ResolvesLengthScaleToMeters()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCSIUNIT(*,.LENGTHUNIT.,.MILLI.,.METRE.);
            #2=IFCMEASUREWITHUNIT(IFCLENGTHMEASURE(25.4),#1);
            #3=IFCDIMENSIONALEXPONENTS(1,0,0,0,0,0,0);
            #4=IFCCONVERSIONBASEDUNIT(#3,.LENGTHUNIT.,'INCH',#2);
            #5=IFCUNITASSIGNMENT((#4));
            #6=IFCPROJECT('p',$,'P',$,$,$,$,(#5));
            """);

        Assert.That(model.Context.LengthScale, Is.EqualTo(0.0254).Within(1e-9));
    }

    [Test]
    public void MappedFacetedBrep_InchUnits_EmitsInstance()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCSIUNIT(*,.LENGTHUNIT.,.MILLI.,.METRE.);
            #2=IFCMEASUREWITHUNIT(IFCLENGTHMEASURE(25.4),#1);
            #3=IFCDIMENSIONALEXPONENTS(1,0,0,0,0,0,0);
            #4=IFCCONVERSIONBASEDUNIT(#3,.LENGTHUNIT.,'INCH',#2);
            #5=IFCUNITASSIGNMENT((#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCDIRECTION((1.,0.,0.));
            #9=IFCAXIS2PLACEMENT3D(#6,#7,#8);
            #10=IFCCARTESIANPOINT((0.,0.,0.));
            #11=IFCCARTESIANPOINT((1.,0.,0.));
            #12=IFCCARTESIANPOINT((1.,1.,0.));
            #13=IFCCARTESIANPOINT((0.,1.,0.));
            #14=IFCCARTESIANPOINT((0.,0.,1.));
            #15=IFCCARTESIANPOINT((1.,0.,1.));
            #16=IFCCARTESIANPOINT((1.,1.,1.));
            #17=IFCCARTESIANPOINT((0.,1.,1.));
            #18=IFCPOLYLOOP((#10,#11,#12,#13));
            #19=IFCFACEOUTERBOUND(#18,.T.);
            #20=IFCFACE((#19));
            #21=IFCPOLYLOOP((#15,#16,#17,#14));
            #22=IFCFACEOUTERBOUND(#21,.T.);
            #23=IFCFACE((#22));
            #24=IFCPOLYLOOP((#14,#17,#12,#13));
            #25=IFCFACEOUTERBOUND(#24,.T.);
            #26=IFCFACE((#25));
            #27=IFCPOLYLOOP((#16,#15,#11,#12));
            #28=IFCFACEOUTERBOUND(#27,.T.);
            #29=IFCFACE((#28));
            #30=IFCPOLYLOOP((#15,#14,#10,#11));
            #31=IFCFACEOUTERBOUND(#30,.T.);
            #32=IFCFACE((#31));
            #33=IFCPOLYLOOP((#17,#16,#12,#13));
            #34=IFCFACEOUTERBOUND(#33,.T.);
            #35=IFCFACE((#34));
            #36=IFCCLOSEDSHELL((#20,#23,#26,#29,#32,#35));
            #37=IFCFACETEDBREP(#36);
            #38=IFCSHAPEREPRESENTATION($,'Body','Brep',(#37));
            #39=IFCREPRESENTATIONMAP(#9,#38);
            #40=IFCCARTESIANTRANSFORMATIONOPERATOR3D($,$,#6,1.,$);
            #41=IFCMAPPEDITEM(#39,#40);
            #42=IFCSHAPEREPRESENTATION($,'Body','MappedRepresentation',(#41));
            #43=IFCPRODUCTDEFINITIONSHAPE($,$,(#42));
            #44=IFCLOCALPLACEMENT($,#9);
            #45=IFCMEMBER('g',$,'M',$,$,#44,#43,$);
            #46=IFCPROJECT('p',$,'P',$,$,$,$,(#5));
            """);

        Assert.That(model.Context.File, Is.Not.Null);
        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);
        Assert.That(built.Instances, Has.Count.EqualTo(1));
        Assert.That(built.Meshes.Sum(m => m.FaceIndices.Count), Is.GreaterThan(0));
    }

    [Test]
    [Category("Slow")]
    public void AiscSculpture_MappedBrepCoverage()
    {
        var ifcPath = new FilePath(@"c:\Users\cdigg\git\studio\data\171210AISC_Sculpture_brep.ifc");
        TestFiles.RequireExists(ifcPath);

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var candIds = model.Instances.Select(i => i.EntityIndex).ToHashSet();
        var oracleIds = oracle.Instances.Select(i => i.EntityIndex).ToHashSet();
        var oracleOnly = oracleIds.Except(candIds).Count();

        TestContext.WriteLine(
            $"inst {model.Instances.Count}/{oracle.Instances.Count}, " +
            $"meshes {model.Meshes.Count}/{oracle.Meshes.Count}, " +
            $"tris {model.Meshes.Sum(m => m.FaceIndices.Count)}/{oracle.Meshes.Sum(m => m.FaceIndices.Count)}, " +
            $"oracleOnly={oracleOnly}");

        Assert.That(oracleOnly, Is.EqualTo(0), "Every oracle product id should emit at least one instance");
        Assert.That(model.Meshes.Count, Is.GreaterThanOrEqualTo(100));
        Assert.That(model.Meshes.Sum(m => m.FaceIndices.Count), Is.GreaterThan(4000));
    }
}
