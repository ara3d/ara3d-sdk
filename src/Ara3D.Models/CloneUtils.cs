using Ara3D.Geometry;

namespace Ara3D.Models;

public static class CloneUtils
{
    public static Model3D Clone(this TriangleMesh3D mesh, Material material, IReadOnlyList<Point3D> points)
        => mesh.Clone(material, points.Map(p => Matrix4x4.CreateTranslation(p.Vector3)));

    public static Model3D Clone(this TriangleMesh3D mesh, Material material, IReadOnlyList<Matrix4x4> transforms)
        => new Model3D([mesh], [material], transforms,
            transforms.Count.MapRange(i => new ElementStruct(i, 0, 0, i)), null);
    
    public static Model3D CloneAlong(this TriangleMesh3D mesh, Func<Number, Point3D> curveFunc, Integer count)
    {
        var transforms = count.LinearSpaceExclusive.Map(curveFunc).Map(p => Matrix4x4.CreateTranslation(p));
        return Clone(mesh, Material.Default, transforms);
    }
}