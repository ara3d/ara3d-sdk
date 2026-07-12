using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class WpW6Tests
{
    // WP-W6: mesh dedup fingerprint was too coarse — bolt caps share identical hex-head
    // vertices at the start of the buffer while shank lengths differ; first-16-point sampling
    // merged ~25 distinct sculpture meshes. Fix: spread vertex/triangle samples + topology verify.

    [Test]
    public void MeshDedup_SameProfileDifferentDepths_KeepSeparateMeshes()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'p',$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCMEMBER('a',$,'A',$,$,#8,#5,$);
            #10=IFCEXTRUDEDAREASOLID(#1,$,#2,2.);
            #11=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#10));
            #12=IFCPRODUCTDEFINITIONSHAPE($,$,(#11));
            #13=IFCCARTESIANPOINT((3.,0.,0.));
            #14=IFCAXIS2PLACEMENT3D(#13,$,$);
            #15=IFCLOCALPLACEMENT($,#14);
            #16=IFCMEMBER('b',$,'B',$,$,#15,#12,$);
            """);

        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);

        Assert.That(built.Instances, Has.Count.EqualTo(2));
        Assert.That(built.Meshes, Has.Count.EqualTo(2),
            "same profile with different extrusion depth must not fingerprint-merge");
    }

    [Test]
    public void MeshDedup_IdenticalExtrusions_ShareOneMesh()
    {
        using var model = MicroIfc.WriteTemp("""
            #1=IFCRECTANGLEPROFILEDEF(.AREA.,'p',$,1.,1.);
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCEXTRUDEDAREASOLID(#1,$,#2,1.);
            #4=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#3));
            #5=IFCPRODUCTDEFINITIONSHAPE($,$,(#4));
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCAXIS2PLACEMENT3D(#6,$,$);
            #8=IFCLOCALPLACEMENT($,#7);
            #9=IFCMEMBER('a',$,'A',$,$,#8,#5,$);
            #10=IFCCARTESIANPOINT((3.,0.,0.));
            #11=IFCAXIS2PLACEMENT3D(#10,$,$);
            #12=IFCLOCALPLACEMENT($,#11);
            #13=IFCMEMBER('b',$,'B',$,$,#12,#5,$);
            """);

        var (built, _) = ModelAssembler.BuildModel(model.Context.File!);

        Assert.That(built.Instances, Has.Count.EqualTo(2));
        Assert.That(built.Meshes, Has.Count.EqualTo(1),
            "identical local topology should still dedup");
    }

    [Test]
    [Explicit("WP-W6 diagnosis: dedup bucket analysis")]
    [Category("Slow")]
    public void AiscSculpture_DedupDiagnosis()
    {
        var ifcPath = TestFiles.AiscSculptureBrep;
        TestFiles.RequireExists(ifcPath);

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var uniqueOracleGroups = GroupByTopology(oracle.Meshes.ToList());

        TestContext.WriteLine(
            $"candidate meshes={model.Meshes.Count}, oracle meshes={oracle.Meshes.Count}, " +
            $"unique oracle topologies={uniqueOracleGroups.Count}");
        Assert.That(model.Meshes.Count, Is.EqualTo(oracle.Meshes.Count));
    }

    static List<List<int>> GroupByTopology(IReadOnlyList<TriangleMesh3D> meshes)
    {
        var groups = new List<List<int>>();
        for (var i = 0; i < meshes.Count; i++)
        {
            var found = -1;
            for (var g = 0; g < groups.Count; g++)
            {
                if (TopologyEqual(meshes[i], meshes[groups[g][0]]))
                {
                    found = g;
                    break;
                }
            }
            if (found < 0)
                groups.Add(new List<int> { i });
            else
                groups[found].Add(i);
        }
        return groups;
    }

    static bool TopologyEqual(TriangleMesh3D a, TriangleMesh3D b)
    {
        if (a.Points.Count != b.Points.Count || a.FaceIndices.Count != b.FaceIndices.Count)
            return false;
        for (var i = 0; i < a.Points.Count; i++)
            if (!a.Points[i].Equals(b.Points[i]))
                return false;
        for (var i = 0; i < a.FaceIndices.Count; i++)
        {
            var fa = a.FaceIndices[i];
            var fb = b.FaceIndices[i];
            if (fa.A != fb.A || fa.B != fb.B || fa.C != fb.C)
                return false;
        }
        return true;
    }

    [Test]
    [Category("Slow")]
    public void AiscSculpture_MeshCount_AtLeast130()
    {
        var ifcPath = TestFiles.AiscSculptureBrep;
        TestFiles.RequireExists(ifcPath);

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);
        var oracle = ModelComparer.LoadOracle(ifcPath);

        TestContext.WriteLine(
            $"meshes {model.Meshes.Count}/{oracle.Meshes.Count}, " +
            $"inst {model.Instances.Count}/{oracle.Instances.Count}");

        Assert.That(model.Instances.Count, Is.EqualTo(oracle.Instances.Count));
        Assert.That(model.Meshes.Count, Is.GreaterThanOrEqualTo(130),
            "sculpture dedup should retain at least 130/145 oracle mesh buckets");
    }

    [Test]
    [Category("Slow")]
    public void AiscSculpture_Parity_AtLeast092()
    {
        var ifcPath = TestFiles.AiscSculptureBrep;
        TestFiles.RequireExists(ifcPath);

        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath))
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        var result = ModelComparer.CompareFile(ifcPath);
        TestContext.WriteLine(ModelComparer.FormatResult(result));

        Assert.That(result.ParityScore, Is.GreaterThanOrEqualTo(0.92));
        Assert.That(result.EntityInstances.KeyJaccard, Is.EqualTo(1.0).Within(0.001));
        Assert.That(result.MeshCount.Candidate, Is.GreaterThanOrEqualTo(130));
    }

    [Test]
    [Category("IfcMesherParity")]
    public void QuickFiles_NoInstanceJaccardRegression()
    {
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var result = ModelComparer.CompareFile(ifcPath);
            TestContext.WriteLine(
                $"{ifcPath.GetFileName()}: parity={result.ParityScore:F3} instJ={result.EntityInstances.KeyJaccard:F3}");
            Assert.That(result.EntityInstances.KeyJaccard, Is.GreaterThanOrEqualTo(0.99),
                $"{ifcPath.GetFileName()} instance Jaccard regressed");
        }
    }
}
