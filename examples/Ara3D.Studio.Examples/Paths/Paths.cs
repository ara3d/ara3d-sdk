namespace Ara3D.Studio.Samples.Paths;

[Category(Cat.Curves)]
[Description("A closed regular polygon outline as line geometry, with configurable sides and radius.")]
public class Polygon : IGenerator
{
    [Range(3, 64)] public int Sides = 3;
    [Range(0, 10)] public float Radius = 1f;

    public LineMesh3D Eval()
    {
        var poly = new RegularPolygon(Point2D.Zero, Sides);
        return poly.Points.Map(p => p * Radius).To3D().ToLineMesh(true);
    }
}


[Category(Cat.Curves)]
[Description("A single straight line segment from the origin to the given point.")]
public class Line : IGenerator
{
    [Range(0f, 10f)] public float X = 5f;
    [Range(0f, 10f)] public float Y = 5f;
    [Range(0f, 10f)] public float Z = 0f;

    public LineMesh3D Eval()
        => new([Point3D.Default, (X, Y, Z)], [(0, 1)]);
}

// TODO: add some curves