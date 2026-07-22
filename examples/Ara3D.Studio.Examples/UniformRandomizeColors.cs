using Color = Ara3D.Geometry.Color;

namespace Ara3D.Studio.Samples;

public static class UniformColors
{
    // Public entry point
    public static List<Color> GenerateColors(int n,
        double minLightness = 0.62, double maxLightness = 0.88, double chromaMargin = 0.90)
    {
        if (n <= 0) return new List<Color>();
        var colors = new List<Color>(n);

        // Low-discrepancy constants
        const double phi = 0.6180339887498949;      // golden ratio conjugate
        const double phi2 = 0.7548776662466927;     // another low-discrepancy step (1/φ^2)

        for (int i = 0; i < n; i++)
        {
            // Even coverage of hue and lightness via 2D low-discrepancy sequence
            double h = Frac(i * phi);                       // hue in [0,1)
            double tL = Frac(i * phi2);                     // [0,1)
            double L = minLightness + tL * (maxLightness - minLightness);

            // Find max in-gamut chroma for this L,h (binary search in OKLCh)
            double cMax = MaxChromaForLH(L, h);

            // Use a safe fraction of cMax to avoid edge clipping
            double C = chromaMargin * cMax;

            // Convert OKLCh -> sRGB8
            var (r, g, b) = OKLCh_to_sRGB(L, C, h);
            colors.Add(Color.Create(
                (float)r,
                (float)g,
                (float)b,
                1
            ));
        }

        return colors;
    }

    // ---- OKLab / OKLCh utilities ----

    // Compute max chroma for fixed L,h such that linear sRGB stays in [0,1]
    private static double MaxChromaForLH(double L, double h)
    {
        // Conservative upper bound for chroma in OKLab inside sRGB gamut
        double lo = 0.0, hi = 0.5; // 0.5 is generous; typical usable chroma << 0.5
        for (int k = 0; k < 24; k++)
        {
            double mid = 0.5 * (lo + hi);
            var (R, G, B) = OKLCh_to_linear_sRGB(L, mid, h);
            if (InUnitInterval(R) && InUnitInterval(G) && InUnitInterval(B))
                lo = mid; // still in gamut, can increase chroma
            else
                hi = mid; // out of gamut, reduce chroma
        }
        return lo;
    }

    private static (double r, double g, double b) OKLCh_to_sRGB(double L, double C, double h)
    {
        var (R, G, B) = OKLCh_to_linear_sRGB(L, C, h);
        return (LinearToSrgb(R), LinearToSrgb(G), LinearToSrgb(B));
    }

    private static (double R, double G, double B) OKLCh_to_linear_sRGB(double L, double C, double h)
    {
        double hr = 2.0 * Math.PI * h;
        double a = Math.Cos(hr) * C;
        double b = Math.Sin(hr) * C;
        return OKLab_to_linear_sRGB(L, a, b);
    }

    // OKLab -> linear sRGB (Bjørn Ottosson's transforms)
    // https://bottosson.github.io/posts/oklab/
    private static (double R, double G, double B) OKLab_to_linear_sRGB(double L, double a, double b)
    {
        double l_ = L + 0.3963377774 * a + 0.2158037573 * b;
        double m_ = L - 0.1055613458 * a - 0.0638541728 * b;
        double s_ = L - 0.0894841775 * a - 1.2914855480 * b;

        double l = l_ * l_ * l_;
        double m = m_ * m_ * m_;
        double s = s_ * s_ * s_;

        double R = +4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
        double G = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
        double B = -0.0041960863 * l - 0.7034186147 * m + 1.7076147010 * s;

        return (R, G, B);
    }

    // Linear sRGB -> encoded sRGB
    private static double LinearToSrgb(double x)
    {
        x = Clamp01(x);
        if (x <= 0.0031308) return 12.92 * x;
        return 1.055 * Math.Pow(x, 1.0 / 2.4) - 0.055;
    }

    // ---- helpers ----
    private static bool InUnitInterval(double x) => x >= 0.0 && x <= 1.0;
    private static double Clamp01(double x) => x < 0 ? 0 : (x > 1 ? 1 : x);
    private static double Frac(double x)
    {
        double f = x - Math.Floor(x);
        return f < 0 ? f + 1 : f;
    }
}

[Category(Cat.Color)]
[Description("Gives each instance a random material with controlled lightness, metalness, and roughness.")]
public class UniformRandomizeColors : IModifier
{
    [Range(0.0f, 1.0f)] public float MinLightness = 0.62f;
    [Range(0.0f, 1.0f)] public float MaxLightness = 0.88f;
    [Range(0.0f, 1.0f)] public float ChromaMargin = 0.90f;
    [Range(0.0f, 1.0f)] public float Metallic = 0.90f;
    [Range(0.0f, 1.0f)] public float Roughness = 0.90f;
    
    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var nIds = model3D.Instances.Select(es => es.MeshIndex).ToIndexedSet();
        var n = nIds.Count;
        var colors = UniformColors.GenerateColors(n, MinLightness, MaxLightness, ChromaMargin);
        var mats = colors.Select(c => new Material(c, (float)Metallic, (float)Roughness));
        return model3D.WithInstances(model3D.Instances.Select((node) =>
            node.WithMaterial(node.MeshIndex >= 0 ? mats[node.MeshIndex] : Material.Default)));
    }
}