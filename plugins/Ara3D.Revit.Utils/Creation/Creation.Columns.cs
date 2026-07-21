using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Creation
{
    /// <summary>
    /// Places a column family instance at <paramref name="at"/> on <paramref name="level"/>,
    /// activating <paramref name="type"/> first if needed. Requires an open transaction.
    /// </summary>
    public static FamilyInstance CreateColumn(this Document doc, Point3D at, FamilySymbol type, Level level)
    {
        type.EnsureActive();
        return doc.PlaceInstance(type, at, level);
    }
}
