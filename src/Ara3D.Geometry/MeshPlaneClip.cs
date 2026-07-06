namespace Ara3D.Geometry;

/// <summary>Clips triangle meshes against planes and polygons.</summary>
public static class MeshPlaneClip
{
    public const float DefaultEpsilon = 1e-5f;

    /// <summary>Keeps geometry on or below the plane (DotCoordinate &lt;= epsilon).</summary>
    public static TriangleMesh3D ClipBelow(this TriangleMesh3D mesh, Plane plane, float eps = DefaultEpsilon)
        => ClipMesh(mesh, plane, eps, keepBelow: true);

    /// <summary>Keeps geometry on or above the plane (DotCoordinate &gt;= -epsilon).</summary>
    public static TriangleMesh3D ClipAbove(this TriangleMesh3D mesh, Plane plane, float eps = DefaultEpsilon)
        => ClipMesh(mesh, plane, eps, keepBelow: false);

    static TriangleMesh3D ClipMesh(TriangleMesh3D mesh, Plane plane, float eps, bool keepBelow)
    {
        var builder = new TriangleMesh3DBuilder();

        foreach (var face in mesh.FaceIndices)
        {
            foreach (var tri in ClipTriangle(mesh.Triangle(face), plane, eps, keepBelow))
            {
                var i0 = builder.Points.Count;
                builder.Points.Add(tri.A);
                var i1 = builder.Points.Count;
                builder.Points.Add(tri.B);
                var i2 = builder.Points.Count;
                builder.Points.Add(tri.C);
                builder.Faces.Add((i0, i1, i2));
            }
        }

        return builder.ToTriangleMesh3D();
    }

    public static List<Triangle3D> ClipTriangle(this Triangle3D tri, Plane plane, float eps, bool keepBelow)
    {
        var polygon = ClipPolygon([tri.A, tri.B, tri.C], plane, eps, keepBelow);
        return FanTriangulate(polygon);
    }

    public static List<Point3D> ClipPolygon(this IReadOnlyList<Point3D> polygon, Plane plane, float eps, bool keepBelow)
    {
        if (polygon.Count < 3)
            return [];

        var output = new List<Point3D>();

        for (var i = 0; i < polygon.Count; i++)
        {
            var start = polygon[i];
            var end = polygon[(i + 1) % polygon.Count];
            var ds = plane.DotCoordinate(start);
            var de = plane.DotCoordinate(end);

            if (keepBelow ? ds <= eps : ds >= -eps)
                output.Add(start);

            if (ds * de < 0)
                output.Add(IntersectEdge(start, ds, end, de));
        }

        return output;
    }

    public static Point3D IntersectEdge(Point3D a, float da, Point3D b, float db)
    {
        var t = da / (da - db);
        return a.Lerp(b, t);
    }

    public static List<Triangle3D> FanTriangulate(this IReadOnlyList<Point3D> polygon)
    {
        if (polygon.Count < 3)
            return [];

        if (polygon.Count == 3)
            return [new Triangle3D(polygon[0], polygon[1], polygon[2])];

        var triangles = new List<Triangle3D>();
        var v0 = polygon[0];
        for (var i = 1; i < polygon.Count - 1; i++)
            triangles.Add(new Triangle3D(v0, polygon[i], polygon[i + 1]));

        return triangles;
    }
}
