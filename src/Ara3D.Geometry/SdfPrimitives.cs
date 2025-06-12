namespace Ara3D.Geometry;

public static class SdfPrimitives
{
    public static Number AbsDist(this Number a, Number b)
        => (a - b).Abs;

    public static Sdf3D Sphere(Number radius)
        => new(p => p.Vector3.Length - radius);

    public static Sdf3D XAxis(Number radius)
        => new(p => p.Vector3.YZ.Length - radius);
    
    public static Sdf3D YAxis(Number radius)
        => new(p => p.Vector3.XZ.Length - radius);

    public static Sdf3D ZAxis(Number radius)
        => new(p => p.Vector3.XY.Length - radius);

    public static Sdf3D AllAxis(Number radius)
        => new(p => 
            Math.Min(
                Math.Min(p.Vector3.YZ.Length - radius, p.Vector3.XZ.Length - radius),
                p.Vector3.XY.Length - radius));
}