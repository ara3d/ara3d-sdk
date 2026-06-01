namespace Ara3D.Geometry;

public readonly struct Sphere
{
    public Point3D Center { get; }
    public float Radius { get; }
    public Sphere(Point3D center, float radius)
    {
        Center = center;
        Radius = radius;
    }
}