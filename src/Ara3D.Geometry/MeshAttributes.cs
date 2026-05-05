using Ara3D.Collections;
using Ara3D.Utils;

namespace Ara3D.Geometry;

public interface IMeshAttributes
{
    IVertexAttributes Vertices { get; }
    IFaceAttributes Faces { get; }
    IEdgeAttributes Edges { get; }
    ICornerAttributes Corners { get; }
}

public enum VertexAttributeEnum
{
    // Position components
    X,
    Y,
    Z,
    Magnitude,

    // Topological
    Valence,
    IsBoundary,

    // Normal components
    AreaWeightedNormalX,
    AreaWeightedNormalY,
    AreaWeightedNormalZ,
    AngleWeightedNormalX,
    AngleWeightedNormalY,
    AngleWeightedNormalZ,

    // Area / angle
    BarycentricArea,
    CornerAngleSum,

    // Intrinsic curvature
    GaussianCurvature,
    GaussianCurvatureDensity,

    // Extrinsic curvature
    MeanCurvatureNormalX,
    MeanCurvatureNormalY,
    MeanCurvatureNormalZ,
    MeanCurvatureNormalLength,
    SignedMeanCurvature,
    SignedMeanCurvatureDensity,

    // Bending
    UnsignedEdgeBending,
    SignedEdgeBending,
}

public static class MeshAttributeExtensions
{
    public static IReadOnlyList<double> GetAttribute(this IVertexAttributes _attributes, VertexAttributeEnum attribute)
    {
        var n = _attributes.Count;
        return attribute switch
        {
            VertexAttributeEnum.X => n.MapRange(i => (double)_attributes.Positions[i].X),
            VertexAttributeEnum.Y => n.MapRange(i => (double)_attributes.Positions[i].Y),
            VertexAttributeEnum.Z => n.MapRange(i => (double)_attributes.Positions[i].Z),
            VertexAttributeEnum.Magnitude => n.MapRange(i => (double)_attributes.Positions[i].Length()),

            VertexAttributeEnum.Valence => n.MapRange(i => (double)_attributes.Valences[i]),
            VertexAttributeEnum.IsBoundary => n.MapRange(i => _attributes.IsBoundary[i] ? 1.0 : 0.0),
            VertexAttributeEnum.AreaWeightedNormalX => n.MapRange(i => (double)_attributes.AreaWeightedNormals[i].X),
            VertexAttributeEnum.AreaWeightedNormalY => n.MapRange(i => (double)_attributes.AreaWeightedNormals[i].Y),
            VertexAttributeEnum.AreaWeightedNormalZ => n.MapRange(i => (double)_attributes.AreaWeightedNormals[i].Z),

            VertexAttributeEnum.AngleWeightedNormalX => n.MapRange(i => (double)_attributes.AngleWeightedNormals[i].X),
            VertexAttributeEnum.AngleWeightedNormalY => n.MapRange(i => (double)_attributes.AngleWeightedNormals[i].Y),
            VertexAttributeEnum.AngleWeightedNormalZ => n.MapRange(i => (double)_attributes.AngleWeightedNormals[i].Z),

            VertexAttributeEnum.BarycentricArea => n.MapRange(i => _attributes.BarycentricAreas[i]),
            VertexAttributeEnum.CornerAngleSum => n.MapRange(i => _attributes.CornerAngleSums[i]),

            VertexAttributeEnum.GaussianCurvature => n.MapRange(i => _attributes.GaussianCurvatures[i]),
            VertexAttributeEnum.GaussianCurvatureDensity => n.MapRange(i => _attributes.GaussianCurvatureDensities[i]),

            VertexAttributeEnum.MeanCurvatureNormalX => n.MapRange(i => (double)_attributes.MeanCurvatureNormals[i].X),
            VertexAttributeEnum.MeanCurvatureNormalY => n.MapRange(i => (double)_attributes.MeanCurvatureNormals[i].Y),
            VertexAttributeEnum.MeanCurvatureNormalZ => n.MapRange(i => (double)_attributes.MeanCurvatureNormals[i].Z),
            VertexAttributeEnum.MeanCurvatureNormalLength => n.MapRange(i => _attributes.MeanCurvatureNormalLengths[i]),

            VertexAttributeEnum.SignedMeanCurvature => n.MapRange(i => _attributes.SignedMeanCurvatures[i]),
            VertexAttributeEnum.SignedMeanCurvatureDensity => n.MapRange(i => _attributes.SignedMeanCurvatureDensities[i]),

            VertexAttributeEnum.SignedEdgeBending => n.MapRange(i => _attributes.SignedEdgeBending[i]),

            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };
    }
}

public interface IVertexAttributes
{
    int Count { get; }

    IReadOnlyList<Vector3> Positions { get; }
    IReadOnlyList<int> Valences { get; }
    IReadOnlyList<bool> IsBoundary { get; }
    IReadOnlyList<Vector3> AreaWeightedNormals { get; }
    IReadOnlyList<Vector3> AngleWeightedNormals { get; }
    IReadOnlyList<double> BarycentricAreas { get; }
    IReadOnlyList<double> CornerAngleSums { get; }
    IReadOnlyList<double> GaussianCurvatures { get; }
    IReadOnlyList<double> GaussianCurvatureDensities { get; }
    IReadOnlyList<Vector3> MeanCurvatureNormals { get; }
    IReadOnlyList<double> MeanCurvatureNormalLengths { get; }
    IReadOnlyList<double> SignedMeanCurvatures { get; }
    IReadOnlyList<double> SignedMeanCurvatureDensities { get; }
    IReadOnlyList<double> SignedEdgeBending { get; }
}

public interface IFaceAttributes
{
    int Count { get; }
    IReadOnlyList<double> Areas { get; }
    IReadOnlyList<Vector3> Centroids { get; }
    IReadOnlyList<Vector3> Normals { get; }
}

public interface IEdgeAttributes
{
    int Count { get; }
    IReadOnlyList<Vector3> MidPoints { get; }
    IReadOnlyList<double> DihedralAngles { get; }
    IReadOnlyList<double> Lengths { get; }
}

public interface ICornerAttributes
{
    int Count { get; }
    IReadOnlyList<double> Angles { get; }
}

public sealed class VertexAttributes : IVertexAttributes
{
    public int Count { get; set; }
    public IReadOnlyList<Vector3> Positions {get; set; }
    public IReadOnlyList<double> Magnitudes { get; set; }
    public IReadOnlyList<int> Valences { get; set; }
    public IReadOnlyList<bool> IsBoundary { get; set; }
    public IReadOnlyList<Vector3> AreaWeightedNormals { get; set; }
    public IReadOnlyList<Vector3> AngleWeightedNormals { get; set; }
    public IReadOnlyList<double> BarycentricAreas { get; set; }
    public IReadOnlyList<double> CornerAngleSums { get; set; }
    public IReadOnlyList<double> GaussianCurvatures { get; set; }
    public IReadOnlyList<double> GaussianCurvatureDensities { get; set; }
    public IReadOnlyList<Vector3> MeanCurvatureNormals { get; set; }
    public IReadOnlyList<double> MeanCurvatureNormalLengths { get; set; }
    public IReadOnlyList<double> SignedMeanCurvatures { get; set; }
    public IReadOnlyList<double> SignedMeanCurvatureDensities { get; set; }
    public IReadOnlyList<double> SignedEdgeBending { get; set; }
}

public class FaceAttributes : IFaceAttributes
{
    public int Count { get; set; }
    public IReadOnlyList<double> Areas { get; set; }
    public IReadOnlyList<Vector3> Centroids { get; set; }
    public IReadOnlyList<Vector3> Normals { get; set; }
}

public class EdgeAttributes : IEdgeAttributes
{
    public int Count { get; set; }
    public IReadOnlyList<Vector3> MidPoints { get; set; }
    public IReadOnlyList<double> DihedralAngles { get; set; }
    public IReadOnlyList<double> Lengths { get; set; }
}

public class CornerAttributes : ICornerAttributes
{
    public int Count { get; set; }
    public IReadOnlyList<double> Angles { get; set; }
}

/// <summary>
/// This materializes (converts to arrays) a number of useful mesh attributes
/// associated with edges, corners, and faces, such as face normals,
/// edge lengths, corner angles, etc., which are useful when computing later
/// statistics, especially since they may need to get called multiple times.  
/// </summary>
public sealed class MeshAttributes : IMeshAttributes
{
    public TriangleMesh3D Mesh { get; }
    public Topology Topology { get; }

    private readonly CornerAttributes _corners = new();
    private readonly FaceAttributes _faces = new();
    private readonly EdgeAttributes _edges = new();
    private readonly VertexAttributes _vertices = new();

    public IVertexAttributes Vertices => _vertices;
    public IEdgeAttributes Edges => _edges;
    public IFaceAttributes Faces => _faces;
    public ICornerAttributes Corners => _corners;

    public MeshAttributes(TriangleMesh3D mesh, Topology topology)
    {
        Mesh = mesh;
        Topology = topology ?? throw new ArgumentNullException(nameof(topology));

        var triangles = Mesh.Triangles.ToArray();
        var lines = Topology.GetUndirectedEdgeIds().Select(Topology.GetLine).ToArray();
        var edgeIds = Topology.GetUndirectedEdgeIds();
        var vertexIds = Topology.GetVertexIds();

        _faces.Count = Mesh.FaceIndices.Count;
        _faces.Centroids = triangles.Select(t => t.Center.Vector3);
        _faces.Normals = triangles.Select(t => t.Normal);
        _faces.Areas = triangles.Select(t => (double)t.Area);

        _corners.Count = Mesh.FaceIndices.Count * 3;
        _corners.Angles = Topology.GetHalfEdgeIds().Select(ComputeCornerAngle);

        _edges.Count = Topology.EdgeCount;
        _edges.MidPoints = lines.Select(l => l.Center.Vector3);
        _edges.Lengths = lines.Select(l => (double)l.Length);
        _edges.DihedralAngles = edgeIds.Select(ComputeSignedDihedralAngle);

        _vertices.Count = Mesh.Points.Count;
        _vertices.Positions = Mesh.Points.Select(p => p.Vector3);
        _vertices.Magnitudes = Mesh.Points.Select(p => (double)p.Vector3.Length);
        _vertices.AreaWeightedNormals = vertexIds.Select(ComputeAreaWeightedVertexNormal);
        _vertices.AngleWeightedNormals = vertexIds.Select(ComputeAngleWeightedVertexNormal); 
        _vertices.Valences = vertexIds.Select(Topology.Valence);
        _vertices.IsBoundary = vertexIds.Select(Topology.IsBoundary);
        _vertices.BarycentricAreas = vertexIds.Select(ComputeBarycentricVertexArea);
        _vertices.CornerAngleSums = vertexIds.Select(ComputeCornerAngleSum);
        _vertices.GaussianCurvatures = vertexIds.Select(ComputeGaussianCurvature);
        _vertices.GaussianCurvatureDensities = vertexIds.Select(ComputeGaussianCurvatureDensity);
        _vertices.MeanCurvatureNormals = vertexIds.Select(ComputeMeanCurvatureNormal);
        _vertices.MeanCurvatureNormalLengths = vertexIds.Select(v => (double)ComputeMeanCurvatureNormal(v).Length);
        _vertices.SignedMeanCurvatures = vertexIds.Select(ComputeIntegratedSignedMeanCurvature);
        _vertices.SignedMeanCurvatureDensities = vertexIds.Select(ComputeSignedMeanCurvatureDensity);
        _vertices.SignedEdgeBending = vertexIds.Select(ComputeSignedEdgeBending);
    }

    //==============================================================================
    // Cached value accessors
    //==============================================================================

    public Vector3 GetFaceCentroid(FaceId id)
        => Faces.Centroids[(int)id];

    public Vector3 GetFaceNormal(FaceId id)
        => Faces.Normals[(int)id];

    public double GetFaceArea(FaceId id)
        => Faces.Areas[(int)id];

    public Vector3 GetEdgeMidpoint(UndirectedEdgeId id)
        => Edges.MidPoints[(int)id];

    public double GetEdgeLength(UndirectedEdgeId id)
        => Edges.Lengths[(int)id];

    public double GetSignedDihedralAngle(UndirectedEdgeId id)
        => Edges.DihedralAngles[(int)id];

    public double GetCornerAngle(HalfEdgeId id)
        => Corners.Angles[(int)id];

    public Vector3 GetCornerNormal(HalfEdgeId id)
        => GetFaceNormal(Topology.GetAssociatedFaceId(id)); 

    //==============================================================================
    // Incident values
    //==============================================================================

    public IReadOnlyList<Vector3> GetIncidentFaceNormals(VertexId id)
        => Topology.GetIncidentFaceIds(id).Select(GetFaceNormal);

    public IReadOnlyList<double> GetIncidentFaceAreas(VertexId id)
        => Topology.GetIncidentFaceIds(id).Select(GetFaceArea);

    public IReadOnlyList<Vector3> GetIncidentFaceCentroids(VertexId id)
        => Topology.GetIncidentFaceIds(id).Select(GetFaceCentroid);

    public IReadOnlyList<Vector3> GetIncidentEdgeMidpoints(VertexId id)
        => Topology.GetIncidentUndirectedEdgeIds(id).Select(GetEdgeMidpoint);

    public IReadOnlyList<double> GetIncidentEdgeLengths(VertexId id)
        => Topology.GetIncidentUndirectedEdgeIds(id).Select(GetEdgeLength);

    public IReadOnlyList<double> GetIncidentSignedDihedralAngles(VertexId id)
        => Topology.GetIncidentUndirectedEdgeIds(id).Select(GetSignedDihedralAngle);

    public IReadOnlyList<double> GetIncidentCornerAngles(VertexId id)
        => Topology.GetOutgoingHalfEdgeIds(id).Select(GetCornerAngle);

    //==============================================================================
    // Statistics
    //==============================================================================

    public Vector3WeightedStatistics GetFaceNormalStatisticsWeightedByArea(VertexId id)
        => new(GetIncidentFaceNormals(id), GetIncidentFaceAreas(id), true);

    public Vector3WeightedStatistics GetFaceNormalStatisticsWeightedByAngle(VertexId id)
        => new(GetIncidentFaceNormals(id), GetIncidentCornerAngles(id), true);

    public ScalarWeightedStatistics GetSignedDihedralAngleStatistics(VertexId id)
        => new(GetIncidentSignedDihedralAngles(id), GetIncidentEdgeLengths(id), true);

    public ScalarStatistics GetEdgeLengthStatistics(VertexId id)
        => new(GetIncidentEdgeLengths(id), true);

    //==============================================================================
    // Intrinsic curvature
    //==============================================================================

    public double ComputeCornerAngleSum(VertexId vertex)
        => Topology.GetOutgoingHalfEdgeIds(vertex).Sum(GetCornerAngle);

    /// <summary>
    /// Intrinsic discrete Gaussian curvature using angle defect.
    /// Positive for locally spherical/convex angle deficit.
    /// Negative for saddle-like angle excess.
    /// </summary>
    public double ComputeGaussianCurvature(VertexId vertex)
        => Topology.IsBoundary(vertex) ? Math.PI : Math.Tau - ComputeCornerAngleSum(vertex);

    /// <summary>
    /// Area-normalized intrinsic Gaussian curvature estimate.
    /// Useful for comparing meshes with different tessellation density.
    /// </summary>
    public double ComputeGaussianCurvatureDensity(VertexId vertex)
        => ComputeGaussianCurvature(vertex).SafeDivide(ComputeBarycentricVertexArea(vertex));

    //==============================================================================
    // Extrinsic curvature / bending
    //==============================================================================

    /// <summary>
    /// Length-weighted average unsigned dihedral angle around a vertex.
    /// Useful as a simple extrinsic "creases and bending" signal for coloring.
    /// Always non-negative.
    /// </summary>
    public double ComputeExtrinsicCurvature(VertexId vertex)
        => Topology.GetIncidentUndirectedEdgeIds(vertex).WeightedAverage(ComputeUnsignedDihedralAngle, GetEdgeLength);

    /// <summary>
    /// Length-weighted average signed dihedral angle around a vertex.
    /// Positive/negative depends on face winding and normal orientation.
    /// This is an extrinsic bending signal, not intrinsic Gaussian curvature.
    /// </summary>
    public double ComputeSignedEdgeBending(VertexId vertex)
        => Topology.GetIncidentUndirectedEdgeIds(vertex).WeightedAverage(GetSignedDihedralAngle, GetEdgeLength);

    /// <summary>
    /// Integrated mean curvature approximation around a vertex:
    ///     H_i ~= 1/4A_i * sum_e length(e) * signed_dihedral(e)
    ///
    /// The factor convention varies by source. For vertex coloring, consistency
    /// matters more than the exact convention.
    /// </summary>
    public double ComputeIntegratedSignedMeanCurvature(VertexId vertex)
        => Topology.GetIncidentUndirectedEdgeIds(vertex).WeightedSum(GetSignedDihedralAngle, GetEdgeLength) * 0.25;

    /// <summary>
    /// Area-normalized signed mean curvature estimate.
    /// Positive/negative depends on normal orientation and face winding.
    /// </summary>
    public double ComputeSignedMeanCurvatureDensity(VertexId vertex)
        => ComputeIntegratedSignedMeanCurvature(vertex).SafeDivide(ComputeBarycentricVertexArea(vertex));

    /// <summary>
    /// Cotangent Laplace-Beltrami mean curvature normal approximation.
    /// This is often a better smooth extrinsic curvature signal than raw dihedral angle.
    ///
    /// Returns approximately 2H * n, depending on convention.
    /// </summary>
    public Vector3 ComputeMeanCurvatureNormal(VertexId vertex)
    {
        var area = ComputeBarycentricVertexArea(vertex);
        if (area <= GeometryUtil.Epsilon)
            return Vector3.Zero;

        var p = Topology.GetPoint(vertex).Vector3;
        var sum = Vector3.Zero;

        foreach (var h in Topology.GetOutgoingHalfEdgeIds(vertex))
        {
            var q = Topology.GetPoint(Topology.GetEndVertex(h)).Vector3;

            var cotA = ComputeOppositeCotangent(h);

            var cotB = 0.0;
            if (Topology.TryGetTwin(h, out var twin))
                cotB = ComputeOppositeCotangent(twin);

            var weight = cotA + cotB;

            if (weight.IsFinite())
                sum += (float)weight * (q - p);
        }

        return sum / (float)(2.0 * area);
    }

    //==============================================================================
    // Normals
    //==============================================================================

    public Vector3 ComputeAreaWeightedVertexNormal(VertexId vertex)
        => Topology.GetIncidentFaceIds(vertex).WeightedAverage(GetFaceNormal, GetFaceArea);

    public Vector3 ComputeAngleWeightedVertexNormal(VertexId vertex)
        => Topology.GetOutgoingHalfEdgeIds(vertex).WeightedAverage(GetCornerNormal, GetCornerAngle);

    //==============================================================================
    // Area
    //==============================================================================

    /// <summary>
    /// Simple stable barycentric vertex area: one third of each incident triangle area.
    /// This is robust and good enough for coloring/feature extraction.
    /// Later you may replace this with Voronoi/mixed area.
    /// </summary>
    public double ComputeBarycentricVertexArea(VertexId vertex)
        => Topology.GetIncidentFaceIds(vertex).Sum(GetFaceArea) / 3.0;

    //==============================================================================
    // Angles
    //==============================================================================

    public double ComputeCornerAngle(HalfEdgeId id)
    {
        var p = Topology.GetPoint(Topology.GetStartVertex(id)).Vector3;
        var prev = Topology.GetPoint(Topology.GetStartVertex(Topology.GetPrevious(id))).Vector3;
        var next = Topology.GetPoint(Topology.GetEndVertex(id)).Vector3;

        return (prev - p).AngleBetween(next - p);
    }

    public double ComputeCornerAngle(FaceId face, int localVertex)
        => ComputeCornerAngle(Topology.GetHalfEdgeId(face, localVertex));

    public double ComputeUnsignedDihedralAngle(HalfEdgeId id)
    {
        if (!Topology.TryGetTwin(id, out var twin))
            return 0.0;

        var f0 = Topology.GetAssociatedFaceId(id);
        var f1 = Topology.GetAssociatedFaceId(twin);

        return GetFaceNormal(f0).AngleBetween(GetFaceNormal(f1));
    }

    public double ComputeUnsignedDihedralAngle(UndirectedEdgeId id)
    {
        var halfEdges = Topology.GetHalfEdgeIds(id);
        return halfEdges.Count != 2 ? 0.0 : ComputeUnsignedDihedralAngle(halfEdges[0]);
    }

    /// <summary>
    /// Signed dihedral angle around a half-edge.
    ///
    /// Sign convention:
    ///     sign = sign(dot(cross(n0, n1), edgeDirection))
    ///
    /// This depends on face winding consistency. If winding is inconsistent,
    /// signed values will be noisy.
    /// </summary>
    public double ComputeSignedDihedralAngle(HalfEdgeId id)
    {
        if (!Topology.TryGetTwin(id, out var twin))
            return 0.0;

        var f0 = Topology.GetAssociatedFaceId(id);
        var f1 = Topology.GetAssociatedFaceId(twin);

        var n0 = GetFaceNormal(f0);
        var n1 = GetFaceNormal(f1);

        var a = Topology.GetPoint(Topology.GetStartVertex(id)).Vector3;
        var b = Topology.GetPoint(Topology.GetEndVertex(id)).Vector3;
        var edgeDir = (b - a).SafeNormalize();

        var sin = Vector3.Dot(Vector3.Cross(n0, n1), edgeDir);
        var cos = Vector3.Dot(n0, n1);

        return Math.Atan2(sin, cos);
    }

    public double ComputeSignedDihedralAngle(UndirectedEdgeId id)
    {
        var halfEdges = Topology.GetHalfEdgeIds(id);
        if (halfEdges.Count != 2)
            return 0.0;

        return ComputeSignedDihedralAngle(halfEdges[0]);
    }

    /// <summary>
    /// Cotangent of the angle opposite the given half-edge in its triangle.
    ///
    /// For half-edge v0 -> v1, the opposite vertex is the end vertex of Next(h).
    /// </summary>
    public double ComputeOppositeCotangent(HalfEdgeId h)
    {
        var next = Topology.GetNext(h);

        var v0 = Topology.GetStartVertex(h);
        var v1 = Topology.GetEndVertex(h);
        var v2 = Topology.GetEndVertex(next);

        var p0 = Topology.GetPoint(v0).Vector3;
        var p1 = Topology.GetPoint(v1).Vector3;
        var p2 = Topology.GetPoint(v2).Vector3;

        var a = p0 - p2;
        var b = p1 - p2;

        return a.Cotangent(b);
    }
}