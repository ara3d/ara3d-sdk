using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

/// <summary>
/// WP prerequisite — reproduce/fix the native AccessViolation that blocked regenerating the
/// schependomlaan web-ifc oracle, then regenerate it so placement metrics measure against a fresh
/// oracle rather than a stale 2026-07-07 BFAST.
/// </summary>
[TestFixture]
public sealed class WpOracleRegenTests
{
    static FilePath SchependomlaanPath()
    {
        var p = TestFiles.LocalIfcDir.RelativeFile("schependomlaan.ifc");
        if (!p.Exists())
            p = new FilePath(@"C:\Users\cdigg\git\3d-format-shootout\data\git-repo-copies\web-ifc\schependomlaan.ifc");
        return p;
    }

    /// <summary>
    /// Low-level probe: walk every geometry/mesh via the raw web-ifc handles and count degenerate
    /// native handles (zero mesh ptr, zero vertex/index/transform/color buffer) WITHOUT dereferencing
    /// null — so this cannot itself AV. If any count is non-zero, that is the AV source in the
    /// blind ToModel3D() copy loop.
    /// </summary>
    [Test]
    [Explicit("diagnostic: locate the native AV in web-ifc mesh iteration on schependomlaan")]
    [Category("Slow")]
    public void Schependomlaan_ProbeNativeMeshHandles()
    {
        var ifcPath = SchependomlaanPath();
        if (!ifcPath.Exists())
        {
            Assert.Ignore($"schependomlaan.ifc not found at {ifcPath}");
            return;
        }

        using var file = new IfcFile(ifcPath, includeGeometry: true);
        var api = file.ApiPtr;
        var model = file.Model.ModelPtr;

        var geomCount = WebIfcDll.GetNumGeometries(api, model);
        var geoms = 0;
        var meshes = 0;
        var zeroGeom = 0;
        var zeroMesh = 0;
        var zeroVerts = 0;
        var zeroIndices = 0;
        var zeroTransform = 0;
        var zeroColor = 0;
        var nullBufNonZeroCount = 0;

        for (var gi = 0; gi < geomCount; gi++)
        {
            var gPtr = WebIfcDll.GetGeometryFromIndex(api, model, gi);
            if (gPtr == IntPtr.Zero)
            {
                zeroGeom++;
                continue;
            }
            geoms++;
            var numMeshes = WebIfcDll.GetNumMeshes(api, gPtr);
            for (var mi = 0; mi < numMeshes; mi++)
            {
                var mPtr = WebIfcDll.GetMesh(api, gPtr, mi);
                if (mPtr == IntPtr.Zero)
                {
                    zeroMesh++;
                    continue;
                }
                meshes++;
                var nv = WebIfcDll.GetNumVertices(api, mPtr);
                var ni = WebIfcDll.GetNumIndices(api, mPtr);
                if (WebIfcDll.GetVertices(api, mPtr) == IntPtr.Zero)
                {
                    zeroVerts++;
                    if (nv > 0) nullBufNonZeroCount++;
                }
                if (WebIfcDll.GetIndices(api, mPtr) == IntPtr.Zero)
                {
                    zeroIndices++;
                    if (ni > 0) nullBufNonZeroCount++;
                }
                if (WebIfcDll.GetTransform(api, mPtr) == IntPtr.Zero) zeroTransform++;
                if (WebIfcDll.GetColor(api, mPtr) == IntPtr.Zero) zeroColor++;
            }
        }

        TestContext.WriteLine(
            $"PROBE schependomlaan: geomCount={geomCount} nonNullGeoms={geoms} zeroGeom={zeroGeom} " +
            $"meshes={meshes} zeroMesh={zeroMesh} zeroVerts={zeroVerts} zeroIndices={zeroIndices} " +
            $"zeroTransform={zeroTransform} zeroColor={zeroColor} nullBufNonZeroCount={nullBufNonZeroCount}");
    }

    [Test]
    [Explicit("regenerate the schependomlaan web-ifc BFAST oracle after the AV fix")]
    [Category("Slow")]
    public void Schependomlaan_RegenerateOracle()
    {
        var ifcPath = SchependomlaanPath();
        if (!ifcPath.Exists())
        {
            Assert.Ignore($"schependomlaan.ifc not found at {ifcPath}");
            return;
        }

        WebIfcBfastOracle.Generate(ifcPath, s => TestContext.WriteLine(s));
        var oracle = WebIfcBfastOracle.OraclePath(ifcPath);
        Assert.That(oracle.Exists(), Is.True, $"expected regenerated oracle at {oracle}");
        Assert.That(WebIfcBfastOracle.HasInstances(oracle), Is.True);
    }

    /// <summary>
    /// Regression lock for the use-after-free AV: reading the cached BFAST oracle and then comparing
    /// against live web-ifc (the path that dereferences per-entity mesh bounds) must not AV-crash.
    /// </summary>
    [Test]
    [Explicit("verify the stale-check no longer AV-crashes on the 50 MB schependomlaan oracle")]
    [Category("Slow")]
    public void Schependomlaan_StaleCheck_DoesNotCrash()
    {
        var ifcPath = SchependomlaanPath();
        if (!ifcPath.Exists())
        {
            Assert.Ignore($"schependomlaan.ifc not found at {ifcPath}");
            return;
        }
        if (!WebIfcBfastOracle.OraclePath(ifcPath).Exists())
        {
            Assert.Ignore("oracle BFAST missing — run Schependomlaan_RegenerateOracle first");
            return;
        }

        var stale = WebIfcBfastOracle.IsStaleRelativeToLive(ifcPath);
        TestContext.WriteLine($"IsStaleRelativeToLive(schependomlaan) = {stale}");
    }

    /// <summary>
    /// Decide whether the "degenerate oracle" signature (oracleDet≈0, 12-tri) is native web-ifc output
    /// or a BFAST round-trip artifact: compare, per entity, the LIVE web-ifc mesh (no BFAST) against
    /// the on-disk oracle mesh for the top max-Δ entities.
    /// </summary>
    [Test]
    [Explicit("compare live web-ifc vs BFAST oracle per entity to attribute the degenerate signature")]
    [Category("Slow")]
    public void Schependomlaan_LiveVsOracle_PerEntity()
    {
        var ifcPath = SchependomlaanPath();
        if (!ifcPath.Exists())
        {
            Assert.Ignore($"schependomlaan.ifc not found at {ifcPath}");
            return;
        }

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var live = file.ToModel3D();
        var oracle = ModelComparer.LoadOracle(ifcPath);

        var liveEntities = ModelComparer.EntityMeshes(live);
        var oracleEntities = ModelComparer.EntityMeshes(oracle);

        var emptyLive = live.Instances.Count(i =>
            i.MeshIndex >= 0 && i.MeshIndex < live.Meshes.Count && live.Meshes[i.MeshIndex].FaceIndices.Count == 0);
        TestContext.WriteLine(
            $"live meshes={live.Meshes.Count} instances={live.Instances.Count} " +
            $"instancesPointingAtEmptyMesh={emptyLive}; oracle meshes={oracle.Meshes.Count} " +
            $"instances={oracle.Instances.Count}");

        int[] ids = { 948940, 897508, 267823, 792551, 968920, 616154 };
        foreach (var id in ids)
        {
            var hasLive = liveEntities.TryGetValue(id, out var lm);
            var hasOracle = oracleEntities.TryGetValue(id, out var om);
            var lTri = hasLive ? lm.FaceIndices.Count : -1;
            var oTri = hasOracle ? om.FaceIndices.Count : -1;
            var lB = hasLive ? MeshHelpers.GetBounds(lm) : default;
            var oB = hasOracle ? MeshHelpers.GetBounds(om) : default;
            TestContext.WriteLine(
                $"#{id}: liveTri={lTri} oracleTri={oTri} liveBounds={Fmt(lB)} oracleBounds={Fmt(oB)}");
        }
    }

    static string Fmt(Ara3D.Geometry.Bounds3D b)
        => $"[{b.Min.X.Value:F2},{b.Min.Y.Value:F2},{b.Min.Z.Value:F2}]-[{b.Max.X.Value:F2},{b.Max.Y.Value:F2},{b.Max.Z.Value:F2}]";
}
