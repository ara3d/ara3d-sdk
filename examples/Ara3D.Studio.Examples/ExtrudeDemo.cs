namespace Ara3D.Studio.Samples;

public class ExtrudeDemo : IModelGenerator
{
    [Range(3, 64)] public int Sides = 3;
    [Range(-20, 20)] public float Height = 5f;
    [Range(-10, 10)] public float Scale = 1f;

    public Model3D Eval(EvalContext eval)
    {
        var poly = new RegularPolygon(Point2D.Zero, Sides);
        var mesh = poly.Extrude(Height);
        return mesh.Triangulate().Scale(Scale);
    }
}