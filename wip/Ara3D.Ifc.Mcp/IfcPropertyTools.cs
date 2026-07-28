using Ara3D.IfcLoader;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>Tools that read property sets and quantity sets. Both come from the same index —
/// IFC models an element quantity as a property set whose members are quantities.</summary>
public static class IfcPropertyTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_properties",
                "Returns the properties attached to an element, grouped by property set name. "
                + "Quantities are excluded; use ifc_quantities for those.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the element.", required: true)
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Properties(args.Session(cache), args.GetRequiredInt("id"), false, args.Skip(), args.Take()),
                    ["ifc_quantities", "ifc_relations"]))
            .Tool(
                "ifc_quantities",
                "Returns the quantities (length, area, volume, count, weight, time) attached to an element.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the element.", required: true)
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Properties(args.Session(cache), args.GetRequiredInt("id"), true, args.Skip(), args.Take())))
            .Tool(
                "ifc_property_sets",
                "Lists the names and ids of the property sets attached to an element, without their values.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the element.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => PropertySets(args.Session(cache), args.GetRequiredInt("id")),
                    ["ifc_properties"]));

    private static object Properties(IfcSession session, int id, bool quantities, int skip, int take)
    {
        var entity = IfcEntityTools.Resolve(session, id);
        var data = session.Properties;
        var document = session.File.Document;
        var result = new List<IfcProperty>();

        foreach (var propSet in data.GetPropSets(id))
            foreach (var value in data.GetProperties(propSet))
            {
                if ((value.Kind == IfcPropKind.Quantity) != quantities)
                    continue;

                // A quantity holds a bare number, so its own entity name is the only measure type.
                var measure = value.GetMeasureType();
                if (measure.Length == 0)
                    measure = value.EntityName;

                result.Add(new IfcProperty(
                    propSet.Name,
                    value.Name,
                    value.Kind.ToString(),
                    measure,
                    value.GetValueText(document)));
            }

        return new
        {
            entity = entity.Summarize(),
            properties = IfcShapes.Page(result, skip, take),
        };
    }

    private static object PropertySets(IfcSession session, int id)
    {
        var entity = IfcEntityTools.Resolve(session, id);
        var sets = session.Properties
            .GetPropSets(id)
            .Select(set => new
            {
                id = set.Id,
                name = set.Name,
                isQuantitySet = IsQuantitySet(session, set),
                memberCount = set.Ids.Length,
            })
            .ToList();

        return new
        {
            entity = entity.Summarize(),
            propertySets = sets,
        };
    }

    private static bool IsQuantitySet(IfcSession session, IfcPropSet set)
        => session.Resolver.GetEntityOrDefault(set.Id)?.GetEntityName()
            .Equals("IFCELEMENTQUANTITY", StringComparison.OrdinalIgnoreCase) ?? false;
}
