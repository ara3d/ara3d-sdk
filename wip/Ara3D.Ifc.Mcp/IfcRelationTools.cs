using Ara3D.IfcLoader;
using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>Tools over the parsed relationship graph. Every edge runs child to parent: a wall is
/// <c>ContainedIn</c> a storey, a storey is <c>MemberOf</c> a building.</summary>
public static class IfcRelationTools
{
    /// <summary>The spatial containers, in the order IFC nests them. Read from the relation graph
    /// rather than from BOS output, which filters these entities out.</summary>
    private static readonly string[] SpatialTypes =
    [
        "IFCPROJECT", "IFCSITE", "IFCBUILDING", "IFCBUILDINGSTOREY", "IFCSPACE", "IFCZONE",
        "IFCSPATIALZONE", "IFCGRID",
    ];

    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_relations",
                "Returns the relationship edges touching an entity. Edges run child to parent. "
                + "Set 'direction' to 'from' for edges where the entity is the child, 'to' for "
                + "edges where it is the parent, or leave it unset for both.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the entity.", required: true)
                    .String("direction", "'from', 'to', or unset for both.")
                    .String("kind", "Optional relation kind filter, e.g. ContainedIn, MemberOf, Voids.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Relations(
                        args.Session(cache),
                        args.GetRequiredInt("id"),
                        args.GetString("direction"),
                        args.GetString("kind"),
                        args.Skip(),
                        args.Take()),
                    ["ifc_entity", "ifc_element_containment"]))
            .Tool(
                "ifc_spatial_tree",
                "Returns the spatial hierarchy — project, site, building, storey, space — with the "
                + "number of elements contained directly in each node.",
                IfcToolArgs.Model().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => SpatialTree(args.Session(cache)),
                    ["ifc_spatial_contents", "ifc_element_containment"]))
            .Tool(
                "ifc_spatial_contents",
                "Lists the elements contained directly in one spatial container, such as a storey.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the spatial container.", required: true)
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => SpatialContents(args.Session(cache), args.GetRequiredInt("id"), args.Skip(), args.Take()),
                    ["ifc_entity", "ifc_properties"]))
            .Tool(
                "ifc_element_containment",
                "Returns the chain of spatial containers above an element, nearest first — the "
                + "answer to 'which storey is this door on?'.",
                IfcToolArgs.Model()
                    .Integer("id", "STEP id of the element.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Containment(args.Session(cache), args.GetRequiredInt("id"))));

    private static IfcPage<IfcRelationEdge> Relations(
        IfcSession session,
        int id,
        string? direction,
        string? kind,
        int skip,
        int take)
    {
        var wantFrom = direction == null || direction.Equals("from", StringComparison.OrdinalIgnoreCase);
        var wantTo = direction == null || direction.Equals("to", StringComparison.OrdinalIgnoreCase);
        if (!wantFrom && !wantTo)
            throw new ArgumentException($"direction must be 'from', 'to', or unset; got '{direction}'.");

        var edges = new List<IfcRelationEdge>();
        foreach (var relation in session.Relations.Relations)
        {
            var matches = (wantFrom && relation.From == id) || (wantTo && relation.To == id);
            if (!matches)
                continue;
            if (kind != null && !relation.Kind.ToString().Equals(kind, StringComparison.OrdinalIgnoreCase))
                continue;

            edges.Add(Edge(session, relation));
        }

        return IfcShapes.Page(edges, skip, take);
    }

    private static IfcRelationEdge Edge(IfcSession session, IfcRelation relation)
        => new(
            relation.From,
            TypeOf(session, relation.From),
            relation.To,
            TypeOf(session, relation.To),
            relation.Kind.ToString());

    private static string TypeOf(IfcSession session, int id)
        => session.Resolver.GetEntityOrDefault(id)?.GetEntityName() ?? "";

    private static object SpatialTree(IfcSession session)
    {
        var childrenOf = ChildrenByParent(session);
        var roots = session.Resolver
            .GetEntities()
            .Where(entity => entity.GetEntityName().Equals("IFCPROJECT", StringComparison.OrdinalIgnoreCase))
            .OrderBy(entity => entity.Id)
            .Select(entity => Node(session, entity.Id, childrenOf, []))
            .ToList();

        return new
        {
            path = session.Path.FullPath,
            roots,
        };
    }

    /// <summary>Builds the parent-to-children map once per call. <paramref name="visited"/> guards
    /// against a malformed file whose containment edges form a cycle.</summary>
    private static IfcSpatialNode Node(
        IfcSession session,
        int id,
        IReadOnlyDictionary<int, List<int>> childrenOf,
        HashSet<int> visited)
    {
        var entity = session.Resolver.GetEntityOrDefault(id);
        var type = entity?.GetEntityName() ?? "";
        var name = entity?.GetEntityLabel() ?? $"#{id}";

        if (!visited.Add(id) || !childrenOf.TryGetValue(id, out var children))
            return new IfcSpatialNode(id, type, name, 0, []);

        var spatialChildren = new List<IfcSpatialNode>();
        var elementCount = 0;
        foreach (var childId in children)
        {
            if (IsSpatial(session, childId))
                spatialChildren.Add(Node(session, childId, childrenOf, visited));
            else
                elementCount++;
        }

        spatialChildren.Sort((a, b) => a.Id.CompareTo(b.Id));
        return new IfcSpatialNode(id, type, name, elementCount, spatialChildren);
    }

    private static Dictionary<int, List<int>> ChildrenByParent(IfcSession session)
    {
        var result = new Dictionary<int, List<int>>();
        foreach (var relation in session.Relations.Relations)
        {
            if (relation.Kind is not (IfcRelationKind.ContainedIn or IfcRelationKind.MemberOf))
                continue;
            if (!result.TryGetValue(relation.To, out var children))
                result[relation.To] = children = [];
            children.Add(relation.From);
        }

        return result;
    }

    private static IfcPage<IfcEntitySummary> SpatialContents(IfcSession session, int id, int skip, int take)
    {
        var contents = new List<IfcEntitySummary>();
        foreach (var relation in session.Relations.Relations)
        {
            if (relation.To != id)
                continue;
            if (relation.Kind is not (IfcRelationKind.ContainedIn or IfcRelationKind.MemberOf))
                continue;

            var entity = session.Resolver.GetEntityOrDefault(relation.From);
            if (entity != null)
                contents.Add(entity.Summarize());
        }

        contents.Sort((a, b) => a.Id.CompareTo(b.Id));
        return IfcShapes.Page(contents, skip, take);
    }

    private static object Containment(IfcSession session, int id)
    {
        var element = IfcEntityTools.Resolve(session, id);
        var parentOf = new Dictionary<int, int>();
        foreach (var relation in session.Relations.Relations)
            if (relation.Kind is IfcRelationKind.ContainedIn or IfcRelationKind.MemberOf)
                parentOf.TryAdd(relation.From, relation.To);

        var chain = new List<IfcEntitySummary>();
        var seen = new HashSet<int> { id };
        var current = id;
        while (parentOf.TryGetValue(current, out var parent) && seen.Add(parent))
        {
            var entity = session.Resolver.GetEntityOrDefault(parent);
            if (entity == null)
                break;
            chain.Add(entity.Summarize());
            current = parent;
        }

        return new
        {
            element = element.Summarize(),
            containers = chain,
        };
    }

    private static bool IsSpatial(IfcSession session, int id)
    {
        var type = TypeOf(session, id);
        foreach (var spatial in SpatialTypes)
            if (type.Equals(spatial, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
