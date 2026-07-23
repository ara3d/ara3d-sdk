namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Adds a vertical sine wave to any Curve3D flowing through the graph, offsetting Z by a function of X. Demonstrates a curve-to-curve modifier; the result is still a parametric curve, drawn as a polyline by the default renderer.")]
public class CurveWaveModifier : IModifier
{
    [Range(0f, 5f)] public float Amplitude = 1f;
    [Range(0.1f, 10f)] public float Frequency = 2f;

    public Curve3D Eval(Curve3D curve)
        => curve.Deform(p =>
        {
            var offset = (Number)Amplitude * (p.X * (Number)Frequency).Turns.Sin;
            return new Point3D(p.X, p.Y, p.Z + offset);
        });
}
