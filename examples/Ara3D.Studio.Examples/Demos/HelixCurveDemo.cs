namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a helix as a parametric Curve3D. The curve flows through the graph and is drawn as a polyline by the default curve renderer (no meshing in the script).")]
public class HelixCurveDemo : IGenerator
{
    [Range(0.1f, 10f)] public float Radius = 3f;
    [Range(0.1f, 20f)] public float Height = 6f;
    [Range(0.5f, 12f)] public float Turns = 4f;

    public Curve3D Eval(EvalContext context)
        => new(t =>
        {
            var angle = t.Turns.Multiply((Number)Turns);
            var x = angle.Cos * (Number)Radius;
            var y = angle.Sin * (Number)Radius;
            var z = t * (Number)Height;
            return new Point3D(x, y, z);
        });
}
