using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    /// <summary>
    /// See also:
    /// Ara3D.Mathematics.MathOps
    /// Ara3D.Mathematics.Easing
    /// </summary>
    public static class CurveFunctions1D
    {
        // https://en.wikipedia.org/wiki/Linear_equation#Equation_of_a_line
        public static float Line(float x, float m, float b)
            => m * x + b;

        public static float Quadratic(float x, float a, float b, float c)
            => a * x.Sqr() + b * x + c;

        // https://en.wikipedia.org/wiki/Parabola
        public static float Parabola(float x)
            => x.Sqr();

        // https://mathworld.wolfram.com/StaircaseFunction.html
        public static float StaircaseFloor(float x, int steps)
            => (x * steps).Floor() / steps;

        // https://mathworld.wolfram.com/StaircaseFunction.html
        public static float StaircaseCeiling(float x, int steps)
            => (x * steps).Ceiling() / steps;

        // https://mathworld.wolfram.com/StaircaseFunction.html
        public static float StaircaseRound(float x, int steps)
            => (x * steps).Round() / steps;
    }
}