using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IModelLoader
{
    string FileType { get; }
    string FileFilter { get; }
    string Description { get; }
    Task<IModel3D> Import(FilePath filePath, ILogger logger);
}