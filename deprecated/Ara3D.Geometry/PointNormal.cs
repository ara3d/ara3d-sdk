using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class PointNormal
    {
        public Vector3 Point { get; }
        public Vector3 Normal { get; }

        public PointNormal(Vector3 point, Vector3 normal)
            => (Point, Normal) = (point, normal);
    }   

    public class PointNormal2D
    {
        public Vector2 Point { get; }
        public Vector2 Normal { get; }

        public PointNormal2D(Vector2 point, Vector2 normal)
            => (Point, Normal) = (point, normal);
    }

}