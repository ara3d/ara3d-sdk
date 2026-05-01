using Ara3D.Geometry;
using Ara3D.Utils;

public sealed class MeshFeatures
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
    public double PcaEigenEntropy => EntropyTerm(PcaNormalizedEigenvalue1) + EntropyTerm(PcaNormalizedEigenvalue2) + EntropyTerm(PcaNormalizedEigenvalue3);

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
    public double SurfaceCompactness => SafeRatio(SurfaceArea * SurfaceArea * SurfaceArea, EstimatedVolume * EstimatedVolume);

    // =========================================================================
    // Face normal distribution
    // =========================================================================

    public Vector3 FaceNormalAverage => Stats.FaceNormalStats.Average;
    public Vector3 FaceNormalVariance => Stats.FaceNormalStats.Variance;
    public Vector3 FaceNormalStdDev => Stats.FaceNormalStats.StdDev;

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
    public bool IsHorizontalElement => PcaPrincipalAxisVerticality < 0.25 && Math.Max(AabbSizeX, AabbSizeY) > Height * 3.0;
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

    // =========================================================================
    // Labels
    // =========================================================================

    public string CoarseShapeLabel()
    {
        if (IsEmpty) return "Empty";
        if (IsPointLike) return "PointLike";
        if (IsRodLike) return "RodLike";
        if (IsSheetLike) return "SheetLike";
        if (IsLikelySolid && IsBlobLike) return "BlobLikeSolid";
        if (IsLikelySolid) return "Solid";
        if (IsOpenSurface) return "OpenSurface";
        if (IsPlaneLike) return "Planar";
        if (IsLineLike) return "Linear";
        return "GeneralMesh";
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    public static double SafeRatio(double numerator, double denominator)
        => Math.Abs(denominator) > Eps ? numerator / denominator : double.NaN;

    public static double BoxSurfaceArea(Vector3 size)
        => 2.0 * (size.X * size.Y + size.X * size.Z + size.Y * size.Z);

    public static double MaxComponent(Vector3 v)
        => Math.Max(v.X, Math.Max(v.Y, v.Z));

    public static double MinComponent(Vector3 v)
        => Math.Min(v.X, Math.Min(v.Y, v.Z));

    public static double MinNonZeroComponent(Vector3 v)
    {
        var min = double.PositiveInfinity;

        if (Math.Abs(v.X) > Eps) min = Math.Min(min, Math.Abs(v.X));
        if (Math.Abs(v.Y) > Eps) min = Math.Min(min, Math.Abs(v.Y));
        if (Math.Abs(v.Z) > Eps) min = Math.Min(min, Math.Abs(v.Z));

        return double.IsPositiveInfinity(min) ? double.NaN : min;
    }

    public static int TryInt(Func<int> f)
    {
        try
        {
            return f();
        }
        catch
        {
            return 0;
        }
    }

    public static double Clamp01(double x)
        => double.IsNaN(x) ? 0.0 : Math.Max(0.0, Math.Min(1.0, x));

    public static double AverageFinite(params double[] xs)
        => AverageFinite((IEnumerable<double>)xs);

    public static double AverageFinite(IEnumerable<double> xs)
    {
        var sum = 0.0;
        var count = 0;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            sum += x;
            count++;
        }

        return count > 0 ? sum / count : double.NaN;
    }

    public static double EntropyTerm(double x)
        => x > Eps ? -x * Math.Log(x) : 0.0;

    public static double PcaRadialDistanceYZ(Point3D p)
    {
        var y = p.Vector3.Y;
        var z = p.Vector3.Z;
        return Math.Sqrt(y * y + z * z);
    }

    public double PointRatioNearX(double x, double tolerance)
    {
        if (NormalizedPoints.Count == 0 || tolerance <= Eps)
            return double.NaN;

        var count = 0;

        foreach (var p in NormalizedPoints)
        {
            if (Math.Abs(p.Vector3.X - x) <= tolerance)
                count++;
        }

        return SafeRatio(count, NormalizedPoints.Count);
    }

    public static double MinFinite(IEnumerable<double> xs)
    {
        var min = double.PositiveInfinity;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            min = Math.Min(min, x);
        }

        return double.IsPositiveInfinity(min) ? double.NaN : min;
    }

    public static double MaxFinite(IEnumerable<double> xs)
    {
        var max = double.NegativeInfinity;

        foreach (var x in xs)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                continue;

            max = Math.Max(max, x);
        }

        return double.IsNegativeInfinity(max) ? double.NaN : max;
    }

    public static double StdDevFinite(IEnumerable<double> xs)
    {
        var values = xs
            .Where(x => !double.IsNaN(x) && !double.IsInfinity(x))
            .ToArray();

        if (values.Length == 0)
            return double.NaN;

        var avg = values.Average();
        var sumSq = 0.0;

        foreach (var x in values)
        {
            var d = x - avg;
            sumSq += d * d;
        }

        return Math.Sqrt(sumSq / values.Length);
    }

    // =========================================================================
    // Suggestions
    // =========================================================================
    //
    // 1. Split this class into nested groups or separate feature records:
    //    CountFeatures, AabbFeatures, ObbFeatures, SurfaceFeatures,
    //    VertexDistributionFeatures, PcaFeatures, NormalFeatures,
    //    TopologyFeatures, TessellationFeatures, and ClassificationFeatures.
    //    The current single flat class is convenient for tabular export, but it is
    //    becoming hard to maintain.
    //
    // 2. Consider generating the scalar pass-through properties automatically.
    //    Many properties are just Vector3 component expansions for table export.
    //    A descriptor-based export layer could expand Vector3 fields to X/Y/Z
    //    without requiring hand-written duplicate properties.
    //
    // 3. Move thresholds into a MeshFeatureThresholds options object.
    //    Values such as 0.75, 0.85, 20.0, and 10000.0 are domain assumptions.
    //    Making them configurable will make the feature set more useful across
    //    BIM, CAD, scan meshes, mechanical parts, and visualization assets.
    //
    // 4. Prefer explicit names over aliases in new code.
    //    For example, prefer AabbVolume over BoundsVolume, FaceNormalAverage over
    //    MeanNormal, and PcaPrincipalAxisVerticality over Verticality. The aliases
    //    are useful for compatibility but can eventually be removed.
    //
    // 5. Cache expensive topology-derived values if this class is accessed through
    //    data binding or table export. Properties such as BoundaryEdgeCount,
    //    NonManifoldEdgeCount, and BoundaryLoopCount may enumerate or allocate.
    //
    // 6. Avoid catch-all TryInt for expected geometry states.
    //    It is useful for exploratory analytics, but production code should
    //    distinguish "not computed", "invalid topology", and "unexpected bug".
    //
    // 7. Add unit-aware metadata. Features such as area, volume, length, density,
    //    and elevation should carry unit semantics, especially when exported from
    //    Revit, IFC, GLB, or mixed source models.
    //
    // 8. Add percentile-based statistics. For skewed meshes, P05/P50/P95 edge
    //    lengths, face areas, triangle aspect ratios, and vertex elevations are
    //    often more robust than min/max.
    //
    // 9. Add per-axis and oriented footprint features. AabbFootprintArea is useful,
    //    but OBB footprint, convex-hull footprint, and projected XY area may be
    //    better for BIM classification.
    //
    // 10. Add domain-specific BIM heuristics only if they are clearly separated
    //     from raw features. For example: IsWallLike, IsFloorLike, IsColumnLike,
    //     IsBeamLike, IsPipeLike, IsDuctLike, IsPanelLike, and IsFurnitureLike.
    //
    // 11. Remove or de-emphasize weak features if they do not help downstream
    //     models. Examples to validate empirically: BoxLikeScore, SurfaceLikeScore,
    //     SolidLikeScore, VerticalFacingScore, and SurfaceCompactness.
    //
    // 12. Consider adding feature provenance and reliability flags. Some features
    //     are reliable for any mesh, while others only make sense for closed,
    //     manifold, consistently oriented triangle meshes.
}
