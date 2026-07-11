using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>
/// WP-T2 — transform introspection: level-by-level placement-chain dump + corpus determinant audit,
/// used to classify the schependomlaan top-20 max-Δ clusters (candidate-chain / mirror / oracle / quirk).
/// </summary>
[TestFixture]
public sealed class WpPlacementIntrospectionTests
{
    [Test]
    public void DeterminantAudit_DetectsMirrorOperator()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCDIRECTION((1.,0.,0.));
            #2=IFCDIRECTION((0.,-1.,0.));
            #3=IFCCARTESIANPOINT((0.,0.,0.));
            #4=IFCDIRECTION((0.,0.,1.));
            #5=IFCCARTESIANTRANSFORMATIONOPERATOR3D(#1,#2,#3,1.,#4);
            #6=IFCCARTESIANPOINT((0.,0.,0.));
            #7=IFCDIRECTION((0.,0.,1.));
            #8=IFCDIRECTION((1.,0.,0.));
            #9=IFCAXIS2PLACEMENT3D(#6,#7,#8);
            """,
            lengthScaleOverride: 0.001);

        var audit = PlacementIntrospection.AuditDeterminants(model.Context);
        Assert.That(audit.OperatorCount, Is.EqualTo(1));
        Assert.That(audit.MirroredOperatorCount, Is.EqualTo(1), "Axis2=(0,-1,0) is a reflection");
        Assert.That(audit.ExplicitAxis2Count, Is.EqualTo(1));
        Assert.That(audit.MirroredPlacementCount, Is.EqualTo(0),
            "placements are forced right-handed; a mirror can only enter via an operator");
        Assert.That(audit.MirroredOperators, Has.Count.EqualTo(1));
        Assert.That(audit.MirroredOperators[0].Determinant, Is.LessThan(0));
    }

    [Test]
    public void PlacementChainDump_RendersLevelsAndOperator()
    {
        using var model = MicroIfc.Parse(
            """
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCDIRECTION((0.,0.,1.));
            #3=IFCDIRECTION((1.,0.,0.));
            #4=IFCAXIS2PLACEMENT3D(#1,#2,#3);
            #5=IFCLOCALPLACEMENT($,#4);
            #6=IFCCARTESIANPOINT((1000.,2000.,3000.));
            #7=IFCAXIS2PLACEMENT3D(#6,#2,#3);
            #8=IFCLOCALPLACEMENT(#5,#7);
            #9=IFCRECTANGLEPROFILEDEF(.AREA.,'P',$,2000.,2000.);
            #10=IFCDIRECTION((0.,0.,1.));
            #11=IFCEXTRUDEDAREASOLID(#9,$,#10,4000.);
            #12=IFCSHAPEREPRESENTATION($,'Body','SweptSolid',(#11));
            #13=IFCPRODUCTDEFINITIONSHAPE($,$,(#12));
            #14=IFCBUILDINGELEMENTPROXY('0000000000000000000000',$,'proxy',$,$,#8,#13,$);
            """,
            lengthScaleOverride: 0.001);

        var dump = PlacementIntrospection.DumpPlacementChain(model.Context, 14);
        TestContext.WriteLine(dump);
        Assert.That(dump, Does.Contain("IFCLOCALPLACEMENT levels"));
        Assert.That(dump, Does.Contain("cumOrigin=(1.000,2.000,3.000)"));
    }

    static FilePath SchependomlaanPath()
    {
        var p = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        if (!p.Exists())
            p = new FilePath(@"C:\Users\cdigg\git\3d-format-shootout\data\git-repo-copies\web-ifc\schependomlaan.ifc");
        return p;
    }

    [Test]
    [Explicit("WP-T2: schependomlaan determinant audit + top-20 max-Δ chain classification")]
    [Category("Slow")]
    public void Schependomlaan_DeterminantAudit_And_Top20Classification()
    {
        var ifcPath = SchependomlaanPath();
        if (!ifcPath.Exists())
        {
            Assert.Ignore($"schependomlaan.ifc not found at {ifcPath}");
            return;
        }

        var oraclePath = WebIfcBfastOracle.OraclePath(ifcPath);
        // NOTE: WebIfcBfastOracle.IsStaleRelativeToLive AccessViolation-crashes on this 50 MB file
        // (iterating web-ifc's native Point3D buffers). Use the cached BFAST as-is; if it is missing
        // there is nothing to measure against.
        if (!oraclePath.Exists())
        {
            Assert.Ignore($"oracle BFAST missing at {oraclePath}");
            return;
        }

        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var ctx = new MeshingContext(file);
        var audit = PlacementIntrospection.AuditDeterminants(ctx);
        TestContext.WriteLine(
            $"DETERMINANT AUDIT: operators={audit.OperatorCount} mirrored={audit.MirroredOperatorCount} " +
            $"explicitAxis2={audit.ExplicitAxis2Count}; placements={audit.PlacementCount} " +
            $"mirroredPlacements={audit.MirroredPlacementCount}");
        foreach (var m in audit.MirroredOperators.Take(20))
            TestContext.WriteLine($"  mirror op #{m.Id} {m.Name} axis2Explicit={m.HasExplicitAxis2} det={m.Determinant:F3}");

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var oracle = ModelComparer.LoadOracle(ifcPath);
        var summary = TransformComparison.Compare(candidate, oracle);
        TestContext.WriteLine(
            $"paired={summary.PairedInstanceCount} meanCenterΔ={summary.MeanBoundsCenterDistance:F4} " +
            $"maxCenterΔ={summary.MaxBoundsCenterDistance:F4} " +
            $"split matrix/shape/matched={summary.PlacementMatrixCount}/{summary.ShapeInflatedCenterCount}/{summary.MatchedPlacementCount}");

        TestContext.WriteLine("\nTOP-20 MAX-Δ ENTITIES (candidate chain vs oracle):");
        foreach (var d in summary.WorstInstances)
        {
            var name = file.EntityResolver.GetEntityOrDefault(d.EntityId)?.GetEntityName() ?? "?";
            var candDet = InstanceDeterminant(candidate, d.EntityId);
            var oracleDet = InstanceDeterminant(oracle, d.EntityId);
            var verdict = Classify(d, candDet, oracleDet);
            TestContext.WriteLine(
                $"  #{d.EntityId} {name}: centerΔ={d.BoundsCenterDistance:F3} rot={d.MatrixDelta.RotationAngleDeg:F1} " +
                $"trans={d.MatrixDelta.TranslationDistance:F3} conf={d.PairConfidence:F2} " +
                $"candDet={candDet:F2} oracleDet={oracleDet:F2} tri={d.CandidateTriCount}/{d.OracleTriCount} => {verdict}");
            TestContext.WriteLine(PlacementIntrospection.DumpPlacementChain(ctx, d.EntityId));
        }
    }

    static double InstanceDeterminant(Model3D model, int entityId)
    {
        foreach (var inst in model.Instances)
            if (inst.EntityIndex == entityId && inst.MeshIndex >= 0 && inst.MeshIndex < model.Meshes.Count)
                return MeshHelpers.LinearDeterminant(inst.Matrix4x4);
        return double.NaN;
    }

    static string Classify(InstanceTransformDelta d, double candDet, double oracleDet)
    {
        if (double.IsFinite(candDet) && double.IsFinite(oracleDet) && Math.Sign(candDet) != Math.Sign(oracleDet))
            return "MIRROR (det sign differs — should be fixed by WP-T3)";
        if (d.PairConfidence < 0.3)
            return "LOW-CONFIDENCE PAIR (likely pairing noise / oracle quirk)";
        if (d.MatrixDelta.RotationAngleDeg > 2 && d.BoundsCenterDistance > 1.0)
            return "CANDIDATE-CHAIN (rotation + center disagree)";
        if (d.MatrixDelta.TranslationDistance > 1.0 && d.BoundsCenterDistance > 1.0)
            return "CANDIDATE-CHAIN or ORACLE (translation-dominant)";
        return "SHAPE-INFLATED / minor";
    }
}
