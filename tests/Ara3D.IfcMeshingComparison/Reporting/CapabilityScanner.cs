using System.Text;
using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Reporting;

public static class CapabilityScanner
{
    static readonly HashSet<string> GeometryEntityNames =
    [
        "IFCEXTRUDEDAREASOLID", "IFCREVOLVEDAREASOLID", "IFCSWEPTDISKSOLID", "IFCSWEPTDISKSOLIDPOLYGONAL",
        "IFCSURFACECURVESWEPTAREASOLID", "IFCFIXEDREFERENCESWEPTAREASOLID",
        "IFCSURFACEOFLINEAREXTRUSION",
        "IFCTRIANGULATEDFACESET", "IFCPOLYGONALFACESET", "IFCFACETEDBREP", "IFCADVANCEDBREP",
        "IFCFACEBASEDSURFACEMODEL", "IFCSHELLBASEDSURFACEMODEL",
        "IFCBOOLEANRESULT", "IFCBOOLEANCLIPPINGRESULT", "IFCHALFSPACESOLID",
        "IFCMAPPEDITEM", "IFCSHAPEREPRESENTATION", "IFCPRODUCTDEFINITIONSHAPE",
        "IFCRECTANGLEPROFILEDEF", "IFCCIRCLEPROFILEDEF", "IFCARBITRARYCLOSEDPROFILEDEF",
        "IFCARBITRARYPROFILEDEFWITHVOIDS", "IFCCOMPOSITEPROFILEDEF", "IFCDERIVEDPROFILEDEF",
        "IFCPOLYLINE", "IFCCOMPOSITECURVE", "IFCTRIMMEDCURVE", "IFCBSPLINECURVE",
    ];

    public static IReadOnlyList<CapabilityRow> Scan(
        IReadOnlyList<FilePath> ifcFiles,
        IReadOnlyList<IMeshingBackend> backends,
        IReadOnlyList<MeshingResult> results)
    {
        var catalogCounts = ScanCatalogEntityCounts(ifcFiles);
        var backendStatuses = backends.ToDictionary(
            b => b.Name,
            b => CollectBackendStatuses(b.Name, results));

        return catalogCounts
            .OrderByDescending(kv => kv.Value)
            .ThenBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv =>
            {
                var perBackend = backends.ToDictionary(
                    b => b.Name,
                    b => backendStatuses[b.Name].GetValueOrDefault(kv.Key, GeometrySupportStatus.Unsupported));
                return new CapabilityRow(kv.Key, kv.Value, perBackend);
            })
            .ToList();
    }

    static Dictionary<string, int> ScanCatalogEntityCounts(IReadOnlyList<FilePath> ifcFiles)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var file in ifcFiles)
        {
            if (!file.Exists())
                continue;
            using var step = new IfcFile(file, includeGeometry: false);
            foreach (var entity in step.EntityResolver.GetEntities())
            {
                var name = entity.GetEntityName();
                if (!IsGeometryEntity(name))
                    continue;
                counts[name] = counts.GetValueOrDefault(name) + 1;
            }
        }
        return counts;
    }

    static Dictionary<string, GeometrySupportStatus> CollectBackendStatuses(
        string backendName,
        IReadOnlyList<MeshingResult> results)
    {
        var statuses = new Dictionary<string, GeometrySupportStatus>(StringComparer.Ordinal);
        foreach (var result in results.Where(r => r.BackendName == backendName))
        {
            if (result.Diagnostics is null)
                continue;
            foreach (var (entity, status) in result.Diagnostics.EntityStatus)
            {
                if (!statuses.TryGetValue(entity, out var existing) || status < existing)
                    statuses[entity] = status;
            }
        }
        return statuses;
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
}
