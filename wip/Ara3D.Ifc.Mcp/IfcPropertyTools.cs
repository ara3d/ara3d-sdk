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
        var values = session.Properties.PropValues;
        var document = session.File.Document;
        var result = new List<IfcProperty>();

        foreach (var set in IfcPropertySets.ForElement(session, id))
            foreach (var memberId in set.MemberIds)
            {
                if (!values.TryGetValue(memberId, out var value))
                    continue;
                if ((value.Kind == IfcPropKind.Quantity) != quantities)
                    continue;
                var (measure, text) = IfcPropertyText.Read(value, document);
                result.Add(new IfcProperty(set.Name, value.Name, value.Kind.ToString(), measure, text));
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
        var sets = IfcPropertySets
            .ForElement(session, id)
            .Select(set => new
            {
                id = set.Id,
                name = set.Name,
                isQuantitySet = set.IsQuantitySet,
                memberCount = set.MemberIds.Count,
            })
            .ToList();

        return new
        {
            entity = entity.Summarize(),
            propertySets = sets,
        };
    }
}
