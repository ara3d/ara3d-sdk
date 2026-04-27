namespace Ara3D.Studio.Samples;

public class MeshStats
{
    public Topology Topology { get; }
    public PrincipalComponentAnalysis Pca { get; }
    public TriangleMesh3D Mesh { get; }
    public IReadOnlyList<Point3D> NormalizedPoints { get; }

    public MeshStats(TriangleMesh3D mesh)
    {

    }

    public int PointCount { get; }
    public int EdgeCount { get; }
    public int FaceCount { get; }

    public readonly double SumX;
    public readonly double SumY;
    public readonly double SumZ;

    public readonly double MeanX;
    public readonly double MeanY;
    public readonly double MeanZ;

    public readonly double EigenX;
    public readonly double EigenY;
    public readonly double EigenZ;

    public readonly double LargestEigenValue;
    public readonly double MiddleEigenValue;
    public readonly double SmallestEigenValue;

    public readonly Vector3 PrincipalAxis;
    public readonly Vector3 SecondaryAxis;
    public readonly Vector3 TertiaryAxis;

    public readonly double Linearity;
    public readonly double Planarity;
    public readonly double Scattering;
}