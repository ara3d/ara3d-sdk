using Ara3D.Collections;
using Ara3D.Utils;

namespace Ara3D.Geometry;

public sealed class MeshAttributeCache
{
    public TriangleMesh3D Mesh { get; }
    public Topology Topology { get; }

    // Per face
    public int NumFaces { get; }
    public Vector3[] FaceCentroids { get; }
    public Vector3[] FaceNormals { get; }
    public double[] FaceAreas { get; }

    // Per undirected edge
    public int NumEdges { get; }
    public Vector3[] EdgeMidpoints { get; }
    public double[] EdgeLengths { get; }
    public double[] EdgeDihedralAngles { get; }

    // Per half-edge / corner
    public int NumCorners { get; }
    public double[] CornerAngles { get; }

    //==============================================================================
    // Constructor 
    //==============================================================================

    public MeshAttributeCache(TriangleMesh3D mesh, Topology topology)
    {
        Mesh = mesh;
        Topology = topology ?? throw new ArgumentNullException(nameof(topology));

        NumFaces = Mesh.FaceIndices.Count;
        FaceCentroids = new Vector3[NumFaces];
        FaceNormals = new Vector3[NumFaces];
        FaceAreas = new double[NumFaces];
        for (var i = 0; i < NumFaces; i++)
        {
            var face = Mesh.GetTriangle(i);
            FaceCentroids[i] = face.Center;
            FaceNormals[i] = face.Normal;
            FaceAreas[i] = face.Area;
        }

        NumEdges = Topology.EdgeCount;
        EdgeMidpoints = new Vector3[NumEdges];
        EdgeLengths = new double[NumEdges];
        EdgeDihedralAngles = new double[NumEdges];
        for (var i = 0; i < NumEdges; i++)
        {
            var line = Topology.GetLine((UndirectedEdgeId)i);
            EdgeMidpoints[i] = line.Center;
            EdgeLengths[i] = line.Length;
            EdgeDihedralAngles[i] = GetDihedralAngle((UndirectedEdgeId)i);
        }

        NumCorners = Topology.HalfEdgeCount;
        CornerAngles = new double[NumCorners];
        for (var i = 0; i < NumCorners; i++)
        {
            CornerAngles[i] = ComputeCornerAngle((HalfEdgeId)i);
        }
    }

    //==============================================================================
    // Retrieve the cached values
    //==============================================================================

    public Vector3 GetFaceCentroid(FaceId id) => FaceCentroids[(int)id];
    public Vector3 GetFaceNormal(FaceId id) => FaceNormals[(int)id];
    public double GetFaceArea(FaceId id) => FaceAreas[(int)id];

    public Vector3 GetEdgeMidpoint(UndirectedEdgeId id) => EdgeMidpoints[(int)id];
    public double GetEdgeLength(UndirectedEdgeId id) => EdgeLengths[(int)id];
    public double GetEdgeDihedralAngle(UndirectedEdgeId id) => EdgeDihedralAngles[(int)id];

    public double GetCornerAngle(HalfEdgeId id) => CornerAngles[(int)id];

    //==============================================================================
    // Incident values
    //==============================================================================

    public IReadOnlyList<Vector3> GetIncidentFaceNormals(VertexId id) =>
        Topology.GetIncidentFaceIds(id).Select(GetFaceNormal);

    public IReadOnlyList<double> GetIncidentFaceAreas(VertexId id) =>
        Topology.GetIncidentFaceIds(id).Select(GetFaceArea);

    public IReadOnlyList<Vector3> GetIncidentFaceCentroids(VertexId id) =>
        Topology.GetIncidentFaceIds(id).Select(GetFaceCentroid);

    public IReadOnlyList<Vector3> GetIncidentEdgeCentroids(VertexId id) =>
        Topology.GetIncidentUndirectedEdgeIds(id).Select(GetEdgeMidpoint);

    public IReadOnlyList<double> GetIncidentEdgeLengths(VertexId id) =>
        Topology.GetIncidentUndirectedEdgeIds(id).Select(GetEdgeLength);

    public IReadOnlyList<double> GetIncidentEdgeDihedralAngle(VertexId id) =>
        Topology.GetIncidentUndirectedEdgeIds(id).Select(GetEdgeDihedralAngle);

    public IReadOnlyList<double> GetIncidentCornerAngles(VertexId id) =>
        Topology.GetOutgoingHalfEdgeIds(id).Select(GetCornerAngle);

    //==============================================================================
    // Statistics
    //==============================================================================

    public Vector3WeightedStatistics GetFaceNormalStatisticsWeightedByArea(VertexId id)
        => new(GetIncidentFaceNormals(id), GetIncidentFaceAreas(id), true);

    public Vector3WeightedStatistics GetFaceNormalStatisticsWeightedByAngle(VertexId id)
        => new(GetIncidentFaceNormals(id), GetIncidentCornerAngles(id), true);

    public ScalarWeightedStatistics GetEdgeDihedralAngleStatistics(VertexId id)
        => new(GetIncidentEdgeDihedralAngle(id), GetIncidentEdgeLengths(id), true);

    public ScalarStatistics GetEdgeLengthStatistics(VertexId id)
        => new(GetIncidentEdgeLengths(id), true);

    //==============================================================================
    // Angle and value computations
    //==============================================================================

    public double GetSignedBending(VertexId vertex)
    {
        var sum = 0.0;
        var count = 0;
        foreach (var edge in Topology.GetOutgoingHalfEdgeIds(vertex))
        {
            if (!Topology.TryGetTwin(edge, out _))
                continue;
            sum += GetDihedralAngle(edge);
            count++;
        }
        return count > 0 ? sum / count : 0;
    }
    
    public double ComputeCornerAngle(HalfEdgeId id)
    {
        var p = Topology.GetPoint(Topology.GetStartVertex(id)).Vector3;
        var prev = Topology.GetPoint(Topology.GetStartVertex(Topology.GetPrevious(id))).Vector3;
        var next = Topology.GetPoint(Topology.GetEndVertex(id)).Vector3;
        return AngleBetween(prev - p, next - p);
    }

    public double ComputeCornerAngle(FaceId face, int localVertex)
        => ComputeCornerAngle(Topology.GetHalfEdgeId(face, localVertex));

    public double GetAngleSum(VertexId id)
        => Topology.GetOutgoingHalfEdgeIds(id).Select(GetCornerAngle).Sum();

    public double GetAngleDefect(VertexId vertex)
    {
        var target = Topology.IsBoundary(vertex) ? Math.PI : 2 * Math.PI;
        return target - GetAngleSum(vertex);
    }

    public double GetDihedralAngle(HalfEdgeId id)
    {
        if (!Topology.TryGetTwin(id, out var twin))
            return 0f;
        var f0 = Topology.GetAssociatedFaceId(id);
        var f1 = Topology.GetAssociatedFaceId(twin);
        return AngleBetween(GetFaceNormal(f0), GetFaceNormal(f1));
    }

    public double GetDihedralAngle(UndirectedEdgeId id)
    {
        var halfEdges = Topology.GetHalfEdgeIds(id);
        if (halfEdges.Count != 2)
            return 0f;
        return GetDihedralAngle(halfEdges[0]);
    }

    //==============================================================================
    // Helpers
    //==============================================================================

    public const float Epsilon = 1e-15f;

    private static Vector3 SafeNormalize(Vector3 v)
    {
        var len = v.Length();
        return len <= Epsilon ? Vector3.Zero : v / len;
    }

    public static double AngleBetween(Vector3 a, Vector3 b)
    {
        var la = a.Length();
        var lb = b.Length();
        if (la <= Epsilon || lb <= Epsilon)
            return 0f;
        var cos = Vector3.Dot(a, b) / (la * lb);
        return Math.Acos(Clamp(cos, -1, 1));
    }

    public static double Clamp(double v, double a, double b)
        => Math.Max(a, Math.Min(v, b));

    public static float Clamp(float v, float a, float b)
        => Math.Max(a, Math.Min(v, b));
}