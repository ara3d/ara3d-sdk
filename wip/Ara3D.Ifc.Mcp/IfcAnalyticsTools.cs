using Ara3D.MCP;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>Whole-model analytics: convert a model to BIM Open Schema, then ask SQL questions of
/// the result. Every tool here builds the conversion on demand, so <c>ifc_to_bos</c> is a way to
/// pay that cost deliberately rather than a prerequisite.
/// A conversion that fails — for example on an IFC relation kind
/// <c>IfcRelationMapping.ToBos</c> does not map — comes back as a tool error, not a server crash,
/// because every handler runs through <see cref="ToolRunner"/>.
/// Spatial containers do survive the conversion — <c>IfcToBosConverter.HiddenIfcNames</c> flags
/// geometry instances as hidden, it does not filter entities — but the converted relations are a
/// flattened edge list, so <c>ifc_spatial_tree</c> is still the way to read the hierarchy.</summary>
public static class IfcAnalyticsTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_to_bos",
                "Converts a model to BIM Open Schema (a zip of Parquet tables) and builds the DuckDB "
                + "database that ifc_sql and ifc_table query. Set 'outputPath' to also save the .bos "
                + "file somewhere permanent. Set 'refresh' to true to redo a conversion after the "
                + "IFC file has changed on disk. Expensive: it re-reads the whole file.",
                IfcToolArgs.Model()
                    .String("outputPath", "Optional path to save a copy of the .bos file to.")
                    .Boolean("refresh", "Rebuild even if this model has already been converted. Default false.")
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => ToBos(args.Session(cache), args.GetString("outputPath"), args.GetBool("refresh") ?? false),
                    ["ifc_table", "ifc_sql"]))
            .Tool(
                "ifc_table",
                "Lists the tables and views available to ifc_sql with their row counts and column "
                + "types, so a query can be written without guessing. Set 'table' to describe just "
                + "one. Start with the EntityText, ParameterText and RelationText views: the raw "
                + "tables store interned integer indexes, not names.",
                IfcToolArgs.Model()
                    .String("table", "Optional single table to describe, e.g. Entities.")
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Tables(args.Session(cache), args.GetString("table"), args.Skip(), args.Take()),
                    ["ifc_sql"]))
            .Tool(
                "ifc_sql",
                "Runs a read-only SQL query (DuckDB dialect) over a model's BIM Open Schema tables and "
                + "returns a page of rows plus the unpaged row count. One SELECT or WITH statement only. "
                + "Query the EntityText, ParameterText and RelationText views unless you need the raw "
                + "interned tables. Call ifc_table first to see what exists.",
                IfcToolArgs.Model()
                    .String("sql", "A single SELECT or WITH statement.", required: true)
                    .Paged()
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => IfcDuck.Query(
                        args.Session(cache).Bos.DatabasePath,
                        args.GetRequiredString("sql"),
                        args.Skip(),
                        args.Take())))
            .Tool(
                "ifc_sql_export",
                "Runs a read-only SQL query and writes every row to a file. The format follows the "
                + "output extension: .parquet, .json, or CSV with a header row for anything else. "
                + "Use this instead of paging through ifc_sql for a large result.",
                IfcToolArgs.Model()
                    .String("sql", "A single SELECT or WITH statement.", required: true)
                    .String("outputPath", "Path of the file to write.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Export(
                        args.Session(cache),
                        args.GetRequiredString("sql"),
                        args.GetRequiredString("outputPath"))));

    private static object ToBos(IfcSession session, string? outputPath, bool refresh)
    {
        var bos = refresh || !session.BosIsBuilt ? session.RebuildBos() : session.Bos;
        var saved = outputPath == null ? null : CopyTo(bos.BosPath, new FilePath(outputPath)).FullPath;
        return new
        {
            path = session.Path.FullPath,
            bosPath = bos.BosPath.FullPath,
            databasePath = bos.DatabasePath.FullPath,
            bosBytes = bos.BosBytes,
            builtUtc = bos.BuiltUtc,
            savedTo = saved,
        };
    }

    private static IfcPage<IfcTableInfo> Tables(IfcSession session, string? table, int skip, int take)
        => IfcShapes.Page(IfcDuck.Tables(session.Bos.DatabasePath, table), skip, take);

    private static object Export(IfcSession session, string sql, string outputPath)
    {
        var output = new FilePath(outputPath);
        var rows = IfcDuck.Export(session.Bos.DatabasePath, sql, output);
        return new
        {
            path = output.FullPath,
            rows,
            bytes = new FileInfo(output.FullPath).Length,
        };
    }

    private static FilePath CopyTo(FilePath source, FilePath destination)
    {
        var directory = Path.GetDirectoryName(destination.FullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
        File.Copy(source.FullPath, destination.FullPath, overwrite: true);
        return destination;
    }
}
