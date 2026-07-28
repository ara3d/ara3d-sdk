using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

/// <summary>The argument and schema fragments every IFC tool shares. Tools resolve their model
/// through <see cref="Session"/>, which opens the file if it is not already open, so no tool
/// depends on <c>ifc_open</c> having been called first.</summary>
internal static class IfcToolArgs
{
    public const int DefaultTake = 100;

    public static IfcSession Session(this McpToolArgs args, IfcSessionCache cache)
        => cache.Get(args.GetRequiredString("path"));

    public static int Skip(this McpToolArgs args)
        => args.GetInt("skip") ?? 0;

    public static int Take(this McpToolArgs args)
        => args.GetInt("take") ?? DefaultTake;

    public static McpSchemaBuilder Model()
        => McpSchema.Object().String("path", "Absolute path to the .ifc file.", required: true);

    public static McpSchemaBuilder Paged(this McpSchemaBuilder builder)
        => builder
            .Integer("skip", "Number of items to skip. Default 0.")
            .Integer("take", $"Maximum items to return. Default {DefaultTake}.");

    /// <summary>The optional entity-id filter shared by the geometry tools, taken as a comma-separated
    /// string because the schema builder has no integer-array primitive.</summary>
    public static McpSchemaBuilder Ids(this McpSchemaBuilder builder)
        => builder.String("ids", "Optional comma-separated entity ids to restrict to, e.g. '173,180'. Omit for the whole model.");

    public static IReadOnlyList<int>? GetIds(this McpToolArgs args)
    {
        var text = args.GetString("ids");
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var parts = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ids = new int[parts.Length];
        for (var i = 0; i < parts.Length; i++)
            if (!int.TryParse(parts[i], out ids[i]))
                throw new ArgumentException($"'ids' must be a comma-separated list of integers; '{parts[i]}' is not one.");

        return ids;
    }
}
