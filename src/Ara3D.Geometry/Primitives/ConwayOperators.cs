using System.Diagnostics;

namespace Ara3D.Geometry;

/// <summary>
/// Conway polyhedron operators on closed manifold <see cref="PolygonMesh3D"/> meshes.
/// </summary>
public static class ConwayOperators
{
    public static PolygonMesh3D Dual(this PolygonMesh3D mesh)
    {
        var faceCount = mesh.FaceCount;
        var newPoints = faceCount.MapRange(f => mesh.FaceCentroid(f));
        var edgeFace = DirectedEdgeFaces(mesh);
        var faces = mesh.Points.Count.MapRange(v => VertexFigure(mesh, v, edgeFace));
        return newPoints.ToPolygonMesh(faces).WithNormalizedPoints();
    }

    public static PolygonMesh3D Ambo(this PolygonMesh3D mesh)
    {
        var (edges, edgeIndex) = UndirectedEdges(mesh);
        var newPoints = edges.Map(e => Midpoint(mesh.Points[e.A], mesh.Points[e.B]));
        var edgeFace = DirectedEdgeFaces(mesh);

        var fromFaces = mesh.FaceCount.MapRange(f =>
        {
            var vs = mesh.FaceVerticesOf(f);
            return vs.Count.MapRange(i =>
            {
                var a = vs[i];
                var b = vs[(i + 1) % vs.Count];
                return edgeIndex[CanonicalEdge(a, b)];
            });
        });

        var fromVerts = mesh.Points.Count.MapRange(v =>
        {
            var ring = VertexNeighborRing(mesh, v, edgeFace);
            // Reverse so vertex faces share outward orientation with face faces.
            return ring.Count.MapRange(i => edgeIndex[CanonicalEdge(v, ring[ring.Count - 1 - i])]);
        });

        return newPoints.ToPolygonMesh(ConcatFaces(fromFaces, fromVerts)).WithNormalizedPoints();
    }

    public static PolygonMesh3D Truncate(this PolygonMesh3D mesh)
    {
        var edgeFace = DirectedEdgeFaces(mesh);
        var halfEdgeIndex = new Dictionary<(int, int), int>();
        var newPoints = new List<Point3D>();

        for (var f = 0; f < mesh.FaceCount; ++f)
        {
            var vs = mesh.FaceVerticesOf(f);
            for (var i = 0; i < vs.Count; ++i)
            {
                var a = vs[i];
                var b = vs[(i + 1) % vs.Count];
                var key = (a, b);
                if (halfEdgeIndex.ContainsKey(key))
                    continue;
                halfEdgeIndex[key] = newPoints.Count;
                newPoints.Add(Lerp(mesh.Points[a], mesh.Points[b], 1f / 3f));
            }
        }

        var fromFaces = mesh.FaceCount.MapRange(f =>
        {
            var vs = mesh.FaceVerticesOf(f);
            var ring = new int[vs.Count * 2];
            for (var i = 0; i < vs.Count; ++i)
            {
                var a = vs[i];
                var b = vs[(i + 1) % vs.Count];
                ring[i * 2] = halfEdgeIndex[(a, b)];
                ring[i * 2 + 1] = halfEdgeIndex[(b, a)];
            }
            return (IReadOnlyList<int>)ring;
        });

        var fromVerts = mesh.Points.Count.MapRange(v =>
        {
            var neighbors = VertexNeighborRing(mesh, v, edgeFace);
            return neighbors.Count.MapRange(i =>
                halfEdgeIndex[(v, neighbors[neighbors.Count - 1 - i])]);
        });

        return newPoints.ToPolygonMesh(ConcatFaces(fromFaces, fromVerts)).WithNormalizedPoints();
    }

    static Point3D Midpoint(Point3D a, Point3D b)
        => Lerp(a, b, 0.5f);

    static Point3D Lerp(Point3D a, Point3D b, float t)
        => (Point3D)(a.Vector3 + (b.Vector3 - a.Vector3) * t);

    static (int A, int B) CanonicalEdge(int a, int b)
        => a < b ? (a, b) : (b, a);

    static Dictionary<(int, int), int> DirectedEdgeFaces(PolygonMesh3D mesh)
    {
        var map = new Dictionary<(int, int), int>();
        for (var f = 0; f < mesh.FaceCount; ++f)
        {
            var vs = mesh.FaceVerticesOf(f);
            for (var i = 0; i < vs.Count; ++i)
            {
                var a = vs[i];
                var b = vs[(i + 1) % vs.Count];
                map[(a, b)] = f;
            }
        }
        return map;
    }

    static (IReadOnlyList<(int A, int B)> Edges, Dictionary<(int, int), int> Index) UndirectedEdges(
        PolygonMesh3D mesh)
    {
        var edges = new List<(int A, int B)>();
        var index = new Dictionary<(int, int), int>();
        for (var f = 0; f < mesh.FaceCount; ++f)
        {
            var vs = mesh.FaceVerticesOf(f);
            for (var i = 0; i < vs.Count; ++i)
            {
                var key = CanonicalEdge(vs[i], vs[(i + 1) % vs.Count]);
                if (index.ContainsKey(key))
                    continue;
                index[key] = edges.Count;
                edges.Add(key);
            }
        }
        return (edges, index);
    }

    static IReadOnlyList<int> VertexNeighborRing(
        PolygonMesh3D mesh,
        int vertex,
        Dictionary<(int, int), int> edgeFace)
    {
        var startFace = -1;
        var startNext = -1;
        for (var f = 0; f < mesh.FaceCount && startFace < 0; ++f)
        {
            var vs = mesh.FaceVerticesOf(f);
            for (var i = 0; i < vs.Count; ++i)
            {
                if (vs[i] != vertex)
                    continue;
                startFace = f;
                startNext = vs[(i + 1) % vs.Count];
                break;
            }
        }

        Debug.Assert(startFace >= 0);

        var ring = new List<int>();
        var face = startFace;
        var next = startNext;
        do
        {
            ring.Add(next);
            var twinFace = edgeFace[(next, vertex)];
            var vs = mesh.FaceVerticesOf(twinFace);
            var i = IndexOf(vs, vertex);
            next = vs[(i + 1) % vs.Count];
            face = twinFace;
        } while (face != startFace);

        return ring;
    }

    static IReadOnlyList<int> VertexFigure(
        PolygonMesh3D mesh,
        int vertex,
        Dictionary<(int, int), int> edgeFace)
    {
        var neighbors = VertexNeighborRing(mesh, vertex, edgeFace);
        var faces = new int[neighbors.Count];
        for (var i = 0; i < neighbors.Count; ++i)
            faces[i] = edgeFace[(vertex, neighbors[i])];
        return faces;
    }

    static int IndexOf(IReadOnlyList<int> xs, int value)
    {
        for (var i = 0; i < xs.Count; ++i)
            if (xs[i] == value)
                return i;
        throw new InvalidOperationException($"Vertex {value} not found in face.");
    }

    static IReadOnlyList<IReadOnlyList<int>> ConcatFaces(
        IReadOnlyList<IReadOnlyList<int>> a,
        IReadOnlyList<IReadOnlyList<int>> b)
    {
        var result = new IReadOnlyList<int>[a.Count + b.Count];
        for (var i = 0; i < a.Count; ++i)
            result[i] = a[i];
        for (var i = 0; i < b.Count; ++i)
            result[a.Count + i] = b[i];
        return result;
    }
}
