
namespace Ara3D.Studio.Samples
{
    public class Topology
    {
        public TopoVertex Get(VertexId id) => new(this, id);
        public TopoFace Get(FaceId id) => new(this, id);
        public TopoDirectedEdge Get(DirectedEdgeId id) => new(this, id);
        public TopoUndirectedEdge Get(UndirectedEdgeId id) => new(this, id);
    }

    public enum VertexId : int;

    public enum DirectedEdgeId : int;

    public enum UndirectedEdgeId : int;

    public enum FaceId : int;

    public readonly record struct TopoVertex(Topology Topology, VertexId Id);

    public readonly record struct TopoDirectedEdge(Topology Topology, DirectedEdgeId Id);

    public readonly record struct TopoUndirectedEdge(Topology Topology, UndirectedEdgeId Id);
    
    public readonly record struct TopoFace(Topology Topology, FaceId Id);

    public record TopoVertexSet(Topology Topology, HashSet<VertexId> Id);

    public record TopoDirectedEdgeSet(Topology Topology, HashSet<DirectedEdgeId> Id);

    public record TopoUndirectedEdgeSet(Topology Topology, HashSet<UndirectedEdgeId> Id);

    public record TopoFaceSet(Topology Topology, HashSet<FaceId> Id);
}
