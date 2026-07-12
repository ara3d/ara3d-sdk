using Ara3D.Geometry;

namespace Ara3D.IfcMeshingComparison.Harness.GeometryOracles;

/// <summary>Watertight / open-edge / non-manifold topology checks.</summary>
public static class TopologyOracle
{
    public static int CountOpenEdges(TriangleMesh3D mesh)
    {
        var counts = new Dictionary<(int, int), int>();
        void Add(int a, int b)
        {
            var key = a < b ? (a, b) : (b, a);
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }
        foreach (var f in mesh.FaceIndices)
        {
            Add(f.A, f.B);
            Add(f.B, f.C);
            Add(f.C, f.A);
        }
        return counts.Count(kv => kv.Value != 2);
    }

    public static bool IsWatertight(TriangleMesh3D mesh)
        => mesh.FaceIndices.Count > 0 && new Topology(mesh).IsWatertight;

    public static bool HasNonManifoldEdges(TriangleMesh3D mesh)
        => mesh.FaceIndices.Count > 0 && new Topology(mesh).HasNonManifoldEdges;

    public static bool MeetsOpenEdgeBudget(TriangleMesh3D mesh, int maxOpenEdges)
        => CountOpenEdges(mesh) <= maxOpenEdges;
}
