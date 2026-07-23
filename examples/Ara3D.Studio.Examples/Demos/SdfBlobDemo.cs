namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates an implicit solid (SDF) by unioning a sphere and a torus. The Sdf3D is infinite/procedural; the default SDF renderer voxelizes it over the view bounds and draws the inside cells as instanced boxes.")]
public class SdfBlobDemo : IGenerator
{
    [Range(0.5f, 8f)] public float SphereRadius = 3f;
    [Range(1f, 10f)] public float TorusMajor = 5f;
    [Range(0.2f, 4f)] public float TorusMinor = 1.2f;

    public Sdf3D Eval(EvalContext context)
        => SdfPrimitives.Sphere((Number)SphereRadius)
            .Union(SdfPrimitives.TorusZ((Number)TorusMajor, (Number)TorusMinor));
}
