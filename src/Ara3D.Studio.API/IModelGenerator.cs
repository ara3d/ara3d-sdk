using Ara3D.Models;
using Ara3D.SceneEval;

namespace Ara3D.Studio.API;

public interface IModelGenerator
{
    Model3D Eval(EvalContext context);
}