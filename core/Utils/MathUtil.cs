using System;

namespace Ara3D.Utils
{
    public static class MathUtil
    {
        public static double Abs(this double x) => Math.Abs(x);
        public static double Acos(this double x) => Math.Acos(x);
        public static double Asin(this double x) => Math.Asin(x);
        public static double Atan(this double x) => Math.Atan(x);
        public static double Cos(this double x) => Math.Cos(x);
        public static double Cosh(this double x) => Math.Cosh(x);
        public static double Exp(this double x) => Math.Exp(x);
        public static double Log10(this double x) => Math.Log10(x);
        public static double Sin(this double x) => Math.Sin(x);
        public static double Sinh(this double x) => Math.Sinh(x);
        public static double Sqrt(this double x) => Math.Sqrt(x);
        public static double Tan(this double x) => Math.Tan(x);
        public static double Tanh(this double x) => Math.Tanh(x);

        public static float Abs(this float x) => (float)Math.Abs(x);
        public static float Acos(this float x) => (float)Math.Acos(x);
        public static float Asin(this float x) => (float)Math.Asin(x);
        public static float Atan(this float x) => (float)Math.Atan(x);
        public static float Cos(this float x) => (float)Math.Cos(x);
        public static float Cosh(this float x) => (float)Math.Cosh(x);
        public static float Exp(this float x) => (float)Math.Exp(x);
        public static float Log10(this float x) => (float)Math.Log10(x);
        public static float Sin(this float x) => (float)Math.Sin(x);
        public static float Sinh(this float x) => (float)Math.Sinh(x);
        public static float Sqrt(this float x) => (float)Math.Sqrt(x);
        public static float Tan(this float x) => (float)Math.Tan(x);
        public static float Tanh(this float x) => (float)Math.Tanh(x);

        public static double Atan2(this double x, double y) => Math.Atan2(x, y);
        public static double Log(this double x, double y) => Math.Log(x, y);
        public static double Pow(this double x, double y) => Math.Pow(x, y);

        public static float Atan2(this float x, float y) => (float)Math.Atan2(x, y);
        public static float Log(this float x, float y) => (float)Math.Log(x, y);
        public static float Pow(this float x, float y) => (float)Math.Pow(x, y);

        public static int Sign(this float x) => x > 0 ? 1 : x < 0 ? -1 : 0;
        public static float Inverse(this float x) => (float)1 / x;
        public static float Truncate(this float x) => (float)Math.Truncate(x);
        public static float Ceiling(this float x) => (float)Math.Ceiling(x);
        public static float Floor(this float x) => (float)Math.Floor(x);
        public static float Round(this float x) => (float)Math.Round(x);
        public static float ToRadians(this float x) => (float)(x * Math.PI / 180.0);
        public static float ToDegrees(this float x) => (float)(x * 180.0 / Math.PI);
        public static float Distance(this float v1, float v2) => (v1 - v2).Abs();
        public static bool IsInfinity(this float v) => float.IsInfinity(v);
        public static bool IsNaN(this float v) => float.IsNaN(v);
        public static float Smoothstep(this float v) => v * v * (3 - 2 * v);
        public static int Sign(this double x) => x > 0 ? 1 : x < 0 ? -1 : 0;

        public static double Truncate(this double x) => (double)Math.Truncate(x);
        public static double Ceiling(this double x) => (double)Math.Ceiling(x);
        public static double Floor(this double x) => (double)Math.Floor(x);
        public static double Round(this double x) => (double)Math.Round(x);
        public static double ToRadians(this double x) => (x * Math.PI / 180.0);
        public static double ToDegrees(this double x) => (x * 180.0 / Math.PI);
        public static double Distance(this double v1, double v2) => (v1 - v2).Abs();
        public static bool IsInfinity(this double v) => double.IsInfinity(v);
        public static bool IsNaN(this double v) => double.IsNaN(v);
        public static double Smoothstep(this double v) => v * v * (3 - 2 * v);

        public static bool AlmostEquals(this double x, double y, double tolerance = 1E-15) => (x - y).Abs() <= Math.Max(x.Abs(), y.Abs()) * tolerance;
        public static bool AlmostZero(this double x, double tolerance = 1E-15) => x.AlmostEquals(0.0, tolerance);

        // https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
        // https://learn.microsoft.com/en-gb/archive/blogs/ericlippert/whats-the-difference-remainder-vs-modulus
        public static int Mod(this int x, int m) => m < 0 ? Mod(x, -m) : (x % m + m) % m;
        public static int Rem(this int x, int m) => x % m;
        public static long Mod(this long x, long m) => m < 0 ? Mod(x, -m) : (x % m + m) % m;
        public static long Rem(this long x, long m) => x % m;
        public static float Mod(this float x, float m) => m < 0 ? Mod(x, -m) : (x % m + m) % m;
        public static float Rem(this float x, float m) => x % m;
        public static double Mod(this double x, double m) => m < 0 ? Mod(x, -m) : (x % m + m) % m;
        public static double Rem(this double x, double m) => x % m;
    }
}
