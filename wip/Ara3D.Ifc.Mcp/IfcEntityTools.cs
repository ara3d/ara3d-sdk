using Ara3D.IfcLoader;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>Tools that read individual entities and their raw STEP attributes.</summary>
public static class IfcEntityTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_entity",
                "Returns one entity by its STEP id (the number after # in the file), with its type, "
                + "display name, GlobalId, and raw attributes.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the entity, e.g. 4213 for #4213.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Entity(args.Session(cache), args.GetRequiredInt("id")),
                    ["ifc_properties", "ifc_relations"]))
            .Tool(
                "ifc_entities_of_type",
                "Lists every entity of one IFC type, e.g. IFCWALLSTANDARDCASE. Type matching is "
                + "exact and case-insensitive; use ifc_type_counts to see the types present.",
                IfcToolArgs.Model()
                    .String("type", "IFC type name, e.g. IFCDOOR.", required: true)
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => EntitiesOfType(args.Session(cache), args.GetRequiredString("type"), args.Skip(), args.Take()),
                    ["ifc_entity", "ifc_properties"]))
            .Tool(
                "ifc_attributes",
                "Returns the raw STEP attributes of an entity, by position. Use this when a value "
                + "is not exposed as a named property.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the entity.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Attributes(args.Session(cache), args.GetRequiredInt("id"))));

    private static object Entity(IfcSession session, int id)
    {
        var entity = Resolve(session, id);
        return new
        {
            entity = entity.Summarize(),
            attributes = entity.Attributes(),
        };
    }

    private static IfcPage<IfcEntitySummary> EntitiesOfType(IfcSession session, string type, int skip, int take)
    {
        var matches = new List<IfcEntitySummary>();
        foreach (var entity in session.Resolver.GetEntities())
            if (entity.GetEntityName().Equals(type, StringComparison.OrdinalIgnoreCase))
                matches.Add(entity.Summarize());

        matches.Sort((a, b) => a.Id.CompareTo(b.Id));
        return IfcShapes.Page(matches, skip, take);
    }

    private static object Attributes(IfcSession session, int id)
    {
        var entity = Resolve(session, id);
        return new
        {
            id,
            type = entity.GetEntityName(),
            attributes = entity.Attributes(),
        };
    }

    internal static IfcEntity Resolve(IfcSession session, int id)
        => session.Resolver.GetEntityOrDefault(id)
           ?? throw new KeyNotFoundException($"No entity #{id} in {session.Path.FullPath}.");
}
