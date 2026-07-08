using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class ScorecardTests
{
    static FilePath ScorecardPath => TestFiles.ReportsDir.RelativeFile("scorecard.json");

    [Test]
    [Category("IfcMesherScore")]
    public void ScoreQuickComparisonFiles()
    {
        WebIfcBfastOracle.GenerateAll(TestFiles.QuickComparisonFiles(), TestContext.WriteLine);

        var results = new List<ModelComparisonResult>();
        foreach (var ifcPath in TestFiles.QuickComparisonFiles())
        {
            TestFiles.RequireExists(ifcPath);
            var result = ModelComparer.CompareFile(ifcPath);
            results.Add(result);
            TestContext.WriteLine(ModelComparer.FormatResult(result));
        }

        ModelComparer.WriteScorecard(results, ScorecardPath);
        TestContext.WriteLine($"Wrote {ScorecardPath}");

        Assert.That(ScorecardPath.Exists(), Is.True);
        Assert.That(results, Has.Count.EqualTo(3));
        Assert.That(results.All(r => r.ParityScore >= 0 && r.ParityScore <= 1), Is.True);
        Assert.That(results.All(r => r.MeshCount.Oracle > 0), Is.True, "Oracle should produce geometry");
    }

    [Test]
    [Explicit("WP-H-stretch: AC20-FZK-Haus parity and gap diagnosis")]
    public void ScoreAc20FzkHausStretch()
    {
        var ifcPath = TestFiles.Ac20FzkHaus;
        TestFiles.RequireExists(ifcPath);

        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath))
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        OracleEntityMap.Write(ifcPath);
        var map = OracleEntityMap.Build(ifcPath);
        TestContext.WriteLine($"Oracle map: {map.OracleInstanceCount} instances, {map.OracleMeshCount} meshes");

        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var (model, diagnostics) = ModelAssembler.BuildModel(stepFile);
        TestContext.WriteLine(
            $"Candidate: {model.Instances.Count} instances, {model.Meshes.Count} meshes, " +
            $"{model.Meshes.Sum(m => m.FaceIndices.Count)} tris");

        var result = ModelComparer.Compare(model, ModelComparer.LoadOracle(ifcPath), ifcPath.GetFileName());
        TestContext.WriteLine(ModelComparer.FormatResult(result));

        var candidateEntities = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();
        var oracleTriByEntity = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TriangleCount));

        var candidateInstByEntity = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());
        var oracleInstByEntity = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Count());

        TestContext.WriteLine("Top oracle-only entities by triangle count:");
        foreach (var (entityId, triCount) in oracleTriByEntity
                     .Where(kv => !candidateEntities.Contains(kv.Key))
                     .OrderByDescending(kv => kv.Value)
                     .Take(20))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(entityId);
            var name = entity?.GetEntityName() ?? "?";
            TestContext.WriteLine($"  #{entityId} {name}: {triCount} tris");
        }

        TestContext.WriteLine("Instance count deltas (oracle > candidate):");
        var missingInst = 0;
        foreach (var entityId in oracleInstByEntity.Keys
                     .Where(id => oracleInstByEntity[id] > candidateInstByEntity.GetValueOrDefault(id))
                     .OrderByDescending(id => oracleTriByEntity.GetValueOrDefault(id, 0)))
        {
            var delta = oracleInstByEntity[entityId] - candidateInstByEntity.GetValueOrDefault(entityId);
            missingInst += delta;
            var entity = stepFile.EntityResolver.GetEntityOrDefault(entityId);
            var name = entity?.GetEntityName() ?? "?";
            TestContext.WriteLine(
                $"  #{entityId} {name}: oracle={oracleInstByEntity[entityId]} cand={candidateInstByEntity.GetValueOrDefault(entityId)} " +
                $"tris={oracleTriByEntity.GetValueOrDefault(entityId, 0)}");
        }
        TestContext.WriteLine($"Total missing instances from deltas: {missingInst}");

        var extraInst = candidateInstByEntity.Keys
            .Where(id => candidateInstByEntity[id] > oracleInstByEntity.GetValueOrDefault(id))
            .Sum(id => candidateInstByEntity[id] - oracleInstByEntity.GetValueOrDefault(id));
        TestContext.WriteLine($"Total extra candidate instances: {extraInst}");

        var memberIds = map.ProductRepresentationTrees
            .Where(t => t.EntityName == "IFCMEMBER")
            .Select(t => t.EntityId)
            .ToList();
        var memberPresent = memberIds.Count(id => candidateInstByEntity.ContainsKey(id));
        TestContext.WriteLine($"IFCMEMBER coverage: {memberPresent}/{memberIds.Count}");

        TestContext.WriteLine("Diagnostics unsupported:");
        foreach (var (name, count) in diagnostics.EntityCounts
                     .Where(kv => diagnostics.EntityStatus.GetValueOrDefault(kv.Key) == GeometrySupportStatus.Unsupported)
                     .OrderByDescending(kv => kv.Value)
                     .Take(15))
            TestContext.WriteLine($"  {name}: {count}");

        TestContext.WriteLine("Diagnostics messages (sample):");
        foreach (var msg in diagnostics.Messages.Take(30))
            TestContext.WriteLine($"  {msg}");
    }

    [Test]
    [Explicit("WP-M-milestones: duplex.ifc parity baseline")]
    public void ScoreDuplexStretch()
    {
        var ifcPath = TestFiles.Duplex;
        TestFiles.RequireExists(ifcPath);

        var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
        if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath))
            WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);

        var result = ModelComparer.CompareFile(ifcPath);
        TestContext.WriteLine(ModelComparer.FormatResult(result));
    }
}
