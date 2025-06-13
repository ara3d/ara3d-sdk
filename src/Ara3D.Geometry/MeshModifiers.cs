using Ara3D.Collections;

namespace Ara3D.Geometry;

public static class MeshModifiers
{
    public static TriangleMesh3D Triangulate(this QuadMesh3D mesh)
        => new(mesh.Points, mesh.FaceIndices.QuadFacesToTriFaces());

    public static TriangleMesh3D FlipFaces(this TriangleMesh3D mesh)
        => new(mesh.Points, mesh.FaceIndices.Map(f => new Integer3(f.C, f.B, f.A)));

    public static TriangleMesh3D AddFaces(this TriangleMesh3D mesh, IReadOnlyList<Integer3> indices)
        => mesh.WithFaceIndices(mesh.FaceIndices.Concat(indices));

    public static TriangleMesh3D DoubleSided(this TriangleMesh3D mesh)
        => mesh.AddFaces(mesh.FlipFaces().FaceIndices);

    public static Vector3[] VertexNormals(this TriangleMesh3D mesh)
    {
        var numTriangles = mesh.Triangles.Count;
        var normals = new Vector3[mesh.Points.Count];
        for (var i=0; i < numTriangles; i++)
        {
            var face = mesh.FaceIndices[i];
            var tri = mesh.Triangles[i];
            var normal = tri.Normal;
            normals[face.A] += normal;
            normals[face.B] += normal;
            normals[face.C] += normal;
        }
        for (var i = 0; i < normals.Length; i++)
        {
            normals[i] = normals[i].Normalize;
        }
        return normals;
    }

    public static Point3D Apply<T>(this T transform, Point3D point) where T: ITransform3D
        => point.Vector3.Transform(transform.Matrix);

    public static IReadOnlyList<Point3D> Transform<T>(this IReadOnlyList<Point3D> points, T transform) where T: ITransform3D
        => points.Map(p => transform.Apply(p));

    public static IReadOnlyList<Point3D> Transform<T>(this IReadOnlyList<Point3D> points, IReadOnlyList<T> transforms) where T : ITransform3D
        => transforms.Zip(points, Apply);

    public static Translation3D ToTranslation3D(this Vector3 vector)
        => new(vector);

    public static IReadOnlyList<Translation3D> Translations(this IReadOnlyList<Vector3> vectors)
        => vectors.Map(ToTranslation3D);

    public static IReadOnlyList<Point3D> Translate(this IReadOnlyList<Point3D> points, Vector3 vector)
        => points.Transform(vector.ToTranslation3D());

    public static IReadOnlyList<Point3D> Translate(this IReadOnlyList<Point3D> points, IReadOnlyList<Vector3> vectors)
        => points.Transform(vectors.Translations());

    public static TriangleMesh3D PushVertices(this TriangleMesh3D mesh, Number amount)
        => mesh.WithPoints(mesh.Points.Translate(mesh.VertexNormals().Map(n => n * amount)));
}