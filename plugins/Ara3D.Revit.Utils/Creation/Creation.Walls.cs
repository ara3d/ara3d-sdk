using Ara3D.Geometry;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static partial class Creation
{
    private const double DefaultWallHeightMillimeters = 3000;

    /// <summary>
    /// Creates a rectangular-profile wall along <paramref name="baseline"/> on <paramref name="level"/>.
    /// Unconnected height is the distance to the next level above, or a 3 m default when there is
    /// none. Requires an open transaction.
    /// </summary>
    public static Wall CreateWall(this Document doc, Line3D baseline, WallType type, Level level)
        => Wall.Create(doc, baseline.ToRevitLine(), type.Id, level.Id,
            level.GetDefaultWallHeight(), 0, false, false);

    private static double GetDefaultWallHeight(this Level level)
        => level.FindLevelAbove() is { } above
            ? above.Elevation - level.Elevation
            : Units.FromMillimeters(DefaultWallHeightMillimeters);
}
