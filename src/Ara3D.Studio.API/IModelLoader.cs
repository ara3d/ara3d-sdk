using Ara3D.Logging;
using Ara3D.Studio.Data;

namespace Ara3D.Studio.API;

public interface IModelLoader
{
    IRenderScene OpenFile(string filePath, ILogger logger, FileLoaderType loaderType = FileLoaderType.AutoDetect);
}