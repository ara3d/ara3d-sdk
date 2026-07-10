using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>WP-W12: DigitalHub merged-triangle tessellation density.</summary>
[TestFixture]
public sealed class WpW12Tests
{
    static FilePath DigitalHubIfc => new(@"c:\Users\cdigg\git\studio\data\FM_ARC_DigitalHub.ifc");

    [Test]
    public void CylindricalAdvancedFace_DensifiesLongArcChords()
    {
        // Quarter-cylinder panel with only 4 boundary corners — densify should add arc samples.
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCDIRECTION((1.,0.,0.));
            #3=IFCDIRECTION((0.,0.,1.));
            #4=IFCAXIS2PLACEMENT3D(#1,#3,#2);
            #5=IFCCYLINDRICALSURFACE(#4,1.);
            #10=IFCCARTESIANPOINT((1.,0.,0.));
            #11=IFCCARTESIANPOINT((0.,1.,0.));
            #12=IFCCARTESIANPOINT((0.,1.,2.));
            #13=IFCCARTESIANPOINT((1.,0.,2.));
            #14=IFCPOLYLOOP((#10,#11,#12,#13));
            #15=IFCFACEOUTERBOUND(#14,.T.);
            #16=IFCADVANCEDFACE((#15),#5,.T.);
            #17=IFCCLOSEDSHELL((#16));
            #18=IFCADVANCEDBREP(#17);
            """);

        var mesh = Brep.BuildAdvancedBrep(model.Context, model.Entity(18));
        Assert.That(mesh.FaceIndices.Count, Is.GreaterThan(2),
            "cylindrical face should densify beyond a single flat quad");
        Assert.That(mesh.Points.Count, Is.GreaterThan(4));
    }

    [Test]
    [Category("Slow")]
    public void DigitalHub_MergedTri_AtLeast075()
    {
        TestFiles.RequireExists(DigitalHubIfc);

        var bfastPath = WebIfcBfastOracle.OraclePath(DigitalHubIfc);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(DigitalHubIfc, bfastPath))
            WebIfcBfastOracle.Generate(DigitalHubIfc, TestContext.WriteLine);

        var result = ModelComparer.CompareFile(DigitalHubIfc);
        TestContext.WriteLine(ModelComparer.FormatResult(result));
        var triRatio = result.MergedMesh.CandidateTriangleCount
            / (double)Math.Max(1, result.MergedMesh.OracleTriangleCount);
        TestContext.WriteLine(
            $"merged tris {result.MergedMesh.CandidateTriangleCount}/{result.MergedMesh.OracleTriangleCount} " +
            $"(ratio {triRatio:F3}, score {result.MergedMesh.Score:F3})");

        Assert.That(triRatio, Is.GreaterThanOrEqualTo(0.75),
            "DigitalHub merged-tri ratio should reach ≥0.75 with surface-local cylindrical densify");
        Assert.That(result.MergedMesh.Score, Is.GreaterThanOrEqualTo(0.65),
            "merged-mesh composite should not regress below prior band");
    }

    [Test]
    [Category("IfcMesherScore")]
    public void QuickFiles_NoRegression_AfterTessellationChange()
    {
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var result = ModelComparer.CompareFile(ifcPath);
            TestContext.WriteLine($"{ifcPath.GetFileName()}: parity={result.ParityScore:F3}");
            Assert.That(result.ParityScore, Is.GreaterThanOrEqualTo(0.85),
                $"{ifcPath.GetFileName()} regressed after tessellation tuning");
        }
    }
}
