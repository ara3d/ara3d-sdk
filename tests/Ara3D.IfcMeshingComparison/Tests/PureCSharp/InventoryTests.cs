using System.Text;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class InventoryTests
{
    static readonly HashSet<string> GeometryEntityNames =
    [
        "IFCEXTRUDEDAREASOLID", "IFCREVOLVEDAREASOLID", "IFCSWEPTDISKSOLID",
        "IFCSURFACECURVESWEPTAREASOLID", "IFCFIXEDREFERENCESWEPTAREASOLID",
        "IFCSURFACEOFLINEAREXTRUSION",
        "IFCTRIANGULATEDFACESET", "IFCPOLYGONALFACESET", "IFCFACETEDBREP",
        "IFCFACEBASEDSURFACEMODEL", "IFCSHELLBASEDSURFACEMODEL",
        "IFCBOOLEANRESULT", "IFCBOOLEANCLIPPINGRESULT", "IFCHALFSPACESOLID",
        "IFCMAPPEDITEM", "IFCSHAPEREPRESENTATION", "IFCPRODUCTDEFINITIONSHAPE",
        "IFCRECTANGLEPROFILEDEF", "IFCCIRCLEPROFILEDEF", "IFCARBITRARYCLOSEDPROFILEDEF",
        "IFCARBITRARYPROFILEDEFWITHVOIDS", "IFCCOMPOSITEPROFILEDEF", "IFCDERIVEDPROFILEDEF",
        "IFCPOLYLINE", "IFCCOMPOSITECURVE", "IFCTRIMMEDCURVE", "IFCBSPLINECURVE",
    ];

    [Test]
    public void Inventory_GeneratesMarkdownForCatalog()
    {
        var rows = ScanAllFiles();
        var output = PathUtil.GetCallerSourceFolder().RelativeFile("GeometryEntityInventory.md");
        WriteInventory(output, rows);
        Assert.That(output.Exists(), Is.True);
        Assert.That(rows, Is.Not.Empty);
    }

    [Test]
    [Explicit("Regenerates inventory across all external IFC corpora")]
    public void Inventory_RegenerateExplicit()
        => Inventory_GeneratesMarkdownForCatalog();

    public static List<InventoryRow> ScanAllFiles()
    {
        var counts = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);
        foreach (var file in TestFiles.AllKnownFiles())
        {
            using var step = new IfcFile(file, includeGeometry: false);
            foreach (var entity in step.EntityResolver.GetEntities())
            {
                var name = entity.GetEntityName();
                if (!IsGeometryEntity(name))
                    continue;
                if (!counts.TryGetValue(name, out var byFile))
                {
                    byFile = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    counts[name] = byFile;
                }
                var fileName = file.GetFileName();
                byFile[fileName] = byFile.GetValueOrDefault(fileName) + 1;
            }
        }
        return counts
            .OrderByDescending(kv => kv.Value.Values.Sum())
            .ThenBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => new InventoryRow(
                kv.Key,
                kv.Value.Values.Sum(),
                string.Join(", ", kv.Value.OrderByDescending(f => f.Value).Select(f => $"{f.Key}({f.Value})")),
                ClassifySupport(kv.Key)))
            .ToList();
    }

    static bool IsGeometryEntity(string name)
        => GeometryEntityNames.Contains(name) ||
           name.Contains("SOLID", StringComparison.Ordinal) ||
           name.Contains("BREP", StringComparison.Ordinal) ||
           name.Contains("PROFILEDEF", StringComparison.Ordinal) ||
           name.Contains("BOOLEAN", StringComparison.Ordinal) ||
           name.Contains("FACESET", StringComparison.Ordinal) ||
           name.Contains("SHELL", StringComparison.Ordinal) ||
           name.Contains("MAPPEDITEM", StringComparison.Ordinal);

    static GeometrySupportStatus ClassifySupport(string entityName)
        => entityName switch
        {
            "IFCEXTRUDEDAREASOLID" or "IFCRECTANGLEPROFILEDEF" or "IFCCIRCLEPROFILEDEF"
                or "IFCARBITRARYCLOSEDPROFILEDEF" or "IFCPOLYLINE" or "IFCLOCALPLACEMENT"
                or "IFCAXIS2PLACEMENT2D" or "IFCAXIS2PLACEMENT3D" or "IFCTRIANGULATEDFACESET"
                or "IFCPOLYGONALFACESET" or "IFCFACETEDBREP" or "IFCFACEBASEDSURFACEMODEL"
                or "IFCSHELLBASEDSURFACEMODEL" or "IFCBOOLEANCLIPPINGRESULT" or "IFCMAPPEDITEM"
                or "IFCSHAPEREPRESENTATION" or "IFCPRODUCTDEFINITIONSHAPE"
                or "IFCCONNECTEDFACESET" or "IFCCLOSEDSHELL" or "IFCOPENSHELL"
                or "IFCRECTANGLEHOLLOWPROFILEDEF" or "IFCCIRCLEHOLLOWPROFILEDEF"
                or "IFCLSHAPEPROFILEDEF" or "IFCTSHAPEPROFILEDEF" or "IFCUSHAPEPROFILEDEF"
                or "IFCATTDRIVENEXTRUDEDSOLID" or "IFCATTDRIVENEXTRUDEDSEGMENT"
                or "IFCADVANCEDBREP" or "IFCSWEPTDISKSOLIDPOLYGONAL"
                => GeometrySupportStatus.Supported,
            "IFCREVOLVEDAREASOLID" or "IFCSWEPTDISKSOLID" or "IFCSURFACECURVESWEPTAREASOLID"
                or "IFCFIXEDREFERENCESWEPTAREASOLID" or "IFCCOMPOSITECURVE" or "IFCTRIMMEDCURVE"
                or "IFCBSPLINECURVE" or "IFCCOMPOSITEPROFILEDEF" or "IFCDERIVEDPROFILEDEF"
                or "IFCARBITRARYPROFILEDEFWITHVOIDS" or "IFCISHAPEPROFILEDEF"
                or "IFCARBITRARYOPENPROFILEDEF" or "IFCARBITRARYPROFILEDEF"
                or "IFCATTDRIVENCLIPPEDEXTRUDEDSOLID" or "IFCHALFSPACESOLID"
                or "IFCSURFACEOFLINEAREXTRUSION"
                => GeometrySupportStatus.Approximate,
            _ => GeometrySupportStatus.Unsupported,
        };

    static void WriteInventory(FilePath path, IReadOnlyList<InventoryRow> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Geometry Entity Inventory");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        sb.AppendLine("| Entity | Count | Files | Status |");
        sb.AppendLine("|---|---:|---|---|");
        foreach (var row in rows)
            sb.AppendLine($"| {row.EntityName} | {row.Count} | {row.Files} | {row.Status} |");
        System.IO.File.WriteAllText(path, sb.ToString());
    }

    public sealed record InventoryRow(string EntityName, int Count, string Files, GeometrySupportStatus Status);
}
