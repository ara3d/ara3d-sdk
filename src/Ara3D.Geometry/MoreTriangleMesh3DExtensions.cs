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
}