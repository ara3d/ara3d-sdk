using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.IfcMeshingComparison.Reporting;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class ComparisonTests
{
    static readonly IMeshingBackend[] Backends =
    [
        new WebIfcBackend(),
        new Approach1Backend(),
    ];

    [Test]
    [Explicit("Copies IFC corpora and generates WebIfc BFAST oracle files under data/")]
    public void GenerateWebIfcBfastOracles()
    {
        void Log(string message) => TestContext.WriteLine(message);

        TestDataManager.EnsureLocalIfcCopied(Log);
        WebIfcBfastOracle.GenerateAll(TestFiles.AllKnownFiles(), Log);
    }

    [Test]
    [Explicit("Compares all backends across the full IFC catalog and writes markdown reports")]
    public void RunFullComparison()
    {
        void Log(string message) => TestContext.WriteLine(message);

        var report = ComparisonReport.Run(Backends, TestFiles.AllKnownFiles(), Log);
        var comparisonPath = MarkdownReportWriter.WriteComparisonReport(report);
        var capabilitiesPath = MarkdownReportWriter.WriteCapabilitiesReport(report);

        Log($"Wrote {comparisonPath}");
        Log($"Wrote {capabilitiesPath}");
        Assert.That(comparisonPath.Exists(), Is.True);
        Assert.That(capabilitiesPath.Exists(), Is.True);
    }

    [Test]
    [Explicit("Compares all backends on small IFC files and writes markdown reports")]
    public void RunQuickComparison()
    {
        void Log(string message) => TestContext.WriteLine(message);

        var files = TestFiles.QuickComparisonFiles().ToList();
        foreach (var file in files)
            TestFiles.RequireExists(file);

        var report = ComparisonReport.Run(Backends, files, Log);
        var comparisonPath = MarkdownReportWriter.WriteComparisonReport(report);
        var capabilitiesPath = MarkdownReportWriter.WriteCapabilitiesReport(report);

        Log($"Wrote {comparisonPath}");
        Log($"Wrote {capabilitiesPath}");

        foreach (var file in files)
        {
            var webIfc = report.Results.First(r => r.IfcPath == file && r.BackendName == "WebIfcDll");
            var approach1 = report.Results.First(r => r.IfcPath == file && r.BackendName == "Approach1");
            Log(GeometryComparison.FormatComparison(file.GetFileName(), ModelStats.FromModel(approach1.Model!), ModelStats.FromModel(webIfc.Model!)));
        }
    }

    [Test]
    [Explicit("Compares pure C# meshing to stored WebIfc BFAST oracles on small IFC files")]
    public void CompareApproach1ToWebIfcBfast_SmallFiles()
    {
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
            var (mine, oracle) = GeometryComparison.CompareFile(ifcPath, bfastPath);
            TestContext.WriteLine(GeometryComparison.FormatComparison(ifcPath.GetFileName(), mine, oracle));

            Assert.That(mine.TriangleCount, Is.GreaterThan(0), $"{ifcPath.GetFileName()}: mine produced no triangles");
            Assert.That(oracle.TriangleCount, Is.GreaterThan(0), $"{ifcPath.GetFileName()}: oracle has no triangles");
        }
    }
}
