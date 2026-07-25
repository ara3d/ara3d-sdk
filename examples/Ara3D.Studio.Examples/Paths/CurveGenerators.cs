namespace Ara3D.Studio.Samples.Paths;

[Category(Cat.Curves)]
[Description("A helical curve with configurable radius, height, and number of revolutions.")]
public class Helix : IGenerator
{
    [Range(0.1f, 10f)] public float Radius { get; set; } = 1f;
    [Range(0f, 20f)] public float Height { get; set; } = 3f;
    [Range(0.5f, 32f)] public float Revolutions { get; set; } = 3f;

    // Returns the analytic Plato helix (an ICurve3D) rather than a sampled or lambda-wrapped
    // curve: the value stays inspectable and exact, and the viewport samples it at the current
    // render resolution (EvalContext.RenderSettings) only when it is drawn.
    public Ara3D.Geometry.Helix Eval()
        => new((Number)Radius, (Number)Height, (Number)Revolutions);
}

[Category(Cat.Curves)]
[Description("A planar spiral curve sweeping from an inner to an outer radius over a number of revolutions.")]
public class Spiral : IGenerator
{
    [Range(1, 32)] public int Revolutions { get; set; }
    [Range(0f, 10f)] public float InnerRadius { get; set; } = 0.5f;
    [Range(0f, 10f)] public float OuterRadius { get; set; } = 2f;

    public Curve3D Eval()
        => Curves.Spiral(Revolutions, InnerRadius, OuterRadius);
}

[Category(Cat.Curves)]
[Description("A sine-wave curve with configurable width, height, phase, and cycle count.")]
public class SineWave : IGenerator
{
    [Range(0f, 20f)] public float WaveWidth { get; set; }
    [Range(0f, 20f)] public float WaveHeight { get; set; }
    [Range(0f, 1f)] public float Phase { get; set; }
    [Range(0f, 10f)] public float Cycles { get; set; }

    public Curve3D Eval()
        => new(t => (
            t * Cycles * WaveWidth,
            (t / Cycles + Phase).Turns.Sin * WaveHeight,
            0));
}
