using System.Diagnostics;

namespace Ara3D.Geometry;

public sealed class PrincipalComponentAnalysis
{
    public int Count { get; }
    public int NonFinitePointCount { get; }
    public Vector3 Mean { get; }
    public SymmetricMatrix3x3 Covariance { get; }
    public EigenDecomposition3D Eigen { get; }

    public double LargestEigenValue => Eigen.LargestValue;
    public double MiddleEigenValue => Eigen.MiddleValue;
    public double SmallestEigenValue => Eigen.SmallestValue;

    public Vector3 PrincipalAxis => Eigen.LargestVector;
    public Vector3 SecondaryAxis => Eigen.MiddleVector;
    public Vector3 TertiaryAxis => Eigen.SmallestVector;

    public Line3D PrincipalLine => new(Mean, Mean + PrincipalAxis);
    public Line3D SecondaryLine => new(Mean, Mean + SecondaryAxis);
    public Line3D TertiaryLine => new(Mean, Mean + TertiaryAxis);

    public double TotalVariance => Eigen.TotalVariance;

    public bool IsPointLike => TotalVariance <= GeometryUtil.DefaultEpsilon;

    public double Linearity { get; }
    public double Planarity { get; }
    public double Scattering { get; }

    public Axes3D Axes => new(PrincipalAxis, SecondaryAxis, TertiaryAxis);
    public Frame3D Frame => new(Mean, Axes.ToOrthonormalBasis());

    public PrincipalComponentAnalysis(
        IReadOnlyList<Vector3> srcPoints,
        IReadOnlyList<double>? srcWeights = null)
    {
        if (srcPoints == null)
            throw new ArgumentNullException(nameof(srcPoints));

        if (srcWeights != null && srcWeights.Count != srcPoints.Count)
            throw new ArgumentException("Weights count must match points count.", nameof(srcWeights));
        
        var points = new List<Vector3>();
        var weights = srcWeights != null ? new List<double>() : null;
        for (var i=0; i < srcPoints.Count; i++)
        {
            var pt = srcPoints[i];
            if (!pt.IsFinite())
                continue;
            points.Add(pt);
            if (srcWeights != null && weights != null) 
                weights.Add(srcWeights[i]);
        }

        Debug.Assert(weights == null || points.Count == weights.Count);

        Count = points.Count;
        NonFinitePointCount = srcPoints.Count - Count;

        if (Count == 0)
            return;

        Covariance = SymmetricMatrix3x3.WeightedCovariance(points, out var mean, weights);
        Mean = mean;
        Eigen = EigenDecomposition3D.Decompose(Covariance);

        if (LargestEigenValue > GeometryUtil.DefaultEpsilon)
        {
            Linearity = (LargestEigenValue - MiddleEigenValue) / LargestEigenValue;
            Planarity = (MiddleEigenValue - SmallestEigenValue) / LargestEigenValue;
            Scattering = SmallestEigenValue / LargestEigenValue;
        }
        else
        {
            Linearity = 0;
            Planarity = 0;
            Scattering = 0;
        }

        Debug.Assert(Mean.IsFinite());
        Debug.Assert(PrincipalAxis.IsUnit());
        Debug.Assert(SecondaryAxis.IsUnit());
        Debug.Assert(TertiaryAxis.IsUnit());
        Debug.Assert(LargestEigenValue >= MiddleEigenValue - GeometryUtil.DefaultEpsilon);
        Debug.Assert(MiddleEigenValue >= SmallestEigenValue - GeometryUtil.DefaultEpsilon);
    }

    public bool IsMostlyLinear(double threshold = 0.8)
        => Linearity >= threshold;

    public bool IsMostlyPlanar(double threshold = 0.8)
        => Planarity >= threshold;

    public bool IsMostlyVolumetric(double threshold = 0.2)
        => Scattering >= threshold;

    public double SignedDistanceAlongPrincipalAxis(Vector3 p)
        => GeometryUtil.SignedDistanceAlongLine(p, Mean, PrincipalAxis);

    public double SignedDistanceAlongSecondaryAxis(Vector3 p)
        => GeometryUtil.SignedDistanceAlongLine(p, Mean, SecondaryAxis);

    public double SignedDistanceAlongTertiaryAxis(Vector3 p)
        => GeometryUtil.SignedDistanceAlongLine(p, Mean, TertiaryAxis);

    public Vector3 ProjectOntoPrincipalLine(Vector3 p)
        => GeometryUtil.ProjectOntoLine(p, Mean, PrincipalAxis);

    public double DistanceToPrincipalLine(Vector3 p)
        => GeometryUtil.DistanceToLine(p, Mean, PrincipalAxis);

    public double SignedDistanceToBestFitPlane(Vector3 p)
        => Vector3.Dot(p - Mean, TertiaryAxis);

    public double DistanceToBestFitPlane(Vector3 p)
        => Math.Abs(SignedDistanceToBestFitPlane(p));

    public Vector3 ProjectOntoBestFitPlane(Vector3 p)
        => p - (float)SignedDistanceToBestFitPlane(p) * TertiaryAxis;
}