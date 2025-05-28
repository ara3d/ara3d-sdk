using Plato;

namespace Ara3D.Geometry
{
    /// <summary>
    /// Based on
    /// https://rodolphe-vaillant.fr/entry/33/curvature-of-a-triangle-mesh-definition-and-computation 
    /// https://computergraphics.stackexchange.com/questions/1718/what-is-the-simplest-way-to-compute-principal-curvature-for-a-mesh-triangle/1719
    /// </summary>
    public class VertexCurvatureAnalysis
    {
        public VertexCurvatureAnalysis(IVertexNeighbourhood vn)
            => VN = vn;

        public IVertexNeighbourhood VN { get; }

        public float A => VN.BarycentricCellArea();
        public float SumAngles => (float)VN.Angles().Sum();
        public float GaussianCurvature => (Constants.TwoPi - SumAngles) / A;
        public float Kh => MeanCurvature;
        public float Kg => GaussianCurvature;
        public float MaximalCurvature => Kh + (Kh.Sqr() - Kg).Sqrt();
        public float MinimalCurvature => Kh - (Kh.Sqr() - Kg).Sqrt();
        
        public float MeanCurvature
        {
            get
            {
                var dl = VN.DiscreteLaplace();
                var hN = -(dl / 2);
                var absMean = dl.Length() / 2;
                var sign = VN.AverageNormal().Dot(hN);
                if (sign < 0) return -absMean;
                return absMean;
            }
        }
    }

    public static class CurvatureExtensions
    {
        public static VertexCurvatureAnalysis Curvature(this IVertexNeighbourhood neighbourhood)
            => new VertexCurvatureAnalysis(neighbourhood);
    }
}