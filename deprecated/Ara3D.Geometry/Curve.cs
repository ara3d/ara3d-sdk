using System;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class Curve<T> : Procedural<float, T>, ICurve<T>
    {
        public Curve(Func<float, T> func, bool closed)
            : base(func)
            => Closed = closed;

        public bool Closed { get; }
    }

    public class Curve2D : Procedural2D<float>, ICurve2D
    {
        public Curve2D(Func<float, Vector2> func, bool closed) : base(func)
        {
            Closed = closed;
        }

        public bool Closed { get; }
    }
    
    public class Curve3D : Procedural3D<float>, ICurve3D
    {
        public Curve3D(Func<float, Vector3> func, bool closed) : base(func)
        {
            Closed = closed;
        }

        public bool Closed { get; }
    }

    public static class CurveExtensions
    {
        public static IArray<T> Sample<T>(this ICurve<T> self, int n)
            => (self.Closed ? n.InterpolateExclusive() : n.InterpolateInclusive()).Select(self.Eval);
    }
}