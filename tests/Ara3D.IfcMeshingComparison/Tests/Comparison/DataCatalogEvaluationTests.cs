using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Reporting;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class DataCatalogEvaluationTests
{
    static FilePath JsonReportPath => TestFiles.ReportsDir.RelativeFile("studio_catalog_evaluation.json");
    static FilePath MarkdownReportPath => TestFiles.ReportsDir.RelativeFile("studio_catalog_evaluation.md");

    [Test]
    [Category("IfcMesherCatalog")]
    [Category("Slow")]
    public void EvaluateStudioDataCatalog()
    {
        var files = TestFiles.StudioDataFiles().ToList();
        if (files.Count == 0)
            Assert.Ignore($"No IFC files found in {TestFiles.StudioDataDir}");

        void Log(string message) => TestContext.WriteLine(message);
        Log($"Evaluating {files.Count} files from {TestFiles.StudioDataDir}");

        var document = CatalogEvaluationReport.Run(files, "studio-data", Log);
        var jsonPath = CatalogEvaluationReport.WriteJson(document, JsonReportPath);
        var mdPath = CatalogEvaluationReport.WriteMarkdown(document, MarkdownReportPath);

        Log($"Wrote {jsonPath}");
        Log($"Wrote {mdPath}");

        PrintSummary(document);

        Assert.That(jsonPath.Exists(), Is.True);
        Assert.That(document.Files, Has.Count.EqualTo(files.Count));
        Assert.That(document.WorkPriorities, Is.Not.Empty);

        var successful = document.Files.Count(f => !f.ComparisonFailed);
        Assert.That(successful, Is.GreaterThan(0), "At least one file should compare successfully");
    }

    [Test]
    [Explicit("Regenerates studio data catalog evaluation and markdown report")]
    [Category("IfcMesherCatalog")]
    [Category("Slow")]
    public void EvaluateStudioDataCatalog_Explicit()
        => EvaluateStudioDataCatalog();

    static void PrintSummary(CatalogEvaluationDocument document)
    {
        TestContext.WriteLine("");
        TestContext.WriteLine("=== Studio Data Catalog Summary ===");
        TestContext.WriteLine(
            "| File | Parity | Inst c/o | Tris c/o | Oracle-only |");
        foreach (var file in document.Files.OrderByDescending(f => f.Comparison?.ParityScore ?? -1))
        {
            var parity = file.ComparisonFailed ? "FAIL" : $"{file.Comparison!.ParityScore:F3}";
            var oracleOnly = file.GapAnalysis.OracleOnlyByProductType.Values.Sum();
            TestContext.WriteLine(
                $"| {file.FileName} | {parity} | " +
                $"{file.CandidateStats.Instances}/{file.OracleStats.Instances} | " +
                $"{file.CandidateStats.Triangles}/{file.OracleStats.Triangles} | {oracleOnly} |");
        }

        TestContext.WriteLine("");
        TestContext.WriteLine("=== Top work priorities ===");
        foreach (var item in document.WorkPriorities.Take(15))
        {
            TestContext.WriteLine(
                $"  {item.EntityName}: priority={item.PriorityScore}, catalog={item.TotalCatalogCount}, " +
                $"files={item.FilesAffected}, oracleOnly={item.OracleOnlyProductCount}, " +
                $"triLoss={item.OracleOnlyTriangleLoss}, support={item.BacklogSupport}");
        }
    }
}
