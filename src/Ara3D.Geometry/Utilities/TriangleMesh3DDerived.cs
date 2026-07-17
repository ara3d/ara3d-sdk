namespace Ara3D.Geometry;

/// <summary>
/// On-demand, cached derived data for triangle meshes (tracker studio-045). Results are memoized
/// in <see cref="DerivedDataCache.Default"/>: the owner key is the mesh's point list, with the face-index list
/// as a secondary dependency for face-dependent values. Copies of the same mesh (which share the
/// same component lists) return the same cached instance; a mesh built with new components starts
/// fresh, and cached values are collected with the point list.
/// </summary>
public static class TriangleMesh3DDerived
{
    public static Bounds3D DerivedBounds(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, nameof(DerivedBounds), () => mesh.Bounds);

    public static Vector3[] DerivedVertexNormals(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedVertexNormals), mesh.FaceIndices),
            () => mesh.VertexNormals());

    public static Vector3[] DerivedFaceNormals(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedFaceNormals), mesh.FaceIndices),
            () => ComputeFaceNormals(mesh));

    public static AabbTree DerivedAabbTree(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedAabbTree), mesh.FaceIndices),
            () => ComputeAabbTree(mesh));

    public static Topology DerivedTopology(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedTopology), mesh.FaceIndices),
            () => new Topology(mesh));

    public static MeshAttributes DerivedAttributes(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedAttributes), mesh.FaceIndices),
            () => new MeshAttributes(mesh, mesh.DerivedTopology()));

    public static MeshStatistics DerivedStatistics(this TriangleMesh3D mesh)
        => DerivedDataCache.Default.GetOrCompute(mesh.Points, new DerivedKey(nameof(DerivedStatistics), mesh.FaceIndices),
            () => new MeshStatistics(mesh));

    private static Vector3[] ComputeFaceNormals(TriangleMesh3D mesh)
    {
        var triangles = mesh.Triangles;
        var normals = new Vector3[triangles.Count];
        for (var i = 0; i < normals.Length; i++)
            normals[i] = triangles[i].Normal;
        return normals;
    }

    private static AabbTree ComputeAabbTree(TriangleMesh3D mesh)
    {
        var triangles = mesh.Triangles;
        var bounds = new Bounds3D[triangles.Count];
        for (var i = 0; i < bounds.Length; i++)
            bounds[i] = triangles[i].Bounds();
        return new AabbTree(bounds);
    }
}
