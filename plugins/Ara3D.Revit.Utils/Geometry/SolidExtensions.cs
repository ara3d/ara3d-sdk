using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Mass properties straight from Revit's exact BRep. Values are in Revit internal units
/// (cubic feet for volume, feet for the centroid). The sdk-typed, Revit-free counterparts on
/// <see cref="TriangleMesh3D"/> live in <see cref="MeshMassProperties"/>.
/// </summary>
public static class SolidExtensions
{
    /// <summary>Exact solid volume in Revit internal units (cubic feet).</summary>
    public static double GetVolume(this Solid solid)
        => solid.Volume;

    /// <summary>Exact solid centroid in model coordinates (feet).</summary>
    public static Point3D GetCentroid(this Solid solid)
        => solid.ComputeCentroid().ToPoint3D();
}
