using Ara3D.Models;
using Ara3D.SceneEval;

namespace Ara3D.Studio.API;

public interface IModelGenerator
{
    IModel3D Eval(EvalContext context);
}

public static class ApiExtensions
{
    public static IModelGenerator CreateDefault(this IModelGenerator self)
        => Activator.CreateInstance(self.GetType()) as IModelGenerator
            ?? throw new Exception($"Failed to make a copy of {self}");

    public static IModelModifier CreateDefault(this IModelModifier self)
        => Activator.CreateInstance(self.GetType()) as IModelModifier
            ?? throw new Exception($"Failed to make a copy of {self}");
}