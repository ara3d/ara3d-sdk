using Ara3D.Models;
using Ara3D.Studio.API;
using Plato;

namespace Ara3D.Studio.Samples
{
    public class StairsAndPlatforms : IModelGenerator
    {
        public int NumberOfRowsOfPlatforms;
        public int NumberOfColumnsOfPlatforms;
        public float MinPlatformElevation;
        public float MaxPlatformElevation;
        public float MinPlatformWidth;
        public float PlatformSide;
        public float PlatformThickness;
        public float StairWidth;
        public float StairThickness;

        public List<Bounds3D> CreateStairs(Line3D line, int count)
        {
            // TODO:
            throw new NotImplementedException();
        }

        public static IReadOnlyList<Vector3> SideCenters(Quad3D quad)
            => [quad.LineA.Center, quad.LineB.Center, quad.LineC.Center, quad.LineD.Center];

        public static Line3D ClosestPoints(IReadOnlyList<Vector3> pointsA, IReadOnlyList<Vector3> pointsB)
            => pointsA.SelectMany(a => Enumerable.Select(pointsB, b => new Line3D(a, b))).MinBy(line => line.Length);

        public Vector2 Platform(Quad3D a, Quad3D b)
        {
            throw new NotImplementedException();
        }

        public Model Evaluate()
        {
            throw new NotImplementedException();
        }
    }
}
