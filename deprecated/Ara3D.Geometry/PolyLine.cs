using System;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public abstract class PolyLine<T> : IPolyLine<T>
    {
        protected PolyLine(IArray<T> points, bool closed)
            => (Points, Closed) = (points, closed);
        public bool Closed { get; }
        public IArray<T> Points { get; }
        public abstract ILineSegment<T> Segment(int i);
    }

    public class PolyLine2D : PolyLine<Vector2>, 
        IPolyLine2D, IDeformable2D<PolyLine2D>
    {
        public PolyLine2D(IArray<Vector2> points, bool closed)
            : base(points, closed) { }
            
        public override ILineSegment<Vector2> Segment(int i)
            => new LineSegment2D(this.Point(i), this.Point(i + 1));

        IPolyLine2D IDeformable2D<IPolyLine2D>.Deform2D(Func<Vector2, Vector2> f)
            => new PolyLine2D(Points.Select(f), Closed);

        PolyLine2D IDeformable2D<PolyLine2D>.Deform2D(Func<Vector2, Vector2> f)
            => new PolyLine2D(Points.Select(f), Closed);
    }

    public class PolyLine3D : PolyLine<Vector3>, 
        IPolyLine3D, IDeformable<PolyLine3D>
    {
        public PolyLine3D(IArray<Vector3> points, bool closed)
            : base(points, closed) { }

        public override ILineSegment<Vector3> Segment(int i)
            => new LineSegment3D(this.Point(i), this.Point(i + 1));

        public PolyLine3D Deform(Func<Vector3, Vector3> f)
            => new PolyLine3D(Points.Select(f), Closed);

        IPolyLine3D IDeformable<IPolyLine3D>.Deform(Func<Vector3, Vector3> f)
            => Deform(f);

        PolyLine3D ITransformable<PolyLine3D>.Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));

        public IPolyLine3D Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));
    }

    public static class PolyLineExtensions
    {
        public static int NumSegments<T>(this IPolyLine<T> self)
            => self.Points.Count - 1 + (self.Closed ? 1 : 0);

        public static T Point<T>(this IPolyLine<T> self, int i)
            => self.Points.ElementAtModulo(i);

        public static IArray<ILineSegment<T>> Segments<T>(this IPolyLine<T> self)
            => self.NumSegments().Select(self.Segment);

        public static PolyLineCurve<T> Curve<T>(this IPolyLine<T> self) 
            => new PolyLineCurve<T>(self);

        public static IPolyLine3D To3D(this IPolyLine2D self)
            => new PolyLine3D(self.Points.Select(p => p.ToVector3()), self.Closed);

        public static IPolyLine2D ToPolyLine2D(this IArray<Vector2> self, bool closed = false)
            => new PolyLine2D(self, closed);
    }
}