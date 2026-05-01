namespace Ara3D.Geometry;

public static class GeometryExtensions
{
    public static LineMesh3D Subdivide(this LineMesh3D mesh, int count)
    {
        if (count <= 1)
            return mesh;
        var pts = mesh.Points.ToList();
        var lines = new List<Integer2>();
        foreach (var f in mesh.FaceIndices)
        {
            var pt0 = mesh.Points[f.A];
            var pt1 = mesh.Points[f.B];
            for (var i = 0; i <= count; i++)
            {
                var pt = pt0.Lerp(pt1, (float)i / count);
                var n = pts.Count;
                if (i == 0)
                {
                    pts.Add(pt);
                    lines.Add((f.A, n));
                }
                else if (i == count)
                {
                    lines.Add((n - 1, f.B));
                }
                else
                {
                    pts.Add(pt);
                    lines.Add((n - 1, n));
                }
            }
        }
        return new LineMesh3D(pts, lines);
    }

    public static LineMesh3D ToLineMesh(this Quad3D q)
        => new(q.Points, [(0, 1), (1, 2), (2, 3), (3, 0)]);

    public static LineMesh3D ToLineMesh(this Quad2D q)
        => q.To3D.ToLineMesh();

}