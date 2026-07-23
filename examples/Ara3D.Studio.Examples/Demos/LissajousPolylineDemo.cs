namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a Lissajous space curve as an explicit Polyline3D. The polyline flows through the graph and is drawn as connected line segments by the default polyline renderer.")]
public class LissajousPolylineDemo : IGenerator
{
    [Range(16, 1000)] public int Count = 300;
    [Range(0.5f, 10f)] public float Size = 4f;
    [Range(1, 8)] public int FreqX = 3;
    [Range(1, 8)] public int FreqY = 2;
    [Range(1, 8)] public int FreqZ = 5;

    public Polyline3D Eval(EvalContext context)
    {
        var points = new List<Point3D>(Count);
        for (var i = 0; i < Count; i++)
        {
            var a = i / (float)(Count - 1) * 2f * MathF.PI;
            points.Add(new Point3D(
                MathF.Sin(a * FreqX) * Size,
                MathF.Sin(a * FreqY) * Size,
                MathF.Sin(a * FreqZ) * Size));
        }
        return new Polyline3D(points, true);
    }
}
