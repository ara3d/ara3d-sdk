using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public class AssetSource : IAssetSource
{
    public string Name { get; private set; }
    public string FileSize { get; private set; }
    public string LoadTime { get; private set; }
    public FilePath FilePath { get; }
    public ILoader Loader { get; }
    public IAsset? Asset { get; private set; }
    public string LoaderTypeName { get; }
    public int NumMeshes { get; private set; }
    public int NumInstances { get; private set; }
    public long NumPoints { get; private set; }
    public long NumFaces { get; private set; }
    public string FileType => FilePath.GetExtension();
    public Action OpenFileInExplorer => () => FilePath.OpenFileInExplorer();

    public AssetSource(FilePath filePath, ILoader loader)
    {
        FilePath = filePath;
        Name = FilePath.GetFileName();
        Loader = loader;
        LoaderTypeName = Loader.GetType().Name;
    }

    public async Task<IAsset> InitialLoad(ILogger logger)
    {
        if (Asset != null)
            throw new Exception("Asset already loaded");
        logger.Log($"STARTED loading model from {FilePath} using the {LoaderTypeName} loader");
        FileSize = FilePath.GetFileSizeAsString(); 
        var sw = Stopwatch.StartNew();
        Asset = await Loader.Load(FilePath, logger);
        LoadTime = sw.PrettyPrintTimeElapsed();
        logger.Log($"COMPLETED loading model from {FilePath}");
        if (Asset.Value is RenderModelData rmd)
        {
            NumMeshes = rmd.MeshCount;
            NumPoints = rmd.TotalVertexCount;
            NumInstances = rmd.InstanceCount;
            NumFaces = rmd.TotalFaceCount;
        }

        return Asset;
    }

    public IAsset Eval(EvalContext context)
    {
        return Asset;
    }

    public void Dispose()
    {
        Asset?.Dispose();
        Asset = null;
    }
}