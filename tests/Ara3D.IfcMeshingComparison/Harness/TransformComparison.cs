using System.Text;
using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

public sealed record MatrixDelta(
    double Frobenius,
    double TranslationDistance,
    double RotationAngleDeg,
    double ScaleRatioMax);

public sealed record InstanceTransformDelta(
    int EntityId,
    int CandidateMeshIndex,
    int OracleMeshIndex,
    int CandidateTriCount,
    int OracleTriCount,
    MatrixDelta MatrixDelta,
    double BoundsCenterDistance,
    bool Unpaired,
    PlacementDeltaClass Classification = PlacementDeltaClass.Unknown,
    double PairConfidence = 1.0);

/// <summary>
/// Separates true instance-matrix / placement disagreement from bounds-center Δ driven by
/// different mesh extents (shape/instancing) while the transform itself mostly matches.
/// </summary>
public enum PlacementDeltaClass
{
    Unknown,
    /// <summary>Matrix translation or rotation disagrees materially with the oracle.</summary>
    PlacementMatrix,
    /// <summary>Matrix mostly matches; bounds-center Δ is inflated by shape / mesh extent mismatch.</summary>
    ShapeInflatedCenter,
    /// <summary>Both matrix and local mesh extents agree; residual center Δ is small or pairing noise.</summary>
    Matched,
}

public sealed record TransformComparisonSummary(
    int SharedEntityCount,
    int PairedInstanceCount,
    int UnpairedCandidateInstances,
    int UnpairedOracleInstances,
    double MeanFrobenius,
    double MaxFrobenius,
    double MeanTranslationDistance,
    double MaxTranslationDistance,
    double MeanRotationAngleDeg,
    double MaxRotationAngleDeg,
    double MeanBoundsCenterDistance,
    double MaxBoundsCenterDistance,
    IReadOnlyList<InstanceTransformDelta> WorstInstances,
    int PlacementMatrixCount = 0,
    int ShapeInflatedCenterCount = 0,
    int MatchedPlacementCount = 0,
    double MeanCenterDistancePlacementOnly = 0,
    double MaxCenterDistancePlacementOnly = 0,
    double MeanCenterDistanceShapeInflated = 0,
    double MaxCenterDistanceShapeInflated = 0);

public static class TransformComparison
{
    public static MatrixDelta CompareMatrices(Matrix4x4 candidate, Matrix4x4 oracle)
    {
        var fro = FrobeniusNorm(candidate - oracle);
        var tCand = candidate.Translation;
        var tOracle = oracle.Translation;
        var translation = (tCand - tOracle).Length();

        var rotCand = ExtractRotation(candidate);
        var rotOracle = ExtractRotation(oracle);
        var x = new Vector3(rotCand.M11, rotCand.M12, rotCand.M13);
        var y = new Vector3(rotCand.M21, rotCand.M22, rotCand.M23);
        var z = new Vector3(rotCand.M31, rotCand.M32, rotCand.M33);
        var ox = new Vector3(rotOracle.M11, rotOracle.M12, rotOracle.M13);
        var oy = new Vector3(rotOracle.M21, rotOracle.M22, rotOracle.M23);
        var oz = new Vector3(rotOracle.M31, rotOracle.M32, rotOracle.M33);
        var dot = Math.Clamp(
            Vector3.Dot(x, ox) + Vector3.Dot(y, oy) + Vector3.Dot(z, oz),
            -1.0,
            3.0);
        var angleDeg = Math.Acos(Math.Clamp((dot - 1.0) * 0.5, -1.0, 1.0)) * (180.0 / Math.PI);

        var scaleRatio = MaxScaleRatio(candidate, oracle);
        return new MatrixDelta(fro, translation, angleDeg, scaleRatio);
    }

    public static TransformComparisonSummary Compare(Model3D candidate, Model3D oracle)
    {
        var candByEntity = InstancesByEntity(candidate);
        var oracleByEntity = InstancesByEntity(oracle);
        var shared = candByEntity.Keys.Intersect(oracleByEntity.Keys).ToList();

        var deltas = new List<InstanceTransformDelta>();
        var unpairedCand = 0;
        var unpairedOracle = 0;

        foreach (var entityId in shared)
        {
            var pairs = PairInstances(candidate, oracle, candByEntity[entityId], oracleByEntity[entityId]);
            foreach (var (candInst, oracleInst, unpaired, confidence) in pairs)
            {
                if (unpaired)
                {
                    if (candInst.EntityIndex >= 0)
                        unpairedCand++;
                    if (oracleInst.EntityIndex >= 0)
                        unpairedOracle++;
                    continue;
                }

                var candMesh = candidate.Meshes[candInst.MeshIndex];
                var oracleMesh = oracle.Meshes[oracleInst.MeshIndex];
                var matrixDelta = CompareMatrices(candInst.Matrix4x4, oracleInst.Matrix4x4);
                var candBounds = MeshHelpers.GetBounds(MeshHelpers.Transform(candMesh, candInst.Matrix4x4));
                var oracleBounds = MeshHelpers.GetBounds(MeshHelpers.Transform(oracleMesh, oracleInst.Matrix4x4));
                var centerDist = (candBounds.Center.Vector3 - oracleBounds.Center.Vector3).Length();
                var classification = ClassifyDelta(matrixDelta, centerDist, candMesh, oracleMesh);

                deltas.Add(new InstanceTransformDelta(
                    entityId,
                    candInst.MeshIndex,
                    oracleInst.MeshIndex,
                    candMesh.FaceIndices.Count,
                    oracleMesh.FaceIndices.Count,
                    matrixDelta,
                    centerDist,
                    false,
                    classification,
                    confidence));
            }
        }

        unpairedCand += candByEntity.Keys.Except(oracleByEntity.Keys)
            .Sum(id => candByEntity[id].Count);
        unpairedOracle += oracleByEntity.Keys.Except(candByEntity.Keys)
            .Sum(id => oracleByEntity[id].Count);

        if (deltas.Count == 0)
        {
            return new TransformComparisonSummary(
                shared.Count,
                0,
                unpairedCand,
                unpairedOracle,
                0, 0, 0, 0, 0, 0, 0, 0,
                []);
        }

        var worst = deltas
            .OrderByDescending(d => d.BoundsCenterDistance)
            .ThenByDescending(d => d.MatrixDelta.Frobenius)
            .Take(20)
            .ToList();

        var placement = deltas.Where(d => d.Classification == PlacementDeltaClass.PlacementMatrix).ToList();
        var shapeInflated = deltas.Where(d => d.Classification == PlacementDeltaClass.ShapeInflatedCenter).ToList();
        var matched = deltas.Where(d => d.Classification == PlacementDeltaClass.Matched).ToList();

        return new TransformComparisonSummary(
            shared.Count,
            deltas.Count,
            unpairedCand,
            unpairedOracle,
            deltas.Average(d => d.MatrixDelta.Frobenius),
            deltas.Max(d => d.MatrixDelta.Frobenius),
            deltas.Average(d => d.MatrixDelta.TranslationDistance),
            deltas.Max(d => d.MatrixDelta.TranslationDistance),
            deltas.Average(d => d.MatrixDelta.RotationAngleDeg),
            deltas.Max(d => d.MatrixDelta.RotationAngleDeg),
            deltas.Average(d => d.BoundsCenterDistance),
            deltas.Max(d => d.BoundsCenterDistance),
            worst,
            placement.Count,
            shapeInflated.Count,
            matched.Count,
            placement.Count == 0 ? 0 : placement.Average(d => d.BoundsCenterDistance),
            placement.Count == 0 ? 0 : placement.Max(d => d.BoundsCenterDistance),
            shapeInflated.Count == 0 ? 0 : shapeInflated.Average(d => d.BoundsCenterDistance),
            shapeInflated.Count == 0 ? 0 : shapeInflated.Max(d => d.BoundsCenterDistance));
    }

    /// <summary>
    /// Classifies a paired instance: matrix disagreement vs shape-inflated bounds-center Δ.
    /// Prefers world bounds-center + rotation over raw Frobenius — candidate/oracle often disagree
    /// on local-frame baking (high Frobenius) while still agreeing on world placement.
    /// </summary>
    public static PlacementDeltaClass ClassifyDelta(
        MatrixDelta matrixDelta,
        double boundsCenterDistance,
        TriangleMesh3D candidateMesh,
        TriangleMesh3D oracleMesh)
    {
        const double matrixTransTol = 0.25; // metres
        const double matrixRotTolDeg = 2.0;
        const double centerTol = 0.25;
        const double centerLarge = 1.0;

        var rotationDisagree = matrixDelta.RotationAngleDeg > matrixRotTolDeg;
        var translationDisagree = matrixDelta.TranslationDistance > matrixTransTol;
        var centerDisagree = boundsCenterDistance > centerTol;
        var centerLargeDisagree = boundsCenterDistance > centerLarge;

        // Orientation wrong → true placement bug regardless of center.
        if (rotationDisagree)
            return PlacementDeltaClass.PlacementMatrix;

        // World centers agree: treat as matched even when Frobenius is large (baked vs unbaked frames).
        if (!centerDisagree)
            return PlacementDeltaClass.Matched;

        // Centers diverge a lot with matching rotation:
        // - matrix translation also diverges → placement/mapping bug
        // - matrix translation agrees → shape/extent inflated the bounds center
        if (centerLargeDisagree && translationDisagree)
            return PlacementDeltaClass.PlacementMatrix;

        _ = candidateMesh;
        _ = oracleMesh;
        return PlacementDeltaClass.ShapeInflatedCenter;
    }

    public static Model3D LoadBfastModel(FilePath bfastPath)
    {
        var data = RenderModelBfastSerializer.Load(bfastPath);
        try
        {
            var meshes = data.Meshes
                .Select(m => new TriangleMesh3D(m.Points.ToList(), m.FaceIndices.ToList()))
                .ToList();
            return new Model3D(meshes, data.InstanceData.ToList());
        }
        finally
        {
            data.Dispose();
        }
    }

    public static TransformComparisonSummary CompareToBfastRoundTrip(Model3D model, FilePath bfastPath)
    {
        TestFiles.ReportsDir.Create();
        var tempPath = TestFiles.ReportsDir.RelativeFile("_transform_roundtrip_probe.bfast");
        using (var renderData = new RenderModelData(3))
        {
            renderData.Update(model);
            renderData.Write(tempPath);
        }

        var loaded = LoadBfastModel(tempPath);
        try { File.Delete(tempPath); } catch { /* probe only */ }
        return Compare(model, loaded);
    }

    public static string FormatSummary(string label, TransformComparisonSummary summary)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"## {label}");
        sb.AppendLine($"- Shared entities: {summary.SharedEntityCount}");
        sb.AppendLine($"- Paired instances: {summary.PairedInstanceCount}");
        sb.AppendLine($"- Unpaired candidate/oracle instances: {summary.UnpairedCandidateInstances}/{summary.UnpairedOracleInstances}");
        sb.AppendLine($"- Mean matrix Frobenius: {summary.MeanFrobenius:F4} (max {summary.MaxFrobenius:F4})");
        sb.AppendLine($"- Mean translation distance: {summary.MeanTranslationDistance:F4} (max {summary.MaxTranslationDistance:F4})");
        sb.AppendLine($"- Mean rotation angle (deg): {summary.MeanRotationAngleDeg:F2} (max {summary.MaxRotationAngleDeg:F2})");
        sb.AppendLine($"- Mean bounds-center distance: {summary.MeanBoundsCenterDistance:F4} (max {summary.MaxBoundsCenterDistance:F4})");
        sb.AppendLine(
            $"- Placement split: matrix={summary.PlacementMatrixCount}, " +
            $"shape-inflated-center={summary.ShapeInflatedCenterCount}, matched={summary.MatchedPlacementCount}");
        sb.AppendLine(
            $"- Center Δ (placement-matrix only): mean={summary.MeanCenterDistancePlacementOnly:F4} " +
            $"(max {summary.MaxCenterDistancePlacementOnly:F4})");
        sb.AppendLine(
            $"- Center Δ (shape-inflated only): mean={summary.MeanCenterDistanceShapeInflated:F4} " +
            $"(max {summary.MaxCenterDistanceShapeInflated:F4})");
        sb.AppendLine();
        sb.AppendLine("| Entity | CandMesh | OracleMesh | Tri(c/o) | Frobenius | Trans | Rot° | CenterΔ | Conf | Class |");
        sb.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|");
        foreach (var d in summary.WorstInstances)
        {
            sb.AppendLine(
                $"| {d.EntityId} | {d.CandidateMeshIndex} | {d.OracleMeshIndex} | " +
                $"{d.CandidateTriCount}/{d.OracleTriCount} | {d.MatrixDelta.Frobenius:F4} | " +
                $"{d.MatrixDelta.TranslationDistance:F4} | {d.MatrixDelta.RotationAngleDeg:F2} | " +
                $"{d.BoundsCenterDistance:F4} | {d.PairConfidence:F2} | {d.Classification} |");
        }
        return sb.ToString();
    }

    static Dictionary<int, List<InstanceStruct>> InstancesByEntity(Model3D model)
    {
        var map = new Dictionary<int, List<InstanceStruct>>();
        foreach (var inst in model.Instances)
        {
            if (inst.EntityIndex < 0 || inst.MeshIndex < 0 || inst.MeshIndex >= model.Meshes.Count)
                continue;
            if (!map.TryGetValue(inst.EntityIndex, out var list))
                map[inst.EntityIndex] = list = [];
            list.Add(inst);
        }
        return map;
    }

    readonly record struct PairCandidate(InstanceStruct Inst, Vector3 Centroid, float Diagonal, int Tri);

    /// <summary>
    /// Pairs candidate and oracle instances of one entity by world-centroid nearest-assignment
    /// (WP-T1). The previous exact-triangle-count pairing mixed dissimilar 12-tri stubs and put a
    /// ~0.5 m noise floor under every placement metric. Triangle-count agreement is only a
    /// tiebreak/confidence signal now, not a hard gate.
    /// </summary>
    static List<(InstanceStruct Cand, InstanceStruct Oracle, bool Unpaired, double Confidence)> PairInstances(
        Model3D candidate,
        Model3D oracle,
        List<InstanceStruct> candList,
        List<InstanceStruct> oracleList)
    {
        var pairs = new List<(InstanceStruct, InstanceStruct, bool, double)>();

        // Empty / degenerate meshes have no finite world centroid; they cannot be placement-compared,
        // so route them straight to unpaired instead of polluting the mean with NaN/∞ distances.
        var cands = new List<PairCandidate>();
        foreach (var i in candList)
        {
            var d = Describe(candidate, i);
            if (IsFinite(d.Centroid)) cands.Add(d);
            else pairs.Add((i, default, true, 0.0));
        }
        var oracles = new List<PairCandidate>();
        foreach (var i in oracleList)
        {
            var d = Describe(oracle, i);
            if (IsFinite(d.Centroid)) oracles.Add(d);
            else pairs.Add((default, i, true, 0.0));
        }

        // All cross pairs, cheapest world-centroid distance first; triangle-count delta breaks ties.
        var candidates = new List<(double Dist, int TriDelta, int Ci, int Oj)>(cands.Count * oracles.Count);
        for (var ci = 0; ci < cands.Count; ci++)
            for (var oj = 0; oj < oracles.Count; oj++)
                candidates.Add((
                    (cands[ci].Centroid - oracles[oj].Centroid).Length(),
                    Math.Abs(cands[ci].Tri - oracles[oj].Tri),
                    ci,
                    oj));
        candidates.Sort((a, b) => a.Dist != b.Dist
            ? a.Dist.CompareTo(b.Dist)
            : a.TriDelta.CompareTo(b.TriDelta));

        var usedCand = new bool[cands.Count];
        var usedOracle = new bool[oracles.Count];
        foreach (var (dist, _, ci, oj) in candidates)
        {
            if (usedCand[ci] || usedOracle[oj])
                continue;
            usedCand[ci] = true;
            usedOracle[oj] = true;
            pairs.Add((cands[ci].Inst, oracles[oj].Inst, false, PairingConfidence(cands[ci], oracles[oj], dist)));
        }

        for (var ci = 0; ci < cands.Count; ci++)
            if (!usedCand[ci])
                pairs.Add((cands[ci].Inst, default, true, 0.0));
        for (var oj = 0; oj < oracles.Count; oj++)
            if (!usedOracle[oj])
                pairs.Add((default, oracles[oj].Inst, true, 0.0));

        return pairs;
    }

    static bool IsFinite(Vector3 v)
        => float.IsFinite(v.X.Value) && float.IsFinite(v.Y.Value) && float.IsFinite(v.Z.Value);

    static PairCandidate Describe(Model3D model, InstanceStruct inst)
    {
        var mesh = model.Meshes[inst.MeshIndex];
        var bounds = MeshHelpers.GetBounds(MeshHelpers.Transform(mesh, inst.Matrix4x4));
        var diagonal = (bounds.Max - bounds.Min).Length();
        return new PairCandidate(inst, bounds.Center.Vector3, diagonal, mesh.FaceIndices.Count);
    }

    /// <summary>
    /// Confidence that a pairing is real: close world centroids relative to instance size and
    /// matching triangle counts push toward 1; distant centroids or divergent tessellation push
    /// toward 0. Lets callers down-weight noisy pairs instead of trusting equal-tri coincidences.
    /// </summary>
    static double PairingConfidence(PairCandidate cand, PairCandidate oracle, double centroidDistance)
    {
        var refSize = Math.Max(Math.Max(cand.Diagonal, oracle.Diagonal), 1e-3f);
        var distanceScore = Math.Max(0.0, 1.0 - centroidDistance / refSize);
        var maxTri = Math.Max(cand.Tri, oracle.Tri);
        var triScore = maxTri == 0 ? 1.0 : (double)Math.Min(cand.Tri, oracle.Tri) / maxTri;
        return 0.7 * distanceScore + 0.3 * triScore;
    }

    static double FrobeniusNorm(Matrix4x4 m)
    {
        double sum = 0;
        sum += m.M11 * m.M11 + m.M12 * m.M12 + m.M13 * m.M13 + m.M14 * m.M14;
        sum += m.M21 * m.M21 + m.M22 * m.M22 + m.M23 * m.M23 + m.M24 * m.M24;
        sum += m.M31 * m.M31 + m.M32 * m.M32 + m.M33 * m.M33 + m.M34 * m.M34;
        sum += m.M41 * m.M41 + m.M42 * m.M42 + m.M43 * m.M43 + m.M44 * m.M44;
        return Math.Sqrt(sum);
    }

    static Matrix4x4 ExtractRotation(Matrix4x4 m)
    {
        var x = new Vector3(m.M11, m.M12, m.M13);
        var y = new Vector3(m.M21, m.M22, m.M23);
        var z = new Vector3(m.M31, m.M32, m.M33);
        var sx = x.Length();
        var sy = y.Length();
        var sz = z.Length();
        if (sx > 1e-9f) x /= sx;
        if (sy > 1e-9f) y /= sy;
        if (sz > 1e-9f) z /= sz;
        return new Matrix4x4(
            x.X, x.Y, x.Z, 0,
            y.X, y.Y, y.Z, 0,
            z.X, z.Y, z.Z, 0,
            0, 0, 0, 1);
    }

    static double MaxScaleRatio(Matrix4x4 a, Matrix4x4 b)
    {
        static double Scale(Matrix4x4 m)
        {
            var x = new Vector3(m.M11, m.M12, m.M13).Length();
            var y = new Vector3(m.M21, m.M22, m.M23).Length();
            var z = new Vector3(m.M31, m.M32, m.M33).Length();
            return Math.Max(x, Math.Max(y, z));
        }

        var sa = Scale(a);
        var sb = Scale(b);
        if (sa <= 1e-9 && sb <= 1e-9)
            return 1.0;
        if (sa <= 1e-9 || sb <= 1e-9)
            return double.PositiveInfinity;
        return Math.Max(sa, sb) / Math.Min(sa, sb);
    }
}
