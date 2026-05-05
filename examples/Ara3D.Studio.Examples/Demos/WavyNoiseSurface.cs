namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class WavyNoiseSurface : IGenerator
{
    [Range(2, 512)] public int NumRows = 96;    
    [Range(2, 512)] public int NumColumns = 96;

    [Range(0.1f, 100f)] public float Width = 10f;
    [Range(0.1f, 100f)] public float Depth = 10f;

    [Range(0f, 10f)] public float WaveAmplitude = 0.6f;
    [Range(0f, 20f)] public float WaveFrequencyX = 2.5f;
    [Range(0f, 20f)] public float WaveFrequencyY = 2.0f;

    [Range(0f, 10f)] public float NoiseAmplitude = 0.25f;
    [Range(0.1f, 20f)] public float NoiseFrequency = 3.0f;

    [Range(0f, 10f)] public float RippleAmplitude = 0.2f;
    [Range(0f, 40f)] public float RippleFrequency = 10f;

    [Range(0, 10000)] public int Seed = 1234;

    [Range(-100f, 100f)] public float OffsetX;
    [Range(-100f, 100f)] public float OffsetY;

    public Point3D PointOnSurface(int i, int j)
    {
        var u = i / (float)(NumColumns - 1);
        var v = j / (float)(NumRows - 1);

        var x = (u - 0.5f) * Width;
        var y = (v - 0.5f) * Depth;

        var wave =
            MathF.Sin((u + OffsetX) * MathF.Tau * WaveFrequencyX) *
            MathF.Cos((v + OffsetY) * MathF.Tau * WaveFrequencyY) *
            WaveAmplitude;

        var noise = FractalValueNoise(
            u * NoiseFrequency + OffsetX,
            v * NoiseFrequency + OffsetY,
            Seed) * NoiseAmplitude;

        var dx = u - 0.5f;
        var dy = v - 0.5f;
        var r = MathF.Sqrt(dx * dx + dy * dy);

        var ripple =
            MathF.Sin(r * RippleFrequency - OffsetX) *
            RippleAmplitude *
            SmoothFalloff(r * 2f);

        var z = wave + noise + ripple;

        return new Point3D(x, y, z);
    }

    public QuadGrid3D Eval()
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(
            NumColumns,
            NumRows,
            PointOnSurface);

        return new QuadGrid3D(points, false, false);
    }

    public static float FractalValueNoise(float x, float y, int seed)
    {
        var sum = 0f;
        var amp = 1f;
        var freq = 1f;
        var norm = 0f;

        for (var octave = 0; octave < 4; octave++)
        {
            sum += ValueNoise(x * freq, y * freq, seed + octave * 1013) * amp;
            norm += amp;

            amp *= 0.5f;
            freq *= 2f;
        }

        return sum / norm;
    }

    public static float ValueNoise(float x, float y, int seed)
    {
        var x0 = (int)MathF.Floor(x);
        var y0 = (int)MathF.Floor(y);
        var x1 = x0 + 1;
        var y1 = y0 + 1;

        var tx = SmoothStep(x - x0);
        var ty = SmoothStep(y - y0);

        var a = RandomSigned(x0, y0, seed);
        var b = RandomSigned(x1, y0, seed);
        var c = RandomSigned(x0, y1, seed);
        var d = RandomSigned(x1, y1, seed);

        var ab = Lerp(a, b, tx);
        var cd = Lerp(c, d, tx);

        return Lerp(ab, cd, ty);
    }

    public static float RandomSigned(int x, int y, int seed)
        => Random01(x, y, seed) * 2f - 1f;

    public static float Random01(int x, int y, int seed)
    {
        unchecked
        {
            var n = seed;
            n ^= x * 374761393;
            n ^= y * 668265263;
            n = (n ^ (n >> 13)) * 1274126177;
            n ^= n >> 16;

            return (n & 0x00FFFFFF) / (float)0x01000000;
        }
    }

    public static float SmoothStep(float t)
        => t * t * (3f - 2f * t);

    public static float SmoothFalloff(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return 1f - t * t * (3f - 2f * t);
    }

    public static float Lerp(float a, float b, float t)
        => a + (b - a) * t;
}