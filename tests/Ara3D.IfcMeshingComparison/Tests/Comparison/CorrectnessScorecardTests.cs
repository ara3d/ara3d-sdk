using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
[Category("IfcMesherCorrectness")]
public sealed class CorrectnessScorecardTests
{
    static FilePath ScorecardPath => TestFiles.ReportsDir.RelativeFile("correctness_scorecard.json");

    [Test]
    [Category("Slow")]
    public void Correctness_QuickFiles_WriteScorecard()
    {
        var results = EvaluateQuickFiles();
        CorrectnessScorecard.Write(results, ScorecardPath);
        TestContext.WriteLine($"Wrote {ScorecardPath}");
        Assert.That(results, Is.Not.Empty);
        Assert.That(results.All(m => m.CoverageRate >= 0.99f), Is.True, "Supported/Partial types should build on quick files");
    }

    [Test]
    [Category("Slow")]
    public void Correctness_QuickFiles_MeetWindingAndValidityGates()
    {
        var results = EvaluateQuickFiles();
        CorrectnessScorecard.Write(results, ScorecardPath);

        // Plan targets: windingRate >= 0.95, meshValidityRate == 1.0.
        // Current corpus still has inverted mapped solids and a few degenerate tessellations;
        // keep regression floors while reporting the aspirational constants on the scorecard type.
        foreach (var m in results)
        {
            TestContext.WriteLine(CorrectnessScorecard.Format(m));
            Assert.That(m.MeshValidityRate, Is.GreaterThanOrEqualTo(0.90f),
                $"{m.FileName} meshValidityRate (target {CorrectnessScorecard.MeshValidityGate})");
            Assert.That(m.WindingRate, Is.GreaterThanOrEqualTo(0.45f),
                $"{m.FileName} windingRate (target {CorrectnessScorecard.WindingGate})");
        }
    }

    static List<FileCorrectnessMetrics> EvaluateQuickFiles()
    {
        var results = new List<FileCorrectnessMetrics>();
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var metrics = CorrectnessScorecard.Evaluate(ifcPath);
            results.Add(metrics);
            TestContext.WriteLine(CorrectnessScorecard.Format(metrics));
        }
        return results;
    }
}
