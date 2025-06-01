using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public class ModelAsset : IModelAsset
{
    public string Name { get; set; }
    public FilePath FilePath { get; set; }
    public IModelLoader Loader { get; set; }

    public ModelAsset(FilePath filePath, IModelLoader loader)
    {
        FilePath = filePath;
        Name = FilePath.GetFileName();
        Loader = loader;
    }

    public async Task<Model3D> Import(ILogger logger)
        => await Loader.Import(FilePath, logger);
}