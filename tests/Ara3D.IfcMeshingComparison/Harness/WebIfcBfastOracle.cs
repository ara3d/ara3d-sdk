using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>Result of comparing live web-ifc <c>ToModel3D()</c> output with a cached BFAST oracle.</summary>
public sealed record BfastLiveParityReport(
    bool EntityAssignmentMatches,
    int SharedEntityCount,
    int MismatchedEntityCount,
    IReadOnlyList<int> MismatchedEntityIds);

/// <summary>Generates WebIfc geometry oracles as BFAST files under <c>data/bfast/webifc/</c>.</summary>
public static class WebIfcBfastOracle
{
    public static FilePath OraclePath(FilePath ifcPath)
        => TestFiles.WebIfcBfastDir.RelativeFile(ifcPath.GetFileNameWithoutExtension() + ".bfast");

    public static bool NeedsRegeneration(FilePath ifcPath, FilePath bfastPath)
        => !bfastPath.Exists()
           || File.GetLastWriteTimeUtc(ifcPath) > File.GetLastWriteTimeUtc(bfastPath)
           || !HasInstances(bfastPath)
           || IsStaleRelativeToLive(ifcPath, bfastPath);

    public static bool HasInstances(FilePath bfastPath)
    {
        if (!bfastPath.Exists())
            return false;
        using var data = RenderModelBfastSerializer.Load(bfastPath);
        return data.InstanceData.Count > 0;
    }

    public static void Generate(FilePath ifcPath, Action<string>? log = null)
    {
        TestFiles.WebIfcBfastDir.Create();
        var outPath = OraclePath(ifcPath);

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var model = file.ToModel3D();
        using var renderData = new RenderModelData(3);
        renderData.Update(model);
        renderData.Write(outPath);

        var triangleCount = model.Meshes.Sum(m => m.FaceIndices.Count);
        log?.Invoke(
            $"{ifcPath.GetFileName()}: meshes={model.Meshes.Count}, instances={model.Instances.Count}, " +
            $"triangles={triangleCount} -> {outPath}");
    }

    public static void GenerateAll(IEnumerable<FilePath> ifcFiles, Action<string>? log = null)
    {
        foreach (var ifcPath in ifcFiles)
        {
            if (!ifcPath.Exists())
            {
                log?.Invoke($"Skipping missing IFC: {ifcPath}");
                continue;
            }

            var outPath = OraclePath(ifcPath);
            if (!NeedsRegeneration(ifcPath, outPath))
            {
                log?.Invoke($"Up to date: {ifcPath.GetFileName()}");
                continue;
            }

            Generate(ifcPath, log);
        }
    }

    /// <summary>True when the on-disk BFAST no longer matches a fresh web-ifc <c>ToModel3D()</c>.</summary>
    public static bool IsStaleRelativeToLive(FilePath ifcPath, FilePath? bfastPath = null)
    {
        var path = bfastPath ?? OraclePath(ifcPath);
        if (!path.Exists())
            return true;

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var live = file.ToModel3D();
        var cached = LoadBfastModel(path);
        if (CompareEntityAssignments(live, cached).EntityAssignmentMatches)
            return false;

        // Mesh-index reordering during BFAST I/O can change tri-count multisets while geometry is
        // unchanged; fall back to per-entity world-space bounds to detect real assignment drift.
        return !EntityBoundsMatch(live, cached);
    }

    /// <summary>Compare on-disk BFAST against live web-ifc (no write).</summary>
    public static BfastLiveParityReport CompareOnDiskWithLive(FilePath ifcPath)
    {
        TestFiles.RequireExists(ifcPath);
        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var live = file.ToModel3D();
        var bfastPath = OraclePath(ifcPath);
        if (!bfastPath.Exists())
            return new BfastLiveParityReport(false, 0, 0, []);

        var cached = LoadBfastModel(bfastPath);
        return CompareEntityAssignments(live, cached);
    }

    /// <summary>Round-trip an in-memory model through BFAST write/load to verify I/O fidelity.</summary>
    public static BfastLiveParityReport CompareRoundTrip(Model3D live)
    {
        TestFiles.WebIfcBfastDir.Create();
        var tempPath = TestFiles.WebIfcBfastDir.RelativeFile("_roundtrip_probe.bfast");
        using (var renderData = new RenderModelData(3))
        {
            renderData.Update(live);
            renderData.Write(tempPath);
        }

        var cached = LoadBfastModel(tempPath);
        try { File.Delete(tempPath); } catch { /* probe file only */ }
        return CompareEntityAssignments(live, cached);
    }

    static Model3D LoadBfastModel(FilePath bfastPath)
    {
        var data = RenderModelBfastSerializer.Load(bfastPath);
        try
        {
            return new Model3D(data.Meshes.ToList(), data.InstanceData.ToList());
        }
        finally
        {
            data.Dispose();
        }
    }

    static BfastLiveParityReport CompareEntityAssignments(Model3D live, Model3D cached)
    {
        var liveKeys = InstanceKeys(live);
        var cachedKeys = InstanceKeys(cached);
        var shared = liveKeys.Keys.Intersect(cachedKeys.Keys).ToList();
        var mismatched = shared.Where(id => !liveKeys[id].SequenceEqual(cachedKeys[id])).ToList();

        return new BfastLiveParityReport(
            mismatched.Count == 0,
            shared.Count,
            mismatched.Count,
            mismatched);
    }

    static Dictionary<int, int[]> InstanceKeys(Model3D model)
    {
        var keys = new Dictionary<int, List<int>>();
        foreach (var inst in model.Instances)
        {
            if (inst.EntityIndex < 0 || inst.MeshIndex < 0 || inst.MeshIndex >= model.Meshes.Count)
                continue;
            var triCount = model.Meshes[inst.MeshIndex].FaceIndices.Count;
            if (!keys.TryGetValue(inst.EntityIndex, out var list))
                keys[inst.EntityIndex] = list = [];
            list.Add(triCount);
        }
        return keys.ToDictionary(kv => kv.Key, kv => kv.Value.OrderBy(x => x).ToArray());
    }

    static bool EntityBoundsMatch(Model3D live, Model3D cached, double relTolerance = 0.02)
    {
        var liveMeshes = ModelComparer.EntityMeshes(live);
        var cachedMeshes = ModelComparer.EntityMeshes(cached);
        var shared = liveMeshes.Keys.Intersect(cachedMeshes.Keys).ToList();
        if (shared.Count == 0)
            return liveMeshes.Count == 0 && cachedMeshes.Count == 0;

        var matched = shared.Count(id =>
            BoundsClose(MeshHelpers.GetBounds(liveMeshes[id]), MeshHelpers.GetBounds(cachedMeshes[id]), relTolerance));
        return matched == shared.Count;
    }

    static bool BoundsClose(Bounds3D a, Bounds3D b, double relTolerance)
    {
        if (a.Equals(Bounds3D.Empty) || b.Equals(Bounds3D.Empty))
            return a.Equals(b);
        var sizeA = a.Max - a.Min;
        var sizeB = b.Max - b.Min;
        var refSize = Math.Max(sizeA.Length(), Math.Max(sizeB.Length(), 1e-3f));
        return (a.Min - b.Min).Length() <= refSize * relTolerance &&
               (a.Max - b.Max).Length() <= refSize * relTolerance;
    }
}
