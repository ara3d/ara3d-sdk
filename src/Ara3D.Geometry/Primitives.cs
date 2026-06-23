namespace Ara3D.Geometry;

public static class Primitives
{
    public static QuadGrid3D Cylinder(int sides, float radius, float height, int segments)
        => sides.GetCircularPoints(radius).Extrude(height / segments, segments);

    public static QuadGrid3D UpArrow(int sides, float shaftHeight, float shaftWidth, float tipHeight, float tipWidth)
    {
        var totalHeight = shaftHeight + tipHeight;
        var halfOutLine = new Point3D[]
        {
            (0, 0, 0),
            (shaftWidth / 2, 0, 0),
            (shaftWidth / 2, 0, shaftHeight),
            (tipWidth / 2, 0, shaftHeight),
            (0, 0, totalHeight),
        };

        return halfOutLine.Revolve(Vector3.UnitZ, sides);
    }

    public static QuadGrid3D UnitUpArrow(int sides, float shaftWidth, float tipWidth, float tipRatio)
        => UpArrow(sides, 1.0f - tipRatio, shaftWidth, tipRatio, tipWidth);

    public static QuadGrid3D DefaultUpArrow()
        => UnitUpArrow(16, 0.05f, 0.2f, 0.2f);
}