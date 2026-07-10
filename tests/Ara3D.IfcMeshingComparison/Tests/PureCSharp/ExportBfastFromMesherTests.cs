using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class ExportBfastFromMesherTests
{
    static FilePath BfastPath(FilePath ifcPath) => ifcPath.ChangeExtension(".bfast");

    [Test]
    [Explicit("Meshes each project IFC via pure-C# Approach1 and writes a BFAST next to the input")]
    [Category("Slow")]
    public void ExportApproach1BfastAlongsideEachIfc()
    {
        void Log(string message) => TestContext.WriteLine(message);

        TestDataManager.EnsureLocalIfcCopied(Log);

        var backend = new Approach1Backend();
        var files = TestFiles.AllKnownFiles().ToList();
        if (files.Count == 0)
            Assert.Ignore("No IFC test files available");

        Log($"Exporting BFAST for {files.Count} IFC files via {backend.Name}");

        var exported = 0;
        var failed = new List<string>();
        foreach (var ifcPath in files)
        {
            if (!ifcPath.Exists())
            {
                Log($"Skipping missing IFC: {ifcPath}");
                continue;
            }

            var result = backend.Build(ifcPath);
            if (!result.Success)
            {
                var message = $"{ifcPath.GetFileName()}: {string.Join("; ", result.Errors)}";
                Log($"FAILED {message}");
                failed.Add(message);
                continue;
            }

            IModel3D model = result.Model!;
            var bfastPath = BfastPath(ifcPath);

            using (var renderData = new RenderModelData(3))
            {
                renderData.Update(model);
                renderData.Write(bfastPath);
            }

            Assert.That(bfastPath.Exists(), Is.True);

            using (var loaded = RenderModelBfastSerializer.Load(bfastPath))
            {
                Assert.That(loaded.InstanceData.Count, Is.EqualTo(model.Instances.Count));
                Assert.That(loaded.MeshSliceData.Count, Is.EqualTo(model.Meshes.Count));
            }

            var tris = model.Meshes.Sum(m => m.FaceIndices.Count);
            Log(
                $"{ifcPath.GetFileName()}: meshes={model.Meshes.Count}, instances={model.Instances.Count}, " +
                $"triangles={tris} -> {bfastPath}");
            exported++;
        }

        Assert.That(exported, Is.GreaterThan(0));
        Log($"Exported {exported} BFAST files");
        if (failed.Count > 0)
            Log($"Mesh failures ({failed.Count}):\n  {string.Join("\n  ", failed)}");
    }
}
