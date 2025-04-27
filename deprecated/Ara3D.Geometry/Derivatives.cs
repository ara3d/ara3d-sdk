using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class Derivatives
    {
        public const float DefaultSampleResolution = 1.0e-6f;

        public static (T1, T1) Select<T0, T1>(this (T0, T0) self, Func<T0, T1> f)
            => (f(self.Item1), f(self.Item2));

        public static (T, T) OutputInterval<T>(Func<float, T> f, float t, float res = DefaultSampleResolution)
            => InputInterval(t, res).Select(f);

        public static float Subtract(this (float, float) self)
            => self.Item2 - self.Item1;

        public static Vector2 Subtract(this (Vector2, Vector2) self)
            => self.Item2 - self.Item1;

        public static Vector3 Subtract(this (Vector3, Vector3) self)
            => self.Item2 - self.Item1;

        public static Vector4 Subtract(this (Vector4, Vector4) self)
            => self.Item2 - self.Item1;

        public static (float, float) InputInterval(float t, float res = DefaultSampleResolution)
        {
            var t1 = t + res;
            if (t1 > 1.0)
            {
                t1 = t - res;
                return (t1, t);
            }
            return (t, t1);
        }

        public static float Derivative(this Func<float, float> self, float t, float res = DefaultSampleResolution)
            => OutputInterval(self, t, res).Subtract();

        public static Vector2 Derivative(this Func<float, Vector2> self, float t, float res = DefaultSampleResolution)
            => OutputInterval(self, t, res).Subtract();

        public static Vector3 Derivative(this Func<float, Vector3> self, float t, float res = DefaultSampleResolution)
            => OutputInterval(self, t, res).Subtract();

        public static Vector4 Derivative(this Func<float, Vector4> self, float t, float res = DefaultSampleResolution)
            => OutputInterval(self, t, res).Subtract();
    }
}