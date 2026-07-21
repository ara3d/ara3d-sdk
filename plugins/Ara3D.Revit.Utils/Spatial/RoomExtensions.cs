using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Room queries: collection and room-membership lookups for family instances. Boundary
/// loops live on the shared <see cref="SpatialElementExtensions.GetBoundaryLoops"/>.
/// </summary>
public static class RoomExtensions
{
    public static IReadOnlyList<Room> GetRooms(this Document doc)
        => doc.GetElements<Room>();

    /// <summary>The instance's containing room, or null if it isn't in one (expected-absence Get, per C6 precedent).</summary>
    public static Room? GetRoom(this FamilyInstance instance)
        => instance.Room;

    /// <summary>Instances of <paramref name="category"/> whose <see cref="FamilyInstance.Room"/> is this room.</summary>
    public static IReadOnlyList<FamilyInstance> GetElementsInRoom(this Room room, BuiltInCategory category)
        => room.Document.GetElements<FamilyInstance>(category)
            .Where(fi => fi.Room?.Id == room.Id)
            .ToList();

    /// <summary>Groups instances by containing room id (-1 for instances not in any room).</summary>
    public static IReadOnlyDictionary<long, IReadOnlyList<FamilyInstance>> GroupByRoom(this IReadOnlyList<FamilyInstance> instances)
    {
        var result = new Dictionary<long, IReadOnlyList<FamilyInstance>>();
        foreach (var group in instances.GroupBy(fi => fi.Room?.Id.ToLong() ?? -1L))
            result[group.Key] = group.ToList();
        return result;
    }
}
