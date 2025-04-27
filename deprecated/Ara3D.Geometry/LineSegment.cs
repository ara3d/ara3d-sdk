using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public interface ILineSegment<T>
    {
        T A { get; }
        T B { get; }
        float Length { get; }
        T Lerp(float f);
    }

    public abstract class LineSegment<T> : ILineSegment<T>
    {
        protected LineSegment(T a, T b)
            => (A, B) = (a, b);
        public T A { get; }
        public T B { get; }
        public abstract float Length { get; }
        public abstract T Lerp(float t);
    }

    public class LineSegment2D : LineSegment<Vector2>
    {
        public LineSegment2D(Vector2 a, Vector2 b)
            : base(a, b) { }
        public override float Length => A.Distance(B);
        public override Vector2 Lerp(float f) => A.Lerp(B, f);
    }

    public class LineSegment3D : LineSegment<Vector3>
    {
        public LineSegment3D(Vector3 a, Vector3 b)
            : base(a, b) { }
        public override float Length => A.Distance(B);
        public override Vector3 Lerp(float f) => A.Lerp(B, f);
    }

}