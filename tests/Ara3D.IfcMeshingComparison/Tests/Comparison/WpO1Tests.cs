using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcLoader;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class WpO1Tests
{
    const int DuplexRoofSlab = 22492;
    const int DuplexFloorFinishSlab = 21658;

    [Test]
    [Category("Slow")]
    public void LiveWebIfc_MatchesCandidate_ForDuplexSlabCluster()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);

        var candidate = ModelComparer.LoadCandidate(ifcPath);
        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var live = file.ToModel3D();
        var pairing = OracleTrustedPairingBuilder.Build(candidate, live);

        Assert.That(pairing.IsMisTagged(DuplexRoofSlab), Is.False,
            "fresh web-ifc correctly tags roof slab #22492");
        Assert.That(pairing.IsMisTagged(DuplexFloorFinishSlab), Is.False,
            "fresh web-ifc correctly tags floor finish #21658");

        var candMeshes = ModelComparer.EntityMeshes(candidate);
        var liveMeshes = ModelComparer.EntityMeshes(live);
        Assert.That(ExtentRatio(candMeshes[DuplexRoofSlab], liveMeshes[DuplexRoofSlab]), Is.GreaterThan(0.95));
        Assert.That(ExtentRatio(candMeshes[DuplexFloorFinishSlab], liveMeshes[DuplexFloorFinishSlab]), Is.GreaterThan(0.95));
    }

    [Test]
    [Category("Slow")]
    public void RegenerateBfast_FixesStaleDuplexSlabMisTags()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);

        if (WebIfcBfastOracle.IsStaleRelativeToLive(ifcPath, bfastPath))
        {
            var stale = ModelComparer.LoadOracle(ifcPath);
            var stalePairing = OracleTrustedPairingBuilder.Build(candidate, stale);
            TestContext.WriteLine(
                $"Stale BFAST had {stalePairing.MisTaggedCount} mis-tagged entities before regen");
        }

        WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);
        Assert.That(WebIfcBfastOracle.IsStaleRelativeToLive(ifcPath, bfastPath), Is.False,
            "regenerated on-disk BFAST should match live web-ifc entity bounds");
    }

    [Test]
    [Category("IfcMesherParity")]
    public void BfastRoundTrip_PreservesEntityAssignment_QuickFiles()
    {
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
            var report = WebIfcBfastOracle.CompareRoundTrip(file.ToModel3D());
            Assert.That(report.EntityAssignmentMatches, Is.True, $"{ifcPath.GetFileName()} BFAST I/O drift");
        }
    }

    [Test]
    [Category("Slow")]
    public void TrustedPairing_DetectsStaleCacheMisTags()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);
        var candidate = ModelComparer.LoadCandidate(ifcPath);
        var onDisk = WebIfcBfastOracle.CompareOnDiskWithLive(ifcPath);

        if (!onDisk.EntityAssignmentMatches)
        {
            var staleOracle = ModelComparer.LoadOracle(ifcPath);
            var pairing = OracleTrustedPairingBuilder.Build(candidate, staleOracle);
            Assert.That(pairing.MisTaggedCount, Is.GreaterThan(0),
                "stale on-disk BFAST should produce detectable mis-tags vs candidate");
            TestContext.WriteLine(
                $"On-disk BFAST stale: {onDisk.MismatchedEntityCount}/{onDisk.SharedEntityCount} entities differ from live");
        }
        else
        {
            TestContext.WriteLine("On-disk BFAST already matches live — run after regen to skip stale branch");
        }
    }

    static double ExtentRatio(TriangleMesh3D a, TriangleMesh3D b)
    {
        var sa = MeshHelpers.GetBounds(a).Max - MeshHelpers.GetBounds(a).Min;
        var sb = MeshHelpers.GetBounds(b).Max - MeshHelpers.GetBounds(b).Min;
        var ea = new[] { sa.X.Value, sa.Y.Value, sa.Z.Value }.OrderBy(x => x).ToArray();
        var eb = new[] { sb.X.Value, sb.Y.Value, sb.Z.Value }.OrderBy(x => x).ToArray();
        return new[]
        {
            Ratio(ea[0], eb[0]), Ratio(ea[1], eb[1]), Ratio(ea[2], eb[2]),
        }.Average();
    }

    static double Ratio(float x, float y)
    {
        if (x <= 0 && y <= 0) return 1.0;
        if (x <= 0 || y <= 0) return 0.0;
        var r = Math.Max(x, y) / Math.Min(x, y);
        return r <= 1.01 ? 1.0 : 1.0 / r;
    }
}
