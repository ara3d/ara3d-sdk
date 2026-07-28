using Ara3D.Ifc.Mesher;
using Ara3D.IO.GltfExporter;
using Ara3D.MCP;
using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>Geometry tools: mesh statistics, bounds, volume, GLB export, and a look at what failed to
/// mesh. Meshing is done by the Approach1 mesher, a pure-C# reader of STEP geometry definitions. It
/// needs no native tessellator and no geometry-enabled reopen, so these tools run against the same
/// <see cref="IfcSession"/> the data tools opened with <c>includeGeometry: false</c>; the meshed model
/// is built once per session and reused. A model whose mesher failed is reported through
/// <c>ifc_meshing_diagnostics</c> rather than crashing the other tools.</summary>
public static class IfcGeometryTools
{
    public static McpServer Register(this McpServer mcp, IfcSessionCache cache)
        => mcp
            .Tool(
                "ifc_mesh",
                "Reports the triangle geometry of a model per element: instance, triangle and vertex "
                + "counts, signed volume, surface area and bounds. Set 'ids' to a comma-separated list "
                + "to restrict to specific elements. This returns measurements, not raw vertices — use "
                + "ifc_export_glb to get the geometry itself.",
                IfcToolArgs.Model().Ids().Paged().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Mesh(args.Session(cache), args.GetIds(), args.Skip(), args.Take()),
                    ["ifc_export_glb", "ifc_bounds", "ifc_volume"]))
            .Tool(
                "ifc_bounds",
                "Returns the axis-aligned bounding box of the whole model and, unless 'ids' narrows it, "
                + "of each element. Every box is given as min, max, center and size.",
                IfcToolArgs.Model().Ids().Paged().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Bounds(args.Session(cache), args.GetIds(), args.Skip(), args.Take())))
            .Tool(
                "ifc_volume",
                "Returns signed volume and surface area computed from the meshed geometry: model totals "
                + "plus a per-element breakdown. Signed volume is negative when a solid's faces wind "
                + "inward, so its magnitude is the physical volume. Set 'ids' to restrict the breakdown.",
                IfcToolArgs.Model().Ids().Paged().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Volume(args.Session(cache), args.GetIds(), args.Skip(), args.Take())))
            .Tool(
                "ifc_export_glb",
                "Meshes a model and writes it to a binary glTF (.glb) file, returning the path and byte "
                + "size. Set 'ids' to export only specific elements. The whole model is meshed either "
                + "way; 'ids' only filters which instances are written.",
                IfcToolArgs.Model()
                    .Ids()
                    .String("outputPath", "Path of the .glb file to write.", required: true)
                    .Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Export(args.Session(cache), args.GetIds(), args.GetRequiredString("outputPath"))))
            .Tool(
                "ifc_meshing_diagnostics",
                "Reports whether the mesher succeeded and every message and error it produced — the way "
                + "to see what geometry could not be built and why. Messages are paged; errors are "
                + "returned whole.",
                IfcToolArgs.Model().Paged().Build(),
                (args, _) => ToolRunner.RunAsync(
                    () => Diagnostics(args.Session(cache), args.Skip(), args.Take())));

    private static object Mesh(IfcSession session, IReadOnlyList<int>? ids, int skip, int take)
        => new
        {
            path = session.Path.FullPath,
            model = ModelSummary(session.Meshing),
            elements = IfcShapes.Page(session.Elements(ids), skip, take),
        };

    private static object Bounds(IfcSession session, IReadOnlyList<int>? ids, int skip, int take)
    {
        var elements = session.Elements(ids);
        var boxes = new IfcElementBox[elements.Count];
        for (var i = 0; i < elements.Count; i++)
            boxes[i] = new IfcElementBox(elements[i].Id, elements[i].Type, elements[i].Name, elements[i].Bounds);

        return new
        {
            path = session.Path.FullPath,
            model = IfcBox.From(session.Meshing.Bounds),
            elements = IfcShapes.Page<IfcElementBox>(boxes, skip, take),
        };
    }

    private static object Volume(IfcSession session, IReadOnlyList<int>? ids, int skip, int take)
    {
        var elements = session.Elements(ids);
        var quantities = new IfcElementQuantity[elements.Count];
        for (var i = 0; i < elements.Count; i++)
            quantities[i] = new IfcElementQuantity(
                elements[i].Id, elements[i].Type, elements[i].Name, elements[i].SignedVolume, elements[i].SurfaceArea);

        return new
        {
            path = session.Path.FullPath,
            model = new { signedVolume = session.Meshing.SignedVolume, triangleCount = session.Meshing.TriangleCount },
            elements = IfcShapes.Page<IfcElementQuantity>(quantities, skip, take),
        };
    }

    private static object Export(IfcSession session, IReadOnlyList<int>? ids, string outputPath)
    {
        var model = session.Filtered(ids);
        var output = new FilePath(outputPath);
        var directory = Path.GetDirectoryName(output.FullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        model.WriteGlb(output);
        return new
        {
            path = session.Path.FullPath,
            outputPath = output.FullPath,
            bytes = new FileInfo(output.FullPath).Length,
            meshCount = model.Meshes.Count,
            instanceCount = model.Instances.Count,
        };
    }

    private static object Diagnostics(IfcSession session, int skip, int take)
    {
        var result = session.Meshing;
        return new
        {
            path = session.Path.FullPath,
            mesherName = result.MesherName,
            success = result.Success,
            meshCount = result.MeshCount,
            instanceCount = result.InstanceCount,
            triangleCount = result.TriangleCount,
            errorCount = result.Errors.Count,
            errors = result.Errors,
            messages = IfcShapes.Page(result.Messages, skip, take),
        };
    }

    private static object ModelSummary(IfcMeshingResult result)
        => new
        {
            success = result.Success,
            meshCount = result.MeshCount,
            instanceCount = result.InstanceCount,
            triangleCount = result.TriangleCount,
            signedVolume = result.SignedVolume,
            bounds = IfcBox.From(result.Bounds),
        };

    private readonly record struct IfcElementBox(int Id, string Type, string Name, IfcBox Bounds);

    private readonly record struct IfcElementQuantity(
        int Id, string Type, string Name, double SignedVolume, double SurfaceArea);
}
