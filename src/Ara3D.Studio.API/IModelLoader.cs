using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IModelLoader
{
    string FileTypeName { get; }
    string FileFilter { get; }
    Task<Model3D> Import(FilePath filePath, ILogger logger);
}