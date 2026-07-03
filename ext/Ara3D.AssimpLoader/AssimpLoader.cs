using Ara3D.Logging;
using Ara3D.Studio.API;
using Ara3D.Utils;

namespace Ara3D.AssimpLoader;

public class AssimpLoader : ILoader
{
    public async Task<IAsset> Load(FilePath filePath, ILogger logger)
    {
        return await Task.Run(() => new AssimpScene(filePath).Model.ToRenderableAsset());
    }
}