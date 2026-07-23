namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a single SDF primitive (sphere, cube, torus, or octahedron). Pair it with the SDF shell modifier to hollow it. Rendered as voxels by the default SDF renderer.")]
public class SdfPrimitiveDemo : IGenerator
{
    public enum Kind { Sphere, Cube, TorusZ, Octahedron }

    public Kind Shape = Kind.Cube;
    [Range(0.5f, 8f)] public float Size = 3f;
    [Range(0.2f, 4f)] public float TubeRadius = 1f;

    public Sdf3D Eval(EvalContext context)
        => Shape switch
        {
            Kind.Sphere => SdfPrimitives.Sphere(Size),
            Kind.Cube => SdfPrimitives.Cube(Size),
            Kind.TorusZ => SdfPrimitives.TorusZ(Size, TubeRadius),
            Kind.Octahedron => SdfPrimitives.Octahedron(Size),
            _ => SdfPrimitives.Sphere(Size),
        };
}
