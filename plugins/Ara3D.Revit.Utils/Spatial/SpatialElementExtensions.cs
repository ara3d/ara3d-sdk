using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// The shared spatial-element geometry bridge — rooms and spaces both resolve their true
/// bounded solid via <see cref="SpatialElementGeometryCalculator"/> (accounts for sub-faces,
/// e.g. a room-bounding wall split by a level), crossed to an sdk mesh through C4's
/// <c>ToTriangleMesh</c> (P2).
/// </summary>
public static class SpatialElementExtensions
{
    public static TriangleMesh3D GetGeometry(this SpatialElement element)
    {
        var calculator = new SpatialElementGeometryCalculator(element.Document);
        var results = calculator.CalculateSpatialElementGeometry(element);
        return results.GetGeometry().ToTriangleMesh();
    }

    /// <summary>Spatial-element area (rooms, spaces, areas) in Revit internal units (square feet) — no conversion (P7); go through C5 for display units.</summary>
    public static double GetArea(this SpatialElement element)
        => element.Area;

    /// <summary>
    /// Each boundary loop's segments sampled to sdk points (straight segments: endpoints only;
    /// curved segments: <paramref name="curveSamples"/> points along the curve). Works for
    /// rooms, spaces, and areas.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<Point3D>> GetBoundaryLoops(this SpatialElement element, int curveSamples = 8)
    {
        var boundaries = element.GetBoundarySegments(new SpatialElementBoundaryOptions());
        if (boundaries == null)
            return [];
        var loops = new List<IReadOnlyList<Point3D>>(boundaries.Count);
        foreach (var loop in boundaries)
        {
            var points = new List<Point3D>();
            foreach (var segment in loop)
                points.AddRange(segment.GetCurve().SamplePoints(curveSamples));
            loops.Add(points);
        }
        return loops;
    }
}
