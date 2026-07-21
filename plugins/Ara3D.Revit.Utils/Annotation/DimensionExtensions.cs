using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class DimensionExtensions
{
    /// <summary>
    /// The dimension's origin. `Dimension.Origin` throws on a multi-segment dimension, so
    /// this falls back to the first segment's origin in that case.
    /// </summary>
    public static XYZ GetOrigin(this Dimension dimension)
        => dimension.Segments.IsEmpty ? dimension.Origin : dimension.Segments.get_Item(0).Origin;

    /// <summary>
    /// Points along the dimension's line marking each segment boundary (start, each break,
    /// end), in model coordinates. Empty when the dimension's curve isn't a straight line.
    /// </summary>
    public static IReadOnlyList<Point3D> GetSegmentPoints(this Dimension dimension)
    {
        if (dimension.Curve is not Line line) return Array.Empty<Point3D>();

        var direction = (line.GetEndPoint(1) - line.GetEndPoint(0)).Normalize();
        var origin = dimension.GetOrigin();
        var points = new List<XYZ>();

        if (dimension.Segments.IsEmpty)
        {
            var half = 0.5 * dimension.Value.GetValueOrDefault() * direction;
            points.Add(origin - half);
            points.Add(origin + half);
        }
        else
        {
            var p = origin;
            foreach (DimensionSegment segment in dimension.Segments)
            {
                var v = segment.Value.GetValueOrDefault() * direction;
                if (points.Count == 0) points.Add(p = origin - (0.5 * v));
                points.Add(p = p + v);
            }
        }

        return points.Select(pt => pt.ToPoint3D()).ToList();
    }
}
