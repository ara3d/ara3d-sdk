using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Ara3D.BimOpenSchema;

/// <summary>
/// Builds JSON objects for plumbing/fixture entities that may be toilets, matched by IFC category only.
/// </summary>
public static class BosToiletJson
{
    public static bool IsToiletCategory(string category)
        => category is
            "IFCFLOWTERMINAL" 
            or "IFCSANITARYTERMINAL"
            or "IFCSANITARYTERMINALTYPE"
            or "IFCSYSTEMFURNITUREELEMENT"
            or "IFCDISCRETEACCESSORY";

    public static bool IsToiletEntity(this EntityModel entity)
        => entity.IsNotTypeOrCategory && IsToiletCategory(entity.Category);

    public static IReadOnlyList<JsonObject> ToToiletJsonObjects(this IBimData data)
        => data.ToEntityJsonObjects(e => e.IsToiletEntity());

    public static IReadOnlyList<JsonObject> ToToiletJsonObjects(this BimObjectModel model)
        => model.ToEntityJsonObjects(e => e.IsToiletEntity());
}
