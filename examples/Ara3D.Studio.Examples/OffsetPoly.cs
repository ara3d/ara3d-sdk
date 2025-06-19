namespace Ara3D.Studio.Samples;

public class OffsetPoly : IModelGenerator
{
    [Range(3, 64)] public int Sides { get; set; } = 3;
    [Range(-5, 5)] public float Radius { get; set; } = 2f;
    [Range(-1f, 1f)] public float Offset { get; set; } = 0.1f;

    public Model3D Eval(EvalContext context)
    {
        var poly = new RegularPolygon(Point2D.Zero, Sides);
        var points = poly.Points.Map(p => p.Vector2 * Radius);
        var newPoints = PolygonUtils.OffsetPolygon(points, Offset);

        throw new NotImplementedException("OffsetPoly is not yet implemented.");
    }
}