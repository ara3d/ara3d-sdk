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
    bool Unpaired);

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
    IReadOnlyList<InstanceTransformDelta> WorstInstances);

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
            foreach (var (candInst, oracleInst, unpaired) in pairs)
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

                deltas.Add(new InstanceTransformDelta(
                    entityId,
                    candInst.MeshIndex,
                    oracleInst.MeshIndex,
                    candMesh.FaceIndices.Count,
                    oracleMesh.FaceIndices.Count,
                    matrixDelta,
                    centerDist,
                    false));
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
            worst);
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
        sb.AppendLine();
        sb.AppendLine("| Entity | CandMesh | OracleMesh | Tri(c/o) | Frobenius | Trans | Rot° | CenterΔ |");
        sb.AppendLine("|---:|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var d in summary.WorstInstances)
        {
            sb.AppendLine(
                $"| {d.EntityId} | {d.CandidateMeshIndex} | {d.OracleMeshIndex} | " +
                $"{d.CandidateTriCount}/{d.OracleTriCount} | {d.MatrixDelta.Frobenius:F4} | " +
                $"{d.MatrixDelta.TranslationDistance:F4} | {d.MatrixDelta.RotationAngleDeg:F2} | {d.BoundsCenterDistance:F4} |");
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

    static List<(InstanceStruct Cand, InstanceStruct Oracle, bool Unpaired)> PairInstances(
        Model3D candidate,
        Model3D oracle,
        List<InstanceStruct> candList,
        List<InstanceStruct> oracleList)
    {
        var candKeys = candList
            .Select(i => (Inst: i, Tri: candidate.Meshes[i.MeshIndex].FaceIndices.Count))
            .OrderBy(x => x.Tri)
            .ThenBy(x => x.Inst.MeshIndex)
            .ToList();
        var oracleKeys = oracleList
            .Select(i => (Inst: i, Tri: oracle.Meshes[i.MeshIndex].FaceIndices.Count))
            .OrderBy(x => x.Tri)
            .ThenBy(x => x.Inst.MeshIndex)
            .ToList();

        var pairs = new List<(InstanceStruct, InstanceStruct, bool)>();
        var usedOracle = new bool[oracleKeys.Count];

        foreach (var (candInst, candTri) in candKeys)
        {
            var best = -1;
            var bestScore = double.MaxValue;
            for (var j = 0; j < oracleKeys.Count; j++)
            {
                if (usedOracle[j])
                    continue;
                var (oracleInst, oracleTri) = oracleKeys[j];
                if (oracleTri != candTri)
                    continue;

                var score = CompareMatrices(candInst.Matrix4x4, oracleInst.Matrix4x4).Frobenius;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = j;
                }
            }

            if (best >= 0)
            {
                usedOracle[best] = true;
                pairs.Add((candInst, oracleKeys[best].Inst, false));
            }
            else
            {
                pairs.Add((candInst, default, true));
            }
        }

        for (var j = 0; j < oracleKeys.Count; j++)
        {
            if (!usedOracle[j])
                pairs.Add((default, oracleKeys[j].Inst, true));
        }

        return pairs;
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
