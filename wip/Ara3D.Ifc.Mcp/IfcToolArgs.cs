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
}
