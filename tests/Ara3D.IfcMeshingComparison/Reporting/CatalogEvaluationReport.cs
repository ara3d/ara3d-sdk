using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Reporting;

public sealed record CatalogEvaluationDocument(
    DateTime GeneratedUtc,
    string CatalogName,
    IReadOnlyList<CatalogFileEvaluation> Files,
    IReadOnlyList<EntityWorkPriority> WorkPriorities);

public sealed record CatalogFileEvaluation(
    string FileName,
    double FileSizeMb,
    IReadOnlyDictionary<string, int> GeometryEntityCounts,
    IReadOnlyDictionary<string, int> ProductTypeCounts,
    ModelComparisonResult? Comparison,
    BuildStats CandidateStats,
    BuildStats OracleStats,
    FileGapAnalysis GapAnalysis,
    bool ComparisonFailed,
    string? ComparisonError);

public sealed record BuildStats(int Instances, int Meshes, int Triangles, long ElapsedMs);

public sealed record FileGapAnalysis(
    IReadOnlyDictionary<string, int> OracleOnlyByProductType,
    IReadOnlyDictionary<string, int> OracleOnlyByGeometryEntity,
    IReadOnlyDictionary<string, long> OracleOnlyTriLossByGeometryEntity,
    IReadOnlyList<OracleOnlyEntityRecord> TopOracleOnlyEntities,
    IReadOnlyDictionary<string, int> UnsupportedDiagnostics,
    IReadOnlyDictionary<string, int> ApproximateDiagnostics);

public sealed record OracleOnlyEntityRecord(int EntityId, string EntityName, int OracleTriangles);

public sealed record EntityWorkPriority(
    string EntityName,
    int TotalCatalogCount,
    int FilesAffected,
    int OracleOnlyProductCount,
    long OracleOnlyTriangleLoss,
    GeometryCreationSupport BacklogSupport,
    string Notes,
    int PriorityScore);

public static class CatalogEvaluationReport
{
    public static CatalogEvaluationDocument Run(
        IEnumerable<FilePath> ifcFiles,
        string catalogName,
        Action<string>? log = null,
        bool generateOracles = true)
    {
        var fileList = ifcFiles.Where(f => f.Exists()).OrderBy(f => f.GetFileName()).ToList();
        if (generateOracles)
            WebIfcBfastOracle.GenerateAll(fileList, log);

        var evaluations = new List<CatalogFileEvaluation>();
        foreach (var ifcPath in fileList)
        {
            log?.Invoke($"Evaluating {ifcPath.GetFileName()}...");
            evaluations.Add(EvaluateFile(ifcPath, log));
        }

        return new CatalogEvaluationDocument(
            DateTime.UtcNow,
            catalogName,
            evaluations,
            RankWorkPriorities(evaluations));
    }

    public static FilePath WriteJson(CatalogEvaluationDocument document, FilePath path)
    {
        TestFiles.ReportsDir.Create();
        path.GetParent().Create();
        File.WriteAllText(path, JsonSerializer.Serialize(document, JsonOptions));
        return path;
    }

    public static FilePath WriteMarkdown(CatalogEvaluationDocument document, FilePath path)
    {
        TestFiles.ReportsDir.Create();
        path.GetParent().Create();
        File.WriteAllText(path, FormatMarkdown(document));
        return path;
    }

    static CatalogFileEvaluation EvaluateFile(FilePath ifcPath, Action<string>? log)
    {
        var fileName = ifcPath.GetFileName();
        var fileSizeMb = new FileInfo(ifcPath).Length / (1024.0 * 1024.0);

        var geometryCounts = ScanGeometryEntities(ifcPath);
        var productCounts = ScanProductTypes(ifcPath);

        ModelComparisonResult? comparison = null;
        BuildStats candidateStats = default;
        BuildStats oracleStats = default;
        FileGapAnalysis gapAnalysis = EmptyGapAnalysis();
        var comparisonFailed = false;
        string? comparisonError = null;

        try
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
            var (candidate, diagnostics) = ModelAssembler.BuildModel(stepFile);
            sw.Stop();
            candidateStats = new BuildStats(
                candidate.Instances.Count,
                candidate.Meshes.Count,
                candidate.Meshes.Sum(m => m.FaceIndices.Count),
                sw.ElapsedMilliseconds);

            var oracle = ModelComparer.LoadOracle(ifcPath);
            oracleStats = new BuildStats(
                oracle.Instances.Count,
                oracle.Meshes.Count,
                oracle.Meshes.Sum(m => m.FaceIndices.Count),
                0);

            comparison = ModelComparer.Compare(candidate, oracle, fileName);
            try
            {
                gapAnalysis = AnalyzeGaps(stepFile, candidate, oracle, diagnostics);
            }
            catch (Exception gapEx)
            {
                log?.Invoke($"  {fileName}: gap analysis failed — {gapEx.Message}");
            }

            log?.Invoke(
                $"  {fileName}: parity={comparison.ParityScore:F3}, " +
                $"inst {candidateStats.Instances}/{oracleStats.Instances}, " +
                $"tris {candidateStats.Triangles}/{oracleStats.Triangles}");
        }
        catch (Exception ex)
        {
            comparisonFailed = true;
            comparisonError = ex.Message;
            log?.Invoke($"  {fileName}: FAILED — {ex.Message}");
        }

        return new CatalogFileEvaluation(
            fileName,
            fileSizeMb,
            geometryCounts,
            productCounts,
            comparison,
            candidateStats,
            oracleStats,
            gapAnalysis,
            comparisonFailed,
            comparisonError);
    }

    static FileGapAnalysis AnalyzeGaps(
        IfcFile stepFile,
        Model3D candidate,
        Model3D oracle,
        MeshingDiagnostics diagnostics)
    {
        var map = OracleEntityMap.Build(stepFile.FilePath, oracle);

        var candidateEntities = candidate.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();

        var oracleTriByEntity = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TriangleCount));

        var oracleOnlyByProduct = new Dictionary<string, int>(StringComparer.Ordinal);
        var oracleOnlyByGeometry = new Dictionary<string, int>(StringComparer.Ordinal);
        var oracleOnlyTriLossByGeometry = new Dictionary<string, long>(StringComparer.Ordinal);
        var topOracleOnly = new List<OracleOnlyEntityRecord>();

        foreach (var (entityId, triCount) in oracleTriByEntity.Where(kv => !candidateEntities.Contains(kv.Key)))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(entityId);
            var name = entity?.GetEntityName() ?? "?";
            oracleOnlyByProduct[name] = oracleOnlyByProduct.GetValueOrDefault(name) + 1;
            topOracleOnly.Add(new OracleOnlyEntityRecord(entityId, name, triCount));

            var tree = map.ProductRepresentationTrees.FirstOrDefault(t => t.EntityId == entityId);
            if (tree is null)
                continue;

            foreach (var geoName in WalkTree(tree))
            {
                oracleOnlyByGeometry[geoName] = oracleOnlyByGeometry.GetValueOrDefault(geoName) + 1;
                oracleOnlyTriLossByGeometry[geoName] = oracleOnlyTriLossByGeometry.GetValueOrDefault(geoName) + triCount;
            }
        }

        topOracleOnly.Sort((a, b) => b.OracleTriangles.CompareTo(a.OracleTriangles));

        var unsupported = diagnostics.EntityCounts
            .Where(kv => diagnostics.EntityStatus.GetValueOrDefault(kv.Key) == GeometrySupportStatus.Unsupported)
            .OrderByDescending(kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

        var approximate = diagnostics.EntityCounts
            .Where(kv => diagnostics.EntityStatus.GetValueOrDefault(kv.Key) == GeometrySupportStatus.Approximate)
            .OrderByDescending(kv => kv.Value)
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.Ordinal);

        return new FileGapAnalysis(
            oracleOnlyByProduct,
            oracleOnlyByGeometry,
            oracleOnlyTriLossByGeometry,
            topOracleOnly.Take(25).ToList(),
            unsupported,
            approximate);
    }

    static IEnumerable<string> WalkTree(RepresentationTreeNode node)
    {
        if (IsGeometryLeaf(node.EntityName))
            yield return node.EntityName;

        foreach (var child in node.Children)
        {
            foreach (var name in WalkTree(child))
                yield return name;
        }
    }

    static bool IsGeometryLeaf(string name)
        => name.Contains("SOLID", StringComparison.Ordinal) ||
           name.Contains("BREP", StringComparison.Ordinal) ||
           name.Contains("FACESET", StringComparison.Ordinal) ||
           name.Contains("BOOLEAN", StringComparison.Ordinal) ||
           name.Contains("SHELL", StringComparison.Ordinal) ||
           name is "IFCMAPPEDITEM" or "IFCHALFSPACESOLID";

    static Dictionary<string, int> ScanGeometryEntities(FilePath ifcPath)
    {
        using var step = new IfcFile(ifcPath, includeGeometry: false);
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entity in step.EntityResolver.GetEntities())
        {
            var name = entity.GetEntityName();
            if (!GeometryCreationBacklog.IsLikelyGeometryCreationEntity(name))
                continue;
            counts[name] = counts.GetValueOrDefault(name) + 1;
        }
        return counts;
    }

    static Dictionary<string, int> ScanProductTypes(FilePath ifcPath)
    {
        using var step = new IfcFile(ifcPath, includeGeometry: false);
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entity in step.EntityResolver.GetEntities())
        {
            var name = entity.GetEntityName();
            if (!name.StartsWith("IFC", StringComparison.Ordinal) || !name.EndsWith("TYPE", StringComparison.Ordinal))
            {
                if (IsProductEntity(name))
                    counts[name] = counts.GetValueOrDefault(name) + 1;
            }
        }
        return counts;
    }

    static bool IsProductEntity(string name)
        => name is "IFCWALL" or "IFCWALLSTANDARDCASE" or "IFCSLAB" or "IFCBEAM" or "IFCCOLUMN"
           or "IFCDOOR" or "IFCWINDOW" or "IFCMEMBER" or "IFCPLATE" or "IFCROOF" or "IFCSTAIR"
           or "IFCRAILING" or "IFCFOOTING" or "IFCPILE" or "IFCMEMBER" or "IFCBUILDINGELEMENTPROXY"
           or "IFCFURNISHINGELEMENT" or "IFCOPENINGELEMENT" or "IFCSPACE" or "IFCCOVERING"
           or "IFCCURTAINWALL" or "IFCCHIMNEY" or "IFCSHADINGDEVICE" or "IFCDISCRETEACCESSORY";

    static FileGapAnalysis EmptyGapAnalysis()
        => new(
            new Dictionary<string, int>(),
            new Dictionary<string, int>(),
            new Dictionary<string, long>(),
            [],
            new Dictionary<string, int>(),
            new Dictionary<string, int>());

    static List<EntityWorkPriority> RankWorkPriorities(IReadOnlyList<CatalogFileEvaluation> files)
    {
        var backlog = GeometryCreationBacklog.KnownItems.ToDictionary(i => i.EntityName);
        var agg = new Dictionary<string, (int catalog, int files, int oracleOnly, long triLoss, int unsupported)>(
            StringComparer.Ordinal);

        foreach (var file in files)
        {
            foreach (var (entity, count) in file.GeometryEntityCounts)
                MergeAgg(agg, entity, catalog: count, files: 1);

            foreach (var (entity, count) in file.GapAnalysis.OracleOnlyByGeometryEntity)
                MergeAgg(agg, entity, oracleOnly: count);

            foreach (var (entity, count) in file.GapAnalysis.UnsupportedDiagnostics)
                MergeAgg(agg, entity, unsupported: count);

            foreach (var (entity, triLoss) in file.GapAnalysis.OracleOnlyTriLossByGeometryEntity)
                MergeAgg(agg, entity, triLoss: triLoss);
        }

        return agg
            .Select(kv =>
            {
                var known = backlog.GetValueOrDefault(kv.Key);
                var support = known?.Support ?? GeometryCreationSupport.Planned;
                var notes = known?.Notes ?? "Not in geometry backlog; needs triage.";
                var priorityScore =
                    kv.Value.catalog +
                    kv.Value.oracleOnly * 50 +
                    kv.Value.unsupported * 25 +
                    (int)Math.Min(kv.Value.triLoss / 100, 500) +
                    (support == GeometryCreationSupport.Planned ? 200 :
                        support == GeometryCreationSupport.Partial ? 100 : 0);

                return new EntityWorkPriority(
                    kv.Key,
                    kv.Value.catalog,
                    kv.Value.files,
                    kv.Value.oracleOnly,
                    kv.Value.triLoss,
                    support,
                    notes,
                    priorityScore);
            })
            .Where(p => p.BacklogSupport != GeometryCreationSupport.Supported || p.OracleOnlyProductCount > 0 || p.TotalCatalogCount > 0)
            .OrderByDescending(p => p.PriorityScore)
            .ThenByDescending(p => p.TotalCatalogCount)
            .Take(40)
            .ToList();
    }

    static void MergeAgg(
        Dictionary<string, (int catalog, int files, int oracleOnly, long triLoss, int unsupported)> agg,
        string entity,
        int catalog = 0,
        int files = 0,
        int oracleOnly = 0,
        long triLoss = 0,
        int unsupported = 0)
    {
        if (!agg.TryGetValue(entity, out var cur))
            cur = (0, 0, 0, 0, 0);
        agg[entity] = (
            cur.catalog + catalog,
            cur.files + files,
            cur.oracleOnly + oracleOnly,
            cur.triLoss + triLoss,
            cur.unsupported + unsupported);
    }

    static string FormatMarkdown(CatalogEvaluationDocument document)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Studio Data Catalog Evaluation");
        sb.AppendLine();
        sb.AppendLine($"Generated: {document.GeneratedUtc:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Catalog: {document.CatalogName}");
        sb.AppendLine();

        sb.AppendLine("## Scorecard Summary");
        sb.AppendLine();
        sb.AppendLine("| File | Size MB | Parity | Inst (c/o) | Tris (c/o) | Entity BBox | Oracle-only products |");
        sb.AppendLine("|---|---:|---:|---:|---:|---:|---:|");
        foreach (var file in document.Files)
        {
            var parity = file.Comparison?.ParityScore.ToString("F3") ?? "FAIL";
            var entityBbox = file.Comparison is null
                ? "-"
                : $"{file.Comparison.EntityBoundingBox.MatchedCount}/{file.Comparison.EntityBoundingBox.ComparedCount}";
            var oracleOnly = file.GapAnalysis.OracleOnlyByProductType.Values.Sum();
            sb.AppendLine(
                $"| {file.FileName} | {file.FileSizeMb:F1} | {parity} | " +
                $"{file.CandidateStats.Instances}/{file.OracleStats.Instances} | " +
                $"{file.CandidateStats.Triangles}/{file.OracleStats.Triangles} | {entityBbox} | {oracleOnly} |");
        }
        sb.AppendLine();

        sb.AppendLine("## Recommended Work Priorities");
        sb.AppendLine();
        sb.AppendLine("| Entity | Catalog count | Files | Support | Priority | Notes |");
        sb.AppendLine("|---|---:|---:|---|---:|---|");
        foreach (var item in document.WorkPriorities.Take(25))
        {
            sb.AppendLine(
                $"| {item.EntityName} | {item.TotalCatalogCount} | {item.FilesAffected} | " +
                $"{item.BacklogSupport} | {item.PriorityScore} | {item.Notes} |");
        }
        sb.AppendLine();

        foreach (var file in document.Files)
        {
            sb.AppendLine($"## {file.FileName}");
            sb.AppendLine();
            if (file.ComparisonFailed)
            {
                sb.AppendLine($"**Comparison failed:** {file.ComparisonError}");
                sb.AppendLine();
            }

            sb.AppendLine("### Top geometry entities in file");
            sb.AppendLine();
            foreach (var (entity, count) in file.GeometryEntityCounts.OrderByDescending(kv => kv.Value).Take(15))
                sb.AppendLine($"- {entity}: {count}");
            sb.AppendLine();

            if (file.GapAnalysis.UnsupportedDiagnostics.Count > 0)
            {
                sb.AppendLine("### Unsupported diagnostics");
                sb.AppendLine();
                foreach (var (entity, count) in file.GapAnalysis.UnsupportedDiagnostics.Take(15))
                    sb.AppendLine($"- {entity}: {count}");
                sb.AppendLine();
            }

            if (file.GapAnalysis.TopOracleOnlyEntities.Count > 0)
            {
                sb.AppendLine("### Top oracle-only entities");
                sb.AppendLine();
                foreach (var record in file.GapAnalysis.TopOracleOnlyEntities.Take(10))
                    sb.AppendLine($"- #{record.EntityId} {record.EntityName}: {record.OracleTriangles} tris");
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters = { new JsonStringEnumConverter() },
    };
}
