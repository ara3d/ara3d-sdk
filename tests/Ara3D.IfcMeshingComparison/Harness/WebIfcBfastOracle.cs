using Ara3D.IfcLoader;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>Generates WebIfc geometry oracles as BFAST files under <c>data/bfast/webifc/</c>.</summary>
public static class WebIfcBfastOracle
{
    public static FilePath OraclePath(FilePath ifcPath)
        => TestFiles.WebIfcBfastDir.RelativeFile(ifcPath.GetFileNameWithoutExtension() + ".bfast");

    public static bool NeedsRegeneration(FilePath ifcPath, FilePath bfastPath)
        => !bfastPath.Exists()
           || File.GetLastWriteTimeUtc(ifcPath) > File.GetLastWriteTimeUtc(bfastPath)
           || !HasInstances(bfastPath);

    public static bool HasInstances(FilePath bfastPath)
    {
        if (!bfastPath.Exists())
            return false;
        using var data = RenderModelBfastSerializer.Load(bfastPath);
        return data.InstanceData.Count > 0;
    }

    public static void Generate(FilePath ifcPath, Action<string>? log = null)
    {
        TestFiles.WebIfcBfastDir.Create();
        var outPath = OraclePath(ifcPath);

        using var file = TestFiles.LoadWithOracleGeometry(ifcPath);
        var model = file.ToModel3D();
        using var renderData = new RenderModelData(3);
        renderData.Update(model);
        renderData.Write(outPath);

        var triangleCount = model.Meshes.Sum(m => m.FaceIndices.Count);
        log?.Invoke(
            $"{ifcPath.GetFileName()}: meshes={model.Meshes.Count}, instances={model.Instances.Count}, " +
            $"triangles={triangleCount} -> {outPath}");
    }

    public static void GenerateAll(IEnumerable<FilePath> ifcFiles, Action<string>? log = null)
    {
        foreach (var ifcPath in ifcFiles)
        {
            if (!ifcPath.Exists())
            {
                log?.Invoke($"Skipping missing IFC: {ifcPath}");
                continue;
            }

            var outPath = OraclePath(ifcPath);
            if (!NeedsRegeneration(ifcPath, outPath))
            {
                log?.Invoke($"Up to date: {ifcPath.GetFileName()}");
                continue;
            }

            Generate(ifcPath, log);
        }
    }
}
