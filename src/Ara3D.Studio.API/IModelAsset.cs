using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IModelAsset : IAsset
{
    string Name { get; set; }
    FilePath FilePath { get; set; }
    Task<IModel3D> Import(ILogger logger);  
}