using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Bounding-box and spatial-proximity queries (pure, P1). Bounds cross to sdk types through
/// C4's <see cref="RevitConverters.ToBounds3D"/> / <see cref="RevitConverters.ToXyz(Point3D)"/> (P7 —
/// values stay in Revit internal feet, no unit conversion here). Intersection is evaluated on
/// axis-aligned bounding boxes via <see cref="BoundingBoxIntersectsFilter"/>, so results are a
/// conservative superset of true solid intersection.
/// </summary>
public static class SpatialQueries
{
    /// <summary>
    /// The element's axis-aligned bounding box as an sdk <see cref="Bounds3D"/>, or null when the
    /// element has no box (Find contract, P8). Pass <paramref name="view"/> for the view-specific
    /// (cropped, view-dependent) box; null yields the model box.
    /// </summary>
    public static Bounds3D? FindBounds(this Element element, View? view = null)
        => element.get_BoundingBox(view)?.ToBounds3D();

    /// <summary>Throwing form of <see cref="FindBounds"/> (§3.3 <c>Get…</c> contract).</summary>
    public static Bounds3D GetBounds(this Element element, View? view = null)
        => element.FindBounds(view)
           ?? throw new InvalidOperationException($"Element {element.Id} has no bounding box.");

    /// <summary>Instances whose bounding box intersects <paramref name="bounds"/> (conservative, box-based).</summary>
    public static IReadOnlyList<Element> FindElementsIntersecting(this Document doc, Bounds3D bounds)
        => doc.CollectElements()
            .WherePasses(bounds.ToIntersectsFilter()).WhereElementIsNotElementType()
            .ToList();

    /// <summary>Instances of <typeparamref name="T"/> whose bounding box intersects <paramref name="bounds"/> (conservative, box-based).</summary>
    public static IReadOnlyList<T> FindElementsIntersecting<T>(this Document doc, Bounds3D bounds) where T : Element
        => doc.CollectElements().OfClass(typeof(T))
            .WherePasses(bounds.ToIntersectsFilter()).WhereElementIsNotElementType()
            .Cast<T>().ToList();

    /// <summary>
    /// Instances whose bounding box intersects the axis-aligned cube of half-extent
    /// <paramref name="radiusFeet"/> centred on <paramref name="point"/>. An approximation: the query
    /// region is a box, not a sphere, so corner hits up to √3·radius away can pass.
    /// <paramref name="radiusFeet"/> is in Revit internal units (feet, P7).
    /// </summary>
    public static IReadOnlyList<Element> FindElementsNear(this Document doc, Point3D point, double radiusFeet)
    {
        var r = new Vector3((float)radiusFeet, (float)radiusFeet, (float)radiusFeet);
        return doc.FindElementsIntersecting(new Bounds3D(point - r, point + r));
    }

    private static BoundingBoxIntersectsFilter ToIntersectsFilter(this Bounds3D bounds)
        => new(new Outline(bounds.Min.ToXyz(), bounds.Max.ToXyz()));
}
