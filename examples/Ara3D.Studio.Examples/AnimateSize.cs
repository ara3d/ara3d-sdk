namespace Ara3D.Studio.Samples;

public class AnimateSize : IModelModifier, IAnimated
{
    [Range(0, 9999)]
    public int RandomSeed = 345;

    [Range(0.01, 10.0)]
    public float Speed = 5f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Random(int seed, int index)
    {
        // Combine seed and index into one 64‑bit value
        ulong x = unchecked(((ulong)(uint)seed << 32) | (uint)index);

        // SplitMix64 — 3 rounds of mixing; period 2^64
        x += 0x9E3779B97F4A7C15UL;
        x = unchecked((x ^ (x >> 30)) * 0xBF58476D1CE4E5B9UL);
        x = unchecked((x ^ (x >> 27)) * 0x94D049BB133111EBUL);
        x ^= unchecked(x >> 31);

        // Convert the high 53 bits to a IEEE‑754 double in [0,1)
        const double inv = 1.0 / unchecked(1UL << 53);   // 1 / 2^53
        return (x >> 11) * inv;                 // strip 11 low bits
    }

    public Model3D Eval(Model3D model3D, EvalContext context)
    {
        throw new NotImplementedException("To-do: ");
        /*
        var tmp = new List<Element>();
        var i = 0;
        var x = (float)context.AnimationTime * Speed;
        foreach (var item in model3D.Elements)
        {
            var rndOffset = (float)Random(RandomSeed, i++);
            var rndScale = 1 + MathF.Sin(x + rndOffset);
            tmp.Add(item.Scale(rndScale));
        }
        return new Model3D(tmp, model3D.DataTable);
        */
    }
}