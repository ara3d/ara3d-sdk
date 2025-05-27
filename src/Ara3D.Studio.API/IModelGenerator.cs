using Ara3D.Models;

namespace Ara3D.Studio.API;

public interface IModelGenerator
{
    Model Eval();
}

public static class ApiExtensions
{
    public static IModelGenerator CreateDefault(this IModelGenerator self)
        => Activator.CreateInstance(self.GetType()) as IModelGenerator
            ?? throw new Exception($"Failed to make a copy of {self}");

    public static IModelOperator CreateDefault(this IModelOperator self)
        => Activator.CreateInstance(self.GetType()) as IModelOperator
            ?? throw new Exception($"Failed to make a copy of {self}");
}