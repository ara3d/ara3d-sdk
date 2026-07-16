namespace Ara3D.Geometry;

public readonly record struct TopoFace(Topology Topology, FaceId Id)
{
    public Triangle3D Triangle => Topology.GetTriangle(Id);

    public HalfEdgeId Edge0Id => Topology.GetHalfEdgeId(Id, 0);
    public HalfEdgeId Edge1Id => Topology.GetHalfEdgeId(Id, 1);
    public HalfEdgeId Edge2Id => Topology.GetHalfEdgeId(Id, 2);

    public TopoHalfEdge Edge0 => Topology.Get(Edge0Id);
    public TopoHalfEdge Edge1 => Topology.Get(Edge1Id);
    public TopoHalfEdge Edge2 => Topology.Get(Edge2Id);

    public VertexId Vertex0Id => Topology.GetStartVertex(Edge0Id);
    public VertexId Vertex1Id => Topology.GetStartVertex(Edge1Id);
    public VertexId Vertex2Id => Topology.GetStartVertex(Edge2Id);

    public TopoVertex Vertex0 => Topology.Get(Vertex0Id);
    public TopoVertex Vertex1 => Topology.Get(Vertex1Id);
    public TopoVertex Vertex2 => Topology.Get(Vertex2Id);

    public Point3D Point0 => Vertex0.Point;
    public Point3D Point1 => Vertex1.Point;
    public Point3D Point2 => Vertex2.Point;

    public IReadOnlyList<HalfEdgeId> EdgeIds => Topology.GetHalfEdgeIds(Id);
    public bool HasBoundaryEdge => Edge0.IsBoundary || Edge1.IsBoundary || Edge2.IsBoundary;
    public bool IsInterior => !HasBoundaryEdge;
    public Vector3 Normal => Triangle.Normal;
    public float Area => Triangle.Area;
    public Point3D Center => Triangle.Center;

    public HashSet<FaceId> NeighborFaceIds => Topology.GetFaceNeighborIds(Id);
    public IEnumerable<TopoFace> NeighborFaces => Topology.GetFaceNeighbors(Id);
}