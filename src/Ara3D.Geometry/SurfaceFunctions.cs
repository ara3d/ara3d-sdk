namespace Ara3D.Geometry
{
    public static class SurfaceFunctions
    {
        public static Point2D Circle(this Number t)
            => (t.Turns.Sin, t.Turns.Cos);

        public static Point3D To3D(this Point2D p)
            => (p.X, p.Y, 0);

        public static Point3D Circle3D(this Number t)
            => t.Circle().To3D();

        public static Vector3 Sphere(this Vector2 uv)
            => Sphere(uv.X.Turns, uv.Y.Turns);

        public static Vector3 Sphere(Angle u, Angle v)
            => (-u.Cos * v.Sin, v.Cos, u.Sin * v.Sin);

        // https://en.wikipedia.org/wiki/Torus#Geometry
        public static Vector3 Torus(this Vector2 uv, Number r1, Number r2)
            => Torus(uv.X.Turns, uv.Y.Turns, r1, r2);

        public static Vector3 Torus(Angle u, Angle v, Number r1, Number r2)
            => ((r1 + r2 * u.Cos) * v.Cos,
                (r1 + r2 * u.Cos) * v.Sin,
                r2 * u.Sin);

        public static Vector3 Plane(this Vector2 uv)
            => (uv.X, uv.Y, 0);

        public static Vector3 Disc(this Vector2 uv)
            => uv.X.Circle3D() * uv.Y;

        public static Vector3 Cylinder(this Vector2 uv)
            => uv.X.Circle3D().WithZ(uv.Y);

        public static Vector3 ConicalSection(this Vector2 uv, Number r1, Number r2)
            => (uv.X.Circle3D() * r1.Lerp(r2, uv.Y)).WithZ(uv.Y);

        public static Vector3 Trefoil(this Vector2 uv, Number r)
            => Trefoil(uv.X.Turns, uv.Y.Turns, r);

        public static Vector3 Capsule(this Vector2 uv)
        {
            uv *= (1, 2);
            if (uv.Y < 0.5) return Sphere((uv.Y, uv.X));
            if (uv.Y > 1.5) return Sphere((uv.Y - 1, uv.X)) + (0, 0, 1);
            return Cylinder(uv + (0, -0.5f));
        }

        // https://commons.wikimedia.org/wiki/File:Parametric_surface_illustration_(trefoil_knot).png
        public static Vector3 Trefoil(Angle u, Angle v, Number r)
            => (r * (3 * u).Sin / (2 + v.Cos),
                r * (u.Sin + 2 * (2 * u).Sin) / (2 + (v + 1.Turns() / 3).Cos),
                r / 2 * (u.Cos - 2 * (2 * u).Cos) * (2 + v.Cos) * (2 + (v + 1.Turns() / 3).Cos) / 4);

        //===
        // Height fields converted into surface functions 
        //===

        public static Func<Vector2, Vector3> ToSurfaceFunction(this Func<Vector2, Number> f)
            => uv => (uv.X, uv.Y, f(uv));

        public static Vector3 MonkeySaddle(this Vector2 uv)
            => ToSurfaceFunction(HeightFieldFunctions.MonkeySaddle)(uv);

        public static Vector3 Handkerchief(this Vector2 uv)
            => ToSurfaceFunction(HeightFieldFunctions.Handkerchief)(uv);

        public static Vector3 CrossedTrough(this Vector2 uv)
            => ToSurfaceFunction(HeightFieldFunctions.CrossedTrough)(uv);

        public static Vector3 SinPlusCos(this Vector2 uv)
            => ToSurfaceFunction(HeightFieldFunctions.SinPlusCos)(uv);

        const float TAU = 6.2831853071795864769252866f; // 2π
        const float PI = 3.1415926535897932384626433f;

        // Created raw by Chat GPT 
        // To-be reviewed and improved .


        // 2 ▸ Ellipsoid (a, b, c)
        public static Vector3 Ellipsoid(Vector2 uv)
        {
            const float a = 1.5f, b = 1f, c = .6f;
            float θ = uv.X * TAU;
            float φ = (uv.Y - .5f) * PI;
            return new(
                a * MathF.Cos(φ) * MathF.Cos(θ),
                b * MathF.Cos(φ) * MathF.Sin(θ),
                c * MathF.Sin(φ));
        }

        // 3 ▸ Torus (R = major, r = minor)
        public static Vector3 Torus(Vector2 uv)
        {
            const float R = 1.0f, r = .35f;
            float θ = uv.X * TAU; // around tube
            float φ = uv.Y * TAU; // around hole
            float x = (R + r * MathF.Cos(θ)) * MathF.Cos(φ);
            float y = (R + r * MathF.Cos(θ)) * MathF.Sin(φ);
            float z = r * MathF.Sin(θ);
            return new(x, y, z);
        }

        // 4 ▸ Super-torus (squarer doughnut)
        public static Vector3 SuperTorus(Vector2 uv)
        {
            const float R = 1f, r = .3f, n = 4f; // n controls squareness
            float θ = (uv.X - .5f) * TAU;
            float φ = uv.Y * TAU;
            float ct = MathF.Cos(θ), st = MathF.Sin(θ);
            float cf = MathF.Cos(φ), sf = MathF.Sin(φ);
            float p = MathF.Pow(MathF.Abs(ct), n);
            float q = MathF.Pow(MathF.Abs(cf), n);
            float denom = MathF.Pow(p + q, 1f / n);
            float x = (R + r * ct / denom) * cf;
            float y = (R + r * ct / denom) * sf;
            float z = r * st / denom;
            return new(x, y, z);
        }

        // 5 ▸ Möbius strip (one-sided band)
        public static Vector3 Mobius(Vector2 uv)
        {
            float u = (uv.X - .5f) * TAU; // twist
            float v = uv.Y - .5f; // width (-½..½)
            float x = (1 + v * MathF.Cos(u * .5f)) * MathF.Cos(u);
            float y = (1 + v * MathF.Cos(u * .5f)) * MathF.Sin(u);
            float z = v * MathF.Sin(u * .5f);
            return new(x, y, z);
        }

        // 6 ▸ Klein bottle immersion (doesn’t self-intersect in 4-D!)
        public static Vector3 KleinBottle(Vector2 uv)
        {
            float u = (uv.X - .5f) * TAU;
            float v = uv.Y * TAU;
            float r = 4 * (1 - MathF.Cos(u) / 2);
            float x = (r * MathF.Cos(u) + 2 * MathF.Cos(v)) / 4f;
            float y = (r * MathF.Sin(u)) / 4f;
            float z = (2 * MathF.Sin(v)) / 4f;
            return new(x, y, z);
        }

        // 7 ▸ Helicoid (minimal spiral ramp)
        public static Vector3 Helicoid(Vector2 uv)
        {
            float u = (uv.X - .5f) * TAU; // radius direction
            float v = (uv.Y - .5f) * 4f * PI;
            float x = u * MathF.Cos(v);
            float y = u * MathF.Sin(v);
            float z = v;
            return new(x, y, z);
        }

        // 8 ▸ Catenoid (soap-film cousin of the helicoid)
        public static Vector3 Catenoid(Vector2 uv)
        {
            float u = (uv.X - .5f) * 2f; // radius
            float v = (uv.Y - .5f) * TAU; // angle
            float coshU = MathF.Cosh(u);
            float x = coshU * MathF.Cos(v);
            float y = coshU * MathF.Sin(v);
            float z = u;
            return new(x, y, z);
        }

        // 9 ▸ Enneper surface (minimal with self-intersections)
        public static Vector3 Enneper(Vector2 uv)
        {
            float u = (uv.X - .5f) * 2f;
            float v = (uv.Y - .5f) * 2f;
            float x = u - (u * u * u) / 3 + u * v * v;
            float y = v - (v * v * v) / 3 + v * u * u;
            float z = u * u - v * v;
            return new Vector3(x, y, z) * .5f;
        }

        // 10 ▸ Boy surface (immersion of the projective plane)
        public static Vector3 Boy(Vector2 uv)
        {
            float θ = uv.X * PI;
            float φ = uv.Y * 2f * PI;
            float a = MathF.Cos(θ) * (2 + MathF.Sin(θ)) / 3;
            float x = a * MathF.Cos(φ);
            float y = a * MathF.Sin(φ);
            float z = MathF.Sin(θ) / 3;
            return new(x, y, z);
        }

        // 11 ▸ Roman surface
        public static Vector3 Roman(Vector2 uv)
        {
            float u = (uv.X - .5f) * PI;
            float v = (uv.Y - .5f) * PI;
            float x = MathF.Sin(2 * u) * MathF.Sin(2 * v);
            float y = MathF.Sin(2 * u) * MathF.Cos(2 * v);
            float z = MathF.Cos(2 * u);
            return new Vector3(x, y, z) * .5f;
        }

        /*
        // 12 ▸ Monkey saddle (has three “valleys”)
        public static Vector3 MonkeySaddle(Vector2 uv)
        {
            float x = (uv.X - .5f) * 2f;
            float y = (uv.Y - .5f) * 2f;
            float z = x * x * x - 3f * x * y * y;
            return new(x, y, z);
        }
        */

        // 13 ▸ Hyperbolic paraboloid (classical saddle)
        public static Vector3 Hypar(Vector2 uv)
        {
            float x = (uv.X - .5f) * 2f;
            float y = (uv.Y - .5f) * 2f;
            float z = x * x - y * y;
            return new(x, y, z);
        }

        // 14 ▸ Circular paraboloid (reflector dish)
        public static Vector3 Paraboloid(Vector2 uv)
        {
            float r = uv.X; // 0‥1
            float θ = uv.Y * TAU;
            float x = r * MathF.Cos(θ);
            float y = r * MathF.Sin(θ);
            float z = r * r; // z = r²
            return new(x, y, z);
        }

        // 15 ▸ Right circular cone
        public static Vector3 Cone(Vector2 uv)
        {
            float r = uv.X; // base → tip
            float θ = uv.Y * TAU;
            float x = (1 - r) * MathF.Cos(θ);
            float y = (1 - r) * MathF.Sin(θ);
            float z = r;
            return new(x, y, z);
        }

        // 17 ▸ Sombrero / radial sinc ripple
        public static Vector3 Sombrero(Vector2 uv)
        {
            float r = uv.X * 4f; // larger disc
            float θ = uv.Y * TAU;
            float s = r == 0 ? 1 : MathF.Sin(r) / r;
            float x = r * MathF.Cos(θ);
            float y = r * MathF.Sin(θ);
            return new(x, y, s);
        }

        // 18 ▸ Seashell (logarithmic spiral surface)
        public static Vector3 Seashell(Vector2 uv)
        {
            float u = uv.X * 4f * PI; // turns
            float v = uv.Y; // 0‥1 across lip
            float r = MathF.Exp(u * .1f);
            float x = r * MathF.Cos(u) * (1 + v);
            float y = r * MathF.Sin(u) * (1 + v);
            float z = v * 2f - 1f;
            return new(x, y, z);
        }

        // 19 ▸ Valentine heart surface
        public static Vector3 Heart(Vector2 uv)
        {
            float θ = uv.X * TAU;
            float r = 1 - MathF.Sin(uv.Y * PI);
            float x = 16 * MathF.Pow(MathF.Sin(θ), 3) * r * .05f;
            float y = (13 * MathF.Cos(θ) - 5 * MathF.Cos(2 * θ)
                                         - 2 * MathF.Cos(3 * θ) - MathF.Cos(4 * θ)) * r * .05f;
            float z = uv.Y * 2 - 1;
            return new(x, y, z);
        }

        // 20 ▸ Super-ellipsoid (superquadric blob)
        public static Vector3 SuperEllipsoid(Vector2 uv)
        {
            const float n1 = .5f, n2 = .8f; // shape exponents
            float θ = (uv.X - .5f) * PI; // latitude
            float φ = uv.Y * TAU; // longitude
            float cθ = MathF.Sign(MathF.Cos(θ)) * MathF.Pow(MathF.Abs(MathF.Cos(θ)), n1);
            float sθ = MathF.Sign(MathF.Sin(θ)) * MathF.Pow(MathF.Abs(MathF.Sin(θ)), n1);
            float cφ = MathF.Sign(MathF.Cos(φ)) * MathF.Pow(MathF.Abs(MathF.Cos(φ)), n2);
            float sφ = MathF.Sign(MathF.Sin(φ)) * MathF.Pow(MathF.Abs(MathF.Sin(φ)), n2);
            return new(cθ * cφ, cθ * sφ, sθ);
        }

        // 21 ▸ Cross-cap (2-sided projective plane)
        public static Vector3 CrossCap(Vector2 uv)
        {
            float u = (uv.X - .5f) * PI;
            float v = uv.Y * 2f * PI;
            float x = MathF.Sin(u) * MathF.Sin(2 * v) / 2;
            float y = MathF.Sin(2 * u) * MathF.Sin(v) * MathF.Sin(v);
            float z = MathF.Cos(u) * MathF.Sin(2 * v) / 2;
            return new(x, y, z);
        }

        // 22 ▸ Dupin cyclide (ring-shaped quartic)
        public static Vector3 Cyclide(Vector2 uv)
        {
            const float a = 1f, b = .5f, d = .3f;
            float u = (uv.X - .5f) * PI; // -π/2..π/2
            float v = uv.Y * TAU;
            float denom = a - b * MathF.Cos(u);
            float x = (d * MathF.Cos(u)) / denom;
            float y = (d * MathF.Sin(u) * MathF.Cos(v)) / denom;
            float z = (d * MathF.Sin(u) * MathF.Sin(v)) / denom;
            return new(x, y, z);
        }

        // 23 ▸ Saddle torus (“peanut” minimal surface)
        public static Vector3 SaddleTorus(Vector2 uv)
        {
            float u = (uv.X - .5f) * TAU;
            float v = (uv.Y - .5f) * TAU;
            float R = 1f / (MathF.Sqrt(2f) + MathF.Cos(v));
            float x = R * MathF.Cos(u);
            float y = R * MathF.Sin(u);
            float z = R * MathF.Sin(v);
            return new(x, y, z);
        }

        // 24 ▸ Breather (periodic minimal surface)
        public static Vector3 Breather(Vector2 uv)
        {
            const float a = .4f;
            float u = (uv.X - .5f) * 10f;
            float v = (uv.Y - .5f) * 10f;
            float denom = 1 - a * a * MathF.Pow(MathF.Sinh(a * u), 2) + a * a * MathF.Pow(MathF.Sin(a * v), 2);
            float x = -u + (2 * (1 - a * a) * MathF.Cosh(a * u) * MathF.Sinh(a * u)) / denom;
            float y = (2 * (1 - a * a) * MathF.Cos(a * v) * MathF.Sin(a * v)) / denom;
            float z = ((1 - a * a) * (MathF.Cosh(a * u) * MathF.Cosh(a * u) + MathF.Sin(a * v) * MathF.Sin(a * v))) /
                      denom;
            return new Vector3(x, y, z) * .2f;
        }

        // 25 ▸ Whitney umbrella (self-intersecting “parasol”)
        public static Vector3 WhitneyUmbrella(Vector2 uv)
        {
            float u = (uv.X - .5f) * 4f;
            float v = (uv.Y - .5f) * 4f;
            float x = u * v;
            float y = u;
            float z = v * v;
            return new(x, y, z);
        }
        // 26 ▸ Dini’s surface (corkscrew pseudosphere)
        public static Vector3 Dini(Vector2 uv)
        {
            const float a = 1f, b = .2f;          // controls tightness
            float u = uv.X * 4f * PI + .01f;      // avoid log(0)
            float v = (uv.Y - .5f) * 2f * PI;
            float x = a * MathF.Cos(v) * MathF.Sin(u);
            float y = a * MathF.Sin(v) * MathF.Sin(u);
            float z = a * (MathF.Cos(u) + MathF.Log(MathF.Tan(u * .5f))) + b * v;
            return new(x, y, z);
        }

        // 27 ▸ Pseudosphere (tractrix revolved)
        public static Vector3 Pseudosphere(Vector2 uv)
        {
            float u = uv.X * 4f;                  // length along tractrix
            float v = uv.Y * TAU;
            float r = 1f - MathF.Tanh(u);
            float x = r * MathF.Cos(v);
            float y = r * MathF.Sin(v);
            float z = u - MathF.Tanh(u);
            return new(x, y, z);
        }

        // 28 ▸ Gyroid patch (triply periodic minimal surface)
        public static Vector3 Gyroid(Vector2 uv)
        {
            float u = (uv.X - .5f) * PI;          // -π/2..π/2
            float v = (uv.Y - .5f) * PI;
            float w = (u + v);                    // phase mix
            float x = MathF.Sin(u);
            float y = MathF.Sin(v);
            float z = MathF.Sin(w);
            return new(x, y, z);
        }

        // 29 ▸ Hyperboloid of one sheet
        public static Vector3 Hyperboloid1(Vector2 uv)
        {
            const float a = 1f, c = 1f;
            float u = (uv.X - .5f) * 2f;          // radial parameter
            float v = uv.Y * TAU;
            float cosh = MathF.Cosh(u);
            float x = a * cosh * MathF.Cos(v);
            float y = a * cosh * MathF.Sin(v);
            float z = c * MathF.Sinh(u);
            return new(x, y, z);
        }

        // 30 ▸ Hyperboloid of two sheets
        public static Vector3 Hyperboloid2(Vector2 uv)
        {
            const float a = 1f, c = 1f;
            float u = (uv.X - .5f) * 2f;          // height
            float v = uv.Y * TAU;
            float sinh = MathF.Sinh(u);
            float x = a * sinh * MathF.Cos(v);
            float y = a * sinh * MathF.Sin(v);
            float z = c * MathF.Cosh(u);
            return new(x, y, z);
        }

        // 31 ▸ Helix tube (spring-like hose)
        public static Vector3 HelixTube(Vector2 uv)
        {
            const float R = 1.0f;                 // helix radius
            const float r = .2f;                  // tube radius
            const float turns = 3f;
            float θ = uv.X * turns * TAU;         // along helix
            float φ = uv.Y * TAU;                 // around tube
            Vector3 center = new(
                R * MathF.Cos(θ),
                R * MathF.Sin(θ),
                θ / TAU);                         // gentle incline
            Vector3 n = new(-MathF.Sin(θ), MathF.Cos(θ), 0);          // normal
            Vector3 b = (
                new Vector3(-R * MathF.Sin(θ), R * MathF.Cos(θ), 1f).Cross(n)).Normalize;
            return center + r * (MathF.Cos(φ) * n + MathF.Sin(φ) * b);
        }

        // 32 ▸ Astroid torus (square-ish torus)
        public static Vector3 AstroidTorus(Vector2 uv)
        {
            const float R = 1f, r = .3f;
            float u = uv.X * TAU;
            float v = uv.Y * TAU;
            float cu = MathF.Cos(u), su = MathF.Sin(u);
            float cv = MathF.Cos(v), sv = MathF.Sin(v);
            float x = (R + r * MathF.Pow(cu, 3)) * cv;
            float y = (R + r * MathF.Pow(cu, 3)) * sv;
            float z = r * MathF.Pow(su, 3);
            return new(x, y, z);
        }

        // 33 ▸ Twisted seashell (double-helix shell)
        public static Vector3 TwistedShell(Vector2 uv)
        {
            float u = uv.X * 6f * PI;             // length
            float v = uv.Y * 1f;                  // lip parameter
            float r = .2f * MathF.Exp(.1f * u);
            float twist = u * .5f;
            float x = r * (1 + v) * MathF.Cos(u + twist);
            float y = r * (1 + v) * MathF.Sin(u + twist);
            float z = v * 2f - 1f + .1f * u;
            return new(x, y, z);
        }

        // 34 ▸ Catalan minimal surface (saddle tower strip)
        public static Vector3 CatalanMinimal(Vector2 uv)
        {
            float u = (uv.X - .5f) * 4f;
            float v = (uv.Y - .5f) * 4f;
            float x = u - MathF.Sinh(u) * MathF.Cosh(v);
            float y = v - MathF.Cosh(u) * MathF.Sinh(v);
            float z = MathF.Cosh(u) * MathF.Cosh(v) - MathF.Sinh(u) * MathF.Sinh(v);
            return new Vector3(x, y, z) * .2f;
        }

        // 35 ▸ Undulating sine wave surface
        public static Vector3 SineWavePlane(Vector2 uv)
        {
            float x = (uv.X - .5f) * 4f;
            float y = (uv.Y - .5f) * 4f;
            float z = .3f * MathF.Sin(PI * x) * MathF.Cos(PI * y);
            return new(x, y, z);
        }

        // 36 ▸ Concentric ripple dish
        public static Vector3 RippleDish(Vector2 uv)
        {
            float r = uv.X * 4f;
            float θ = uv.Y * TAU;
            float x = r * MathF.Cos(θ);
            float y = r * MathF.Sin(θ);
            float z = .3f * MathF.Sin(3f * r) / (r + .1f);
            return new(x, y, z);
        }

        // 37 ▸ Figure-8 immersion of Klein bottle
        public static Vector3 FigureEightKlein(Vector2 uv)
        {
            float u = uv.X * TAU;
            float v = uv.Y * TAU;
            float x = (MathF.Cos(u) * (MathF.Cos(u / 2f) * (MathF.Sqrt(2f) + MathF.Cos(v)) + MathF.Sin(u / 2f) * MathF.Sin(v) * MathF.Cos(v))) / 2f;
            float y = (MathF.Sin(u) * (MathF.Cos(u / 2f) * (MathF.Sqrt(2f) + MathF.Cos(v)) + MathF.Sin(u / 2f) * MathF.Sin(v) * MathF.Cos(v))) / 2f;
            float z = (MathF.Sin(u / 2f) * (MathF.Sqrt(2f) + MathF.Cos(v)) - MathF.Cos(u / 2f) * MathF.Sin(v) * MathF.Cos(v)) / 2f;
            return new(x, y, z);
        }

        // 38 ▸ Stellated sphere (spiky star ball)
        public static Vector3 StellatedSphere(Vector2 uv)
        {
            float θ = uv.X * TAU;
            float φ = uv.Y * PI;
            float r = 1f + .3f * MathF.Sin(5f * θ) * MathF.Sin(5f * φ);
            float x = r * MathF.Sin(φ) * MathF.Cos(θ);
            float y = r * MathF.Sin(φ) * MathF.Sin(θ);
            float z = r * MathF.Cos(φ);
            return new(x, y, z);
        }

        // 39 ▸ Saddle tower minimal surface patch
        public static Vector3 SaddleTower(Vector2 uv)
        {
            float u = (uv.X - .5f) * 2f * PI;
            float v = (uv.Y - .5f) * 2f * PI;
            float denom = 1f + MathF.Cosh(u) * MathF.Cosh(v);
            float x = MathF.Sinh(u) * MathF.Cosh(v) / denom;
            float y = MathF.Cosh(u) * MathF.Sinh(v) / denom;
            float z = (u) / denom;
            return new(x, y, z);
        }

        // 40 ▸ Exponential funnel
        public static Vector3 Funnel(Vector2 uv)
        {
            float r = uv.X * 2f;
            float θ = uv.Y * TAU;
            float z = -MathF.Exp(-r);
            return new(r * MathF.Cos(θ), r * MathF.Sin(θ), z);
        }

        // 41 ▸ Rounded diamond (smooth octahedron)
        public static Vector3 RoundedDiamond(Vector2 uv)
        {
            float θ = uv.X * TAU;
            float h = uv.Y * 2f - 1f;              // -1..1
            float r = 1f - MathF.Abs(h);
            float x = r * MathF.Cos(θ);
            float y = r * MathF.Sin(θ);
            float z = h;
            return new(x, y, z);
        }

        // 42 ▸ Spiral tower (log spiral column)
        public static Vector3 SpiralTower(Vector2 uv)
        {
            float turns = 4f;
            float u = uv.X * turns * TAU;
            float v = uv.Y;                        // 0 bottom ..1 top
            float r = .2f + v * .3f;
            float x = r * MathF.Cos(u);
            float y = r * MathF.Sin(u);
            float z = v * 2f;
            return new(x, y, z);
        }

        // 43 ▸ Spiral cone (whorl shell)
        public static Vector3 SpiralCone(Vector2 uv)
        {
            float u = uv.X * 4f * PI;
            float v = uv.Y;
            float r = (.5f - v * .5f) * u / (4f * PI);
            float x = r * MathF.Cos(u);
            float y = r * MathF.Sin(u);
            float z = v * 2f;
            return new(x, y, z);
        }

        // 44 ▸ Torus knot tube (p,q) = (2,3)
        public static Vector3 TorusKnot(Vector2 uv)
        {
            const int p = 2, q = 3;
            const float R = .8f, r = .2f;
            float θ = uv.X * TAU;                  // along knot
            float φ = uv.Y * TAU;                  // around tube
            Vector3 center = new(
                (R + r * MathF.Cos(q * θ)) * MathF.Cos(p * θ),
                (R + r * MathF.Cos(q * θ)) * MathF.Sin(p * θ),
                r * MathF.Sin(q * θ));
            Vector3 n = (new Vector3(
                -p * (R + r * MathF.Cos(q * θ)) * MathF.Sin(p * θ) - r * q * MathF.Sin(q * θ) * MathF.Cos(p * θ),
                 p * (R + r * MathF.Cos(q * θ)) * MathF.Cos(p * θ) - r * q * MathF.Sin(q * θ) * MathF.Sin(p * θ),
                 r * q * MathF.Cos(q * θ))).Normalize;
            Vector3 b = n.NormalizedCross(Vector3.UnitZ);
            return center + .08f * (MathF.Cos(φ) * n + MathF.Sin(φ) * b);
        }

        // 45 ▸ Vase of revolution (Bezier-like profile)
        public static Vector3 Vase(Vector2 uv)
        {
            float θ = uv.Y * TAU;
            float t = uv.X;
            // cubic Bézier profile points in rz-plane
            Vector3 p0 = new(0f, 0f, 0);
            Vector3 p1 = new(.4f, .1f, 0);
            Vector3 p2 = new(.2f, .8f, 0);
            Vector3 p3 = new(.5f, 1.2f, 0);
            // De Casteljau
            float u = 1 - t;
            float r = u * u * u * p0.X + 3 * u * u * t * p1.X + 3 * u * t * t * p2.X + t * t * t * p3.X;
            float z = u * u * u * p0.Y + 3 * u * u * t * p1.Y + 3 * u * t * t * p2.Y + t * t * t * p3.Y;
            return new(r * MathF.Cos(θ), r * MathF.Sin(θ), z);
        }
    }


}