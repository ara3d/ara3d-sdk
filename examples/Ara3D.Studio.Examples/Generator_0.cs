public class Generator_0 : IGenerator
{
    [Range(0f, 1f)] public float Red = 0.2f;
    [Range(0f, 1f)] public float Green = 0.8f;
    [Range(0f, 1f)] public float Blue = 0.1f;

    public Material Material 
        => new((Red, Green, Blue, 1f), 0f, 1f);

    public IModel3D Eval(EvalContext context)
        => PlatonicSolids.TriangulatedCube.ToModel3D(Material);
}