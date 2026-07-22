namespace Ara3D.Studio.Samples;

public static class ReactionDiffusion2D
{
    /// <summary>
    /// Generates a 2D Gray–Scott reaction–diffusion pattern.
    /// Returns a width x height array of floats in [0,1].
    /// </summary>
    /// <param name="width">Output width (>= 2)</param>
    /// <param name="height">Output height (>= 2)</param>
    /// <param name="steps">Number of simulation steps (e.g., 10_000 for detailed patterns)</param>
    /// <param name="feed">Feed rate F (typical 0.01 .. 0.08)</param>
    /// <param name="kill">Kill rate k (typical 0.03 .. 0.07)</param>
    /// <param name="diffU">Diffusion rate for U, Du (default 0.16)</param>
    /// <param name="diffV">Diffusion rate for V, Dv (default 0.08)</param>
    /// <param name="dt">Time step (default 1.0f)</param>
    /// <param name="seedType">
    /// "center", "random", or "none": how to seed initial V. Center uses a small disk; random adds light noise.
    /// </param>
    /// <param name="seedAmount">Strength of initial V seeding (0..1)</param>
    /// <param name="wrapEdges">If true, toroidal wrapping; else clamped boundary</param>
    /// <param name="outputField">
    /// "V" (default) or "U": which field to normalize and return.
    /// </param>
    public static float[,] Generate(
        int width,
        int height,
        int steps,
        float feed,
        float kill,
        float diffU = 0.16f,
        float diffV = 0.08f,
        float dt = 1.0f,
        string seedType = "center",
        float seedAmount = 1.0f,
        bool wrapEdges = true,
        string outputField = "V")
    {
        if (width < 2 || height < 2) throw new ArgumentException("width/height must be >= 2");
        if (steps < 1) throw new ArgumentException("steps must be >= 1");
        feed = Math.Clamp(feed, 0f, 1f);
        kill = Math.Clamp(kill, 0f, 1f);
        diffU = Math.Max(0f, diffU);
        diffV = Math.Max(0f, diffV);
        dt = Math.Max(1e-6f, dt);
        seedAmount = Math.Clamp(seedAmount, 0f, 1f);
        bool outV = !string.Equals(outputField, "U", StringComparison.OrdinalIgnoreCase);

        // Fields U and V; start at U=1, V=0.
        var U = new float[width, height];
        var V = new float[width, height];
        var U2 = new float[width, height];
        var V2 = new float[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                U[x, y] = 1f;

        // Seed V
        var rng = new Random(12345);
        if (seedType.Equals("center", StringComparison.OrdinalIgnoreCase))
        {
            int cx = width / 2;
            int cy = height / 2;
            int r = Math.Max(4, Math.Min(width, height) / 12);
            float r2 = r * r;
            for (int y = cy - r; y <= cy + r; y++)
            {
                if (y < 0 || y >= height) continue;
                for (int x = cx - r; x <= cx + r; x++)
                {
                    if (x < 0 || x >= width) continue;
                    float dx = x - cx;
                    float dy = y - cy;
                    if (dx * dx + dy * dy <= r2)
                    {
                        V[x, y] = 0.25f * seedAmount + (float)rng.NextDouble() * 0.05f * seedAmount;
                        U[x, y] = 1f - V[x, y];
                    }
                }
            }
        }
        else if (seedType.Equals("random", StringComparison.OrdinalIgnoreCase))
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float noise = (float)rng.NextDouble() * 0.02f * seedAmount;
                    V[x, y] = noise;
                    U[x, y] = 1f - V[x, y];
                }
        }

        // 3x3 Laplacian kernel (sums ~0): center -1.0, cardinals 0.2, diagonals 0.05
        // This is a common, stable stencil for Gray–Scott demos.
        const float c = -1.0f;
        const float nsew = 0.2f;
        const float diag = 0.05f;

        // Helpers for boundary handling
        int Wrap(int a, int max) => (a + max) % max;
        int Clamp(int a, int max) => (a < 0 ? 0 : (a >= max ? max - 1 : a));

        // Simulation loop
        for (int s = 0; s < steps; s++)
        {
            for (int y = 0; y < height; y++)
            {
                int yN = wrapEdges ? Wrap(y - 1, height) : Clamp(y - 1, height);
                int yS = wrapEdges ? Wrap(y + 1, height) : Clamp(y + 1, height);

                for (int x = 0; x < width; x++)
                {
                    int xW = wrapEdges ? Wrap(x - 1, width) : Clamp(x - 1, width);
                    int xE = wrapEdges ? Wrap(x + 1, width) : Clamp(x + 1, width);

                    // Laplacian(U) and Laplacian(V)
                    float u = U[x, y];
                    float v = V[x, y];

                    float lapU =
                        c * u
                        + nsew * (U[xW, y] + U[xE, y] + U[x, yN] + U[x, yS])
                        + diag * (U[xW, yN] + U[xE, yN] + U[xW, yS] + U[xE, yS]);

                    float lapV =
                       c * v
                       + nsew * (V[xW, y] + V[xE, y] + V[x, yN] + V[x, yS])
                       + diag * (V[xW, yN] + V[xE, yN] + V[xW, yS] + V[xE, yS]);

                    // Gray–Scott equations:
                    // du/dt = Du∇²u - u*v² + F*(1 - u)
                    // dv/dt = Dv∇²v + u*v² - (F + k)*v
                    float uvv = u * v * v;
                    float du = diffU * lapU - uvv + feed * (1f - u);
                    float dv = diffV * lapV + uvv - (feed + kill) * v;

                    float uNext = u + du * dt;
                    float vNext = v + dv * dt;

                    // Clamp to [0,1] to keep fields sane
                    U2[x, y] = uNext < 0f ? 0f : (uNext > 1f ? 1f : uNext);
                    V2[x, y] = vNext < 0f ? 0f : (vNext > 1f ? 1f : vNext);
                }
            }

            // Swap buffers
            (U, U2) = (U2, U);
            (V, V2) = (V2, V);
        }

        // Choose field and normalize to [0,1]
        var src = outV ? V : U;
        float min = float.PositiveInfinity, max = float.NegativeInfinity;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float val = src[x, y];
                if (val < min) min = val;
                if (val > max) max = val;
            }

        float range = Math.Max(1e-6f, max - min);
        var result = new float[width, height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, y] = (src[x, y] - min) / range;

        return result;
    }
}

[OnDemand]
[Category(Cat.Fields)]
[Description("A Gray-Scott reaction-diffusion pattern simulated on a grid and output as a height field.")]
public class ReactionDiffusion : IGenerator
{
    // Example parameters known to yield nice patterns:
    //  - Coral-like:   F=0.0545, k=0.062, steps ~ 20_000
    //  - Mitosis:      F=0.0367, k=0.0649
    //  - Spots/stripes: try F in [0.02..0.08], k in [0.03..0.07]

    [Range(0f, 10f)] public float Height = 1f;
    [Range(0f, 100f)] public float Side = 10f;
    [Range(2, 512)] public int SideCount = 128;
    [Range(0f, 0.1f)] public float Feed = 0.055f;
    [Range(0f, 0.1f)] public float Kill = 0.062f;
    [Range(1, 128)] public int StepsLog2 = 12;
    public int Steps => 2 << StepsLog2;

    public TriangleMesh3D HeightField(float[,] values)
    {
        var tmp = values.ToReadOnlyList2D().SampleUV((x, y, z) => new Point3D(x, y, z));
        return QuadGrid3D.Create(tmp, false, false).Triangulate();
    }

    public IModel3D Eval(EvalContext context)
    {
        var pattern = ReactionDiffusion2D.Generate(
            width: SideCount,
            height: SideCount,
            steps: Steps,
            feed: Feed,
            kill: Kill,
            diffU: 0.16f,
            diffV: 0.08f,
            dt: 1.0f,
            seedType: "center",
            seedAmount: 1.0f,
            wrapEdges: true,
            outputField: "V");
        return HeightField(pattern).ToModel3D();
    }
}
