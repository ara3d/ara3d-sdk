using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class HeightFieldFunctions
    {
        // https://en.wikipedia.org/wiki/Monkey_saddle
        // https://mathworld.wolfram.com/MonkeySaddle.html
        public static float MonkeySaddle(this Vector2 uv)
            => uv.X.Cube() - 3 * uv.X * uv.Y.Sqr();

        // https://mathworld.wolfram.com/HandkerchiefSurface.html
        public static float Handkerchief(this Vector2 uv)
            => uv.X.Cube() / 3 + uv.X * (uv.Y.Sqr()) + 2 * (uv.X.Sqr() - uv.Y.Sqr());

        // https://mathworld.wolfram.com/CrossedTrough.html
        public static float CrossedTrough(this Vector2 uv)
            => uv.X.Sqr() * uv.Y.Sqr();

        // https://www.wolframalpha.com/input?i=z%3Dsin%28x%29*cos%28y%29
        public static float SinPlusCos(this Vector2 uv)
            => uv.X.Turns().Sin + uv.Y.Turns().Cos;

        // https://en.wikipedia.org/wiki/Paraboloid#Hyperbolic_paraboloid
        public static float Saddle(this Vector2 uv)
            => uv.X.Sqr() - uv.Y.Sqr();

        // https://math.stackexchange.com/questions/4413193/what-is-a-dog-saddle
        public static float DogSaddle(this Vector2 uv)
            => uv.X.Pow(4) - 6 * uv.X.Sqr() * uv.Y.Sqr() + uv.Y.Pow(4);
    }
}