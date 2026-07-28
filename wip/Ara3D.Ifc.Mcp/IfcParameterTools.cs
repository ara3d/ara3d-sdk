using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>Cross-model parameter questions. The existing property tools answer "what does this
/// element carry" and need an id in hand; these answer "what does the model carry" and "which
/// elements carry what", which is the direction discovery actually runs in.</summary>
public static class IfcParameterTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_parameters",
                "Lists every parameter in the model — properties and quantities alike — with the "
                + "number of elements carrying it, how many distinct values it takes, its numeric "
                + "range, and sample values. Start here: it tells you what is worth asking about "
                + "without opening a single element. Filter with 'name', 'propertySet' or 'type'.",
                IfcToolArgs.Model()
                    .String("name", "Optional substring the parameter name must contain, e.g. 'fire'.")
                    .String("propertySet", "Optional substring the property set name must contain, e.g. 'Common'.")
                    .String("type", "Optional IFC type to count against, e.g. IFCWALLSTANDARDCASE.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Parameters(args, cache),
                    ["ifc_parameter_values", "ifc_find_by_parameter"]))
            .Tool(
                "ifc_parameter_values",
                "Returns the distinct values of one parameter with how many elements hold each, "
                + "most common first — the distribution of a parameter across the model in one call. "
                + "Name it bare ('FireRating') or qualified ('Pset_WallCommon.LoadBearing').",
                IfcToolArgs.Model()
                    .String("name", "Parameter name, optionally qualified with its set.", required: true)
                    .String("propertySet", "Optional exact property set name, if 'name' is bare.")
                    .String("type", "Optional IFC type to restrict to.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Values(args, cache),
                    ["ifc_find_by_parameter"]))
            .Tool(
                "ifc_find_by_parameter",
                "Finds the elements whose parameter satisfies a test, returned in id order with the "
                + "matched value. Operators: exists, eq, ne, contains, gt, ge, lt, le. Values compare "
                + "as numbers when both sides read as numbers, otherwise as case-insensitive text; "
                + "the ordering operators require a numeric value.",
                IfcToolArgs.Model()
                    .String("name", "Parameter name, optionally qualified with its set.", required: true)
                    .String("op", "Test to apply. Default 'eq' when a value is given, 'exists' when not.")
                    .String("value", "Value to compare against.")
                    .String("propertySet", "Optional exact property set name, if 'name' is bare.")
                    .String("type", "Optional IFC type to restrict to.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Find(args, cache),
                    ["ifc_parameter_table", "ifc_entity"]))
            .Tool(
                "ifc_parameter_table",
                "Returns a row per element and a column per named parameter — the way to read the "
                + "same parameters across many elements in one call instead of ifc_properties per "
                + "element. Give 'names' as a comma-separated list. Restrict the rows with 'type' or "
                + "'ids'; with 'type' every element of that type gets a row, missing values as null.",
                IfcToolArgs.Model()
                    .String("names", "Comma-separated parameter names, e.g. 'Height,Width,LoadBearing'.", required: true)
                    .String("propertySet", "Optional exact property set name applied to every bare name.")
                    .String("type", "Optional IFC type whose elements become the rows.")
                    .Ids()
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Table(args, cache),
                    ["ifc_parameters"]));

    private static object Parameters(McpToolArgs args, IfcSessionCache cache)
    {
        var index = args.Session(cache).Parameters;
        var found = IfcParameterQueries.Catalogue(
            index,
            args.GetString("name"),
            args.GetString("propertySet"),
            args.GetString("type"));

        return new
        {
            parameterCount = index.ParameterCount,
            elementsWithParameters = index.ElementCount,
            parameters = IfcShapes.Page(found, args.Skip(), args.Take()),
        };
    }

    private static object Values(McpToolArgs args, IfcSessionCache cache)
    {
        var session = args.Session(cache);
        var name = args.GetRequiredString("name");
        var keys = Resolve(session.Parameters, name, args.GetString("propertySet"));
        var tally = IfcParameterQueries.Tally(session.Parameters, keys, args.GetString("type"));

        return new
        {
            name,
            propertySets = keys.Select(key => key.PropertySet).ToList(),
            values = IfcShapes.Page(tally, args.Skip(), args.Take()),
        };
    }

    private static object Find(McpToolArgs args, IfcSessionCache cache)
    {
        var session = args.Session(cache);
        var value = args.GetString("value");
        var op = IfcParameterQueries.ParseOp(args.GetString("op"), value != null);
        var keys = Resolve(session.Parameters, args.GetRequiredString("name"), args.GetString("propertySet"));
        var matches = IfcParameterQueries.Find(
            session.Parameters,
            session.Resolver,
            keys,
            op,
            value,
            args.GetString("type"));

        return new
        {
            op = op.ToString().ToLowerInvariant(),
            value,
            matches = IfcShapes.Page(matches, args.Skip(), args.Take()),
        };
    }

    private static object Table(McpToolArgs args, IfcSessionCache cache)
    {
        var session = args.Session(cache);
        var (columns, rows) = IfcParameterTable.Build(
            session.Parameters,
            session.Resolver,
            Names(args.GetRequiredString("names")),
            args.GetString("propertySet"),
            args.GetString("type"),
            args.GetIds());

        return new
        {
            columns,
            rows = IfcShapes.Page(rows, args.Skip(), args.Take()),
        };
    }

    private static IReadOnlyList<IfcParamKey> Resolve(IfcParameterIndex index, string name, string? propertySet)
    {
        var keys = index.Resolve(name, propertySet);
        return keys.Count > 0
            ? keys
            : throw new KeyNotFoundException(
                $"No parameter named '{name}' in this model. Call ifc_parameters to see what exists.");
    }

    private static IReadOnlyList<string> Names(string text)
    {
        var names = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return names.Length > 0
            ? names
            : throw new ArgumentException("'names' must list at least one parameter name.");
    }
}
