using System.Globalization;
using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

public static class BimEntityHelpers
{
    /// <summary>IFC wall-like categories, aligned with <see cref="IfcFilter"/> WallsAndPlates.</summary>
    public static readonly HashSet<string> WallIfcCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "IFCWALL",
        "IFCWALLSTANDARDCASE",
        "IFCCURTAINWALL",
        "IFCCURTAINWALLS",
        "IFCPLATE",
    };

    public static bool IsRoom(EntityModel entity)
        => entity.IsNotTypeOrCategory && BosRoomJson.IsRoomCategory(entity.Category);

    public static bool HasValidBounds(Bounds3D bounds)
        => bounds.Min.X <= bounds.Max.X;

    public static bool IsWall(EntityModel entity)
    {
        var cat = entity.Category;
        if (!entity.IsNotTypeOrCategory || cat == null)
            return false;
        if (WallIfcCategories.Contains(cat))
            return true;
        return cat.StartsWith("wall", true, CultureInfo.InvariantCulture)
               || cat.StartsWith("curtain", true, CultureInfo.InvariantCulture)
               || cat.Contains("mullion", StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsDoor(EntityModel entity)
    {
        var cat = entity.Category;
        if (cat == null)
            return false;
        return cat == "IFCDOOR" || cat.StartsWith("door", true, CultureInfo.InvariantCulture);
    }

    public static Bounds3D GetEntityWorldBounds(EntityModel entity, IReadOnlyList<Bounds3D> meshBounds)
    {
        var bounds = Bounds3D.Empty;
        foreach (var inst in entity.Instances)
        {
            if (inst.MeshIndex < 0)
                continue;
            var b = meshBounds[inst.MeshIndex].Transform(inst.Matrix4x4);
            bounds = bounds.Min.X > bounds.Max.X ? b : bounds.Include(b);
        }

        return bounds;
    }

    /// <summary>
    /// IFC doors sit in walls on room boundaries, not inside space volumes — expand the room and test overlap.
    /// </summary>
    public static bool SpatiallyAssociates(Bounds3D room, Bounds3D door, float tolerance)
    {
        if (!HasValidBounds(room) || !HasValidBounds(door))
            return false;

        if (!room.IntervalZ().Intersects(door.IntervalZ()))
            return false;

        if (room.Intersects(door))
            return true;

        var expanded = room.Expand((tolerance, tolerance, 0.1f));
        return expanded.Intersects(door) || expanded.Contains(door.Center);
    }
}
