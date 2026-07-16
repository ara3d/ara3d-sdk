namespace Ara3D.Geometry;

// https://iquilezles.org/articles/distfunctions/
public static class SdfExtensions
{

    public static Sdf3D Union(this Sdf3D a, Sdf3D b)
        => new(p => Math.Min(a.Eval(p), b.Eval(p)));

    public static Sdf3D Subtract(this Sdf3D a, Sdf3D b)
        => new(p => Math.Max(-a.Eval(p), b.Eval(p)));

    public static Sdf3D Intersection(this Sdf3D a, Sdf3D b)
        => new(p => Math.Max(a.Eval(p), b.Eval(p)));

    public static Number SdfXOr(this Number d1, Number d2)
    {
        var sameSide = Math.Sign(d1) == Math.Sign(d2); // both inside or both outside
        var m1 = Math.Abs(d1);
        var m2 = Math.Abs(d2);
        return sameSide
            ? Math.Max(m1, m2) // outside XOR region
            : -Math.Min(m1, m2); // inside exactly one shape
    }

    public static Sdf3D XOr(this Sdf3D a, Sdf3D b)
        => new(p => SdfXOr(a.Eval(p), b.Eval(p)));

    public static Sdf3D Onion(this Sdf3D self, Number thickness)
        => new(p => Math.Abs(self.Eval(p)) - thickness);

    public static Sdf3D Round(this Sdf3D self, Number rad)
        => new(p => self.Eval(p) - rad);

    // NOTE: see discussion about 2D and 3D elongations
    // https://iquilezles.org/articles/distfunctions/
    public static Sdf3D Elongate(this Sdf3D self, Vector3 h)
        => new(p => self.Eval(p - p.Vector3.Clamp(-h, h)));

    public static VoxelizedField Voxelize(this Sdf3D self, Bounds3D bounds, Integer3 gridSize)
        => new(bounds, gridSize, self.Eval);
}