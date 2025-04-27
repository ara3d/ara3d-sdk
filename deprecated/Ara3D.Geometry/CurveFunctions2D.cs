using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class CurveFunctions2D
    {
        public static Vector2 Circle(this float t)
            => t.Turns().Circle();

        public static Vector2 Circle(this Angle t)
            => (t.Sin, t.Cos);

        // https://en.wikipedia.org/wiki/Lissajous_curve
        public static Vector2 Lissajous(this Angle t, int kx, int ky)
            => ((kx * t).Cos, (ky * t).Sin);

        public static Vector2 Lissajous(this float t, int kx, int ky)
            => t.Turns().Lissajous(kx, ky);

        // https://en.wikipedia.org/wiki/Butterfly_curve_(transcendental)
        public static Vector2 ButterflyCurve(this float t)
            => ButterflyCurve(t.Turns() / 6);

        public static Vector2 ButterflyCurve(this Angle t)
            => ((t * (t.Cos.Exp() - 2 * (4 * t).Cos - (t / 12).Sin.Pow(5))).Sin,
                (t * (t.Cos.Exp() - 2 * (4 * t).Cos - (t / 12).Sin.Pow(5))).Cos);
        
        //===
        // 2D parametric functions, created from 1D 
        //===

        public static Func<float, Vector2> Curve1Dto2D(this Func<float, float> f)
            => x => (x, f(x));

        public static Vector2 Parabola(this float t)
            => Curve1Dto2D(CurveFunctions1D.Parabola)(t);

        public static Vector2 Line(this float t, float m, float b)
            => Curve1Dto2D(x => CurveFunctions1D.Line(x, m, b))(t);

        public static Vector2 SinCurve(this float t)
            => Curve1Dto2D(MathOps.Sin)(t);

        public static Vector2 CosCurve(this float t)
            => Curve1Dto2D(MathOps.Cos)(t);

        public static Vector2 TanCurve(this float t)
            => Curve1Dto2D(MathOps.Tan)(t);
    }
}