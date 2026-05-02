namespace Ara3D.Geometry;

public sealed class VertexStatistics
{
    public enum Attribute
    {
        // Position
        X,
        Y,
        Z,
        DistanceFromOrigin,
        DistanceFromCentroid,
        DistanceFromBoundsCenter,
        HeightAboveMinZ,
        HeightRatio,

        // Topology
        Valence,
        FaceCount,
        BoundaryEdgeCount,
        IsBoundary,
        IsIsolated,

        // Area / local scale
        VertexArea,
        TotalIncidentFaceArea,
        AverageFaceArea,
        MinFaceArea,
        MaxFaceArea,

        AverageEdgeLength,
        MinEdgeLength,
        MaxEdgeLength,
        EdgeLengthRange,

        // Normals
        NormalX,
        NormalY,
        NormalZ,
        NormalDotUp,
        Verticality,
        Horizontality,

        // Curvature / bending / sharpness
        GaussianCurvatureAngleDefect,
        GaussianCurvatureDensity,
        NormalVariation,
        MaxNormalDeviation,
        AverageDihedralAngle,
        MaxDihedralAngle,
        SharpEdgeRatio,
    }

    public Topology Topology { get; }

    public int VertexCount { get; }

    public Vector3[] Points { get; }
    public Vector3[] Normals { get; }

    public double[] X { get; }
    public double[] Y { get; }
    public double[] Z { get; }

    public double[] DistanceFromOrigin { get; }
    public double[] DistanceFromCentroid { get; }
    public double[] DistanceFromBoundsCenter { get; }
    public double[] HeightAboveMinZ { get; }
    public double[] HeightRatio { get; }

    public double[] Valence { get; }
    public double[] FaceCount { get; }
    public double[] BoundaryEdgeCount { get; }
    public double[] IsBoundary { get; }
    public double[] IsIsolated { get; }

    public double[] VertexArea { get; }
    public double[] TotalIncidentFaceArea { get; }
    public double[] AverageFaceArea { get; }
    public double[] MinFaceArea { get; }
    public double[] MaxFaceArea { get; }

    public double[] AverageEdgeLength { get; }
    public double[] MinEdgeLength { get; }
    public double[] MaxEdgeLength { get; }
    public double[] EdgeLengthRange { get; }

    public double[] NormalX { get; }
    public double[] NormalY { get; }
    public double[] NormalZ { get; }
    public double[] NormalDotUp { get; }
    public double[] Verticality { get; }
    public double[] Horizontality { get; }

    /// <summary>
    /// Discrete Gaussian curvature as angle defect.
    /// Interior vertex: 2π - angle sum.
    /// Boundary vertex: π - angle sum.
    /// </summary>
    public double[] GaussianCurvatureAngleDefect { get; }

    /// <summary>
    /// Angle defect divided by local vertex area.
    /// This is usually more comparable across different mesh scales.
    /// </summary>
    public double[] GaussianCurvatureDensity { get; }

    /// <summary>
    /// Area-weighted average angular deviation between incident face normals
    /// and the computed vertex normal.
    /// This is often the practical "visual curvature" people expect.
    /// </summary>
    public double[] NormalVariation { get; }

    public double[] MaxNormalDeviation { get; }

    public double[] AverageDihedralAngle { get; }
    public double[] MaxDihedralAngle { get; }
    public double[] SharpEdgeRatio { get; }

    public Vector3 Centroid { get; }
    public Vector3 BoundsMin { get; }
    public Vector3 BoundsMax { get; }
    public Vector3 BoundsCenter { get; }
    public Vector3 BoundsSize { get; }

    public VertexStatistics(
        Topology topology,
        Topology.VertexNormalWeighting normalWeighting = Topology.VertexNormalWeighting.FaceArea,
        double sharpAngleRadians = Math.PI / 6.0)
    {
        Topology = topology ?? throw new ArgumentNullException(nameof(topology));
        VertexCount = topology.VertexCount;

        Points = new Vector3[VertexCount];
        Normals = new Vector3[VertexCount];

        X = new double[VertexCount];
        Y = new double[VertexCount];
        Z = new double[VertexCount];

        DistanceFromOrigin = new double[VertexCount];
        DistanceFromCentroid = new double[VertexCount];
        DistanceFromBoundsCenter = new double[VertexCount];
        HeightAboveMinZ = new double[VertexCount];
        HeightRatio = new double[VertexCount];

        Valence = new double[VertexCount];
        FaceCount = new double[VertexCount];
        BoundaryEdgeCount = new double[VertexCount];
        IsBoundary = new double[VertexCount];
        IsIsolated = new double[VertexCount];

        VertexArea = new double[VertexCount];
        TotalIncidentFaceArea = new double[VertexCount];
        AverageFaceArea = new double[VertexCount];
        MinFaceArea = new double[VertexCount];
        MaxFaceArea = new double[VertexCount];

        AverageEdgeLength = new double[VertexCount];
        MinEdgeLength = new double[VertexCount];
        MaxEdgeLength = new double[VertexCount];
        EdgeLengthRange = new double[VertexCount];

        NormalX = new double[VertexCount];
        NormalY = new double[VertexCount];
        NormalZ = new double[VertexCount];
        NormalDotUp = new double[VertexCount];
        Verticality = new double[VertexCount];
        Horizontality = new double[VertexCount];

        GaussianCurvatureAngleDefect = new double[VertexCount];
        GaussianCurvatureDensity = new double[VertexCount];
        NormalVariation = new double[VertexCount];
        MaxNormalDeviation = new double[VertexCount];

        AverageDihedralAngle = new double[VertexCount];
        MaxDihedralAngle = new double[VertexCount];
        SharpEdgeRatio = new double[VertexCount];

        if (VertexCount == 0)
        {
            BoundsMin = Vector3.Zero;
            BoundsMax = Vector3.Zero;
            BoundsCenter = Vector3.Zero;
            BoundsSize = Vector3.Zero;
            Centroid = Vector3.Zero;
            return;
        }

        // First pass: points, bounds, centroid.
        var min = new Vector3(float.PositiveInfinity);
        var max = new Vector3(float.NegativeInfinity);
        var centroidSum = Vector3.Zero;

        for (var i = 0; i < VertexCount; i++)
        {
            var id = (VertexId)i;
            var p = topology.GetPoint(id).Vector3;

            Points[i] = p;
            centroidSum += p;

            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);

            X[i] = p.X;
            Y[i] = p.Y;
            Z[i] = p.Z;
            DistanceFromOrigin[i] = p.Length();
        }

        BoundsMin = min;
        BoundsMax = max;
        BoundsSize = max - min;
        BoundsCenter = (min + max) * 0.5f;
        Centroid = centroidSum / VertexCount;

        var zRange = BoundsSize.Z;

        // Second pass: vertex-local attributes.
        for (var i = 0; i < VertexCount; i++)
        {
            var id = (VertexId)i;
            var p = Points[i];

            DistanceFromCentroid[i] = Vector3.Distance(p, Centroid);
            DistanceFromBoundsCenter[i] = Vector3.Distance(p, BoundsCenter);
            HeightAboveMinZ[i] = p.Z - BoundsMin.Z;
            HeightRatio[i] = SafeRatio(p.Z - BoundsMin.Z, zRange);

            var outgoing = topology.GetOutgoingHalfEdgeIds(id);
            var faces = topology.GetFaceIds(id);

            Valence[i] = outgoing.Count;
            FaceCount[i] = faces.Count;
            IsBoundary[i] = topology.IsBoundary(id) ? 1.0 : 0.0;
            IsIsolated[i] = outgoing.Count == 0 && faces.Count == 0 ? 1.0 : 0.0;

            var normal = topology.GetVertexNormal(id, normalWeighting);
            Normals[i] = normal;

            NormalX[i] = normal.X;
            NormalY[i] = normal.Y;
            NormalZ[i] = normal.Z;

            NormalDotUp[i] = normal.Z;
            Horizontality[i] = Math.Abs(normal.Z);
            Verticality[i] = 1.0 - Math.Abs(normal.Z);

            ComputeFaceStats(topology, id, faces, i);
            ComputeEdgeStats(topology, id, outgoing, i, sharpAngleRadians);
            ComputeCurvatureStats(topology, id, faces, normal, i);
        }
    }

    private void ComputeFaceStats(
        Topology topology,
        VertexId vertex,
        IReadOnlyList<FaceId> faces,
        int i)
    {
        if (faces.Count == 0)
        {
            VertexArea[i] = 0.0;
            TotalIncidentFaceArea[i] = 0.0;
            AverageFaceArea[i] = 0.0;
            MinFaceArea[i] = 0.0;
            MaxFaceArea[i] = 0.0;
            return;
        }

        var areaSum = 0.0;
        var minArea = double.PositiveInfinity;
        var maxArea = double.NegativeInfinity;

        foreach (var face in faces)
        {
            var area = topology.GetFaceArea(face);

            areaSum += area;
            minArea = Math.Min(minArea, area);
            maxArea = Math.Max(maxArea, area);
        }

        TotalIncidentFaceArea[i] = areaSum;

        // Barycentric vertex area for triangle meshes.
        VertexArea[i] = areaSum / 3.0;

        AverageFaceArea[i] = areaSum / faces.Count;
        MinFaceArea[i] = minArea;
        MaxFaceArea[i] = maxArea;
    }

    private void ComputeEdgeStats(
        Topology topology,
        VertexId vertex,
        IReadOnlyList<HalfEdgeId> outgoing,
        int i,
        double sharpAngleRadians)
    {
        if (outgoing.Count == 0)
        {
            BoundaryEdgeCount[i] = 0.0;
            AverageEdgeLength[i] = 0.0;
            MinEdgeLength[i] = 0.0;
            MaxEdgeLength[i] = 0.0;
            EdgeLengthRange[i] = 0.0;
            AverageDihedralAngle[i] = 0.0;
            MaxDihedralAngle[i] = 0.0;
            SharpEdgeRatio[i] = 0.0;
            return;
        }

        var boundaryCount = 0;

        var edgeLengthSum = 0.0;
        var minEdgeLength = double.PositiveInfinity;
        var maxEdgeLength = double.NegativeInfinity;

        var dihedralSum = 0.0;
        var maxDihedral = 0.0;
        var sharpCount = 0;

        foreach (var he in outgoing)
        {
            if (topology.IsBoundary(he))
                boundaryCount++;

            var length = topology.GetEdgeLength(he);
            edgeLengthSum += length;
            minEdgeLength = Math.Min(minEdgeLength, length);
            maxEdgeLength = Math.Max(maxEdgeLength, length);

            var dihedral = topology.GetDihedralAngle(he).Value;
            dihedralSum += dihedral;
            maxDihedral = Math.Max(maxDihedral, dihedral);

            if (dihedral >= sharpAngleRadians)
                sharpCount++;
        }

        BoundaryEdgeCount[i] = boundaryCount;

        AverageEdgeLength[i] = edgeLengthSum / outgoing.Count;
        MinEdgeLength[i] = minEdgeLength;
        MaxEdgeLength[i] = maxEdgeLength;
        EdgeLengthRange[i] = maxEdgeLength - minEdgeLength;

        AverageDihedralAngle[i] = dihedralSum / outgoing.Count;
        MaxDihedralAngle[i] = maxDihedral;
        SharpEdgeRatio[i] = sharpCount / (double)outgoing.Count;
    }

    private void ComputeCurvatureStats(
        Topology topology,
        VertexId vertex,
        IReadOnlyList<FaceId> faces,
        Vector3 vertexNormal,
        int i)
    {
        var angleDefect = topology.GetCurvature(vertex);

        GaussianCurvatureAngleDefect[i] = angleDefect;
        GaussianCurvatureDensity[i] = SafeRatio(angleDefect, VertexArea[i]);

        if (faces.Count == 0 || vertexNormal.LengthSquared() <= 1e-20f)
        {
            NormalVariation[i] = 0.0;
            MaxNormalDeviation[i] = 0.0;
            return;
        }

        var weightedAngleSum = 0.0;
        var weightSum = 0.0;
        var maxAngle = 0.0;

        foreach (var face in faces)
        {
            var faceNormal = topology.GetFaceNormal(face);
            var area = topology.GetFaceArea(face);

            var angle = SafeAngle(vertexNormal, faceNormal);

            weightedAngleSum += angle * area;
            weightSum += area;
            maxAngle = Math.Max(maxAngle, angle);
        }

        NormalVariation[i] = SafeRatio(weightedAngleSum, weightSum);
        MaxNormalDeviation[i] = maxAngle;
    }

    public IReadOnlyList<double> GetValues(Attribute attribute)
    {
        return attribute switch
        {
            Attribute.X => X,
            Attribute.Y => Y,
            Attribute.Z => Z,
            Attribute.DistanceFromOrigin => DistanceFromOrigin,
            Attribute.DistanceFromCentroid => DistanceFromCentroid,
            Attribute.DistanceFromBoundsCenter => DistanceFromBoundsCenter,
            Attribute.HeightAboveMinZ => HeightAboveMinZ,
            Attribute.HeightRatio => HeightRatio,

            Attribute.Valence => Valence,
            Attribute.FaceCount => FaceCount,
            Attribute.BoundaryEdgeCount => BoundaryEdgeCount,
            Attribute.IsBoundary => IsBoundary,
            Attribute.IsIsolated => IsIsolated,

            Attribute.VertexArea => VertexArea,
            Attribute.TotalIncidentFaceArea => TotalIncidentFaceArea,
            Attribute.AverageFaceArea => AverageFaceArea,
            Attribute.MinFaceArea => MinFaceArea,
            Attribute.MaxFaceArea => MaxFaceArea,

            Attribute.AverageEdgeLength => AverageEdgeLength,
            Attribute.MinEdgeLength => MinEdgeLength,
            Attribute.MaxEdgeLength => MaxEdgeLength,
            Attribute.EdgeLengthRange => EdgeLengthRange,

            Attribute.NormalX => NormalX,
            Attribute.NormalY => NormalY,
            Attribute.NormalZ => NormalZ,
            Attribute.NormalDotUp => NormalDotUp,
            Attribute.Verticality => Verticality,
            Attribute.Horizontality => Horizontality,

            Attribute.GaussianCurvatureAngleDefect => GaussianCurvatureAngleDefect,
            Attribute.GaussianCurvatureDensity => GaussianCurvatureDensity,
            Attribute.NormalVariation => NormalVariation,
            Attribute.MaxNormalDeviation => MaxNormalDeviation,
            Attribute.AverageDihedralAngle => AverageDihedralAngle,
            Attribute.MaxDihedralAngle => MaxDihedralAngle,
            Attribute.SharpEdgeRatio => SharpEdgeRatio,

            _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
        };
    }

    private static double SafeRatio(double numerator, double denominator)
        => Math.Abs(denominator) > 1e-20
            ? numerator / denominator
            : 0.0;

    private static double SafeAngle(Vector3 a, Vector3 b)
    {
        var la2 = a.LengthSquared();
        var lb2 = b.LengthSquared();

        if (la2 <= 1e-20f || lb2 <= 1e-20f)
            return 0.0;

        var d = Vector3.Dot(a, b) / MathF.Sqrt(la2 * lb2);
        d = Math.Clamp(d, -1f, 1f);

        return Math.Acos(d);
    }
}