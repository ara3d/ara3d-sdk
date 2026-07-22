namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Removes model instances whose triangle count falls below a complexity threshold.")]
public class FilterObjects : IModifier
{
    [Range(0, 20)] public int ObjectComplexity = 2;

    public int MinTriangleCount => ObjectComplexity * ObjectComplexity * ObjectComplexity;

    public IModel3D Eval(IModel3D model3D, EvalContext context)
        => model3D.Where(mesh => mesh.Triangles.Count >= MinTriangleCount);
}