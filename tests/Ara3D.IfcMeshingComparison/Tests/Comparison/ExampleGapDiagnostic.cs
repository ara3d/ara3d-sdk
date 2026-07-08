using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;

namespace Ara3D.IfcMeshingComparison.Tests.Comparison;

[TestFixture]
public sealed class ExampleGapDiagnostic
{
    [Test]
    [Explicit("WP-G-example gap diagnosis")]
    public void DiagnoseExampleGaps()
    {
        var ifcPath = TestFiles.Example;
        TestFiles.RequireExists(ifcPath);

        var map = OracleEntityMap.Build(ifcPath);
        using var stepFile = new IfcFile(ifcPath, includeGeometry: false);
        var (model, diagnostics) = ModelAssembler.BuildModel(stepFile);

        var triCount = model.Meshes.Sum(m => m.FaceIndices.Count);
        TestContext.WriteLine($"Candidate: {model.Instances.Count} instances, {model.Meshes.Count} meshes, {triCount} tris");

        var candidateEntities = model.Instances
            .Where(i => i.EntityIndex >= 0)
            .Select(i => i.EntityIndex)
            .ToHashSet();
        var oracleTriByEntity = map.OracleInstances
            .GroupBy(i => i.EntityIndex)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TriangleCount));

        TestContext.WriteLine("Top oracle-only entities by triangle count:");
        foreach (var (entityId, triCount2) in oracleTriByEntity
                     .Where(kv => !candidateEntities.Contains(kv.Key))
                     .OrderByDescending(kv => kv.Value)
                     .Take(30))
        {
            var entity = stepFile.EntityResolver.GetEntityOrDefault(entityId);
            var name = entity?.GetEntityName() ?? "?";
            TestContext.WriteLine($"  #{entityId} {name}: {triCount2} tris");
        }

        TestContext.WriteLine("Diagnostics unsupported:");
        foreach (var (name, count) in diagnostics.EntityCounts
                     .Where(kv => diagnostics.EntityStatus.GetValueOrDefault(kv.Key) == GeometrySupportStatus.Unsupported)
                     .OrderByDescending(kv => kv.Value)
                     .Take(20))
            TestContext.WriteLine($"  {name}: {count}");

        TestContext.WriteLine("Diagnostics messages (sample):");
        foreach (var msg in diagnostics.Messages.Take(40))
            TestContext.WriteLine($"  {msg}");
    }
}
