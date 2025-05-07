using Ara3D.Data;
using Ara3D.Logging;

namespace Ara3D.Studio.API
{
    public interface IImporter
    {
        IRenderScene OpenFile(string filePath, ILogger logger, FileLoaderType loaderType = FileLoaderType.AutoDetect);
    }
}
