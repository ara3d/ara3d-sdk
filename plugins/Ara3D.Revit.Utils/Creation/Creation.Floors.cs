using System.Collections.Generic;
using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Creation
{
    /// <summary>
    /// Creates a floor from a single closed boundary loop on <paramref name="level"/>.
    /// Requires an open transaction.
    /// </summary>
    public static Floor CreateFloor(this Document doc, IReadOnlyList<Point3D> boundary, FloorType type, Level level)
        => Floor.Create(doc, new List<CurveLoop> { boundary.ToCurveLoop() }, type.Id, level.Id);
}
