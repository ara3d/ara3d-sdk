namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a closed 2D profile (a regular polygon or star). The Profile2D flows through the graph and is drawn as a closed polyline on the XY plane by the default profile renderer.")]
public class RegularProfileDemo : IGenerator
{
    [Range(3, 64)] public int Sides = 6;
    [Range(0.5f, 10f)] public float Radius = 3f;
    public bool Star = false;
    [Range(0.1f, 1f)] public float InnerRatio = 0.5f;

    public Profile2D Eval(EvalContext context)
    {
        var points = new List<Vector2>(Sides);
        for (var i = 0; i < Sides; i++)
        {
            var angle = i / (float)Sides * 2f * MathF.PI;
            var r = Star && (i % 2 == 1) ? Radius * InnerRatio : Radius;
            points.Add(new Vector2(MathF.Cos(angle) * r, MathF.Sin(angle) * r));
        }
        return new Profile2D(points);
    }
}
