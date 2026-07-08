using Ara3D.IfcLoader;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Reporting;

public sealed record ComparisonReport(
    DateTime GeneratedUtc,
    IReadOnlyList<IMeshingBackend> Backends,
    IReadOnlyList<FilePath> IfcFiles,
    IReadOnlyList<MeshingResult> Results,
    IReadOnlyList<CapabilityRow> Capabilities)
{
    public static ComparisonReport Run(
        IEnumerable<IMeshingBackend> backends,
        IEnumerable<FilePath> ifcFiles,
        Action<string>? log = null)
    {
        var backendList = backends.ToList();
        var fileList = ifcFiles.ToList();
        var results = new List<MeshingResult>();

        foreach (var ifcPath in fileList)
        {
            if (!ifcPath.Exists())
            {
                log?.Invoke($"Skipping missing IFC: {ifcPath}");
                continue;
            }

            foreach (var backend in backendList)
            {
                log?.Invoke($"[{backend.Name}] {ifcPath.GetFileName()}...");
                results.Add(backend.Build(ifcPath));
            }
        }

        var capabilities = CapabilityScanner.Scan(fileList, backendList, results);
        return new ComparisonReport(DateTime.UtcNow, backendList, fileList, results, capabilities);
    }
}

public sealed record CapabilityRow(
    string EntityName,
    int CatalogCount,
    IReadOnlyDictionary<string, GeometrySupportStatus> BackendStatus);
