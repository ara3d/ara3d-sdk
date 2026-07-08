using Ara3D.Geometry;
using Ara3D.Ifc.Mesher;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Meshers;

public sealed record ModelGeometryStats(
    int InstanceCount,
    int MeshCount,
    int TriangleCount,
    Bounds3D Bounds,
    double SignedVolume);

public static class ModelStats
{
    public static ModelGeometryStats FromModel(Model3D model)
    {
        var stats = IfcModelStats.FromModel(model);
        return new ModelGeometryStats(
            stats.InstanceCount,
            stats.MeshCount,
            stats.TriangleCount,
            stats.Bounds,
            stats.SignedVolume);
    }
}
