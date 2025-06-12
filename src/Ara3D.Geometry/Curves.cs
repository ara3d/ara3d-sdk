namespace Ara3D.Geometry
{

    public static class Curves
    {
        public static Point3D Lerp(Point3D p0, Point3D p1, Number t)
            => p0.Lerp(p1, t);

        public static Curve3D QuadraticBezier(this Point3D p0, Point3D p1, Point3D p2)
            => new(t => Lerp(Lerp(p0, p1, t), Lerp(p1, p2, t), t));

        public static Curve3D RemapInput(this Curve3D curve3D, Number from, Number to)
            => curve3D.RemapInput(t => from.Lerp(to, t));

        public static Curve3D Reverse(this Curve3D curve3D)
            => curve3D.RemapInput(t => 1f - t);

        public static Curve3D RemapInput(this Curve3D curve3D, Func<Number, Number> f)
            => new(t => curve3D.Eval(f(t)));

        public static Curve3D Mix(this Curve3D curve0, Curve3D curve1, Number amount)
            => new(t => Lerp(curve0.Eval(t), curve1.Eval(t), amount));

        public static QuadGrid3D RuledSurface(this Curve3D curve0, Curve3D curve1, Integer samples)
            => curve0.Sample(samples).ToQuadGrid3D(curve1.Sample(samples), false, false);

        public static Curve3D LineCurve(this Line3D line)
            => new(line.Lerp);

        public static Curve3D LineCurve(this Point3D p0, Point3D p1)
            => p0.LineTo(p1).LineCurve();

        public static Curve3D Circle
            => new(t => (t.Turns.Cos, t.Turns.Sin, 0));

        public static Curve3D Helix(Number height, Number revolutions)
            => new(t => (
                (t * revolutions).Turns.Sin,
                (t * revolutions).Turns.Cos,
                t * height));

        public static Curve3D Spiral(Number revolutions, Number height)
            => new(t => new Vector3(
                (t * revolutions).Turns.Sin,
                (t * revolutions).Turns.Cos, height * (1f - t)) * t);
    }
}
