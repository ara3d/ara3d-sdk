using Ara3D.Collections;

namespace Ara3D.Geometry;

public readonly record struct TopoVertex(Topology Topology, VertexId Id)
{
    public Point3D Point => Topology.GetPoint(Id);
    public int Valence => Topology.Valence(Id);
    public bool IsBoundary => Topology.IsBoundary(Id);
    public bool IsInterior => !IsBoundary;
    public IReadOnlyList<HalfEdgeId> OutgoingHalfEdgeIds => Topology.GetOutgoingHalfEdgeIds(Id);
    public IReadOnlyList<HalfEdgeId> IncomingHalfEdgeIds => Topology.GetIncomingHalfEdgeIds(Id);
    public IReadOnlyList<FaceId> FaceIds => Topology.GetFaceIds(Id);
    public IReadOnlyList<TopoHalfEdge> OutgoingHalfEdges => OutgoingHalfEdgeIds.Select(Topology.Get);
    public IReadOnlyList<TopoHalfEdge> IncomingHalfEdges => IncomingHalfEdgeIds.Select(Topology.Get);
    public IReadOnlyList<TopoFace> Faces => FaceIds.Select(Topology.Get);
    public IEnumerable<Point3D> NeighborPoints => NeighborIds.Select(Topology.GetPoint);
    public Angle AngleSum => Topology.GetAngleSum(Id);
    public Vector3 UniformNormal => Topology.GetUniformVertexNormal(Id);
    public Vector3 AreaWeightedNormal => Topology.GetAreaWeightedVertexNormal(Id);
    public Vector3 AngleWeightedNormal => Topology.GetAngleWeightedVertexNormal(Id);
    public HashSet<VertexId> NeighborIds => Topology.GetNeighborVertexIds(Id);
    public IEnumerable<TopoVertex> Neighbors  => NeighborIds.Select(Topology.Get);
    public float Curvature => Topology.GetCurvature(Id);
}