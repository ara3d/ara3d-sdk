using System.Text;
using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Support;

public sealed record MeshStats(int TriangleCount, Bounds3D Bounds, double SignedVolume);

public static class OracleComparison
{
    public static MeshStats ComputeStats(TriangleMesh3D mesh)
        => new(mesh.FaceIndices.Count, MeshHelpers.GetBounds(mesh), MeshHelpers.SignedVolume(mesh));

    public static MeshStats ComputeStats(Model3D model)
    {
        var meshes = model.Meshes;
        var allPoints = new List<Point3D>();
        var triCount = 0;
        double volume = 0;

        foreach (var inst in model.Instances)
        {
            if (inst.MeshIndex < 0 || inst.MeshIndex >= meshes.Count)
                continue;
            var mesh = meshes[inst.MeshIndex];
            var transformed = MeshHelpers.Transform(mesh, inst.Matrix4x4);
            allPoints.AddRange(transformed.Points);
            triCount += transformed.FaceIndices.Count;
            volume += MeshHelpers.SignedVolume(transformed);
        }

        var bounds = allPoints.Count == 0 ? Bounds3D.Empty : allPoints.Bounds();
        return new MeshStats(triCount, bounds, volume);
    }

    public static MeshStats ComputeOracleStats(FilePath path)
    {
        using var file = new IfcFile(path, includeGeometry: true);
        var oracle = file.ToModel3D();
        return ComputeStats(oracle);
    }

    public static (MeshStats Mine, MeshStats Oracle) CompareFile(FilePath path, double boundsTolerance = 0.05, double volumeTolerance = 0.15)
    {
        TestFiles.RequireExists(path);
        using var stepFile = new IfcFile(path, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(stepFile);
        var mine = ComputeStats(model);
        var oracle = ComputeOracleStats(path);
        return (mine, oracle);
    }

    public static string FormatComparison(string label, MeshStats mine, MeshStats oracle)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"| Metric | {label} (mine) | Oracle |");
        sb.AppendLine("|---|---:|---:|");
        sb.AppendLine($"| Triangles | {mine.TriangleCount} | {oracle.TriangleCount} |");
        sb.AppendLine($"| Volume | {mine.SignedVolume:F3} | {oracle.SignedVolume:F3} |");
        sb.AppendLine($"| Min | {Format(mine.Bounds.Min)} | {Format(oracle.Bounds.Min)} |");
        sb.AppendLine($"| Max | {Format(mine.Bounds.Max)} | {Format(oracle.Bounds.Max)} |");
        return sb.ToString();
    }

    public static bool BoundsOverlap(Bounds3D a, Bounds3D b)
    {
        if (a.Equals(Bounds3D.Empty) || b.Equals(Bounds3D.Empty))
            return false;
        return a.Min.X <= b.Max.X && a.Max.X >= b.Min.X
            && a.Min.Y <= b.Max.Y && a.Max.Y >= b.Min.Y
            && a.Min.Z <= b.Max.Z && a.Max.Z >= b.Min.Z;
    }

    public static bool BoundsClose(Bounds3D a, Bounds3D b, double relTolerance)
    {
        if (a.Equals(Bounds3D.Empty) || b.Equals(Bounds3D.Empty))
            return true;
        var sizeA = a.Max - a.Min;
        var sizeB = b.Max - b.Min;
        var refSize = Math.Max(sizeA.Length(), Math.Max(sizeB.Length(), 1e-3f));
        return (a.Min - b.Min).Length() <= refSize * relTolerance &&
               (a.Max - b.Max).Length() <= refSize * relTolerance;
    }

    static string Format(Vector3 v) => $"({v.X:F2},{v.Y:F2},{v.Z:F2})";
}
