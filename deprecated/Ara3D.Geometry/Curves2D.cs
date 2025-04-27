namespace Ara3D.Geometry
{
    public static class Curves2D
    {
        public static readonly Curve2D Circle
            = new Curve2D(CurveFunctions2D.Circle, true);

        public static Curve2D Lissajou(int kx, int ky)
            => new Curve2D(t => t.Lissajous(kx, ky), true);

        public static Curve2D Parabola
            = new Curve2D(CurveFunctions2D.Parabola, false);

        public static Curve2D Line(float m, float b)
            => new Curve2D(t => t.Line(m, b), false);

        public static readonly Curve2D Sin
            = new Curve2D(CurveFunctions2D.SinCurve, false);
        
        public static readonly Curve2D Cos
            = new Curve2D(CurveFunctions2D.CosCurve, false);

        public static readonly Curve2D Tan
            = new Curve2D(CurveFunctions2D.TanCurve, false);
    }
}