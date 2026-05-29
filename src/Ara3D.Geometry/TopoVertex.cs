using Ara3D.Collections;
using Ara3D.Utils;

namespace Ara3D.Geometry;

public readonly record struct TopoVertex(Topology Topology, VertexId Id)
{
    public Point3D Point => Topology.GetPoint(Id);
    public int Valence => Topology.Valence(Id);
    public bool IsBoundary => Topology.IsBoundary(Id);
    public bool IsInterior => !IsBoundary;
    public IReadOnlyList<HalfEdgeId> OutgoingHalfEdgeIds => Topology.GetOutgoingHalfEdgeIds(Id);
    public IReadOnlyList<HalfEdgeId> IncomingHalfEdgeIds => Topology.GetIncomingHalfEdgeIds(Id);
    public IReadOnlyList<FaceId> FaceIds => Topology.GetIncidentFaceIds(Id);
    public IReadOnlyList<TopoHalfEdge> OutgoingHalfEdges => OutgoingHalfEdgeIds.Select(Topology.Get);
    public IReadOnlyList<TopoHalfEdge> IncomingHalfEdges => IncomingHalfEdgeIds.Select(Topology.Get);
    public IReadOnlyList<TopoFace> Faces => FaceIds.Select(Topology.Get);
    public IEnumerable<Point3D> NeighborPoints => NeighborIds.Select(Topology.GetPoint);
    public HashSet<VertexId> NeighborIds => Topology.GetNeighborVertexIds(Id);
    public IEnumerable<TopoVertex> Neighbors  => NeighborIds.Select(Topology.Get);
    public IReadOnlyList<UndirectedEdgeId> EdgeIds => Topology.GetIncidentUndirectedEdgeIds(Id);
    public IReadOnlyList<TopoEdge> Edges => EdgeIds.Select(Topology.Get);
    public IEnumerable<Angle> EdgeAngles => Edges.Select(e => e.Angle).WhereHasValue();
}
