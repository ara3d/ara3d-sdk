using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.SceneEval;
using Ara3D.Utils;

namespace Ara3D.Studio.API;

public interface IModelAsset : IAsset
{
    string Name { get; }
    FilePath FilePath { get; }
    Task<Model3D> Import(ILogger logger);
    Model3D Eval(EvalContext context);
}