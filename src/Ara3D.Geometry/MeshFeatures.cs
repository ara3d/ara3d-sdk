using Ara3D.Geometry;
using Ara3D.Utils;

public sealed partial class MeshFeatures
{
    private const double Eps = 1e-12;

    public MeshFeatures(MeshStatistics stats)
        => Stats = stats ?? throw new ArgumentNullException(nameof(stats));

    // =========================================================================
    // Source objects
    // =========================================================================

    public MeshStatistics Stats { get; }

    public TriangleMesh3D Mesh => Stats.Mesh;
    public Topology Topology => Stats.Topology;
    public TopologyFeatureStats TopologyStats => Stats.TopologyFeatureStats;

    public BoundaryStats BoundaryStats => TopologyStats.Boundary;
    public TessellationStats TessellationStats => TopologyStats.Tessellation;
    public NormalOrientationStats NormalOrientationStats => TopologyStats.NormalOrientation;

    // =========================================================================
    // Counts
    // =========================================================================

    public int PointCount => Mesh.Points.Count;
    public int VertexCount => Topology.VertexCount;
    public int FaceCount => Mesh.FaceIndices.Count;
    public int HalfEdgeCount => FaceCount * 3;
    public int EdgeCount => Topology.EdgeCount;

    public bool IsEmpty => PointCount == 0 || FaceCount == 0;

    // =========================================================================
    // Axis-aligned bounds
    // =========================================================================

    public Bounds3D Aabb => Stats.Bounds;
    public Vector3 AabbSize => Aabb.Size;
    public Vector3 AabbCenter => Aabb.Center;

    public double AabbMinX => Aabb.Min.X;
    public double AabbMinY => Aabb.Min.Y;
    public double AabbMinZ => Aabb.Min.Z;

    public double AabbMaxX => Aabb.Max.X;
    public double AabbMaxY => Aabb.Max.Y;
    public double AabbMaxZ => Aabb.Max.Z;

    public double AabbCenterX => AabbCenter.X;
    public double AabbCenterY => AabbCenter.Y;
    public double AabbCenterZ => AabbCenter.Z;

    public double AabbSizeX => AabbSize.X;
    public double AabbSizeY => AabbSize.Y;
    public double AabbSizeZ => AabbSize.Z;

    public double AabbMinSide => MinComponent(AabbSize);
    public double AabbMaxSide => MaxComponent(AabbSize);
    public double AabbMiddleSide => AabbSize.X + AabbSize.Y + AabbSize.Z - AabbMinSide - AabbMaxSide;

    public double AabbDiagonal => Aabb.DiagonalLength();
    public double AabbRadius => AabbDiagonal * 0.5;
    public double AabbHorizontalDiagonal => Math.Sqrt(AabbSizeX * AabbSizeX + AabbSizeY * AabbSizeY);
    public double AabbHorizontalRadius => AabbHorizontalDiagonal * 0.5;

    public double AabbVolume => Aabb.GetVolume();
    public double AabbSurfaceArea => BoxSurfaceArea(AabbSize);
    public double AabbFootprintArea => AabbSizeX * AabbSizeY;
    public double AabbFootprintAspectRatio => SafeRatio(Math.Max(AabbSizeX, AabbSizeY), Math.Min(AabbSizeX, AabbSizeY));

    public double AabbAspectRatioMaxMin => SafeRatio(AabbMaxSide, AabbMinSide);
    public double AabbAspectRatioMaxMiddle => SafeRatio(AabbMaxSide, AabbMiddleSide);
    public double AabbAspectRatioMiddleMin => SafeRatio(AabbMiddleSide, AabbMinSide);

    public double AabbSlenderness => SafeRatio(AabbMaxSide, MinNonZeroComponent(AabbSize));
    public double AabbFlatness => SafeRatio(MinNonZeroComponent(AabbSize), AabbMaxSide);
    public double AabbVerticalExtentRatio => SafeRatio(AabbSizeZ, AabbDiagonal);
    public double AabbHorizontalExtentRatio => SafeRatio(AabbHorizontalDiagonal, AabbDiagonal);

    public double ElevationMin => AabbMinZ;
    public double ElevationMax => AabbMaxZ;
    public double ElevationCenter => AabbCenterZ;
    public double Height => AabbSizeZ;

    // =========================================================================
    // Oriented bounds
    // =========================================================================

    public OrientedBox3D Obb => Stats.OrientedBounds;
    public Vector3 ObbSize => Obb.Size;

    public double ObbSizeX => ObbSize.X;
    public double ObbSizeY => ObbSize.Y;
    public double ObbSizeZ => ObbSize.Z;

    public double ObbMinSide => MinComponent(ObbSize);
    public double ObbMaxSide => MaxComponent(ObbSize);
    public double ObbMiddleSide => ObbSize.X + ObbSize.Y + ObbSize.Z - ObbMinSide - ObbMaxSide;

    public double ObbDiagonal => ObbSize.Length();
    public double ObbRadius => ObbDiagonal * 0.5;
    public double ObbVolume => Obb.GetVolume();
    public double ObbSurfaceArea => BoxSurfaceArea(ObbSize);

    public double ObbAspectRatioMaxMin => SafeRatio(ObbMaxSide, ObbMinSide);
    public double ObbAspectRatioMaxMiddle => SafeRatio(ObbMaxSide, ObbMiddleSide);
    public double ObbAspectRatioMiddleMin => SafeRatio(ObbMiddleSide, ObbMinSide);

    public double ObbSlenderness => SafeRatio(ObbMaxSide, MinNonZeroComponent(ObbSize));
    public double ObbFlatness => SafeRatio(ObbMinSide, ObbMaxSide);
    public double ObbElongation => SafeRatio(ObbMaxSide - ObbMiddleSide, ObbMaxSide);
    public double ObbPlateLikeRatio => SafeRatio(ObbMiddleSide, ObbMaxSide) * SafeRatio(ObbMinSide, ObbMiddleSide);
    public double ObbRodLikeRatio => SafeRatio(ObbMaxSide, ObbMiddleSide) * SafeRatio(ObbMaxSide, ObbMinSide);

    public double ObbCrossSectionRoundness => SafeRatio(ObbMinSide, ObbMiddleSide);
    public double ObbCylinderAspectRatio => SafeRatio(ObbMaxSide, 0.5 * (ObbMiddleSide + ObbMinSide));
    public bool IsObbCircularCrossSection => ObbCrossSectionRoundness > 0.80;
    public bool IsObbLongCylinderCandidate => ObbCylinderAspectRatio > 2.0 && IsObbCircularCrossSection;

    public double AabbToObbVolumeRatio => SafeRatio(AabbVolume, ObbVolume);
    public double ObbToAabbVolumeRatio => SafeRatio(ObbVolume, AabbVolume);

    // =========================================================================
    // Surface area and face-area statistics
    // =========================================================================

    public double SurfaceArea => Stats.FaceAreaStats.Sum;

    public double FaceAreaSum => Stats.FaceAreaStats.Sum;
    public double FaceAreaAverage => Stats.FaceAreaStats.Average;
    public double FaceAreaMinimum => Stats.FaceAreaStats.Min;
    public double FaceAreaMaximum => Stats.FaceAreaStats.Max;
    public double FaceAreaVariance => Stats.FaceAreaStats.Variance;
    public double FaceAreaStdDev => Stats.FaceAreaStats.StdDev;
    public double FaceAreaCoefficientOfVariation => Stats.FaceAreaStats.CoefficientOfVariation;

    public double SurfaceAreaToAabbSurfaceAreaRatio => SafeRatio(SurfaceArea, AabbSurfaceArea);
    public double SurfaceAreaToAabbVolumeRatio => SafeRatio(SurfaceArea, AabbVolume);
    public double SurfaceAreaToObbSurfaceAreaRatio => SafeRatio(SurfaceArea, ObbSurfaceArea);
    public double AverageFaceAreaPerPoint => SafeRatio(SurfaceArea, PointCount);

    // =========================================================================
    // Vertex position distribution
    // =========================================================================

    public Vector3 VertexAverage => Stats.VertexStats.Average;
    public Vector3 VertexCenter => Stats.VertexStats.Center;
    public Vector3 VertexVariance => Stats.VertexStats.Variance;
    public Vector3 VertexStdDev => Stats.VertexStats.StdDev;

    public double VertexAverageX => VertexAverage.X;
    public double VertexAverageY => VertexAverage.Y;
    public double VertexAverageZ => VertexAverage.Z;

    public double VertexCenterX => VertexCenter.X;
    public double VertexCenterY => VertexCenter.Y;
    public double VertexCenterZ => VertexCenter.Z;

    public double VertexVarianceX => VertexVariance.X;
    public double VertexVarianceY => VertexVariance.Y;
    public double VertexVarianceZ => VertexVariance.Z;
    public double VertexVarianceSum => VertexVariance.X + VertexVariance.Y + VertexVariance.Z;

    public double VertexStdDevX => VertexStdDev.X;
    public double VertexStdDevY => VertexStdDev.Y;
    public double VertexStdDevZ => VertexStdDev.Z;
    public double VertexStdDevMagnitude => VertexStdDev.Length();

    public double VertexAverageDistanceFromOrigin => VertexAverage.Length();
    public double VertexAverageToCenterDistance => VertexAverage.Distance(Stats.VertexStats.Center);

    // =========================================================================
    // PCA of vertex positions
    // =========================================================================

    public Vector3 PcaMean => Stats.Pca.Mean;

    public Vector3 PcaPrincipalAxis => Stats.Pca.PrincipalAxis;
    public Vector3 PcaSecondaryAxis => Stats.Pca.SecondaryAxis;
    public Vector3 PcaTertiaryAxis => Stats.Pca.TertiaryAxis;

    public double PcaPrincipalAxisX => PcaPrincipalAxis.X;
    public double PcaPrincipalAxisY => PcaPrincipalAxis.Y;
    public double PcaPrincipalAxisZ => PcaPrincipalAxis.Z;

    public double PcaPrincipalAxisAbsX => Math.Abs(PcaPrincipalAxis.X);
    public double PcaPrincipalAxisAbsY => Math.Abs(PcaPrincipalAxis.Y);
    public double PcaPrincipalAxisAbsZ => Math.Abs(PcaPrincipalAxis.Z);

    public double PcaSecondaryAxisAbsX => Math.Abs(PcaSecondaryAxis.X);
    public double PcaSecondaryAxisAbsY => Math.Abs(PcaSecondaryAxis.Y);
    public double PcaSecondaryAxisAbsZ => Math.Abs(PcaSecondaryAxis.Z);

    public double PcaTertiaryAxisAbsX => Math.Abs(PcaTertiaryAxis.X);
    public double PcaTertiaryAxisAbsY => Math.Abs(PcaTertiaryAxis.Y);
    public double PcaTertiaryAxisAbsZ => Math.Abs(PcaTertiaryAxis.Z);

    public double PcaPrincipalAxisVerticality => Math.Abs(PcaPrincipalAxis.Z);
    public double PcaSecondaryAxisVerticality => Math.Abs(PcaSecondaryAxis.Z);
    public double PcaTertiaryAxisVerticality => Math.Abs(PcaTertiaryAxis.Z);
    public double PcaPrincipalAxisHorizontality => PcaPrincipalAxis.XY.Length;

    public double PcaLargestEigenvalue => Stats.Pca.LargestEigenValue;
    public double PcaMiddleEigenvalue => Stats.Pca.MiddleEigenValue;
    public double PcaSmallestEigenvalue => Stats.Pca.SmallestEigenValue;
    public double PcaEigenvalueSum => PcaLargestEigenvalue + PcaMiddleEigenvalue + PcaSmallestEigenvalue;
    public double PcaTotalVariance => Stats.Pca.TotalVariance;

    public double PcaLinearity => Stats.Pca.Linearity;
    public double PcaPlanarity => Stats.Pca.Planarity;
    public double PcaScattering => Stats.Pca.Scattering;

    public double PcaMiddleToLargestEigenvalueRatio => SafeRatio(PcaMiddleEigenvalue, PcaLargestEigenvalue);
    public double PcaSmallestToLargestEigenvalueRatio => SafeRatio(PcaSmallestEigenvalue, PcaLargestEigenvalue);
    public double PcaSmallestToMiddleEigenvalueRatio => SafeRatio(PcaSmallestEigenvalue, PcaMiddleEigenvalue);

    public double PcaLargestToSmallestEigenvalueRatio => SafeRatio(PcaLargestEigenvalue, PcaSmallestEigenvalue);
    public double PcaLargestToMiddleEigenvalueRatio => SafeRatio(PcaLargestEigenvalue, PcaMiddleEigenvalue);
    public double PcaMiddleToSmallestEigenvalueRatio => SafeRatio(PcaMiddleEigenvalue, PcaSmallestEigenvalue);

    public double PcaNormalizedEigenvalue1 => SafeRatio(PcaLargestEigenvalue, PcaEigenvalueSum);
    public double PcaNormalizedEigenvalue2 => SafeRatio(PcaMiddleEigenvalue, PcaEigenvalueSum);
    public double PcaNormalizedEigenvalue3 => SafeRatio(PcaSmallestEigenvalue, PcaEigenvalueSum);

    public double PcaAnisotropy => SafeRatio(PcaLargestEigenvalue - PcaSmallestEigenvalue, PcaLargestEigenvalue);

    public double PcaOmnivariance => Math.Pow(PcaLargestEigenvalue * PcaMiddleEigenvalue * PcaSmallestEigenvalue, 1.0 / 3.0);

    public double PcaEigenEntropy => EntropyTerm(PcaNormalizedEigenvalue1) + EntropyTerm(PcaNormalizedEigenvalue2) +
                                     EntropyTerm(PcaNormalizedEigenvalue3);

    // =========================================================================
    // Volume, density, and compactness
    // =========================================================================

    public double EstimatedVolume => TopologyStats.EstimatedVolume;

    public double AabbFillRatio => SafeRatio(EstimatedVolume, AabbVolume);
    public double ObbFillRatio => SafeRatio(EstimatedVolume, ObbVolume);

    public double FaceDensityPerAabbVolume => SafeRatio(FaceCount, AabbVolume);
    public double PointDensityPerAabbVolume => SafeRatio(PointCount, AabbVolume);
    public double FacesPerPoint => SafeRatio(FaceCount, PointCount);

    // Dimensionless. For closed solids, lower generally means more compact.
    public double SurfaceCompactness =>
        SafeRatio(SurfaceArea * SurfaceArea * SurfaceArea, EstimatedVolume * EstimatedVolume);

    // =========================================================================
    // Face normal distribution
    // =========================================================================

    public Vector3 FaceNormalAverage => Stats.FaceNormalByAngleStats.Average;
    public Vector3 FaceNormalVariance => Stats.FaceNormalByAngleStats.Variance;
    public Vector3 FaceNormalStdDev => Stats.FaceNormalByAngleStats.StdDev;

    public double FaceNormalAverageX => FaceNormalAverage.X;
    public double FaceNormalAverageY => FaceNormalAverage.Y;
    public double FaceNormalAverageZ => FaceNormalAverage.Z;

    public double FaceNormalAverageAbsX => Math.Abs(FaceNormalAverage.X);
    public double FaceNormalAverageAbsY => Math.Abs(FaceNormalAverage.Y);
    public double FaceNormalAverageAbsZ => Math.Abs(FaceNormalAverage.Z);

    public double FaceNormalAverageHorizontalMagnitude =>
        Math.Sqrt(FaceNormalAverage.X * FaceNormalAverage.X + FaceNormalAverage.Y * FaceNormalAverage.Y);

    public double FaceNormalAverageVerticality => Math.Abs(FaceNormalAverage.Z);

    public double FaceNormalVarianceX => FaceNormalVariance.X;
    public double FaceNormalVarianceY => FaceNormalVariance.Y;
    public double FaceNormalVarianceZ => FaceNormalVariance.Z;
    public double FaceNormalVarianceSum => FaceNormalVariance.X + FaceNormalVariance.Y + FaceNormalVariance.Z;

    public double FaceNormalStdDevX => FaceNormalStdDev.X;
    public double FaceNormalStdDevY => FaceNormalStdDev.Y;
    public double FaceNormalStdDevZ => FaceNormalStdDev.Z;
    public double FaceNormalStdDevMagnitude => FaceNormalStdDev.Length();

    public double FaceNormalDirectionality => FaceNormalAverage.Length();
    public double UpFacingScore => Math.Max(0.0, FaceNormalAverageZ);
    public double DownFacingScore => Math.Max(0.0, -FaceNormalAverageZ);
    public double VerticalFacingScore => 1.0 - FaceNormalAverageHorizontalMagnitude;

    public double UpFacingAreaRatio => NormalOrientationStats.UpFacingAreaRatio;
    public double DownFacingAreaRatio => NormalOrientationStats.DownFacingAreaRatio;
    public double HorizontalFacingAreaRatio => NormalOrientationStats.HorizontalFacingAreaRatio;
    public double VerticalFacingAreaRatio => NormalOrientationStats.VerticalFacingAreaRatio;
    public double SlopedFacingAreaRatio => NormalOrientationStats.SlopedFacingAreaRatio;

    // =========================================================================
    // PCA of face normals
    // =========================================================================

    public Vector3 FaceNormalPcaPrincipalAxis => Stats.FaceNormalPca.PrincipalAxis;

    public double FaceNormalPcaLinearity => Stats.FaceNormalPca.Linearity;
    public double FaceNormalPcaPlanarity => Stats.FaceNormalPca.Planarity;
    public double FaceNormalPcaScattering => Stats.FaceNormalPca.Scattering;

    public double FaceNormalPcaPrincipalAxisX => FaceNormalPcaPrincipalAxis.X;
    public double FaceNormalPcaPrincipalAxisY => FaceNormalPcaPrincipalAxis.Y;
    public double FaceNormalPcaPrincipalAxisZ => FaceNormalPcaPrincipalAxis.Z;

    public double FaceNormalPcaPrincipalAxisAbsX => Math.Abs(FaceNormalPcaPrincipalAxis.X);
    public double FaceNormalPcaPrincipalAxisAbsY => Math.Abs(FaceNormalPcaPrincipalAxis.Y);
    public double FaceNormalPcaPrincipalAxisAbsZ => Math.Abs(FaceNormalPcaPrincipalAxis.Z);
    public double FaceNormalPcaPrincipalAxisVerticality => Math.Abs(FaceNormalPcaPrincipalAxis.Z);

    // =========================================================================
    // Normalized vertex geometry
    // =========================================================================

    public Vector3 NormalizedVertexMin => Stats.NormalizedVertexStats.Min;
    public Vector3 NormalizedVertexMax => Stats.NormalizedVertexStats.Max;
    public Vector3 NormalizedVertexAverage => Stats.NormalizedVertexStats.Average;
    public Vector3 NormalizedVertexStdDev => Stats.NormalizedVertexStats.StdDev;

    public double NormalizedVertexMinX => NormalizedVertexMin.X;
    public double NormalizedVertexMinY => NormalizedVertexMin.Y;
    public double NormalizedVertexMinZ => NormalizedVertexMin.Z;

    public double NormalizedVertexMaxX => NormalizedVertexMax.X;
    public double NormalizedVertexMaxY => NormalizedVertexMax.Y;
    public double NormalizedVertexMaxZ => NormalizedVertexMax.Z;

    public double NormalizedVertexAverageX => NormalizedVertexAverage.X;
    public double NormalizedVertexAverageY => NormalizedVertexAverage.Y;
    public double NormalizedVertexAverageZ => NormalizedVertexAverage.Z;

    public double NormalizedVertexStdDevX => NormalizedVertexStdDev.X;
    public double NormalizedVertexStdDevY => NormalizedVertexStdDev.Y;
    public double NormalizedVertexStdDevZ => NormalizedVertexStdDev.Z;
    public double NormalizedVertexStdDevMagnitude => NormalizedVertexStdDev.Length();

    // =========================================================================
    // Dihedral angle stats 
    // =========================================================================

    public ScalarStatistics DihedralAngleStats => Stats.DihedralAngleStats;
    public double DihedralAngleMinimum => DihedralAngleStats.Min;
    public double DihedralAngleMaximum => DihedralAngleStats.Max;
    public double DihedralAngleStdDev => DihedralAngleStats.StdDev;
    public double DihedralAngleCoefficientOfVariation => DihedralAngleStats.CoefficientOfVariation;

    // =========================================================================
    // Cylinder-like shape features, using PCA-normalized points
    // =========================================================================

    public IReadOnlyList<Point3D> NormalizedPoints => Stats.NormalizedPoints;

    // Assumption: normalized PCA frame uses X as principal axis, Y/Z as cross-section axes.
    public double PcaCylinderAxisLength => NormalizedVertexMaxX - NormalizedVertexMinX;
    public double PcaCylinderRadiusY => Math.Max(Math.Abs(NormalizedVertexMinY), Math.Abs(NormalizedVertexMaxY));
    public double PcaCylinderRadiusZ => Math.Max(Math.Abs(NormalizedVertexMinZ), Math.Abs(NormalizedVertexMaxZ));
    public double PcaCylinderMeanRadius => 0.5 * (PcaCylinderRadiusY + PcaCylinderRadiusZ);

    public double PcaCylinderCrossSectionRoundness =>
        SafeRatio(Math.Min(PcaCylinderRadiusY, PcaCylinderRadiusZ),
            Math.Max(PcaCylinderRadiusY, PcaCylinderRadiusZ));

    public double PcaCylinderAspectRatio =>
        SafeRatio(PcaCylinderAxisLength, 2.0 * PcaCylinderMeanRadius);

    public bool IsPcaCircularCrossSection =>
        PcaCylinderCrossSectionRoundness > 0.80;

    public bool IsPcaLongCylinderCandidate =>
        IsPcaCircularCrossSection && PcaCylinderAspectRatio > 2.0;

    // Radius statistics in the YZ plane.
    public double PcaCylinderRadiusAverage =>
        AverageFinite(NormalizedPoints.Select(PcaRadialDistanceYZ));

    public double PcaCylinderRadiusMinimum =>
        MinFinite(NormalizedPoints.Select(PcaRadialDistanceYZ));

    public double PcaCylinderRadiusMaximum =>
        MaxFinite(NormalizedPoints.Select(PcaRadialDistanceYZ));

    public double PcaCylinderRadiusRange =>
        PcaCylinderRadiusMaximum - PcaCylinderRadiusMinimum;

    public double PcaCylinderRadiusStdDev =>
        StdDevFinite(NormalizedPoints.Select(PcaRadialDistanceYZ));

    public double PcaCylinderRadiusCoefficientOfVariation =>
        SafeRatio(PcaCylinderRadiusStdDev, PcaCylinderRadiusAverage);

    public double PcaCylinderRadialFitScore =>
        1.0 - Clamp01(PcaCylinderRadiusCoefficientOfVariation);

    // Useful for distinguishing round cylinders from square/rectangular prisms.
    // A perfect circle has low radial variation. A square has noticeably higher variation.
    public bool HasConsistentPcaCylinderRadius =>
        PcaCylinderRadiusCoefficientOfVariation < 0.10;

    public bool IsPcaCylinderLikeByVertices =>
        IsPcaLongCylinderCandidate && HasConsistentPcaCylinderRadius;

    // Measures how much of the point cloud is near the two ends.
    // Useful for capped cylinders, columns, pipes, rods, etc.
    public double PcaCylinderNegativeCapPointRatio =>
        PointRatioNearX(NormalizedVertexMinX, PcaCylinderAxisLength * 0.05);

    public double PcaCylinderPositiveCapPointRatio =>
        PointRatioNearX(NormalizedVertexMaxX, PcaCylinderAxisLength * 0.05);

    public double PcaCylinderCapPointBalanceRatio =>
        SafeRatio(
            Math.Min(PcaCylinderNegativeCapPointRatio, PcaCylinderPositiveCapPointRatio),
            Math.Max(PcaCylinderNegativeCapPointRatio, PcaCylinderPositiveCapPointRatio));

    public double PcaCylinderEndPointRatio =>
        PcaCylinderNegativeCapPointRatio + PcaCylinderPositiveCapPointRatio;

    // =========================================================================
    // Topology
    // =========================================================================

    public int ConnectedFaceComponentCount => TopologyStats.ConnectedFaceComponentCount;
    public int ConnectedVertexComponentCount => TopologyStats.ConnectedVertexComponentCount;
    public int ConnectedComponentCount => ConnectedFaceComponentCount;

    public int BoundaryEdgeCount => TryInt(() => Topology.BoundaryEdges.Count());
    public int NonManifoldEdgeCount => TryInt(() => Topology.NonManifoldEdges.Count());
    public int BoundaryLoopCount => TryInt(() => Topology.GetBoundaryLoops().Count);

    public double BoundaryEdgeRatio => SafeRatio(BoundaryEdgeCount, EdgeCount);
    public double NonManifoldEdgeRatio => SafeRatio(NonManifoldEdgeCount, EdgeCount);

    public double BoundaryLength => BoundaryStats.BoundaryLength;
    public double BoundaryLengthToSurfaceAreaRatio => BoundaryStats.BoundaryLengthToSurfaceAreaRatio;
    public double BoundaryLengthToAabbDiagonalRatio => BoundaryStats.BoundaryLengthToBoundsDiagonalRatio;

    public bool IsClosed => BoundaryEdgeCount == 0;
    public bool HasBoundaries => BoundaryEdgeCount > 0;
    public bool IsOpenSurface => HasBoundaries;
    public bool IsProbablyWatertight => IsClosed && NonManifoldEdgeCount == 0;
    public bool IsLikelySolid => IsProbablyWatertight && NonManifoldEdgeCount == 0 && EstimatedVolume > Eps;
    public bool HasBoundaryLoops => BoundaryLoopCount > 0;
    public bool HasMultipleShells => ConnectedFaceComponentCount > 1;

    public int EulerCharacteristic => TryInt(() => VertexCount - EdgeCount + FaceCount);
    public double GenusEstimate => IsProbablyWatertight ? (2.0 - EulerCharacteristic) / 2.0 : double.NaN;
    public bool IsTopologicallySimpleClosedSolid => IsProbablyWatertight && Math.Abs(GenusEstimate) < 1e-6;

    // =========================================================================
    // Tessellation quality
    // =========================================================================

    public double AverageEdgeLength => TessellationStats.AverageEdgeLength;
    public double EdgeLengthCoefficientOfVariation => TessellationStats.EdgeLengthCoefficientOfVariation;
    public double TriangleAspectRatioAverage => TessellationStats.TriangleAspectRatioAverage;
    public double TriangleAspectRatioMaximum => TessellationStats.TriangleAspectRatioMax;
    public double DegenerateTriangleRatio => TessellationStats.DegenerateTriangleRatio;

    // =========================================================================
    // Shape classification
    // =========================================================================

    public bool IsPointLike => PcaTotalVariance < 0.001;
    public bool IsLineLike => PcaLinearity > 0.85;
    public bool IsPlaneLike => PcaPlanarity > 0.85;
    public bool IsVolumetricLike => PcaScattering > 0.15;

    public bool IsRodLike => PcaLinearity > 0.75 && PcaPlanarity < 0.20;
    public bool IsSheetLike => PcaPlanarity > 0.65 && PcaScattering < 0.05;
    public bool IsBlobLike => PcaScattering > 0.25;

    public bool IsFlatByAabb => AabbFlatness < 0.02;
    public bool IsSlenderByAabb => AabbSlenderness > 20.0;
    public bool IsLargeSurfaceThinVolume => SurfaceAreaToAabbVolumeRatio > 100.0;

    public bool IsLongThin => PcaLinearity > 0.80 && AabbAspectRatioMaxMin > 10.0;
    public bool IsFlatPlateLike => PcaPlanarity > 0.70 && AabbAspectRatioMaxMin > 5.0;
    public bool IsVerticalElement => PcaPrincipalAxisVerticality > 0.85 && Height > Math.Max(AabbSizeX, AabbSizeY);

    public bool IsHorizontalElement =>
        PcaPrincipalAxisVerticality < 0.25 && Math.Max(AabbSizeX, AabbSizeY) > Height * 3.0;

    public bool IsTinyObject => AabbVolume < 1e-6 || SurfaceArea < 1e-6;

    // =========================================================================
    // Orientation classification, assumes Z-up
    // =========================================================================

    public bool IsMostlyUpFacing => UpFacingAreaRatio > 0.75;
    public bool IsMostlyDownFacing => DownFacingAreaRatio > 0.75;
    public bool IsMostlyHorizontal => HorizontalFacingAreaRatio > 0.75;
    public bool IsMostlyVertical => VerticalFacingAreaRatio > 0.75;
    public bool IsMostlySloped => SlopedFacingAreaRatio > 0.50;

    // =========================================================================
    // Normal distribution classification
    // =========================================================================

    public bool HasStrongDominantNormal => FaceNormalDirectionality > 0.80;
    public bool HasBalancedNormals => FaceNormalDirectionality < 0.10;
    public bool HasPlanarNormalDistribution => FaceNormalPcaLinearity > 0.80;
    public bool HasCylindricalNormalDistribution => FaceNormalPcaPlanarity > 0.70 && FaceNormalPcaScattering < 0.10;
    public bool HasScatteredNormalDistribution => FaceNormalPcaScattering > 0.25;

    // =========================================================================
    // Quality flags
    // =========================================================================

    public bool HasTinyFaces => FaceAreaMinimum < FaceAreaAverage * 1e-6;
    public bool HasUnevenFaceAreas => FaceAreaCoefficientOfVariation > 2.0;
    public bool HasUnevenEdgeLengths => EdgeLengthCoefficientOfVariation > 2.0;
    public bool HasBadTriangleAspectRatios => TriangleAspectRatioMaximum > 100.0;
    public bool IsHighlyTessellated => FaceDensityPerAabbVolume > 10000.0;

    // =========================================================================
    // Composite scores
    // =========================================================================

    public double AxisAlignedness =>
        Math.Max(
            Math.Abs(PcaPrincipalAxis.Dot(Vector3.UnitX)),
            Math.Max(
                Math.Abs(PcaPrincipalAxis.Dot(Vector3.UnitY)),
                Math.Abs(PcaPrincipalAxis.Dot(Vector3.UnitZ))));

    public bool IsMostlyAxisAligned => AxisAlignedness > 0.95;

    public double BoxLikeScore =>
        AverageFinite(
            Clamp01(AabbFillRatio),
            AxisAlignedness,
            Clamp01(HorizontalFacingAreaRatio + VerticalFacingAreaRatio));

    public double SurfaceLikeScore =>
        AverageFinite(
            PcaPlanarity,
            AabbFlatness < 1.0 ? 1.0 - AabbFlatness : 0.0,
            HasBoundaries ? 1.0 : 0.0);

    public double SolidLikeScore =>
        AverageFinite(
            IsProbablyWatertight ? 1.0 : 0.0,
            NonManifoldEdgeCount == 0 ? 1.0 : 0.0,
            Clamp01(AabbFillRatio));
}
