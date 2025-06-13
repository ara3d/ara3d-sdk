namespace Ara3D.Geometry;

public struct Voxel
{
    public readonly Point3D Position;
    public readonly Number Value;

    public Voxel(Point3D center, Number value)
    {
        Position = center;
        Value = value;
    }
}