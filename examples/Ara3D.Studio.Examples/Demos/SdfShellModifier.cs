namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Hollows out an implicit solid: takes an Sdf3D and returns a shell of the given thickness (the 'onion' operator). Demonstrates that SDFs flow and are modified as first-class values, with rendering handled by the default SDF renderer.")]
public class SdfShellModifier : IModifier
{
    [Range(0.05f, 3f)] public float Thickness = 0.5f;

    public Sdf3D Eval(Sdf3D sdf)
        => sdf.Onion((Number)Thickness);
}
