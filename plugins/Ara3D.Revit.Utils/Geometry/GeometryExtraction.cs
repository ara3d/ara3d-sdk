using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;
using RevitMesh = Autodesk.Revit.DB.Mesh;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Knobs for pulling geometry off an element. The one sanctioned option-bag in the library
/// (P5): geometry extraction genuinely has several independent, orthogonal knobs.
/// <see cref="Default"/> (Fine detail, descend into instances, skip invisibles) is what the
/// <c>= default</c> parameter resolves to.
/// </summary>
public readonly record struct GeometryExtractionOptions(
    ViewDetailLevel DetailLevel = ViewDetailLevel.Fine,
    bool IncludeInstanceGeometry = true,
    bool IncludeInvisible = false,
    bool ComputeReferences = false)
{
    public static GeometryExtractionOptions Default => new();

    internal Options ToRevitOptions()
        => new()
        {
            DetailLevel = DetailLevel == ViewDetailLevel.Undefined ? ViewDetailLevel.Fine : DetailLevel,
            IncludeNonVisibleObjects = IncludeInvisible,
            ComputeReferences = ComputeReferences
        };
}

/// <summary>
/// Pulls solids, meshes and bounds off elements, descending through geometry instances into
/// symbol geometry as needed. All results are in Revit internal units (feet); meshes and
/// bounds are already sdk types (P2).
/// </summary>
public static class GeometryExtraction
{
    public static IReadOnlyList<Solid> GetSolids(this Element element, GeometryExtractionOptions options = default)
    {
        if (options == default)
            options = GeometryExtractionOptions.Default;
        var geo = element.get_Geometry(options.ToRevitOptions());
        if (geo == null)
            return [];
        var solids = new List<Solid>();
        CollectSolids(geo, options.IncludeInstanceGeometry, solids);
        return solids;
    }

    public static IReadOnlyList<TriangleMesh3D> GetMeshes(this Element element, GeometryExtractionOptions options = default)
    {
        if (options == default)
            options = GeometryExtractionOptions.Default;
        var geo = element.get_Geometry(options.ToRevitOptions());
        if (geo == null)
            return [];
        var meshes = new List<TriangleMesh3D>();
        CollectMeshes(geo, options.IncludeInstanceGeometry, meshes);
        return meshes;
    }

    /// <summary>Model-coordinate axis-aligned bounds; empty (default) when the element has none.</summary>
    public static Bounds3D GetBounds(this Element element)
    {
        var bb = element.get_BoundingBox(null);
        return bb == null ? default : bb.ToBounds3D();
    }

    private static void CollectSolids(GeometryElement geo, bool descendInstances, List<Solid> solids)
    {
        foreach (var obj in geo)
            switch (obj)
            {
                case Solid s when !s.Faces.IsEmpty:
                    solids.Add(s);
                    break;
                case GeometryInstance gi when descendInstances:
                    CollectSolids(gi.GetInstanceGeometry(), descendInstances, solids);
                    break;
            }
    }

    private static void CollectMeshes(GeometryElement geo, bool descendInstances, List<TriangleMesh3D> meshes)
    {
        foreach (var obj in geo)
            switch (obj)
            {
                case Solid s when !s.Faces.IsEmpty && SolidUtils.IsValidForTessellation(s):
                    meshes.Add(s.ToTriangleMesh());
                    break;
                case RevitMesh m:
                    meshes.Add(m.ToTriangleMesh());
                    break;
                case GeometryInstance gi when descendInstances:
                    CollectMeshes(gi.GetInstanceGeometry(), descendInstances, meshes);
                    break;
            }
    }
}
