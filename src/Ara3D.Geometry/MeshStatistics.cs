using Ara3D.Utils;

namespace Ara3D.Geometry;

public class MeshStatistics
{
    public Topology Topology { get; }
    public TriangleMesh3D Mesh { get; }
    public ScalarStatistics FaceAreaStats { get; }
    public ScalarStatistics DihedralAngleStats { get; }
    public Vector3WeightedStatistics FaceNormalStats { get; }
    public PrincipalComponentAnalysis Pca { get; }
    public OrientedBox3D OrientedBounds { get; }
    public IReadOnlyList<Point3D> NormalizedPoints { get; }
    public Vector3Statistics NormalizedVertexStats { get; }
    public Vector3Statistics VertexStats { get; }
    public Bounds3D Bounds => VertexStats.Bounds;
    public PrincipalComponentAnalysis FaceNormalPca { get; }
    public TopologyFeatureStats TopologyFeatureStats { get; }
    
    public MeshStatistics(TriangleMesh3D mesh, bool welded = true)
    {
        Mesh = welded ? mesh.WeldVertices() : mesh;
        var vectors = Mesh.Points.Select(p => p.Vector3).ToList();
        VertexStats = new Vector3Statistics(vectors);
        Pca = new PrincipalComponentAnalysis(vectors);
        var frame = Pca.Frame;
        OrientedBounds = vectors.FitOrientedBox(frame);
        NormalizedPoints = Mesh.Points.Transform(OrientedBounds.LocalToWorldMatrix());
        NormalizedVertexStats = NormalizedPoints.GetStatistics();
        Topology = new Topology(Mesh);

        var triangles = Mesh.Triangles.ToArray();
        var faceAreas = triangles.Map(t => (double)t.Area.Value);
        var faceNormals = triangles.Map(t => t.Normal);
        var dihedralAngles = Topology.DihedralAngles.Select(a => (double)a.Value).ToList();

        FaceAreaStats = new ScalarStatistics(faceAreas);
        FaceNormalStats = new Vector3WeightedStatistics(faceNormals, faceAreas);
        FaceNormalPca = new PrincipalComponentAnalysis(faceNormals, faceAreas);
        DihedralAngleStats = new ScalarStatistics(dihedralAngles);
        TopologyFeatureStats = Topology.GetFeatureStats(Bounds);
    }
}