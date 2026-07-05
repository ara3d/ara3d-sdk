using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Ara3D.BimOpenSchema;

/// <summary>
/// Builds JSON objects for room/space entities from BOS object-model data.
/// </summary>
public static class BosRoomJson
{
    public static bool IsRoomCategory(string category)
        => category is "IFCSPACE" or "IfcSpace" or "Room";

    public static bool IsRoomEntity(this EntityModel entity)
        => entity.IsNotTypeOrCategory && IsRoomCategory(entity.Category);

    public static IReadOnlyList<JsonObject> ToRoomJsonObjects(this IBimData data)
        => data.ToEntityJsonObjects(e => e.IsRoomEntity());

    public static IReadOnlyList<JsonObject> ToRoomJsonObjects(this BimObjectModel model)
        => model.ToEntityJsonObjects(e => e.IsRoomEntity());
}
