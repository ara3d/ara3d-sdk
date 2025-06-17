namespace Ara3D.Geometry
{
    public static class ParametricSurfaces
    {
        public static ParametricSurface Sphere 
            => new(SurfaceFunctions.Sphere, true, true);

        public static ParametricSurface Torus(Number r1, Number r2) 
            => new(uv => uv.Torus(r1, r2), true, true);

        public static ParametricSurface MonkeySaddle 
            => new(SurfaceFunctions.MonkeySaddle, false, false);
        
        public static ParametricSurface Capsule
            => new(SurfaceFunctions.Capsule, true, false);

        /*
        public static ParametricSurface TorusKnot(Number r1, Number r2)
            => new(uv => uv.Torus(r1, r2), true, true);
        */
        
        public static ParametricSurface CrossedTrough
            => new(SurfaceFunctions.CrossedTrough, false, false);

        public static ParametricSurface SinPlusCos
            => new(SurfaceFunctions.SinPlusCos, false, false);

        public static ParametricSurface Plane 
            => new(SurfaceFunctions.Plane, false, false);

        public static ParametricSurface Disc 
            => new(SurfaceFunctions.Disc, false, false);

        public static ParametricSurface Cylinder 
            => new(SurfaceFunctions.Cylinder, false, false);

        public static ParametricSurface ConicalSection(Number r1, Number r2) =>
            new(uv => uv.ConicalSection(r1, r2), true, false);

        public static ParametricSurface Trefoil(Number r) 
            => new(uv => SurfaceFunctions.Trefoil(uv, r), true, true);

        public static Number Sinc(this Angle a)
            => a.AlmostZero ? 1 : (a.Sin / a.Radians);

        public static ParametricSurface PolarHeightFieldSurface(Func<Number, Number> f)
            => new(uv => Disc.Eval(uv).WithZ(f(uv.X)), true, false);

        public static ParametricSurface Sombrero
            => PolarHeightFieldSurface(x => (x * 6).Turns.Sinc());

        // Sinc function
        // Gaussian 

        // Torus knot?
        // Any lofted / swept surface 

        // Quadratic bezier patch? 

        // Any Extrude curve is a parmetric surface ( before it is a quad grid )
        // Any Ruled curve 

        // Could I do an object that is stretched in the middle? That is "discontinuous" ... sort of? For like uv <= 0.5 is one thing, and then after is something else. 

        // Rounded shape? 
        // Box? 


        // Elongation is really a third piece ...

        // Let's experiment with it. We need to cut, Remap the U domain, if above then we have a new parametrization ... if above we 

        // What can I 
    }
}