using System.Collections.Generic;
using System.Linq;
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
        => new BimObjectModel(data, computeParametersAndRelations: true).ToRoomJsonObjects();

    public static IReadOnlyList<JsonObject> ToRoomJsonObjects(this BimObjectModel model)
        => model.Entities
            .Where(e => e.IsRoomEntity())
            .Select(ToRoomJsonObject)
            .ToList();

    static JsonObject ToRoomJsonObject(EntityModel entity)
    {
        var room = new JsonObject
        {
            ["entity"] = ToEntityJson(entity),
            ["properties"] = ToPropertiesJson(entity),
            ["relations"] = ToRelationsJson(entity),
        };
        return room;
    }

    static JsonObject ToEntityJson(EntityModel entity)
        => new()
        {
            ["id"] = (int)entity.Index,
            ["localId"] = entity.LocalId,
            ["globalId"] = entity.GlobalId,
            ["name"] = entity.Name,
            ["category"] = entity.Category,
        };

    static JsonObject ToEntityRef(EntityModel entity)
        => new()
        {
            ["id"] = (int)entity.Index,
            ["name"] = entity.Name,
            ["category"] = entity.Category,
        };

    static JsonObject ToPropertiesJson(EntityModel entity)
    {
        var properties = new JsonObject();
        foreach (var (name, value) in entity.ParameterValues.OrderBy(kv => kv.Key))
            properties[name] = ToJsonValue(value);
        return properties;
    }

    static JsonObject ToRelationsJson(EntityModel entity)
    {
        var relations = new JsonObject
        {
            ["outgoing"] = new JsonArray(entity.OutgoingRelations
                .Select(r => ToRelationJson(r.RelationType.ToString(), r.Target))
                .ToArray()),
            ["incoming"] = new JsonArray(entity.IncomingRelations
                .Select(r => ToRelationJson(r.RelationType.ToString(), r.Target))
                .ToArray()),
        };
        return relations;
    }

    static JsonObject ToRelationJson(string relationType, EntityModel target)
        => new()
        {
            ["type"] = relationType,
            ["entity"] = ToEntityRef(target),
        };

    static JsonNode ToJsonValue(object value)
    {
        if (value == null)
            return null;

        return value switch
        {
            EntityModel entity => ToEntityRef(entity),
            string s => s,
            int i => i,
            float f => f,
            double d => d,
            bool b => b,
            _ => value.ToString(),
        };
    }
}
