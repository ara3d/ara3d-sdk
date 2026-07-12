using System.Text.Json;
using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness.GeometryOracles;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

public sealed record FileCorrectnessMetrics(
    string FileName,
    int MeshCount,
    float MeshValidityRate,
    float WindingRate,
    float AnalyticalVolumeRate,
    float CutIntegrityRate,
    float CoverageRate,
    int OpenEdgeBudgetViolations);

public static class CorrectnessScorecard
{
    public const float WindingGate = 0.95f;
    public const float MeshValidityGate = 1.0f;
    public const int DefaultOpenEdgeBudget = 32;

    public static FileCorrectnessMetrics Evaluate(FilePath ifcPath)
    {
        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);
        return Evaluate(model, diagnostics, ifcPath.GetFileName());
    }

    public static FileCorrectnessMetrics Evaluate(Model3D model, MeshingDiagnostics diagnostics, string fileName)
    {
        var meshes = model.Meshes;
        if (meshes.Count == 0)
            return new FileCorrectnessMetrics(fileName, 0, 1f, 1f, 1f, 1f, CoverageRate(diagnostics), 0);

        var considered = 0;
        var valid = 0;
        var windingOk = 0;
        var windingEligible = 0;
        var analyticalOk = 0;
        var analyticalEligible = 0;
        var cutOk = 0;
        var cutEligible = 0;
        var openEdgeViolations = 0;

        for (var i = 0; i < meshes.Count; i++)
        {
            var mesh = meshes[i];
            if (mesh.FaceIndices.Count == 0)
                continue;
            considered++;

            if (MeshValidity.HasValidIndices(mesh) && MeshValidity.CountDegenerateTriangles(mesh) == 0)
                valid++;

            var openEdges = TopologyOracle.CountOpenEdges(mesh);
            var watertight = TopologyOracle.IsWatertight(mesh);
            if (watertight)
            {
                windingEligible++;
                // Accept either positive signed volume or high outward-normal fraction (mirror-mapped solids).
                if (WindingOracle.HasPositiveSignedVolume(mesh) || WindingOracle.HasOutwardWinding(mesh, 0.85f))
                    windingOk++;

                analyticalEligible++;
                if (AnalyticalOracle.AbsVolume(mesh) > 1e-8)
                    analyticalOk++;
            }
            else if (openEdges > DefaultOpenEdgeBudget)
            {
                openEdgeViolations++;
            }

            if (openEdges > 0 && openEdges <= DefaultOpenEdgeBudget && MeshValidity.HasValidIndices(mesh))
            {
                cutEligible++;
                if (ClipOracle.PostClipFacesOutward(mesh, minFraction: 0.75f))
                    cutOk++;
            }
        }

        if (considered == 0)
            return new FileCorrectnessMetrics(fileName, 0, 1f, 1f, 1f, 1f, CoverageRate(diagnostics), 0);

        return new FileCorrectnessMetrics(
            fileName,
            considered,
            valid / (float)considered,
            windingEligible == 0 ? 1f : windingOk / (float)windingEligible,
            analyticalEligible == 0 ? 1f : analyticalOk / (float)analyticalEligible,
            cutEligible == 0 ? 1f : cutOk / (float)cutEligible,
            CoverageRate(diagnostics),
            openEdgeViolations);
    }

    static float CoverageRate(MeshingDiagnostics diagnostics)
    {
        var supported = GeometryCreationBacklog.KnownItems
            .Where(i => i.Support is GeometryCreationSupport.Supported or GeometryCreationSupport.Partial)
            .Select(i => i.EntityName)
            .ToHashSet(StringComparer.Ordinal);
        var encountered = diagnostics.EntityCounts.Keys.Where(supported.Contains).ToList();
        if (encountered.Count == 0)
            return 1f;
        var built = encountered.Count(e =>
            diagnostics.EntityStatus.TryGetValue(e, out var s) && s != GeometrySupportStatus.Unsupported);
        return built / (float)encountered.Count;
    }

    public static void Write(IReadOnlyList<FileCorrectnessMetrics> results, FilePath path)
    {
        path.GetDirectory().Create();
        var json = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static string Format(FileCorrectnessMetrics m)
        => $"{m.FileName}: valid={m.MeshValidityRate:P1} winding={m.WindingRate:P1} " +
           $"analytical={m.AnalyticalVolumeRate:P1} cut={m.CutIntegrityRate:P1} " +
           $"coverage={m.CoverageRate:P1} openEdgeViolations={m.OpenEdgeBudgetViolations} meshes={m.MeshCount}";
}
