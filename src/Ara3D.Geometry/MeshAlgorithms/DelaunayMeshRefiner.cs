namespace Ara3D.Geometry;

/// <summary>
/// Refines a triangle mesh by inserting interior Steiner points on each oversized face and
/// re-triangulating the face in its own plane with 2D Delaunay triangulation.
/// Face boundaries are never split, so the result is conforming: adjacent faces still share
/// their original edges and the refined surface is geometrically identical to the input.
/// </summary>
public static class DelaunayMeshRefiner
{
    /// <summary>
    /// Refines every face whose longest edge exceeds <paramref name="targetEdgeLength"/> by
    /// inserting a barycentric lattice of interior points at roughly that spacing and
    /// Delaunay-triangulating the face in-plane.
    /// </summary>
    public static TriangleMesh3D DelaunayRefine(this TriangleMesh3D mesh, float targetEdgeLength)
    {
        if (targetEdgeLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetEdgeLength));

        var points = new List<Point3D>(mesh.Points);
        var faces = new List<Integer3>();

        foreach (var face in mesh.FaceIndices)
            RefineFace(points, faces, face, targetEdgeLength);

        return new TriangleMesh3D(points, faces);
    }

    static void RefineFace(List<Point3D> points, List<Integer3> faces, Integer3 face, float target)
    {
        var a = points[face.A].Vector3;
        var b = points[face.B].Vector3;
        var c = points[face.C].Vector3;

        var longest = MathF.Max((b - a).Length(), MathF.Max((c - b).Length(), (a - c).Length()));
        var n = (int)MathF.Ceiling(longest / target);
        if (n < 3)
        {
            faces.Add(face);
            return;
        }

        var normal = Vector3.Cross(b - a, c - a);
        if (normal.LengthSquared() < 1e-12f)
        {
            faces.Add(face);
            return;
        }

        // Strictly interior barycentric lattice points (i + j + k = n, all >= 1).
        var localPoints = new List<Vector3> { a, b, c };
        for (var i = 1; i < n - 1; ++i)
        for (var j = 1; j < n - i; ++j)
        {
            var k = n - i - j;
            localPoints.Add((a * i + b * j + c * k) / n);
        }

        if (localPoints.Count == 3)
        {
            faces.Add(face);
            return;
        }

        var u = (b - a).Normalize;
        var w = normal.Normalize;
        var v = Vector3.Cross(w, u);
        var projected = localPoints
            .Select(p => new Vector2(Vector3.Dot(p - a, u), Vector3.Dot(p - a, v)))
            .ToList();

        var localFaces = DelaunayTriangulator.Triangulate(projected);
        if (localFaces.Count == 0)
        {
            faces.Add(face);
            return;
        }

        var globalIndices = new int[localPoints.Count];
        globalIndices[0] = face.A;
        globalIndices[1] = face.B;
        globalIndices[2] = face.C;
        for (var i = 3; i < localPoints.Count; ++i)
        {
            globalIndices[i] = points.Count;
            points.Add(localPoints[i]);
        }

        foreach (var f in localFaces)
        {
            var fa = projected[f.A];
            var fb = projected[f.B];
            var fc = projected[f.C];
            var ccw = (fb.X - fa.X) * (fc.Y - fa.Y) - (fb.Y - fa.Y) * (fc.X - fa.X) >= 0;
            faces.Add(ccw
                ? new Integer3(globalIndices[f.A], globalIndices[f.B], globalIndices[f.C])
                : new Integer3(globalIndices[f.A], globalIndices[f.C], globalIndices[f.B]));
        }
    }
}
