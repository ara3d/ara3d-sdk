using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Meshers;
using Ara3D.IfcTypes;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class MeshIndexDiagnosticTests
{
    /// <summary>Files that previously crashed on void-profile extrusion index bugs.</summary>
    static readonly string[] CrashCatalogFiles =
    [
        "example.ifc",
        "dental_clinic.ifc",
        "ifcbridge-model01.ifc",
        "ISSUE_034_HouseZ.ifc",
        "ISSUE_068_ARK_NUS_skolebygg.ifc",
        "Office_A_20110811.ifc",
    ];

    /// <summary>Subset for BFAST round-trip (same as index catalog; DigitalHub is covered by WpW11).</summary>
    static readonly string[] CrashCatalogExportFiles = CrashCatalogFiles;

    static FilePath ResolveCatalogIfc(string fileName)
        => TestFiles.ResolveIfc(fileName);

    static int CountInvalidMeshes(Model3D model, out List<string> samples)
    {
        samples = [];
        var bad = 0;
        for (var mi = 0; mi < model.Meshes.Count; mi++)
        {
            var mesh = model.Meshes[mi];
            var pc = mesh.Points.Count;
            foreach (var f in mesh.FaceIndices)
            {
                if (f.A < 0 || f.B < 0 || f.C < 0 || f.A >= pc || f.B >= pc || f.C >= pc)
                {
                    bad++;
                    if (samples.Count < 5)
                        samples.Add($"mesh[{mi}] face ({f.A},{f.B},{f.C}) points={pc}");
                    break;
                }
            }
        }
        return bad;
    }

    [Test]
    [Category("Slow")]
    public void CrashCatalog_AllKnownVoidProfileExtrusions_HaveValidMeshIndices()
    {
        var results = new List<string>();
        foreach (var fileName in CrashCatalogFiles)
        {
            var ifcPath = ResolveCatalogIfc(fileName);
            if (!ifcPath.Exists())
            {
                results.Add($"{fileName}: SKIP (missing)");
                continue;
            }

            using var file = new IfcFile(ifcPath, includeGeometry: false);
            var (model, _) = ModelAssembler.BuildModel(file);
            var bad = CountInvalidMeshes(model, out var samples);
            var status = bad == 0 ? "PASS" : "FAIL";
            results.Add($"{fileName}: {status} badMeshes={bad} meshes={model.Meshes.Count}");
            foreach (var line in samples)
                results.Add($"  {line}");
        }

        foreach (var line in results)
            TestContext.WriteLine(line);

        Assert.That(results.Exists(r => r.Contains(": PASS")), Is.True, "at least one catalog file should run");
        Assert.That(results.Where(r => r.Contains(": FAIL")), Is.Empty,
            string.Join(Environment.NewLine, results.Where(r => r.Contains("FAIL") || r.StartsWith("  "))));
    }

    [Test]
    [Category("Slow")]
    public void CrashCatalog_ExportsBfastWithoutIndexErrors()
    {
        var backend = new Approach1Backend();
        var results = new List<string>();
        foreach (var fileName in CrashCatalogExportFiles)
        {
            var ifcPath = ResolveCatalogIfc(fileName);
            if (!ifcPath.Exists())
            {
                results.Add($"{fileName}: SKIP (missing)");
                continue;
            }

            var build = backend.Build(ifcPath);
            if (!build.Success)
            {
                results.Add($"{fileName}: FAIL build — {string.Join("; ", build.Errors)}");
                continue;
            }

            var model = build.Model!;
            var bad = CountInvalidMeshes(model, out _);
            if (bad > 0)
            {
                results.Add($"{fileName}: FAIL {bad} meshes with out-of-range indices");
                continue;
            }

            var bfastPath = ifcPath.ChangeExtension(".catalog-index-probe.bfast");
            using (var renderData = new RenderModelData(3))
            {
                renderData.Update(model);
                renderData.Write(bfastPath);
            }

            using (var loaded = RenderModelBfastSerializer.Load(bfastPath))
            {
                if (loaded.InstanceData.Count != model.Instances.Count
                    || loaded.MeshSliceData.Count != model.Meshes.Count)
                {
                    results.Add($"{fileName}: FAIL BFAST round-trip instance/mesh count mismatch");
                    continue;
                }
            }

            results.Add($"{fileName}: PASS meshes={model.Meshes.Count} instances={model.Instances.Count}");
        }

        foreach (var line in results)
            TestContext.WriteLine(line);

        Assert.That(results.Where(r => r.Contains(": FAIL")), Is.Empty,
            string.Join(Environment.NewLine, results));
    }

    [Test]
    [Category("Slow")]
    public void ExampleIfc_ExtrudedVoidProfiles_HaveValidMeshIndices()
    {
        var ifcPath = TestFiles.Example;
        TestFiles.RequireExists(ifcPath);
        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);

        for (var mi = 0; mi < model.Meshes.Count; mi++)
        {
            var mesh = model.Meshes[mi];
            var pc = mesh.Points.Count;
            foreach (var f in mesh.FaceIndices)
                Assert.That(f.A >= 0 && f.B >= 0 && f.C >= 0 && f.A < pc && f.B < pc && f.C < pc,
                    $"mesh[{mi}] has out-of-range face ({f.A},{f.B},{f.C}) with {pc} points");
        }
    }

    [Test]
    [Category("Slow")]
    public void ExampleIfc_ExportsBfastWithoutIndexErrors()
    {
        var ifcPath = TestFiles.Example;
        TestFiles.RequireExists(ifcPath);

        var backend = new Approach1Backend();
        var result = backend.Build(ifcPath);
        Assert.That(result.Success, Is.True, string.Join("; ", result.Errors));

        var model = result.Model!;
        var bfastPath = ifcPath.ChangeExtension(".candidate.bfast");
        using (var renderData = new RenderModelData(3))
        {
            renderData.Update(model);
            renderData.Write(bfastPath);
        }

        using (var loaded = RenderModelBfastSerializer.Load(bfastPath))
        {
            Assert.That(loaded.InstanceData.Count, Is.EqualTo(model.Instances.Count));
            Assert.That(loaded.MeshSliceData.Count, Is.EqualTo(model.Meshes.Count));
        }
    }

    [Test]
    [Explicit("Find meshes with out-of-range face indices")]
    public void FindInvalidMeshIndices_OnExample()
    {
        var ifcPath = TestFiles.Example;
        TestFiles.RequireExists(ifcPath);
        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var (model, _) = ModelAssembler.BuildModel(file);

        var instByMesh = model.Instances
            .GroupBy(i => i.MeshIndex)
            .ToDictionary(g => g.Key, g => g.Select(i => i.EntityIndex).Distinct().ToList());

        var badMeshes = new List<string>();
        for (var mi = 0; mi < model.Meshes.Count; mi++)
        {
            var mesh = model.Meshes[mi];
            var pc = mesh.Points.Count;
            for (var fi = 0; fi < mesh.FaceIndices.Count; fi++)
            {
                var f = mesh.FaceIndices[fi];
                if (f.A < 0 || f.B < 0 || f.C < 0 || f.A >= pc || f.B >= pc || f.C >= pc)
                {
                    var entities = instByMesh.GetValueOrDefault(mi, []);
                    badMeshes.Add(
                        $"mesh[{mi}] entities=[{string.Join(",", entities)}] face[{fi}] ({f.A},{f.B},{f.C}) points={pc} tris={mesh.FaceIndices.Count}");
                    break;
                }
            }
        }

        var badInst = model.Instances
            .Where(i => i.MeshIndex < 0 || i.MeshIndex >= model.Meshes.Count)
            .Select(i => $"inst entity={i.EntityIndex} meshIndex={i.MeshIndex}")
            .ToList();

        TestContext.WriteLine($"Meshes={model.Meshes.Count}, instances={model.Instances.Count}");
        TestContext.WriteLine($"Bad meshes: {badMeshes.Count}");
        foreach (var line in badMeshes.Take(20))
            TestContext.WriteLine($"  {line}");
        TestContext.WriteLine($"Bad instances: {badInst.Count}");
        foreach (var line in badInst.Take(10))
            TestContext.WriteLine($"  {line}");

        Assert.That(badMeshes, Is.Empty, "all face indices should be in range");
        Assert.That(badInst, Is.Empty, "all instance mesh indices should be valid");
    }

    [Test]
    [Explicit("Trace invalid indices for SHS column #7663")]
    public void TraceEntity7663_MeshBuild()
    {
        var ifcPath = TestFiles.Example;
        TestFiles.RequireExists(ifcPath);
        using var file = new IfcFile(ifcPath, includeGeometry: false);
        var ctx = new MeshingContext(file);

        void Check(string label, TriangleMesh3D mesh)
        {
            var pc = mesh.Points.Count;
            var bad = mesh.FaceIndices
                .Select((f, i) => (f, i))
                .Where(t => t.f.A < 0 || t.f.B < 0 || t.f.C < 0 || t.f.A >= pc || t.f.B >= pc || t.f.C >= pc)
                .Take(3)
                .ToList();
            TestContext.WriteLine($"{label}: points={pc} tris={mesh.FaceIndices.Count} bad={bad.Count}");
            foreach (var (f, i) in bad)
                TestContext.WriteLine($"  face[{i}] ({f.A},{f.B},{f.C})");
        }

        var entity = ctx.GetEntity(7663);
        var rep = MeshHelpers.ResolveRequired(ctx, entity, IfcProduct.Instance.Representation);
        var parts = new List<CollectedPart>();
        GeometryPartCollector.CollectParts(ctx, rep, Matrix4x4.Identity, 7663, parts);
        foreach (var (part, idx) in parts.Select((p, i) => (p, i)))
            Check($"part[{idx}]", part.Mesh);
    }
}
