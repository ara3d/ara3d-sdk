using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Creation
{
    /// <summary>
    /// Cuts a rectangular opening in <paramref name="wall"/> using the axis-aligned bounding box of
    /// <paramref name="profile"/>. Revit's wall-opening API (unlike roof/floor/ceiling) only accepts
    /// two opposite rectangle corners, not an arbitrary boundary — an arbitrary polygon profile is
    /// not representable, so the bounding box is the closest faithful mapping. Requires an open
    /// transaction.
    /// </summary>
    public static Opening CreateOpening(this Wall wall, IReadOnlyList<Point3D> profile)
    {
        var points = profile.Select(p => p.ToXyz()).ToList();
        var min = new XYZ(points.Min(p => p.X), points.Min(p => p.Y), points.Min(p => p.Z));
        var max = new XYZ(points.Max(p => p.X), points.Max(p => p.Y), points.Max(p => p.Z));
        return wall.Document.Create.NewOpening(wall, min, max);
    }
}
