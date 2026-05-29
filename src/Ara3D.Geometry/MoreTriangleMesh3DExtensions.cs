using Ara3D.Collections;

namespace Ara3D.Geometry;

public static class MoreTriangleMesh3DExtensions
{
    public static Triangle3D GetTriangle(this TriangleMesh3D mesh, int faceIndex)
        => mesh.Triangle(mesh.FaceIndices[faceIndex]);

    public static Point3D GetPointA(this TriangleMesh3D mesh, int halfEdgeIndex)
        => mesh.Points[mesh.GetPointAIndex(halfEdgeIndex)];

    public static Point3D GetPointB(this TriangleMesh3D mesh, int halfEdgeIndex)
        => mesh.Points[mesh.GetPointBIndex(halfEdgeIndex)];

    public static int GetPointAIndex(this TriangleMesh3D mesh, int halfEdgeIndex)
        => mesh.FaceIndices[halfEdgeIndex / 3][halfEdgeIndex % 3];

    public static int GetPointBIndex(this TriangleMesh3D mesh, int halfEdgeIndex)
        => mesh.FaceIndices[halfEdgeIndex / 3][(halfEdgeIndex % 3 + 1) % 3];

    public static Line3D GetLine(this TriangleMesh3D mesh, int halfEdgeIndex)
        => (mesh.GetPointA(halfEdgeIndex), mesh.GetPointB(halfEdgeIndex));

    public static IReadOnlyList<Integer2> GetEdgeIndices(this TriangleMesh3D mesh)
        => mesh.FaceIndices.SelectMany(f => (new Integer2(f.A, f.B), new Integer2(f.B, f.C), new Integer2(f.C, f.A)));

    public static Integer2 ToUndirected(this Integer2 self)
        => (Math.Min(self.A, self.B), Math.Max(self.A, self.B));

    public static HashSet<Integer2> GetUndirectedEdgeIndices(this TriangleMesh3D mesh)
        => mesh.GetEdgeIndices().Select(ToUndirected).ToHashSet();

    public static Line3D ToLine(this TriangleMesh3D mesh, Integer2 edge)
        => (mesh.Points[edge.A], mesh.Points[edge.B]);

    public static IEnumerable<Line3D> GetLines(this TriangleMesh3D mesh)
        => mesh.GetUndirectedEdgeIndices().Select(edge => mesh.ToLine(edge));

    public static LineMesh3D ToLineMesh3D(this TriangleMesh3D mesh)
        => (mesh.Points, mesh.GetUndirectedEdgeIndices().ToList());

    public static Vector3 GetFaceNormal(this TriangleMesh3D mesh, int n)
        => mesh.GetTriangle(n).Normal;

    public static TriangleMesh3D GetMeshFromFaces(this TriangleMesh3D mesh, IReadOnlyList<int> faceIds)
        => new TriangleMesh3D(mesh.Points, mesh.FaceIndices.SelectByIndex(faceIds)).RemoveUnreferencedPoints();

    public static IEnumerable<int> ReferencedPointIndices(this TriangleMesh3D mesh)
        => mesh.CornerIndices().Select(i => i.Value).Distinct().OrderBy(i => i);

    public static Dictionary<int, int> ToIndexMap(this IEnumerable<int> oldIndices)
        => oldIndices
            .Select((oldIndex, newIndex) => (oldIndex, newIndex))
            .ToDictionary(x => x.oldIndex, x => x.newIndex);

    public static Integer3 Remap(this Integer3 face, IReadOnlyDictionary<int, int> map)
        => new(
            map[face.A],
            map[face.B],
            map[face.C]);

    public static TriangleMesh3D RemoveUnreferencedPoints(this TriangleMesh3D mesh)
    {
        var used = mesh.ReferencedPointIndices().ToList();
        var map = used.ToIndexMap();
        var points = used.Select(i => mesh.Points[i]);
        var faces = mesh.FaceIndices.Select(f => f.Remap(map));
        return new TriangleMesh3D(points, faces);
    }


}