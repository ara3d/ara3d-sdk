using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher;

public sealed record IfcMeshingResult(
    string MesherName,
    bool Success,
    Model3D? Model,
    int MeshCount,
    int InstanceCount,
    int TriangleCount,
    Bounds3D Bounds,
    double SignedVolume,
    IReadOnlyList<string> Messages,
    IReadOnlyList<string> Errors)
{
    public static IfcMeshingResult Failed(string mesherName, string error)
        => new(mesherName, false, null, 0, 0, 0, Bounds3D.Empty, 0, [], [error]);

    public static IfcMeshingResult FromModel(
        string mesherName,
        Model3D model,
        IReadOnlyList<string>? messages = null)
    {
        var stats = IfcModelStats.FromModel(model);
        return new IfcMeshingResult(
            mesherName,
            true,
            model,
            stats.MeshCount,
            stats.InstanceCount,
            stats.TriangleCount,
            stats.Bounds,
            stats.SignedVolume,
            messages ?? [],
            []);
    }
}
