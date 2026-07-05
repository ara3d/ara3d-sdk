using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Ara3D.BimOpenSchema;

/// <summary>
/// Builds JSON objects for BOS entity-model instances matching a predicate.
/// </summary>
public static class BosEntityJson
{
    public static JsonObject ToEntityJsonObject(EntityModel entity)
        => new()
        {
            ["entity"] = ToEntityJson(entity),
            ["properties"] = ToPropertiesJson(entity),
            ["relations"] = ToRelationsJson(entity),
        };

    public static IReadOnlyList<JsonObject> ToEntityJsonObjects(
        this BimObjectModel model, Func<EntityModel, bool> predicate)
        => model.Entities
            .Where(predicate)
            .Select(ToEntityJsonObject)
            .ToList();

    public static IReadOnlyList<JsonObject> ToEntityJsonObjects(
        this IBimData data, Func<EntityModel, bool> predicate)
        => new BimObjectModel(data, computeParametersAndRelations: true)
            .ToEntityJsonObjects(predicate);

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
