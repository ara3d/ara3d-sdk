using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Polar_coordinate_system#Polar_equation_of_a_curve
    /// Polar equations are functions from an angle t to a radius r.
    /// </summary>
    public static class PolarFunctions
    {
        public static float Circle(Angle t) 
            => 1;
        
        // https://en.wikipedia.org/wiki/Lima%C3%A7on
        public static float Limacon(Angle t, float a, float b) 
            => b + a * t.Cos;

        // https://en.wikipedia.org/wiki/Cardioid
        public static float Cardoid(Angle t) 
            => Limacon(t, 1, 1);

        // https://en.wikipedia.org/wiki/Rose_(mathematics)
        public static float Rose(Angle t, int k) 
            => k * t.Cos;

        // https://en.wikipedia.org/wiki/Archimedean_spiral
        public static float ArchmideanSpiral(Angle t, float a, float b)  
            => a + b * t;

        // https://en.wikipedia.org/wiki/Conic_section
        public static float ConicSection(Angle t, float eccentricity, float semiLatusRectum) 
            => semiLatusRectum / (1 - eccentricity * t.Cos);

        // https://en.wikipedia.org/wiki/Lemniscate_of_Bernoulli
        public static float LemniscateOfBernoulli(Angle t, float a)
            => (a.Sqr() * (2 * t).Cos).Sqrt();
    }
}