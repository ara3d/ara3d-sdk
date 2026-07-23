namespace Ara3D.Studio.Samples.Generators;

/// <summary>
/// A procedural terrain patch: a square grid in the XY plane whose Z height is fractal
/// value-noise (fractal Brownian motion). Drag the parameters to sculpt rolling hills or
/// jagged mountains. <see cref="Size"/> sets the world width/depth (centered on the origin)
/// and <see cref="Resolution"/> the number of grid cells per side. <see cref="Amplitude"/>
/// scales the peak height, while <see cref="Frequency"/>, <see cref="Octaves"/>,
/// <see cref="Persistence"/> and <see cref="Lacunarity"/> shape the noise: more octaves and
/// higher lacunarity add fine detail, persistence controls how quickly that detail fades.
/// The whole surface is deterministic from <see cref="Seed"/>.
/// </summary>
[Category(Cat.Structures)]
[Description("A procedural terrain patch whose height field is fractal value-noise (fBm) — rolling hills to mountains, deterministic from a seed.")]
public class Terrain : IGenerator
{
    [Range(2, 512)] public int Resolution = 128;
    [Range(1f, 200f)] public float Size = 50f;
    [Range(0f, 40f)] public float Amplitude = 8f;
    [Range(0.2f, 8f)] public float Frequency = 1.5f;
    [Range(1, 8)] public int Octaves = 5;
    [Range(0f, 1f)] public float Persistence = 0.5f;
    [Range(1f, 4f)] public float Lacunarity = 2f;
    public int Seed = 1337;

    public Point3D PointAt(int i, int j)
    {
        var u = i / (float)Resolution;
        var v = j / (float)Resolution;
        var x = (u - 0.5f) * Size;
        var y = (v - 0.5f) * Size;
        var z = Amplitude * Fbm(u * Frequency, v * Frequency);
        return new Point3D(x, y, z);
    }

    public QuadGrid3D Eval()
        => new(new FunctionalReadOnlyList2D<Point3D>(Resolution + 1, Resolution + 1, PointAt), false, false);

    public float Fbm(float x, float y)
    {
        var sum = 0f;
        var amp = 1f;
        var freq = 1f;
        var total = 0f;
        for (var o = 0; o < Octaves; o++)
        {
            sum += amp * ValueNoise(x * freq, y * freq);
            total += amp;
            amp *= Persistence;
            freq *= Lacunarity;
        }
        return total > 0f ? sum / total : 0f;
    }

    public float ValueNoise(float x, float y)
    {
        var x0 = (int)MathF.Floor(x);
        var y0 = (int)MathF.Floor(y);
        var x1 = x0 + 1;
        var y1 = y0 + 1;

        var tx = Fade(x - x0);
        var ty = Fade(y - y0);

        var c00 = Hash(x0, y0, Seed);
        var c10 = Hash(x1, y0, Seed);
        var c01 = Hash(x0, y1, Seed);
        var c11 = Hash(x1, y1, Seed);

        var bottom = Lerp(c00, c10, tx);
        var top = Lerp(c01, c11, tx);
        return Lerp(bottom, top, ty);
    }

    private static float Fade(float t)
        => t * t * (3f - 2f * t);

    private static float Lerp(float a, float b, float t)
        => a + (b - a) * t;

    private static float Hash(int x, int y, int seed)
    {
        var h = (uint)(x * 374761393 + y * 668265263 + seed * 2147483647);
        h = (h ^ (h >> 13)) * 1274126177u;
        h ^= h >> 16;
        return h / (float)uint.MaxValue * 2f - 1f;
    }
}
