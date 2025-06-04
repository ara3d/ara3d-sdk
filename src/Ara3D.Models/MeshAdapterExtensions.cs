using Ara3D.Memory;
using Ara3D.Geometry;

namespace Ara3D.Models;

public static class MeshAdapterExtensions
{
    public static TriangleMesh3D ToMesh(this IMemoryOwner points, IMemoryOwner indices)
        => new MeshAdapter(points, indices).Mesh;

    public static TriangleMesh3D ToMesh(this Vector3[] points)
        => ToMesh(points, Enumerable.Range(0, points.Length).ToArray());

    public static TriangleMesh3D ToMesh(this Point3D[] points)
        => ToMesh(points, Enumerable.Range(0, points.Length).ToArray());

    public static TriangleMesh3D ToMesh(this float[] points)
        => ToMesh(points, Enumerable.Range(0, points.Length / 3).ToArray());

    public static TriangleMesh3D ToMesh(this float[] points, int[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Vector3[] points, int[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Point3D[] points, int[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this float[] points, uint[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Vector3[] points, uint[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Point3D[] points, uint[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this float[] points, Integer3[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Vector3[] points, Integer3[] indices)
        => ToMesh(points.Fix(), indices.Fix());

    public static TriangleMesh3D ToMesh(this Point3D[] points, Integer3[] indices)
        => ToMesh(points.Fix(), indices.Fix());
}