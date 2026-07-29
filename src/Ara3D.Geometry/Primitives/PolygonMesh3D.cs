namespace Ara3D.Geometry;

/// <summary>
/// Indexed polygon mesh in CSR form (matches Plato <c>PolygonMesh3D</c>):
/// face <c>f</c> uses <c>FaceVertices[FaceOffsets[f] .. FaceOffsets[f+1])</c>.
/// </summary>
public readonly struct PolygonMesh3D
{
    public readonly IReadOnlyList<Point3D> Points;
    public readonly IReadOnlyList<int> FaceOffsets;
    public readonly IReadOnlyList<int> FaceVertices;

    public PolygonMesh3D(
        IReadOnlyList<Point3D> points,
        IReadOnlyList<int> faceOffsets,
        IReadOnlyList<int> faceVertices)
    {
        Points = points;
        FaceOffsets = faceOffsets;
        FaceVertices = faceVertices;
    }

    public int FaceCount
        => FaceOffsets.Count - 1;

    public int FaceArity(int face)
        => FaceOffsets[face + 1] - FaceOffsets[face];

    public int FaceVertex(int face, int corner)
        => FaceVertices[FaceOffsets[face] + corner];
}

public static class PolygonMesh3DExtensions
{
    public static PolygonMesh3D ToPolygonMesh(
        this IReadOnlyList<Point3D> points,
        IReadOnlyList<IReadOnlyList<int>> faces)
    {
        var offsets = new int[faces.Count + 1];
        var total = 0;
        for (var f = 0; f < faces.Count; ++f)
        {
            offsets[f] = total;
            total += faces[f].Count;
        }
        offsets[faces.Count] = total;

        var verts = new int[total];
        var cursor = 0;
        for (var f = 0; f < faces.Count; ++f)
        {
            var face = faces[f];
            for (var i = 0; i < face.Count; ++i)
                verts[cursor++] = face[i];
        }

        return new PolygonMesh3D(points, offsets, verts);
    }

    public static IReadOnlyList<int> FaceVerticesOf(this PolygonMesh3D mesh, int face)
    {
        var start = mesh.FaceOffsets[face];
        var count = mesh.FaceArity(face);
        var result = new int[count];
        for (var i = 0; i < count; ++i)
            result[i] = mesh.FaceVertices[start + i];
        return result;
    }

    public static Point3D FaceCentroid(this PolygonMesh3D mesh, int face)
    {
        var start = mesh.FaceOffsets[face];
        var n = mesh.FaceArity(face);
        var sum = Vector3.Zero;
        for (var i = 0; i < n; ++i)
            sum += mesh.Points[mesh.FaceVertices[start + i]].Vector3;
        return (Point3D)(sum / n);
    }

    public static PolygonMesh3D WithNormalizedPoints(this PolygonMesh3D mesh)
        => new(mesh.Points.Map(p => (Point3D)p.Vector3.Normalize), mesh.FaceOffsets, mesh.FaceVertices);

    public static PolygonMesh3D ScalePoints(this PolygonMesh3D mesh, float radius)
        => new(mesh.Points.Map(p => (Point3D)(p.Vector3 * radius)), mesh.FaceOffsets, mesh.FaceVertices);

    /// <summary>Fan-triangulate convex faces (A, B, C), (A, C, D), …</summary>
    public static TriangleMesh3D ToTriangleMesh(this PolygonMesh3D mesh)
    {
        var triCount = 0;
        for (var f = 0; f < mesh.FaceCount; ++f)
            triCount += mesh.FaceArity(f) - 2;

        var tris = new Integer3[triCount];
        var t = 0;
        for (var f = 0; f < mesh.FaceCount; ++f)
        {
            var start = mesh.FaceOffsets[f];
            var n = mesh.FaceArity(f);
            var a = mesh.FaceVertices[start];
            for (var i = 1; i + 1 < n; ++i)
            {
                tris[t++] = (
                    a,
                    mesh.FaceVertices[start + i],
                    mesh.FaceVertices[start + i + 1]);
            }
        }

        return new TriangleMesh3D(mesh.Points, tris);
    }

    public static PolygonMesh3D ToPolygonMesh(this TriangleMesh3D mesh)
        => mesh.Points.ToPolygonMesh(
            mesh.FaceIndices.Map(f => (IReadOnlyList<int>)new[] { f.A.Value, f.B.Value, f.C.Value }));

    public static PolygonMesh3D ToPolygonMesh(this QuadMesh3D mesh)
        => mesh.Points.ToPolygonMesh(
            mesh.FaceIndices.Map(f => (IReadOnlyList<int>)new[] { f.A.Value, f.B.Value, f.C.Value, f.D.Value }));
}
