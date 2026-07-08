using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

public sealed record ModelComparerOptions(
    double BoundsRelativeTolerance = 0.05,
    double ShapeExtentTolerance = 0.10,
    double TriangleRatioTolerance = 3.0,
    IReadOnlyDictionary<string, double>? MetricWeights = null)
{
    public static ModelComparerOptions Default { get; } = new();

    public IReadOnlyDictionary<string, double> Weights => MetricWeights ?? DefaultWeights;

    static readonly IReadOnlyDictionary<string, double> DefaultWeights =
        new Dictionary<string, double>(StringComparer.Ordinal)
        {
            ["meshCount"] = 0.10,
            ["instanceCount"] = 0.15,
            ["entityInstances"] = 0.15,
            ["entityBoundingBox"] = 0.20,
            ["meshBoundingBox"] = 0.10,
            ["meshShape"] = 0.15,
            ["mergedMesh"] = 0.15,
        };
}

public sealed record CountMetricScore(double Score, int Candidate, int Oracle);

public sealed record MergedMeshMetricScore(
    double Score,
    int CandidatePointCount,
    int OraclePointCount,
    int CandidateTriangleCount,
    int OracleTriangleCount,
    bool BoundsClose);

public sealed record EntityInstanceMetricScore(
    double Score,
    double KeyJaccard,
    IReadOnlyDictionary<string, int> DeltaHistogram,
    int SharedEntityCount,
    int CandidateOnlyEntityCount,
    int OracleOnlyEntityCount);

public sealed record BoundingBoxMetricScore(
    double Score,
    int ComparedCount,
    int MatchedCount,
    int CandidateOnlyCount,
    int OracleOnlyCount);

public sealed record ModelComparisonResult(
    string FileName,
    double ParityScore,
    CountMetricScore MeshCount,
    CountMetricScore InstanceCount,
    EntityInstanceMetricScore EntityInstances,
    BoundingBoxMetricScore EntityBoundingBox,
    BoundingBoxMetricScore MeshBoundingBox,
    double MeshShapeScore,
    MergedMeshMetricScore MergedMesh,
    DateTime ComparedUtc)
{
    public IReadOnlyDictionary<string, double> MetricScores => new Dictionary<string, double>
    {
        ["meshCount"] = MeshCount.Score,
        ["instanceCount"] = InstanceCount.Score,
        ["entityInstances"] = EntityInstances.Score,
        ["entityBoundingBox"] = EntityBoundingBox.Score,
        ["meshBoundingBox"] = MeshBoundingBox.Score,
        ["meshShape"] = MeshShapeScore,
        ["mergedMesh"] = MergedMesh.Score,
    };
}

public static class ModelComparer
{
    public static ModelComparisonResult Compare(
        Model3D candidate,
        Model3D oracle,
        string fileName,
        ModelComparerOptions? options = null)
    {
        options ??= ModelComparerOptions.Default;
        var comparedUtc = DateTime.UtcNow;

        var meshCount = CompareCounts(candidate.Meshes.Count, oracle.Meshes.Count);
        var instanceCount = CompareCounts(candidate.Instances.Count, oracle.Instances.Count);
        var entityInstances = CompareEntityInstances(candidate, oracle);
        var entityBoundingBox = CompareEntityBoundingBoxes(candidate, oracle, options.BoundsRelativeTolerance);
        var meshBoundingBox = CompareMeshBoundingBoxes(candidate, oracle, options.BoundsRelativeTolerance);
        var meshShape = CompareMeshShapes(candidate, oracle, options);
        var mergedMesh = CompareMergedMesh(candidate, oracle, options.BoundsRelativeTolerance);

        var weights = options.Weights;
        var parityScore =
            meshCount.Score * weights["meshCount"] +
            instanceCount.Score * weights["instanceCount"] +
            entityInstances.Score * weights["entityInstances"] +
            entityBoundingBox.Score * weights["entityBoundingBox"] +
            meshBoundingBox.Score * weights["meshBoundingBox"] +
            meshShape * weights["meshShape"] +
            mergedMesh.Score * weights["mergedMesh"];

        return new ModelComparisonResult(
            fileName,
            parityScore,
            meshCount,
            instanceCount,
            entityInstances,
            entityBoundingBox,
            meshBoundingBox,
            meshShape,
            mergedMesh,
            comparedUtc);
    }

    public static Model3D LoadCandidate(FilePath ifcPath)
    {
        TestFiles.RequireExists(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(stepFile);
        return model;
    }

    public static Model3D LoadOracle(FilePath ifcPath)
    {
        TestFiles.RequireExists(ifcPath);
        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (bfastPath.Exists())
        {
            var data = RenderModelBfastSerializer.Load(bfastPath);
            try
            {
                var instances = data.InstanceData.ToList();
                if (instances.Count > 0)
                    return new Model3D(CloneMeshes(data.Meshes), instances);
            }
            finally
            {
                data.Dispose();
            }
        }

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        return file.ToModel3D();
    }

    static IReadOnlyList<TriangleMesh3D> CloneMeshes(IReadOnlyList<TriangleMesh3D> meshes)
        => meshes.Select(m => new TriangleMesh3D(m.Points.ToList(), m.FaceIndices.ToList())).ToList();

    public static ModelComparisonResult CompareFile(FilePath ifcPath, ModelComparerOptions? options = null)
    {
        var candidate = LoadCandidate(ifcPath);
        var oracle = LoadOracle(ifcPath);
        return Compare(candidate, oracle, ifcPath.GetFileName(), options);
    }

    public static void WriteScorecard(IReadOnlyList<ModelComparisonResult> results, FilePath path)
    {
        TestFiles.ReportsDir.Create();
        path.GetParent().Create();
        var payload = new ScorecardDocument(
            DateTime.UtcNow,
            results.Select(ScorecardEntry.FromResult).ToList());
        var json = JsonSerializer.Serialize(payload, ScorecardJson.Options);
        File.WriteAllText(path, json);
    }

    public static string FormatResult(ModelComparisonResult result)
    {
        var m = result.MetricScores;
        return $"""
            | Metric | Score | Detail |
            |---|---:|---|
            | **Parity** | **{result.ParityScore:F3}** | |
            | Mesh count | {m["meshCount"]:F3} | {result.MeshCount.Candidate} vs {result.MeshCount.Oracle} |
            | Instance count | {m["instanceCount"]:F3} | {result.InstanceCount.Candidate} vs {result.InstanceCount.Oracle} |
            | Entity instances | {m["entityInstances"]:F3} | Jaccard={result.EntityInstances.KeyJaccard:F3}, shared={result.EntityInstances.SharedEntityCount} |
            | Entity bbox | {m["entityBoundingBox"]:F3} | {result.EntityBoundingBox.MatchedCount}/{result.EntityBoundingBox.ComparedCount} matched |
            | Mesh bbox | {m["meshBoundingBox"]:F3} | {result.MeshBoundingBox.MatchedCount}/{result.MeshBoundingBox.ComparedCount} matched |
            | Mesh shape | {m["meshShape"]:F3} | fingerprint pairs |
            | Merged mesh | {m["mergedMesh"]:F3} | tris {result.MergedMesh.CandidateTriangleCount} vs {result.MergedMesh.OracleTriangleCount} |
            """;
    }

    static CountMetricScore CompareCounts(int candidate, int oracle)
        => new(CountSimilarity(candidate, oracle), candidate, oracle);

    static EntityInstanceMetricScore CompareEntityInstances(Model3D candidate, Model3D oracle)
    {
        var mine = CountInstancesPerEntity(candidate);
        var theirs = CountInstancesPerEntity(oracle);
        var mineKeys = mine.Keys.ToHashSet();
        var oracleKeys = theirs.Keys.ToHashSet();
        var keyJaccard = Jaccard(mineKeys, oracleKeys);
        var shared = mineKeys.Intersect(oracleKeys).ToList();

        var countScore = shared.Count == 0
            ? keyJaccard
            : shared.Select(id => CountSimilarity(mine[id], theirs[id])).Average();

        var score = 0.5 * keyJaccard + 0.5 * countScore;
        var histogram = BuildDeltaHistogram(mine, theirs, shared);

        return new EntityInstanceMetricScore(
            score,
            keyJaccard,
            histogram,
            shared.Count,
            mineKeys.Except(oracleKeys).Count(),
            oracleKeys.Except(mineKeys).Count());
    }

    static BoundingBoxMetricScore CompareEntityBoundingBoxes(
        Model3D candidate,
        Model3D oracle,
        double relTolerance)
    {
        var mine = EntityBounds(candidate);
        var theirs = EntityBounds(oracle);
        return CompareBoundingBoxMaps(mine, theirs, relTolerance);
    }

    static BoundingBoxMetricScore CompareMeshBoundingBoxes(
        Model3D candidate,
        Model3D oracle,
        double relTolerance)
    {
        var pairs = PairMeshes(candidate, oracle);
        if (pairs.Count == 0)
        {
            var mineCount = candidate.Meshes.Count;
            var theirsCount = oracle.Meshes.Count;
            return new BoundingBoxMetricScore(
                mineCount == 0 && theirsCount == 0 ? 1.0 : 0.0,
                0,
                0,
                Math.Max(0, mineCount - theirsCount),
                Math.Max(0, theirsCount - mineCount));
        }

        var matched = 0;
        foreach (var pair in pairs)
        {
            if (PairedMeshBoundsClose(candidate, oracle, pair, relTolerance))
                matched++;
        }

        var compareScore = (double)matched / pairs.Count;
        var countPenalty = CountSimilarity(candidate.Meshes.Count, oracle.Meshes.Count);
        var score = 0.8 * compareScore + 0.2 * countPenalty;
        return new BoundingBoxMetricScore(
            score,
            pairs.Count,
            matched,
            Math.Max(0, candidate.Meshes.Count - oracle.Meshes.Count),
            Math.Max(0, oracle.Meshes.Count - candidate.Meshes.Count));
    }

    static double CompareMeshShapes(Model3D candidate, Model3D oracle, ModelComparerOptions options)
    {
        var pairs = PairMeshes(candidate, oracle);
        if (pairs.Count == 0)
        {
            var mineCount = candidate.Meshes.Count;
            var theirsCount = oracle.Meshes.Count;
            return mineCount == 0 && theirsCount == 0 ? 1.0 : 0.0;
        }

        var scores = pairs
            .Select(pair => ComparePairedMeshShape(candidate, oracle, pair, options))
            .ToArray();

        var pairScore = scores.Average();
        return 0.8 * pairScore + 0.2 * CountSimilarity(candidate.Meshes.Count, oracle.Meshes.Count);
    }

    static MergedMeshMetricScore CompareMergedMesh(Model3D candidate, Model3D oracle, double relTolerance)
    {
        var mine = ToMergedMesh(candidate);
        var theirs = ToMergedMesh(oracle);
        var pointScore = CountSimilarity(mine.Points.Count, theirs.Points.Count);
        var triScore = CountSimilarity(mine.FaceIndices.Count, theirs.FaceIndices.Count);
        var mineBounds = MeshHelpers.GetBounds(mine);
        var oracleBounds = MeshHelpers.GetBounds(theirs);
        var boundsClose = OracleComparison.BoundsClose(mineBounds, oracleBounds, relTolerance);
        var boundsScore = boundsClose ? 1.0 : BoundsRatioScore(mineBounds, oracleBounds);

        var score = (pointScore + triScore + boundsScore) / 3.0;
        return new MergedMeshMetricScore(
            score,
            mine.Points.Count,
            theirs.Points.Count,
            mine.FaceIndices.Count,
            theirs.FaceIndices.Count,
            boundsClose);
    }

    static double CompareMeshShapeFingerprint(
        TriangleMesh3D candidate,
        TriangleMesh3D oracle,
        ModelComparerOptions options)
    {
        try
        {
            var canonicalA = CanonicalizeMeshForComparison(candidate);
            var canonicalB = CanonicalizeMeshForComparison(oracle);
            var statsA = new MeshStatistics(canonicalA);
            var statsB = new MeshStatistics(canonicalB);

            var vertexScore = CountSimilarity(statsA.Mesh.Points.Count, statsB.Mesh.Points.Count);
            var triScore = CountSimilarity(statsA.Mesh.FaceIndices.Count, statsB.Mesh.FaceIndices.Count);
            var boundsScore = CompareOrientedExtents(
                statsA.OrientedBounds,
                statsB.OrientedBounds,
                options.ShapeExtentTolerance);
            var orientedScore = CompareOrientedExtents(
                statsA.OrientedBounds,
                statsB.OrientedBounds,
                options.ShapeExtentTolerance);
            var volumeScore = CompareVolume(
                MeshHelpers.SignedVolume(statsA.Mesh),
                MeshHelpers.SignedVolume(statsB.Mesh),
                options.TriangleRatioTolerance);

            return (vertexScore + triScore + boundsScore + orientedScore + volumeScore) / 5.0;
        }
        catch
        {
            return 0.0;
        }
    }

    static BoundingBoxMetricScore CompareBoundingBoxMaps(
        Dictionary<int, Bounds3D> mine,
        Dictionary<int, Bounds3D> theirs,
        double relTolerance)
    {
        var shared = mine.Keys.Intersect(theirs.Keys).ToList();
        if (shared.Count == 0)
        {
            var emptyScore = mine.Count == 0 && theirs.Count == 0 ? 1.0 : 0.0;
            return new BoundingBoxMetricScore(
                emptyScore,
                0,
                0,
                mine.Keys.Except(theirs.Keys).Count(),
                theirs.Keys.Except(mine.Keys).Count());
        }

        var matched = shared.Count(id =>
            OracleComparison.BoundsClose(mine[id], theirs[id], relTolerance));
        var matchScore = (double)matched / shared.Count;
        var keyScore = Jaccard(mine.Keys.ToHashSet(), theirs.Keys.ToHashSet());
        var score = 0.7 * matchScore + 0.3 * keyScore;
        return new BoundingBoxMetricScore(
            score,
            shared.Count,
            matched,
            mine.Keys.Except(theirs.Keys).Count(),
            theirs.Keys.Except(mine.Keys).Count());
    }

    static Dictionary<int, int> CountInstancesPerEntity(Model3D model)
    {
        var counts = new Dictionary<int, int>();
        foreach (var inst in model.Instances)
        {
            if (inst.EntityIndex < 0)
                continue;
            counts.TryGetValue(inst.EntityIndex, out var c);
            counts[inst.EntityIndex] = c + 1;
        }
        return counts;
    }

    static TriangleMesh3D ToMergedMesh(Model3D model)
    {
        var points = new List<Point3D>();
        var faces = new List<Integer3>();
        var offset = 0;
        foreach (var inst in model.Instances)
        {
            if (inst.MeshIndex < 0 || inst.MeshIndex >= model.Meshes.Count)
                continue;

            var mesh = model.Meshes[inst.MeshIndex];
            var matrix = inst.Matrix4x4;
            if (!matrix.Equals(Matrix4x4.Identity))
            {
                foreach (var p in mesh.Points)
                    points.Add(p.Transform(matrix));
            }
            else
            {
                points.AddRange(mesh.Points);
            }

            foreach (var face in mesh.FaceIndices)
            {
                faces.Add(new Integer3(
                    face.A + offset,
                    face.B + offset,
                    face.C + offset));
            }
            offset = points.Count;
        }
        return new TriangleMesh3D(points, faces);
    }

    static Dictionary<int, Bounds3D> EntityBounds(Model3D model)
    {
        var bounds = new Dictionary<int, Bounds3D>();
        var meshes = model.Meshes;
        foreach (var inst in model.Instances)
        {
            if (inst.EntityIndex < 0 || inst.MeshIndex < 0 || inst.MeshIndex >= meshes.Count)
                continue;

            var transformed = MeshHelpers.Transform(meshes[inst.MeshIndex], inst.Matrix4x4);
            var meshBounds = MeshHelpers.GetBounds(transformed);
            if (bounds.TryGetValue(inst.EntityIndex, out var existing))
                bounds[inst.EntityIndex] = existing.Merge(meshBounds);
            else
                bounds[inst.EntityIndex] = meshBounds;
        }
        return bounds;
    }

    static double ComparePairedMeshShape(
        Model3D candidate,
        Model3D oracle,
        MeshPair pair,
        ModelComparerOptions options)
    {
        var candidateMesh = GetComparisonMesh(candidate, pair.CandidateIndex, pair.SharedEntityId);
        var oracleMesh = GetComparisonMesh(oracle, pair.OracleIndex, pair.SharedEntityId);
        return CompareMeshShapeFingerprint(candidateMesh, oracleMesh, options);
    }

    static TriangleMesh3D GetComparisonMesh(Model3D model, int meshIndex, int? sharedEntityId)
    {
        var mesh = model.Meshes[meshIndex];
        if (sharedEntityId is null)
            return mesh;

        var instance = model.Instances.FirstOrDefault(
            i => i.EntityIndex == sharedEntityId && i.MeshIndex == meshIndex);
        if (instance.EntityIndex != sharedEntityId)
            return mesh;

        return MeshHelpers.Transform(mesh, instance.Matrix4x4);
    }

    static bool PairedMeshBoundsClose(
        Model3D candidate,
        Model3D oracle,
        MeshPair pair,
        double relTolerance)
    {
        var candidateMesh = GetComparisonMesh(candidate, pair.CandidateIndex, pair.SharedEntityId);
        var oracleMesh = GetComparisonMesh(oracle, pair.OracleIndex, pair.SharedEntityId);
        return OracleComparison.BoundsClose(
            MeshHelpers.GetBounds(candidateMesh),
            MeshHelpers.GetBounds(oracleMesh),
            relTolerance);
    }

    static IReadOnlyList<MeshPair> PairMeshes(Model3D candidate, Model3D oracle)
    {
        var entityPairs = PairMeshesByEntityUsage(candidate, oracle);
        if (entityPairs.Count > 0)
            return entityPairs;

        return PairMeshesByFingerprint(candidate.Meshes, oracle.Meshes);
    }

    static IReadOnlyList<MeshPair> PairMeshesByEntityUsage(Model3D candidate, Model3D oracle)
    {
        var candidateByEntity = InstancesByEntity(candidate);
        var oracleByEntity = InstancesByEntity(oracle);
        var votes = new Dictionary<(int CandidateMesh, int OracleMesh), (int Count, int SharedEntity)>();

        foreach (var entityId in candidateByEntity.Keys.Intersect(oracleByEntity.Keys))
        {
            foreach (var candidateInstance in candidateByEntity[entityId])
            {
                if (candidateInstance.MeshIndex < 0 || candidateInstance.MeshIndex >= candidate.Meshes.Count)
                    continue;

                foreach (var oracleInstance in oracleByEntity[entityId])
                {
                    if (oracleInstance.MeshIndex < 0 || oracleInstance.MeshIndex >= oracle.Meshes.Count)
                        continue;

                    var key = (candidateInstance.MeshIndex, oracleInstance.MeshIndex);
                    if (votes.TryGetValue(key, out var existing))
                        votes[key] = (existing.Count + 1, entityId);
                    else
                        votes[key] = (1, entityId);
                }
            }
        }

        if (votes.Count == 0)
            return [];

        var ranked = votes
            .OrderByDescending(kv => kv.Value.Count)
            .ThenByDescending(kv => MeshPairingScore(
                candidate.Meshes[kv.Key.CandidateMesh],
                oracle.Meshes[kv.Key.OracleMesh]))
            .ToList();

        var usedCandidates = new HashSet<int>();
        var usedOracles = new HashSet<int>();
        var pairs = new List<MeshPair>();
        foreach (var (key, vote) in ranked)
        {
            if (usedCandidates.Contains(key.CandidateMesh) || usedOracles.Contains(key.OracleMesh))
                continue;

            usedCandidates.Add(key.CandidateMesh);
            usedOracles.Add(key.OracleMesh);
            pairs.Add(new MeshPair(key.CandidateMesh, key.OracleMesh, vote.SharedEntity));
        }

        return pairs;
    }

    static Dictionary<int, List<InstanceStruct>> InstancesByEntity(Model3D model)
    {
        var map = new Dictionary<int, List<InstanceStruct>>();
        foreach (var instance in model.Instances)
        {
            if (instance.EntityIndex < 0)
                continue;
            if (!map.TryGetValue(instance.EntityIndex, out var list))
            {
                list = [];
                map[instance.EntityIndex] = list;
            }
            list.Add(instance);
        }
        return map;
    }

    static IReadOnlyList<MeshPair> PairMeshesByFingerprint(
        IReadOnlyList<TriangleMesh3D> candidateMeshes,
        IReadOnlyList<TriangleMesh3D> oracleMeshes)
    {
        if (candidateMeshes.Count == 0 || oracleMeshes.Count == 0)
            return [];

        var scored = new List<(double Score, int CandidateIndex, int OracleIndex)>();
        for (var candidateIndex = 0; candidateIndex < candidateMeshes.Count; candidateIndex++)
        {
            for (var oracleIndex = 0; oracleIndex < oracleMeshes.Count; oracleIndex++)
            {
                var score = MeshPairingScore(candidateMeshes[candidateIndex], oracleMeshes[oracleIndex]);
                if (score > 0)
                    scored.Add((score, candidateIndex, oracleIndex));
            }
        }

        scored.Sort((a, b) => b.Score.CompareTo(a.Score));

        var usedCandidates = new HashSet<int>();
        var usedOracles = new HashSet<int>();
        var pairs = new List<MeshPair>();
        foreach (var (score, candidateIndex, oracleIndex) in scored)
        {
            if (usedCandidates.Contains(candidateIndex) || usedOracles.Contains(oracleIndex))
                continue;

            usedCandidates.Add(candidateIndex);
            usedOracles.Add(oracleIndex);
            pairs.Add(new MeshPair(candidateIndex, oracleIndex, null));
        }

        return pairs;
    }

    static double MeshPairingScore(TriangleMesh3D candidate, TriangleMesh3D oracle)
    {
        if (candidate.FaceIndices is null || oracle.FaceIndices is null ||
            candidate.Points is null || oracle.Points is null)
            return 0.0;

        var triScore = CountSimilarity(candidate.FaceIndices.Count, oracle.FaceIndices.Count);
        if (triScore <= 0)
            return 0.0;

        var vertexScore = CountSimilarity(candidate.Points.Count, oracle.Points.Count);
        var candidateExtents = SortedExtents(CenteredBounds(candidate).Max - CenteredBounds(candidate).Min);
        var oracleExtents = SortedExtents(CenteredBounds(oracle).Max - CenteredBounds(oracle).Min);
        var extentScore = (
            ExtentRatioScore(candidateExtents[0], oracleExtents[0]) +
            ExtentRatioScore(candidateExtents[1], oracleExtents[1]) +
            ExtentRatioScore(candidateExtents[2], oracleExtents[2])) / 3.0;

        return triScore * 0.45 + vertexScore * 0.20 + extentScore * 0.35;
    }

    static Bounds3D CenteredBounds(TriangleMesh3D mesh)
        => MeshHelpers.GetBounds(CanonicalizeMeshForComparison(mesh));

    static TriangleMesh3D CanonicalizeMeshForComparison(TriangleMesh3D mesh)
    {
        var bounds = MeshHelpers.GetBounds(mesh);
        if (bounds.Equals(Bounds3D.Empty))
            return mesh;

        var offset = -bounds.Center.Vector3;
        return MeshHelpers.Transform(mesh, Matrix4x4.CreateTranslation(offset));
    }

    static IReadOnlyDictionary<string, int> BuildDeltaHistogram(
        Dictionary<int, int> mine,
        Dictionary<int, int> theirs,
        List<int> shared)
    {
        var histogram = new Dictionary<string, int>
        {
            ["0"] = 0,
            ["1"] = 0,
            ["2-5"] = 0,
            ["6+"] = 0,
        };

        foreach (var id in shared)
        {
            var delta = Math.Abs(mine[id] - theirs[id]);
            var bucket = delta switch
            {
                0 => "0",
                1 => "1",
                <= 5 => "2-5",
                _ => "6+",
            };
            histogram[bucket]++;
        }

        return histogram;
    }

    static double CountSimilarity(int a, int b)
    {
        if (a == 0 && b == 0)
            return 1.0;
        if (a == 0 || b == 0)
            return 0.0;
        return Math.Min(a, b) / (double)Math.Max(a, b);
    }

    static double Jaccard<T>(ISet<T> a, ISet<T> b)
    {
        if (a.Count == 0 && b.Count == 0)
            return 1.0;
        var intersection = a.Intersect(b).Count();
        var union = a.Union(b).Count();
        return union == 0 ? 1.0 : (double)intersection / union;
    }

    static double BoundsRatioScore(Bounds3D a, Bounds3D b)
    {
        if (a.Equals(Bounds3D.Empty) || b.Equals(Bounds3D.Empty))
            return 0.0;

        var sizeA = a.Max - a.Min;
        var sizeB = b.Max - b.Min;
        var extentScore = (
            ExtentRatioScore(sizeA.X, sizeB.X) +
            ExtentRatioScore(sizeA.Y, sizeB.Y) +
            ExtentRatioScore(sizeA.Z, sizeB.Z)) / 3.0;
        var centerDist = (a.Center.Vector3 - b.Center.Vector3).Length();
        var refSize = Math.Max(sizeA.Length(), Math.Max(sizeB.Length(), 1e-3f));
        var centerScore = Math.Max(0.0, 1.0 - centerDist / refSize);
        return 0.6 * extentScore + 0.4 * centerScore;
    }

    static double CompareOrientedExtents(OrientedBox3D a, OrientedBox3D b, double relTolerance)
    {
        var maxRatio = 1.0 + relTolerance;
        var extentsA = SortedExtents(a.Size);
        var extentsB = SortedExtents(b.Size);
        var scores = new[]
        {
            ExtentRatioScore(extentsA[0], extentsB[0], maxRatio),
            ExtentRatioScore(extentsA[1], extentsB[1], maxRatio),
            ExtentRatioScore(extentsA[2], extentsB[2], maxRatio),
        };
        return scores.Average();
    }

    static float[] SortedExtents(Vector3 size)
        => new[] { size.X.Value, size.Y.Value, size.Z.Value }.OrderBy(x => x).ToArray();

    static double ExtentRatioScore(float a, float b, double maxRatio = 3.0)
    {
        if (a <= 0 && b <= 0)
            return 1.0;
        if (a <= 0 || b <= 0)
            return 0.0;
        var ratio = Math.Max(a, b) / Math.Min(a, b);
        if (ratio <= 1.0)
            return 1.0;
        return ratio <= maxRatio ? 1.0 - (ratio - 1.0) / (maxRatio - 1.0) : 0.0;
    }

    static double CompareVolume(double a, double b, double ratioTolerance)
    {
        if (Math.Sign(a) != Math.Sign(b))
            return 0.0;
        var absA = Math.Abs(a);
        var absB = Math.Abs(b);
        if (absA <= 1e-9 && absB <= 1e-9)
            return 1.0;
        if (absA <= 1e-9 || absB <= 1e-9)
            return 0.0;
        var ratio = Math.Max(absA, absB) / Math.Min(absA, absB);
        return ratio <= ratioTolerance ? 1.0 - (ratio - 1.0) / (ratioTolerance - 1.0) : 0.0;
    }

    readonly record struct MeshPair(int CandidateIndex, int OracleIndex, int? SharedEntityId);

    sealed record ScorecardDocument(
        DateTime GeneratedUtc,
        IReadOnlyList<ScorecardEntry> Files);

    sealed record ScorecardEntry(
        string FileName,
        double ParityScore,
        IReadOnlyDictionary<string, double> Metrics,
        CountMetricScore MeshCount,
        CountMetricScore InstanceCount,
        EntityInstanceMetricScore EntityInstances,
        BoundingBoxMetricScore EntityBoundingBox,
        BoundingBoxMetricScore MeshBoundingBox,
        double MeshShapeScore,
        MergedMeshMetricScore MergedMesh,
        DateTime ComparedUtc)
    {
        public static ScorecardEntry FromResult(ModelComparisonResult result)
            => new(
                result.FileName,
                result.ParityScore,
                result.MetricScores,
                result.MeshCount,
                result.InstanceCount,
                result.EntityInstances,
                result.EntityBoundingBox,
                result.MeshBoundingBox,
                result.MeshShapeScore,
                result.MergedMesh,
                result.ComparedUtc);
    }

    static class ScorecardJson
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
