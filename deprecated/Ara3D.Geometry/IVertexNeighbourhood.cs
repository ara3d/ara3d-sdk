using Ara3D.Collections;
using Ara3D.Mathematics;
using System;

namespace Ara3D.Geometry
{
    /// <summary>
    /// One-ring neighbourhood of vertices. Useful for computing
    /// curvature, cotangent weights, subdivision surfaces. 
    /// </summary>
    public interface IVertexNeighbourhood
        : ITransformable<IVertexNeighbourhood>, IDeformable<IVertexNeighbourhood>
    {
        Vector3 Center { get; }
        IArray<Vector3> Neighbours { get; }
    }

    public static class VertexNeighbourhoodExtensions
    {
        public static Vector3 Neighbour(this IVertexNeighbourhood self, int i)
            => self.Neighbours.ElementAtModulo(i);

        public static Vector3 EdgeMidPoint(this IVertexNeighbourhood self, int i)
            => self.Center.Average(self.Neighbour(i));

        public static IArray<Vector3> EdgeMidPoints(this IVertexNeighbourhood self)
            => self.SelectNeighbours(EdgeMidPoint);

        public static Vector3 FaceCenter(this IVertexNeighbourhood self, int i)
            => self.Triangle(i).MidPoint;

        public static IArray<Vector3> FaceCenters(this IVertexNeighbourhood self)
            => self.SelectNeighbours(FaceCenter);

        public static int NumNeighbours(this IVertexNeighbourhood self)
            => self.Neighbours.Count;

        public static IArray<T> SelectNeighbours<T>(this IVertexNeighbourhood self, Func<int, T> func)
            => self.NumNeighbours().Select(func);

        public static IArray<T> SelectNeighbours<T>(this IVertexNeighbourhood self, Func<IVertexNeighbourhood, int, T> func)
            => self.NumNeighbours().Select(i => func(self, i));

        public static IArray<Triangle> Triangles(this IVertexNeighbourhood self)
            => self.SelectNeighbours(Triangle);

        public static Triangle Triangle(this IVertexNeighbourhood self, int i)
            => (self.Center, self.Neighbour(i), self.Neighbour(i + 1));

        public static Vector3 OutVector(this IVertexNeighbourhood self, int i)
            => self.Neighbour(i) - self.Center;

        public static IArray<Vector3> OutVectors(this IVertexNeighbourhood self)
            => self.SelectNeighbours(OutVector);

        public static Vector3 InVector(this IVertexNeighbourhood self, int i)
            => self.Center - self.Neighbour(i);

        public static IArray<Vector3> InVectors(this IVertexNeighbourhood self)
            => self.SelectNeighbours(InVector);

        public static Vector3 Normal(this IVertexNeighbourhood self, int i)
            => self.OutVector(i).Cross(self.OutVector(i + 1)).Normalize();

        public static IArray<Vector3> Normals(this IVertexNeighbourhood self)
            => self.SelectNeighbours(Normal);

        public static Vector3 AverageNormal(this IVertexNeighbourhood self)
            => self.Normals().Average();

        public static float TriangleArea(this IVertexNeighbourhood self, int i)
            => self.Triangle(i).Area;

        public static Vector3 AreaWeightedNormal(this IVertexNeighbourhood self)
            => self.Normals().Average();

        public static float Angle(this IVertexNeighbourhood self, int i)
            => self.OutVector(i).Angle(self.OutVector(i + 1));

        public static IArray<float> Angles(this IVertexNeighbourhood self)
            => self.SelectNeighbours(Angle);

        public static Vector3 AngleWeightedNormal(this IVertexNeighbourhood self)
            => self.Normals().Average(self.Angles());

        public static float BarycentricCellArea(this IVertexNeighbourhood self)
            => (float)self.SelectNeighbours(TriangleArea).Sum() / 3;

        /// <summary>
        /// https://rodolphe-vaillant.fr/entry/69/c-code-for-cotangent-weights-over-a-triangular-mesh
        /// https://doc.cgal.org/latest/Weights/group__PkgWeightsRefCotangentWeights.html
        /// </summary>
        public static float CotangentWeight(this IVertexNeighbourhood self, int i)
        {
            var prev = self.Neighbour(i - 1);
            var next = self.Neighbour(i + 1);
            var other = self.Neighbour(i);
            var p = self.Center;
            var alpha = (p - prev).Angle(other - prev);
            var beta = (p - next).Angle(other - next);
            return alpha.Cot() + beta.Cot();
        }

        public static Vector3 CotangentWeightedVector(this IVertexNeighbourhood self, int i)
            => self.CotangentWeight(i) * self.OutVector(i);

        public static Vector3 SumOfCotangentWeightedVectors(this IVertexNeighbourhood self)
            => self.SelectNeighbours(CotangentWeightedVector).Sum();

        /// <summary>
        /// https://en.wikipedia.org/wiki/Discrete_Laplace_operator
        /// </summary>
        public static Vector3 DiscreteLaplace(this IVertexNeighbourhood self)
            => self.SumOfCotangentWeightedVectors() / (self.BarycentricCellArea() * 2);

        /// <summary>
        /// https://en.wikipedia.org/wiki/Catmull%E2%80%93Clark_subdivision_surface
        /// </summary>
        public static Vector3 CatmullClarkCenter(this IVertexNeighbourhood self)
            => (self.FaceCenters().Average()
                + 2 * self.EdgeMidPoints().Average()
                + (self.NumNeighbours() - 3) * self.Center) / 3;
    }
}