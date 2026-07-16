namespace Ara3D.Geometry;

public readonly record struct TopoEdge(Topology Topology, UndirectedEdgeId Id)
{
    public Line3D Line => Topology.GetLine(Id);

    public bool IsBoundary => Topology.IsBoundary(Id);
    public bool IsManifold => Topology.IsManifold(Id);
    public bool IsNonManifold => Topology.IsNonManifold(Id);
    public IReadOnlyList<HalfEdgeId> HalfEdges => Topology.GetHalfEdgeIds(Id);
    public HalfEdgeId FirstHalfEdge => Topology.GetFirstHalfEdge(Id);

    public VertexId A => Topology.GetVertexA(Id);
    public VertexId B => Topology.GetVertexB(Id);

    public Point3D PointA => Topology.GetPointA(Id);
    public Point3D PointB => Topology.GetPointB(Id);

    public Angle? Angle => Topology.GetAngle(FirstHalfEdge);
}