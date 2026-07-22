namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Animation demo that pulses each instance's scale over time using a per-instance random seed.")]
public class AnimateSize : IModifier, IAnimated
{
    [Range(0, 9999)]
    public int RandomSeed = 345;

    [Range(0.01, 10.0)]
    public float Speed = 5f;

    // -------- Random helpers (deterministic) --------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Random(int seed, int index)
    {
        // Combine seed and index into one 64-bit value
        ulong x = unchecked((ulong)(uint)seed << 32 | (uint)index);

        // SplitMix64
        x += 0x9E3779B97F4A7C15UL;
        x = unchecked((x ^ x >> 30) * 0xBF58476D1CE4E5B9UL);
        x = unchecked((x ^ x >> 27) * 0x94D049BB133111EBUL);
        x ^= unchecked(x >> 31);

        const double inv = 1.0 / unchecked(1UL << 53); // 1 / 2^53
        return (x >> 11) * inv; // → [0,1)
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float R01(int seed, int index) => (float)Random(seed, index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float RRange(int seed, int index, float min, float max)
        => min + (max - min) * R01(seed, index);

    private static Vector3 RandomUnitVector(int seed, int index)
    {
        // Use three decorrelated samples; map to [-1,1], then normalize.
        float x = R01(seed, index) * 2f - 1f;
        float y = R01(seed, index + 911_883) * 2f - 1f;
        float z = R01(seed, index + 1_777_777) * 2f - 1f;
        var v = new Vector3(x, y, z);
        if (v.LengthSquared() < 1e-8f) v = new Vector3(1, 0, 0); // degenerate fallback
        return v.Normalize;
    }

    // -------- Animation parameterization --------
    // You can tweak these ranges to taste.

    // Scale
    private const float ScaleAmpMin = 0.05f;
    private const float ScaleAmpMax = 0.35f;
    private const float ScaleFreqMin = 0.50f; // Hz
    private const float ScaleFreqMax = 2.00f; // Hz

    // Rotation
    private const float RotSpeedMinDeg = 30f;   // deg/s
    private const float RotSpeedMaxDeg = 180f;  // deg/s

    // Translation (sinusoidal along a direction)
    private const float MoveAmpMin = 0.05f;   // world units
    private const float MoveAmpMax = 0.50f;
    private const float MoveFreqMin = 0.10f;  // Hz
    private const float MoveFreqMax = 0.60f;  // Hz

    /// <summary>
    /// Builds a per-instance TRS matrix with animated scale, rotation, and translation.
    /// Deterministic from (seed, index). Time tSeconds should already include Speed scaling.
    /// </summary>
    public static Matrix4x4 RandomMatrix(int seed, int index, float tSeconds)
    {
        // Offsets to decorrelate parameters from the same (seed,index) stream
        const int Oa = 1000003, Of = 2000003, Op = 3000001;
        const int Or = 4000007, Oaxis = 5000011;
        const int Od = 6000013, Odf = 7000021, Odp = 8000027;

        // --- Scale(t) = 1 + A * sin(2π f t + φ)
        float sAmp = RRange(seed, index + Oa, ScaleAmpMin, ScaleAmpMax);
        float sFreq = RRange(seed, index + Of, ScaleFreqMin, ScaleFreqMax);
        float sPhase = RRange(seed, index + Op, 0f, MathF.Tau);
        float s = 1.0f + sAmp * MathF.Sin(MathF.Tau * sFreq * tSeconds + sPhase);
        s = MathF.Max(0.001f, s); // no degeneracy

        // --- Rotation(t): axis * angle(t)
        Vector3 axis = RandomUnitVector(seed, index + Oaxis);
        float rotDegPerSec = RRange(seed, index + Or, RotSpeedMinDeg, RotSpeedMaxDeg);
        float rotAngleRad = MathF.PI * rotDegPerSec / 180F * tSeconds; // no phase needed; axis varies per instance
        var rot = Matrix4x4.CreateFromAxisAngle(axis, rotAngleRad);

        // --- Translation(t) = dir * (A * sin(2π f t + φ))
        Vector3 dir = RandomUnitVector(seed, index + Od);
        float dAmp = RRange(seed, index + Od + 1, MoveAmpMin, MoveAmpMax);
        float dFreq = RRange(seed, index + Odf, MoveFreqMin, MoveFreqMax);
        float dPhase = RRange(seed, index + Odp, 0f, MathF.Tau);
        Vector3 delta = dir * (dAmp * MathF.Sin(MathF.Tau * dFreq * tSeconds + dPhase));
        var trans = Matrix4x4.CreateTranslation(delta);

        // --- Compose TRS. Order: Scale → Rotate → Translate
        // Final matrix = S * R * T (row-major, column vectors on right are fine with System.Numerics)
        var scale = Matrix4x4.CreateScale(s);
        return scale * rot * trans;
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        // Time in seconds with global speed scaling
        float t = (float)context.AnimationTime * Speed;

        var transforms = new List<Matrix4x4>(model3D.Instances.Count);
        int i = 0;
        foreach (var _ in model3D.Instances)
            transforms.Add(RandomMatrix(RandomSeed, i++, t));

        var transformedInstances = model3D.Instances.Zip(transforms,
            (instance, matrix) => instance.Transform(matrix));

        return model3D.WithInstances(transformedInstances);
    }
}