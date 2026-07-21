using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Pure spatial queries over views (P1) — cut planes and crop-region bounds. No transaction.
/// </summary>
public static class ViewGeometry
{
    /// <summary>The horizontal cut plane of a plan view, in model coordinates (feet).</summary>
    public static Autodesk.Revit.DB.Plane GetCutPlane(this ViewPlan plan)
    {
        var levelElevation = plan.GenLevel?.Elevation ?? 0.0;
        var cutPlaneOffset = plan.GetViewRange().GetOffset(PlanViewPlane.CutPlane);
        return Autodesk.Revit.DB.Plane.CreateByNormalAndOrigin(plan.ViewDirection, new XYZ(0, 0, levelElevation + cutPlaneOffset));
    }

    /// <summary>
    /// True if <paramref name="bounds"/> overlaps the view's crop region. The crop region is
    /// approximated by its world-space AABB — conservative for rotated section/elevation
    /// views (may report an overlap the actual, rotated crop doesn't have). Plan views
    /// additionally use the view range's bottom/cut elevations for the vertical extent.
    /// </summary>
    public static bool IntersectsBounds(this View view, Bounds3D bounds)
        => view.GetCropWorldBounds().Overlaps(bounds);

    private static Bounds3D GetCropWorldBounds(this View view)
    {
        var box = view.CropBox;
        var corners = new[]
        {
            new XYZ(box.Min.X, box.Min.Y, box.Min.Z), new XYZ(box.Max.X, box.Min.Y, box.Min.Z),
            new XYZ(box.Min.X, box.Max.Y, box.Min.Z), new XYZ(box.Max.X, box.Max.Y, box.Min.Z),
            new XYZ(box.Min.X, box.Min.Y, box.Max.Z), new XYZ(box.Max.X, box.Min.Y, box.Max.Z),
            new XYZ(box.Min.X, box.Max.Y, box.Max.Z), new XYZ(box.Max.X, box.Max.Y, box.Max.Z),
        }.Select(box.Transform.OfPoint).ToList();

        var min = new XYZ(corners.Min(p => p.X), corners.Min(p => p.Y), corners.Min(p => p.Z));
        var max = new XYZ(corners.Max(p => p.X), corners.Max(p => p.Y), corners.Max(p => p.Z));

        if (view is ViewPlan plan)
        {
            var range = plan.GetViewRange();
            if (plan.Document.GetElement(range.GetLevelId(PlanViewPlane.BottomClipPlane)) is Level bottomLevel &&
                plan.Document.GetElement(range.GetLevelId(PlanViewPlane.CutPlane)) is Level cutLevel)
            {
                min = new XYZ(min.X, min.Y, bottomLevel.Elevation + range.GetOffset(PlanViewPlane.BottomClipPlane));
                max = new XYZ(max.X, max.Y, cutLevel.Elevation + range.GetOffset(PlanViewPlane.CutPlane));
            }
        }

        return new Bounds3D(min.ToPoint3D(), max.ToPoint3D());
    }
}
