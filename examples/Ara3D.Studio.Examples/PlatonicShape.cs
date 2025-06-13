namespace Ara3D.Studio.Samples;

public class PlatonicShape : IModelGenerator
{
    [Range(0, 4)] public int Shape;
    [Range(0f, 1f)] public float Red = 0.2f;
    [Range(0f, 1f)] public float Green = 0.8f;
    [Range(0f, 1f)] public float Blue = 0.1f;
    [Range(0f, 1f)] public float Alpha = 1f;
    [Range(0f, 1f)] public float Metallic = 0f;
    [Range(0f, 1f)] public float Roughness = 0.5f;
    [Range(-3f, 3f)] public float Push = 0f;

    public Material Material =>
        new((Red, Green, Blue, Alpha), Metallic, Roughness);

    public Model3D Eval(EvalContext context)
    {
        var mesh = PlatonicSolids.GetMesh(Shape);
        if (MathF.Abs(Push) > 0.001f)
        {
            mesh = mesh.PushVertices(Push);
        }
        return new Element(mesh, Material);
    }
}