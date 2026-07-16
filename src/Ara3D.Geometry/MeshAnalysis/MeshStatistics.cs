using Ara3D.Utils;
using Ara3D.Collections;

namespace Ara3D.Geometry;

public class MeshStatistics
{
    public Topology Topology { get; }
    public MeshAttributes Attributes { get; }
    public TriangleMesh3D Mesh { get; }
    public ScalarStatistics FaceAreaStats { get; }
    public ScalarStatistics DihedralAngleStats { get; }
    public Vector3Statistics FaceNormalByAreaStats { get; }
    public Vector3Statistics FaceNormalByAngleStats { get; }
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
        Topology = new Topology(Mesh);
        Attributes = new MeshAttributes(Mesh, Topology);

        var vectors = Mesh.Points.Select(p => p.Vector3).ToList();
        VertexStats = new Vector3Statistics(vectors);
        Pca = new PrincipalComponentAnalysis(vectors);
        var frame = Pca.Frame;
        OrientedBounds = vectors.FitOrientedBox(frame);
        NormalizedPoints = Mesh.Points.Transform(OrientedBounds.LocalToWorldMatrix());
        NormalizedVertexStats = NormalizedPoints.GetStatistics();

        FaceAreaStats = new ScalarStatistics(Attributes.Faces.Areas);
        FaceNormalByAngleStats = new Vector3Statistics(Attributes.Vertices.AngleWeightedNormals);
        FaceNormalByAreaStats = new Vector3Statistics(Attributes.Vertices.AreaWeightedNormals);

        FaceNormalPca = new PrincipalComponentAnalysis(Attributes.Faces.Normals, Attributes.Faces.Areas);
        DihedralAngleStats = new ScalarStatistics(Attributes.Edges.DihedralAngles);
    }
}