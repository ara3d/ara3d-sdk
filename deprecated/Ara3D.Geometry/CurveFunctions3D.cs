using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class CurveFunctions3D
    {
        public static Vector3 Circle3D(this float t)
            => t.Turns().Circle3D();

        public static Vector3 Circle3D(this Angle t)
            => (t.Sin, t.Cos, 0);

        // https://en.wikipedia.org/wiki/Torus_knot
        public static Vector3 TorusKnot(this Angle t, int p, int q)
        {
            var r = (q * t).Cos + 2;
            var x = r * (p * t).Cos;
            var y = r * (p * t).Sin;    
            var z = -(q * t).Sin;
            return (x, y, z);
        }

        // https://en.wikipedia.org/wiki/Trefoil_knot
        public static Vector3 TrefoilKnot(this Angle t)
            => (t.Sin + (2f * t).Sin * 2f,
                t.Cos + (2f * t).Cos * 2f,
                -(t * 3f).Sin);

        // https://en.wikipedia.org/wiki/Figure-eight_knot_(mathematics)
        public static Vector3 FigureEightKnot(this Angle t)
            => ((2 + (2 * t).Cos) * (3 * t).Cos,
                (2 + (2 * t).Cos) * (3 * t).Sin,
                (4 * t).Sin);

        // https://en.wikipedia.org/wiki/Parametric_equation#Helix
        public static Vector3 Helix(this float t, float revolutions)
            => ((t * revolutions).Turns().Sin, 
                (t* revolutions).Turns().Cos,
                t);
    }
}