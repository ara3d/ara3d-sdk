using Ara3D.Models;
using Ara3D.SceneEval;

namespace Ara3D.Studio.API;

public interface IModelModifier
{
    IModel3D Eval(IModel3D model3D, EvalContext context);
}
