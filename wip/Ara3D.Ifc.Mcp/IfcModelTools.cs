using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>Tools that answer questions about a model as a whole.</summary>
public static class IfcModelTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_open",
                "Opens an IFC file and reports its schema and size. Optional — every other tool "
                + "opens the file on demand — but useful to pay the parse cost once up front.",
                IfcToolArgs.Model().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Describe(args.Session(cache)),
                    ["ifc_header", "ifc_type_counts", "ifc_spatial_tree"]))
            .Tool(
                "ifc_close",
                "Closes an open IFC file and frees its memory.",
                McpSchema.Object().String("path", "Absolute path to the .ifc file.", required: true).Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => new { closed = cache.Close(args.GetRequiredString("path")) }))
            .Tool(
                "ifc_models",
                "Lists the IFC files currently held open, least recently used first.",
                (_, _) => ToolRunner.RunAsync(
                    () => cache.OpenSessions().Select(Describe).ToList()))
            .Tool(
                "ifc_header",
                "Returns the STEP header of an IFC file: description, originating file name, and schema.",
                IfcToolArgs.Model().Build(),
                (args, _) => ToolRunner.RunAsync(() => Header(args.Session(cache))))
            .Tool(
                "ifc_type_counts",
                "Counts entities by IFC type, most common first. The fastest way to see what a model contains.",
                IfcToolArgs.Model().Paged().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => TypeCounts(args.Session(cache), args.Skip(), args.Take()),
                    ["ifc_entities_of_type"]))
            .Tool(
                "ifc_search",
                "Finds entities whose name, GlobalId, or type contains the given text (case-insensitive). "
                + "Set 'type' to restrict the search to one IFC type.",
                IfcToolArgs.Model()
                    .String("text", "Text to look for in the entity name, GlobalId, or type.", required: true)
                    .String("type", "Optional IFC type filter, e.g. IFCWALL.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Search(
                        args.Session(cache),
                        args.GetRequiredString("text"),
                        args.GetString("type"),
                        args.Skip(),
                        args.Take()),
                    ["ifc_entity", "ifc_properties"]));

    private static object Describe(IfcSession session)
        => new
        {
            path = session.Path.FullPath,
            schema = session.Schema,
            entityCount = session.Resolver.EntityLookup.Count,
            openedUtc = session.OpenedUtc,
        };

    private static object Header(IfcSession session)
    {
        var header = session.File.Document.Header;
        return new
        {
            path = session.Path.FullPath,
            description = header.FileDescription,
            fileName = header.FileName,
            schema = header.FileSchema,
        };
    }

    private static IfcPage<IfcTypeCount> TypeCounts(IfcSession session, int skip, int take)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entity in session.Resolver.GetEntities())
        {
            var name = entity.GetEntityName();
            counts[name] = counts.TryGetValue(name, out var n) ? n + 1 : 1;
        }

        var ordered = counts
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => new IfcTypeCount(pair.Key, pair.Value))
            .ToList();

        return IfcShapes.Page(ordered, skip, take);
    }

    private static IfcPage<IfcEntitySummary> Search(
        IfcSession session,
        string text,
        string? type,
        int skip,
        int take)
    {
        var matches = new List<IfcEntitySummary>();
        foreach (var entity in session.Resolver.GetEntities())
        {
            var entityType = entity.GetEntityName();
            if (type != null && !entityType.Equals(type, StringComparison.OrdinalIgnoreCase))
                continue;

            var summary = entity.Summarize();
            if (Contains(summary.Name, text) || Contains(summary.GlobalId, text) || Contains(entityType, text))
                matches.Add(summary);
        }

        matches.Sort((a, b) => a.Id.CompareTo(b.Id));
        return IfcShapes.Page(matches, skip, take);
    }

    private static bool Contains(string? value, string text)
        => value != null && value.Contains(text, StringComparison.OrdinalIgnoreCase);
}
