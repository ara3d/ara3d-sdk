namespace Plato.Geometry
{
    public static class ParametricSurfaces
    {
        public static ParametricSurface Sphere 
            => new(SurfaceFunctions.Sphere, true, true);

        public static ParametricSurface Torus(Number r1, Number r2) 
            => new(uv => uv.Torus(r1, r2), true, true);

        public static ParametricSurface MonkeySaddle 
            => new(SurfaceFunctions.MonkeySaddle, false, false);

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
    }
}