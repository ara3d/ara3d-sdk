
using Ara3D.Utils;
using Ara3D.Collections;
using System.Diagnostics;

namespace Ara3D.Geometry;

/// <summary>
/// One important note: for boundary loops, the simple TryGetNextBoundaryHalfEdge works well if the mesh is consistently wound.
/// For dirty imported CAD/BIM meshes, I would eventually replace it with a more robust vertex-local angular ordering step,
/// especially if you expect non-manifold boundaries or disconnected shell
/// </summary>
public class Topology
{
    public readonly record struct DirectedEdgeKey(int From, int To);
    
    public readonly record struct UndirectedEdgeKey
    {
        public int A { get; }
        public int B { get; }

        public UndirectedEdgeKey(int a, int b)
        {
            A = Math.Min(a, b);
            B = Math.Max(a, b);
        }
    }

    public TriangleMesh3D Mesh { get; }

    public TopoVertex Get(VertexId id) => new(this, id);
    public TopoFace Get(FaceId id) => new(this, id);
    public TopoHalfEdge Get(HalfEdgeId id) => new(this, id);

    public Point3D GetPoint(VertexId id) => Mesh.Points[(int)id];
    public Line3D GetLine(HalfEdgeId id) => Mesh.GetLine((int)id); 
    public Triangle3D GetTriangle(FaceId id) => Mesh.GetTriangle((int)id);

    private readonly Csr<HalfEdgeId> _outgoingHalfEdgesByVertex;
    private readonly Csr<HalfEdgeId> _incomingHalfEdgesByVertex;
    private readonly Csr<FaceId> _facesByVertex;
    private readonly Csr<HalfEdgeId> _halfEdgesByUndirectedEdge;

    private readonly UndirectedEdgeId[] _halfEdgeToUndirectedEdge;
    private readonly HalfEdgeId[] _twinHalfEdge;

    public IReadOnlyList<HalfEdgeId> GetHalfEdgeIds() => HalfEdgeCount.MapRange(i => (HalfEdgeId)i.Value);
    public IReadOnlyList<UndirectedEdgeId> GetUndirectedEdgeIds() => EdgeCount.MapRange(i => (UndirectedEdgeId)i.Value);
    public IReadOnlyList<FaceId> GetFaceIds() => FaceCount.MapRange(i => (FaceId)i.Value);
    public IReadOnlyList<VertexId> GetVertexIds() => VertexCount.MapRange(i => (VertexId)i.Value);

    public IEnumerable<HalfEdgeId> GetBoundaryHalfEdges() => GetHalfEdgeIds().Where(IsBoundary);
    public IEnumerable<VertexId> GetBoundaryVertices() => GetVertexIds().Where(IsBoundary);

    private readonly Dictionary<DirectedEdgeKey, int> _directedEdgeLookup;
    private readonly Dictionary<UndirectedEdgeKey, int> _undirectedEdgeLookup;

    public int VertexCount => Mesh.Points.Count;
    public int FaceCount => Mesh.FaceIndices.Count;
    public int HalfEdgeCount => FaceCount * 3;
    public int EdgeCount { get; }

    public Topology(TriangleMesh3D mesh)
    {
        Mesh = mesh;

        _directedEdgeLookup = new Dictionary<DirectedEdgeKey, int>(HalfEdgeCount);
        _undirectedEdgeLookup = new Dictionary<UndirectedEdgeKey, int>(HalfEdgeCount);

        _halfEdgeToUndirectedEdge = new UndirectedEdgeId[HalfEdgeCount];
        _twinHalfEdge = new HalfEdgeId[HalfEdgeCount];
        Array.Fill(_twinHalfEdge, (HalfEdgeId)(-1));

        for (var he = 0; he < HalfEdgeCount; he++)
        {
            var a = GetStartVertexIndex(he);
            var b = GetEndVertexIndex(he);

            var directed = new DirectedEdgeKey(a, b);
            var undirected = new UndirectedEdgeKey(a, b);

            // Assumes at most one directed half-edge A -> B.
            // If duplicates are possible, replace this with a CSR lookup too.
            _directedEdgeLookup.TryAdd(directed, he);

            if (!_undirectedEdgeLookup.TryGetValue(undirected, out var undirectedId))
            {
                undirectedId = _undirectedEdgeLookup.Count;
                _undirectedEdgeLookup.Add(undirected, undirectedId);
            }

            _halfEdgeToUndirectedEdge[he] = (UndirectedEdgeId)undirectedId;
        }

        EdgeCount = _undirectedEdgeLookup.Count;
        
        for (var he = 0; he < HalfEdgeCount; he++)
        {
            var a = GetStartVertexIndex(he);
            var b = GetEndVertexIndex(he);

            if (_directedEdgeLookup.TryGetValue(new DirectedEdgeKey(b, a), out var twin))
                _twinHalfEdge[he] = (HalfEdgeId)twin;
        }

        _outgoingHalfEdgesByVertex = Csr<HalfEdgeId>.Build(
            rowCount: VertexCount,
            itemCount: HalfEdgeCount,
            getRow: GetStartVertexIndex,
            getValue: he => (HalfEdgeId)he);

        _incomingHalfEdgesByVertex = Csr<HalfEdgeId>.Build(
            rowCount: VertexCount,
            itemCount: HalfEdgeCount,
            getRow: GetEndVertexIndex,
            getValue: he => (HalfEdgeId)he);

        _facesByVertex = Csr<FaceId>.Build(
            rowCount: VertexCount,
            itemCount: HalfEdgeCount,
            getRow: GetStartVertexIndex,
            getValue: he => (FaceId)(he / 3),
            distinctPerRow: true);

        _halfEdgesByUndirectedEdge = Csr<HalfEdgeId>.Build(
            rowCount: EdgeCount,
            itemCount: HalfEdgeCount,
            getRow: he => (int)_halfEdgeToUndirectedEdge[he],
            getValue: he => (HalfEdgeId)he);

        for (var e = 0; e < EdgeCount; e++)
        {
            Debug.Assert(
                _halfEdgesByUndirectedEdge[e].Count > 0,
                $"Undirected edge {e} has no half-edges.");
        }
    }

    public FaceId GetAssociatedFaceId(HalfEdgeId id) 
        => (FaceId)((int)id / 3);

    public HalfEdgeId GetHalfEdgeId(FaceId face, int localEdge)
        => (HalfEdgeId)((int)face * 3 + localEdge);

    public int GetLocalEdgeIndex(HalfEdgeId id) 
        => (int)id % 3;

    public HalfEdgeId GetNext(HalfEdgeId id)
    {
        var f = (int)id / 3;
        var e = (int)id % 3;
        return (HalfEdgeId)(f * 3 + (e + 1) % 3);
    }

    public HalfEdgeId GetPrevious(HalfEdgeId id)
    {
        var f = (int)id / 3;
        var e = (int)id % 3;
        return (HalfEdgeId)(f * 3 + (e + 2) % 3);
    }

    public bool HasTwin(HalfEdgeId id) 
        => _twinHalfEdge[(int)id] >= 0;

    public HalfEdgeId Twin(HalfEdgeId id)
    {
        var twin = _twinHalfEdge[(int)id];
        if (twin < 0)
            throw new InvalidOperationException("Boundary half-edge has no twin.");

        return twin;
    }

    public bool TryGetTwin(HalfEdgeId id, out HalfEdgeId twin)
    {
        var value = _twinHalfEdge[(int)id];
        twin = value;
        return value >= 0;
    }

    public bool IsBoundary(HalfEdgeId id) 
        => !HasTwin(id);

    public bool IsBoundary(VertexId id)
        => GetOutgoingHalfEdgeIds(id).Any(IsBoundary);

    public VertexId GetStartVertex(HalfEdgeId id)
        => (VertexId)GetStartVertexIndex((int)id);

    public VertexId GetEndVertex(HalfEdgeId id)
        => (VertexId)GetEndVertexIndex((int)id);

    public IReadOnlyList<HalfEdgeId> GetOutgoingHalfEdgeIds(VertexId id)
        => _outgoingHalfEdgesByVertex[(int)id];

    public IReadOnlyList<HalfEdgeId> GetIncomingHalfEdgeIds(VertexId id)
        => _incomingHalfEdgesByVertex[(int)id];

    public IReadOnlyList<HalfEdgeId> GetIncidentHalfEdgeIds(VertexId id)
        => GetOutgoingHalfEdgeIds(id).Concat(GetIncomingHalfEdgeIds(id));

    public IReadOnlyList<UndirectedEdgeId> GetIncidentUndirectedEdgeIds(VertexId id)
        => GetIncidentHalfEdgeIds(id).Select(GetUndirectedEdge).Distinct().ToArray();

    public IReadOnlyList<FaceId> GetIncidentFaceIds(VertexId id)
        => _facesByVertex[(int)id];

    public HalfEdgeId GetFaceHalfEdge(FaceId face, int local)
        => GetHalfEdgeId(face, local);

    public IReadOnlyList<HalfEdgeId> GetHalfEdgeIds(FaceId id)
        => [GetFaceHalfEdge(id, 0), GetFaceHalfEdge(id, 1), GetFaceHalfEdge(id, 2)];

    public IReadOnlyList<HalfEdgeId> GetSiblingHalfEdgeIds(HalfEdgeId id)
    {
        var undirectedId = _halfEdgeToUndirectedEdge[(int)id];
        return _halfEdgesByUndirectedEdge[(int)undirectedId];
    }

    public bool TryFindHalfEdge(VertexId from, VertexId to, out HalfEdgeId id)
    {
        if (_directedEdgeLookup.TryGetValue(new DirectedEdgeKey((int)from, (int)to), out var he))
        {
            id = (HalfEdgeId)he;
            return true;
        }

        id = default;
        return false;
    }

    public int GetStartVertexIndex(int halfEdgeIndex)
    {
        var faceIndex = halfEdgeIndex / 3;
        var local = halfEdgeIndex % 3;
        return Mesh.FaceIndices[faceIndex][local];
    }

    public int GetEndVertexIndex(int halfEdgeIndex)
    {
        var faceIndex = halfEdgeIndex / 3;
        var local = halfEdgeIndex % 3;
        return Mesh.FaceIndices[faceIndex][(local + 1) % 3];
    }

    public int Valence(VertexId id) 
        => GetOutgoingHalfEdgeIds(id).Count;

    public bool IsBoundary(UndirectedEdgeId id)
        => _halfEdgesByUndirectedEdge[(int)id].Count == 1;

    public bool IsManifold(UndirectedEdgeId id)
        => _halfEdgesByUndirectedEdge[(int)id].Count <= 2;

    public bool IsNonManifold(UndirectedEdgeId id)
        => _halfEdgesByUndirectedEdge[(int)id].Count > 2;

    public bool IsWatertight
        => Enumerable.Range(0, EdgeCount).All(e => _halfEdgesByUndirectedEdge[e].Count == 2);

    public bool HasBoundary
        => Enumerable.Range(0, HalfEdgeCount)
            .Any(he => IsBoundary((HalfEdgeId)he));

    public bool HasNonManifoldEdges
        => Enumerable.Range(0, EdgeCount).Any(e => _halfEdgesByUndirectedEdge[e].Count > 2);

    public TopoEdge Get(UndirectedEdgeId id)
        => new(this, id);

    public IReadOnlyList<HalfEdgeId> GetHalfEdgeIds(UndirectedEdgeId id)
        => _halfEdgesByUndirectedEdge[(int)id];

    public HalfEdgeId GetFirstHalfEdge(UndirectedEdgeId id)
    {
        var halfEdges = GetHalfEdgeIds(id);
        if (halfEdges.Count == 0)
            throw new InvalidOperationException("Undirected edge has no half-edges.");
        return halfEdges[0];
    }

    public bool TryGetFirstHalfEdge(UndirectedEdgeId id, out HalfEdgeId halfEdge)
    {
        var halfEdges = GetHalfEdgeIds(id);
        if (halfEdges.Count > 0)
        {
            halfEdge = halfEdges[0];
            return true;
        }

        halfEdge = default;
        return false;
    }

    public IReadOnlyList<TopoEdge> GetEdges()
        => GetUndirectedEdgeIds().Select(Get);

    public UndirectedEdgeId GetUndirectedEdge(HalfEdgeId id)
        => _halfEdgeToUndirectedEdge[(int)id];

    public Line3D GetLine(UndirectedEdgeId id)
        => GetLine(GetFirstHalfEdge(id));

    public VertexId GetVertexA(UndirectedEdgeId id)
        => GetStartVertex(GetFirstHalfEdge(id));

    public VertexId GetVertexB(UndirectedEdgeId id)
        => GetEndVertex(GetFirstHalfEdge(id));

    public Point3D GetPointA(UndirectedEdgeId id)
        => GetPoint(GetVertexA(id));

    public Point3D GetPointB(UndirectedEdgeId id)
        => GetPoint(GetVertexB(id));

    public int GetHalfEdgeCount(UndirectedEdgeId id)
        => GetHalfEdgeIds(id).Count;

    public bool HasExactlyOneHalfEdge(UndirectedEdgeId id)
        => GetHalfEdgeCount(id) == 1;

    public bool HasExactlyTwoHalfEdges(UndirectedEdgeId id)
        => GetHalfEdgeCount(id) == 2;

    public IReadOnlyList<TopoFace> GetFaces()
        => GetFaceIds().Select(Get);

    public IReadOnlyList<TopoFace> GetFaces(VertexId id)
        => GetIncidentFaceIds(id).Select(Get);

    public IReadOnlyList<TopoFace> GetFaces(HalfEdgeId id)
        => GetHalfEdgeIds(GetUndirectedEdge(id)).Select(he => Get(GetAssociatedFaceId(he)));

    public IReadOnlyList<TopoHalfEdge> GetEdges(FaceId id)
        => GetHalfEdgeIds(id).Select(Get);

    public IReadOnlyList<TopoHalfEdge> GetEdges(VertexId id)
        => GetOutgoingHalfEdgeIds(id).Select(Get);

    public IReadOnlyList<TopoVertex> GetVertices(HalfEdgeId id)
        => [Get(GetStartVertex(id)), Get(GetEndVertex(id))];

    public IReadOnlyList<TopoVertex> GetVertices(FaceId id)
        => GetHalfEdgeIds(id).Select(he => Get(GetStartVertex(he)));

    public IReadOnlyList<VertexId> GetVertexIds(FaceId id)
        => GetHalfEdgeIds(id).Select(GetStartVertex);

    public IReadOnlyList<Point3D> GetPoints(FaceId id)
        => GetVertexIds(id).Select(GetPoint);

    private static float Clamp(float x, float min, float max)
        => MathF.Max(min, MathF.Min(max, x));
    
    public IEnumerable<UndirectedEdgeId> InnerEdgeIds
        => GetUndirectedEdgeIds().Where(id => !IsBoundary(id));

    public IEnumerable<HalfEdgeId> BoundaryHalfEdges
        => GetHalfEdgeIds().Where(IsBoundary);

    public IEnumerable<HalfEdgeId> BoundaryEdges
        => GetHalfEdgeIds().Where(IsBoundary);

    public IEnumerable<UndirectedEdgeId> NonManifoldEdges
        => GetUndirectedEdgeIds().Where(IsNonManifold);

    public HashSet<VertexId> BoundaryVertices
        => BoundaryHalfEdges.SelectMany(he => new[] { GetStartVertex(he), GetEndVertex(he) }).ToHashSet();

    public HashSet<FaceId> BoundaryFaces
        => BoundaryHalfEdges.Select(GetAssociatedFaceId).ToHashSet();

    public HashSet<HalfEdgeId> GetBoundaryHalfEdges(VertexId id)
        => GetOutgoingHalfEdgeIds(id).Where(IsBoundary).ToHashSet();

    public IReadOnlyList<IReadOnlyList<HalfEdgeId>> GetBoundaryLoops()
    {
        var boundary = BoundaryHalfEdges.ToHashSet();
        var visited = new HashSet<HalfEdgeId>();
        var loops = new List<IReadOnlyList<HalfEdgeId>>();

        foreach (var start in boundary)
        {
            if (visited.Contains(start))
                continue;

            var loop = new List<HalfEdgeId>();
            var current = start;

            while (true)
            {
                if (!visited.Add(current))
                    break;

                loop.Add(current);

                if (!TryGetNextBoundaryHalfEdge(current, out var next))
                    break;

                current = next;

                if (EqualityComparer<HalfEdgeId>.Default.Equals(current, start))
                    break;
            }

            loops.Add(loop);
        }

        return loops;
    }

    public bool TryGetNextBoundaryHalfEdge(HalfEdgeId current, out HalfEdgeId next)
    {
        var end = GetEndVertex(current);

        // Prefer a boundary half-edge leaving the current end vertex.
        foreach (var he in GetOutgoingHalfEdgeIds(end))
        {
            if (IsBoundary(he) && !EqualityComparer<HalfEdgeId>.Default.Equals(he, current))
            {
                next = he;
                return true;
            }
        }

        next = default;
        return false;
    }

    public IReadOnlyList<IReadOnlyList<VertexId>> GetBoundaryVertexLoops()
        => GetBoundaryLoops()
            .Select(loop => loop.Select(GetStartVertex).ToArray() as IReadOnlyList<VertexId>)
            .ToArray();

    public HashSet<VertexId> GetNeighborVertexIds(VertexId id)
    {
        // Includes both outgoing and incoming so this works better on boundary
        // or inconsistently wound meshes.
        return GetIncidentHalfEdgeIds(id)
            .Select(he =>
            {
                var a = GetStartVertex(he);
                var b = GetEndVertex(he);
                return EqualityComparer<VertexId>.Default.Equals(a, id) ? b : a;
            })
            .ToHashSet();
    }

    public IEnumerable<TopoVertex> GetNeighbors(VertexId id)
        => GetNeighborVertexIds(id).Select(Get);

    public IReadOnlyList<FaceId> GetOneRingFaceIds(VertexId id)
        => GetIncidentFaceIds(id);

    public HashSet<FaceId> GetFaceNeighborIds(FaceId id)
    {
        var result = new List<FaceId>();
        foreach (var he in GetHalfEdgeIds(id))
        {
            foreach (var sibling in GetSiblingHalfEdgeIds(he))
            {
                var f = GetAssociatedFaceId(sibling);
                if (!EqualityComparer<FaceId>.Default.Equals(f, id))
                    result.Add(f);
            }
        }
        return result.ToHashSet();
    }

    public IEnumerable<TopoFace> GetFaceNeighbors(FaceId id)
        => GetFaceNeighborIds(id).Select(Get);
    
    public double GetEstimatedVolume()
    {
        double sum = 0;
        foreach (var f in GetFaceIds())
        {
            var tri = GetTriangle(f);
            sum += Vector3.Dot(tri.A, Vector3.Cross(tri.B, tri.C));
        }

        return sum / 6.0;
    }
}