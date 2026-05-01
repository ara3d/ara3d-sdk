namespace Ara3D.Geometry;

public readonly record struct TopoHalfEdge(Topology Topology, HalfEdgeId Id)
{
    public FaceId FaceId => Topology.GetFaceId(Id);
    public TopoFace Face => Topology.Get(FaceId);

    public VertexId StartVertexId => Topology.GetStartVertex(Id);
    public VertexId EndVertexId => Topology.GetEndVertex(Id);

    public TopoVertex StartVertex => Topology.Get(StartVertexId);
    public TopoVertex EndVertex => Topology.Get(EndVertexId);

    public HalfEdgeId NextId => Topology.GetNext(Id);
    public HalfEdgeId PreviousId => Topology.GetPrevious(Id);

    public TopoHalfEdge Next => Topology.Get(NextId);
    public TopoHalfEdge Previous => Topology.Get(PreviousId);

    public bool HasTwin => Topology.HasTwin(Id);
    public bool IsBoundary => Topology.IsBoundary(Id);

    public HalfEdgeId TwinId => Topology.Twin(Id);
    public TopoHalfEdge Twin => Topology.Get(TwinId);

    public UndirectedEdgeId EdgeId => Topology.GetUndirectedEdge(Id);
    public TopoEdge Edge => Topology.Get(EdgeId);

    public Line3D Line => Topology.GetLine(Id);
    public Point3D StartPoint => Topology.GetPoint(StartVertexId);
    public Point3D EndPoint => Topology.GetPoint(EndVertexId);

    public Vector3 Vector => EndPoint.Vector3 - StartPoint.Vector3;
    public float Length => Vector.Length();

    public Angle CornerAngle => Topology.GetCornerAngle(Id);
    public Angle DihedralAngle => Topology.GetDihedralAngle(Id);
}