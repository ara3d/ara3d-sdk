using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class OracleEntityMapTests
{
    [Test]
    [Category("IfcMesherParity")]
    public void WriteOracleMapsForQuickComparisonFiles()
    {
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
            if (!bfastPath.Exists() || NeedsOracleRegeneration(ifcPath, bfastPath))
                WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

            var mapPath = OracleEntityMap.MapPath(ifcPath);
            OracleEntityMap.Write(ifcPath, mapPath);
            TestContext.WriteLine($"Wrote {mapPath}");

            var document = OracleEntityMap.Build(ifcPath);
            Assert.That(document.OracleInstances, Is.Not.Empty);
            Assert.That(document.ProductRepresentationTrees, Is.Not.Empty);
            Assert.That(mapPath.Exists(), Is.True);
        }
    }

    static bool NeedsOracleRegeneration(FilePath ifcPath, FilePath bfastPath)
        => WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath);
}
