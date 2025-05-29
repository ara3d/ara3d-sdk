namespace Plato.Geometry
{
    public static class HeightFieldFunctions
    {
        // https://en.wikipedia.org/wiki/Monkey_saddle
        // https://mathworld.wolfram.com/MonkeySaddle.html
        public static Number MonkeySaddle(this Vector2 uv)
            => uv.X.Pow3 - 3 * uv.X * uv.Y.Sqr;

        // https://mathworld.wolfram.com/HandkerchiefSurface.html
        public static Number Handkerchief(this Vector2 uv)
            => uv.X.Pow3 / 3 + uv.X * (uv.Y.Sqr) + 2 * (uv.X.Sqr - uv.Y.Sqr);

        // https://mathworld.wolfram.com/CrossedTrough.html
        public static Number CrossedTrough(this Vector2 uv)
            => uv.X.Sqr * uv.Y.Sqr;

        // https://www.wolframalpha.com/input?i=z%3Dsin%28x%29*cos%28y%29
        public static Number SinPlusCos(this Vector2 uv)
            => uv.X.Turns.Sin + uv.Y.Turns.Cos;

        // https://en.wikipedia.org/wiki/Paraboloid#Hyperbolic_paraboloid
        public static Number Saddle(this Vector2 uv)
            => uv.X.Sqr - uv.Y.Sqr;

        // https://math.stackexchange.com/questions/4413193/what-is-a-dog-saddle
        public static Number DogSaddle(this Vector2 uv)
            => uv.X.Pow4 - 6 * uv.X.Sqr * uv.Y.Sqr + uv.Y.Pow4;
    }
}