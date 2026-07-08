using System.Text;
using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

public static class GeometryComparison
{
    public static (ModelGeometryStats Mine, ModelGeometryStats Oracle) CompareFile(
        FilePath ifcPath,
        FilePath bfastOraclePath)
    {
        TestFiles.RequireExists(ifcPath);
        if (!bfastOraclePath.Exists())
            Assert.Ignore($"Missing BFAST oracle: {bfastOraclePath}. Run GenerateWebIfcBfastOracles first.");

        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(stepFile);
        return (ModelStats.FromModel(model), LoadBfastOracleStats(bfastOraclePath));
    }

    public static ModelGeometryStats LoadBfastOracleStats(FilePath bfastPath)
    {
        var data = RenderModelBfastSerializer.Load(bfastPath);
        try
        {
            return ModelStats.FromModel(data.ToModel3D());
        }
        finally
        {
            data.Dispose();
        }
    }

    public static string FormatComparison(string label, ModelGeometryStats mine, ModelGeometryStats oracle)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"| Metric | {label} (mine) | Oracle |");
        sb.AppendLine("|---|---:|---:|");
        sb.AppendLine($"| Instances | {mine.InstanceCount} | {oracle.InstanceCount} |");
        sb.AppendLine($"| Meshes | {mine.MeshCount} | {oracle.MeshCount} |");
        sb.AppendLine($"| Triangles | {mine.TriangleCount} | {oracle.TriangleCount} |");
        sb.AppendLine($"| Volume | {mine.SignedVolume:F3} | {oracle.SignedVolume:F3} |");
        sb.AppendLine($"| Min | {Format(mine.Bounds.Min)} | {Format(oracle.Bounds.Min)} |");
        sb.AppendLine($"| Max | {Format(mine.Bounds.Max)} | {Format(oracle.Bounds.Max)} |");
        return sb.ToString();
    }

    public static bool CountsRoughlyMatch(ModelGeometryStats mine, ModelGeometryStats oracle, double ratioTolerance = 0.5)
    {
        if (mine.TriangleCount == 0 && oracle.TriangleCount == 0)
            return true;
        if (mine.TriangleCount == 0 || oracle.TriangleCount == 0)
            return false;

        var triRatio = (double)mine.TriangleCount / oracle.TriangleCount;
        if (triRatio < 1 - ratioTolerance || triRatio > 1 + ratioTolerance)
            return false;

        if (oracle.MeshCount == 0)
        {
            if (mine.MeshCount != 0)
                return false;
        }
        else
        {
            var meshRatio = (double)mine.MeshCount / oracle.MeshCount;
            if (meshRatio < 1 - ratioTolerance || meshRatio > 1 + ratioTolerance)
                return false;
        }

        return true;
    }

    static string Format(Vector3 v) => $"({v.X:F2},{v.Y:F2},{v.Z:F2})";
}
