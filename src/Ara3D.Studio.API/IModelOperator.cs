using Ara3D.Models;

namespace Ara3D.Studio.API;

public interface IModelOperator
{
    Model Eval(Model model);
}