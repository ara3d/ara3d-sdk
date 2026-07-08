using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Meshers;

public sealed record MeshingResult(
    string BackendName,
    FilePath IfcPath,
    bool Success,
    long ElapsedMs,
    Model3D? Model,
    int MeshCount,
    int InstanceCount,
    int TriangleCount,
    Bounds3D Bounds,
    double SignedVolume,
    MeshingDiagnostics? Diagnostics,
    IReadOnlyList<string> Errors)
{
    public static MeshingResult Failed(string backendName, FilePath ifcPath, long elapsedMs, string error)
        => new(backendName, ifcPath, false, elapsedMs, null, 0, 0, 0, Bounds3D.Empty, 0, null, [error]);

    public static MeshingResult FromModel(
        string backendName,
        FilePath ifcPath,
        long elapsedMs,
        Model3D model,
        MeshingDiagnostics? diagnostics = null)
    {
        var stats = ModelStats.FromModel(model);
        return new MeshingResult(
            backendName,
            ifcPath,
            true,
            elapsedMs,
            model,
            stats.MeshCount,
            stats.InstanceCount,
            stats.TriangleCount,
            stats.Bounds,
            stats.SignedVolume,
            diagnostics,
            []);
    }
}
