namespace Ara3D.Geometry;

public enum Palettes
{
    // --- Sequential (Perceptual) ---
    Viridis,
    Plasma,
    Inferno,
    Magma,
    Cividis,
    Turbo,

    // Additional sequential
    Cubehelix,
    Blues,
    Greens,
    Reds,
    Purples,
    Oranges,
    YlGnBu,
    YlOrRd,

    // --- Diverging ---
    RdBu,
    CoolWarm,
    Spectral,
    RdYlGn,
    PiYG,
    BrBG,
    PurpleGreen,

    // --- Cyclic (angles / orientation) ---
    HSV,
    Twilight,
    Phase,

    // --- Qualitative (categorical) ---
    Category10,
    Category20,
    Set1,
    Set2,
    Set3,
    Pastel1,
    Pastel2,
    Accent,

    // --- Domain-specific / useful ---
    Topographic,
    Terrain,
    Heatmap,
    FireIce,
    Depth,
    Elevation,
    Curvature,
    NormalMap,
    Zebra,

    // --- Debug / utility ---
    Grayscale,
    HighContrast,
    RedGreenBlue
}

public static class PaletteColors
{
    public static Color GetColor(this IReadOnlyList<Color> colors, float value)
    {
        if (colors.Count == 0)
            return new Color(1, 1, 1, 1);

        if (colors.Count == 1)
            return colors[0];

        value = Math.Clamp(value, 0f, 1f);

        var scaled = value * (colors.Count - 1);
        var i = (int)scaled;
        var t = scaled - i;

        if (i < 0)
            return colors[0];

        if (i >= colors.Count - 1)
            return colors[^1];

        return colors[i].Lerp(colors[i + 1], t);
    }

    public static Color GetColor(this Palettes palette, float value)
        => GetColor(palette.GetColors(), value);

    public static Color[] GetColors(this Palettes palette)
    {
        switch (palette)
        {
            // --- Sequential ---
            case Palettes.Viridis:
                return
                [
                    new Color(0.267f, 0.005f, 0.329f, 1),
                    new Color(0.283f, 0.141f, 0.458f, 1),
                    new Color(0.254f, 0.265f, 0.530f, 1),
                    new Color(0.207f, 0.372f, 0.553f, 1),
                    new Color(0.164f, 0.471f, 0.558f, 1),
                    new Color(0.128f, 0.567f, 0.551f, 1),
                    new Color(0.135f, 0.659f, 0.518f, 1),
                    new Color(0.267f, 0.749f, 0.441f, 1),
                    new Color(0.478f, 0.821f, 0.318f, 1),
                    new Color(0.741f, 0.873f, 0.150f, 1),
                ];

            case Palettes.Plasma:
                return
                [
                    new Color(0.050f, 0.030f, 0.528f, 1),
                    new Color(0.290f, 0.070f, 0.660f, 1),
                    new Color(0.507f, 0.100f, 0.670f, 1),
                    new Color(0.710f, 0.140f, 0.580f, 1),
                    new Color(0.880f, 0.260f, 0.420f, 1),
                    new Color(0.980f, 0.500f, 0.200f, 1),
                    new Color(0.940f, 0.750f, 0.150f, 1),
                ];

            case Palettes.Inferno:
                return
                [
                    new Color(0.001f, 0.000f, 0.014f, 1),
                    new Color(0.200f, 0.030f, 0.400f, 1),
                    new Color(0.500f, 0.100f, 0.300f, 1),
                    new Color(0.800f, 0.250f, 0.100f, 1),
                    new Color(0.980f, 0.600f, 0.100f, 1),
                    new Color(0.990f, 0.900f, 0.200f, 1),
                ];

            case Palettes.Magma:
                return
                [
                    new Color(0.001f, 0.000f, 0.015f, 1),
                    new Color(0.200f, 0.050f, 0.300f, 1),
                    new Color(0.400f, 0.100f, 0.400f, 1),
                    new Color(0.700f, 0.200f, 0.300f, 1),
                    new Color(0.900f, 0.400f, 0.200f, 1),
                    new Color(0.990f, 0.800f, 0.300f, 1),
                ];

            case Palettes.Cividis:
                return
                [
                    new Color(0.000f, 0.135f, 0.304f, 1),
                    new Color(0.114f, 0.200f, 0.400f, 1),
                    new Color(0.275f, 0.330f, 0.450f, 1),
                    new Color(0.450f, 0.480f, 0.450f, 1),
                    new Color(0.650f, 0.620f, 0.350f, 1),
                    new Color(0.850f, 0.750f, 0.200f, 1),
                ];

            case Palettes.Turbo:
                return
                [
                    new Color(0.189f, 0.071f, 0.232f, 1),
                    new Color(0.251f, 0.252f, 0.633f, 1),
                    new Color(0.276f, 0.487f, 0.850f, 1),
                    new Color(0.231f, 0.741f, 0.651f, 1),
                    new Color(0.906f, 0.863f, 0.169f, 1),
                    new Color(0.980f, 0.490f, 0.050f, 1),
                    new Color(0.750f, 0.050f, 0.050f, 1),
                ];

            case Palettes.Cubehelix:
                return
                [
                    new Color(0.000f, 0.000f, 0.000f, 1),
                    new Color(0.200f, 0.100f, 0.300f, 1),
                    new Color(0.400f, 0.300f, 0.500f, 1),
                    new Color(0.600f, 0.500f, 0.600f, 1),
                    new Color(0.800f, 0.700f, 0.500f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1)
                ];

            case Palettes.YlGnBu:
                return
                [
                    new Color(1.000f, 1.000f, 0.850f, 1),
                    new Color(0.800f, 0.920f, 0.770f, 1),
                    new Color(0.500f, 0.800f, 0.700f, 1),
                    new Color(0.200f, 0.600f, 0.700f, 1),
                    new Color(0.100f, 0.300f, 0.600f, 1),
                ];

            // --- Diverging ---
            case Palettes.RdBu:
                return
                [
                    new Color(0.800f, 0.000f, 0.000f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1),
                    new Color(0.000f, 0.200f, 0.800f, 1),
                ];

            case Palettes.CoolWarm:
                return
                [
                    new Color(0.230f, 0.299f, 0.754f, 1),
                    new Color(0.865f, 0.865f, 0.865f, 1),
                    new Color(0.706f, 0.016f, 0.150f, 1),
                ];

            case Palettes.Spectral:
                return
                [
                    new Color(0.600f, 0.000f, 0.000f, 1),
                    new Color(0.900f, 0.400f, 0.000f, 1),
                    new Color(1.000f, 1.000f, 0.500f, 1),
                    new Color(0.400f, 0.700f, 0.900f, 1),
                    new Color(0.000f, 0.200f, 0.600f, 1),
                ];

            case Palettes.RdYlGn:
                return
                [
                    new Color(0.800f, 0.000f, 0.000f, 1),
                    new Color(1.000f, 1.000f, 0.000f, 1),
                    new Color(0.000f, 0.600f, 0.000f, 1),
                ];

            case Palettes.FireIce:
                return
                [
                    new Color(0.000f, 0.200f, 0.800f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1),
                    new Color(0.800f, 0.100f, 0.000f, 1),
                ];

            case Palettes.Curvature:
                return
                [
                    new Color(0.200f, 0.200f, 1.000f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1),
                    new Color(1.000f, 0.200f, 0.200f, 1),
                ];

            // --- Cyclic ---
            case Palettes.HSV:
                return
                [
                    new Color(1, 0, 0, 1),
                    new Color(1, 1, 0, 1),
                    new Color(0, 1, 0, 1),
                    new Color(0, 1, 1, 1),
                    new Color(0, 0, 1, 1),
                    new Color(1, 0, 1, 1),
                    new Color(1, 0, 0, 1),
                ];

            // --- Qualitative ---
            case Palettes.Category10:
                return
                [
                    new Color(0.121f, 0.466f, 0.705f, 1),
                    new Color(1.000f, 0.498f, 0.054f, 1),
                    new Color(0.172f, 0.627f, 0.172f, 1),
                    new Color(0.839f, 0.153f, 0.157f, 1),
                    new Color(0.580f, 0.404f, 0.741f, 1),
                    new Color(0.549f, 0.337f, 0.294f, 1),
                    new Color(0.890f, 0.466f, 0.760f, 1),
                    new Color(0.498f, 0.498f, 0.498f, 1),
                    new Color(0.737f, 0.741f, 0.133f, 1),
                    new Color(0.090f, 0.745f, 0.811f, 1),
                ];

            // --- Domain ---
            case Palettes.Terrain:
                return
                [
                    new Color(0.000f, 0.200f, 0.600f, 1),
                    new Color(0.000f, 0.500f, 1.000f, 1),
                    new Color(0.200f, 0.700f, 0.200f, 1),
                    new Color(0.800f, 0.800f, 0.200f, 1),
                    new Color(0.500f, 0.300f, 0.100f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1),
                ];

            case Palettes.Heatmap:
                return
                [
                    new Color(0, 0, 0, 1),
                    new Color(0.800f, 0, 0, 1),
                    new Color(1.000f, 0.500f, 0, 1),
                    new Color(1.000f, 1.000f, 0, 1),
                    new Color(1.000f, 1.000f, 1, 1),
                ];

            case Palettes.Depth:
                return
                [
                    new Color(0.000f, 0.000f, 0.200f, 1),
                    new Color(0.000f, 0.200f, 0.600f, 1),
                    new Color(0.200f, 0.600f, 1.000f, 1),
                    new Color(0.800f, 0.900f, 1.000f, 1),
                ];

            case Palettes.Zebra:
                return
                [
                    new Color(0, 0, 0, 1),
                    new Color(1, 1, 1, 1),
                    new Color(0, 0, 0, 1),
                    new Color(1, 1, 1, 1),
                ];

            // --- Utility ---
            case Palettes.Grayscale:
                return
                [
                    new Color(0, 0, 0, 1),
                    new Color(1, 1, 1, 1),
                ];

            case Palettes.HighContrast:
                return
                [
                    new Color(1, 0, 0, 1),
                    new Color(0, 1, 0, 1),
                    new Color(0, 0, 1, 1),
                    new Color(1, 1, 0, 1),
                    new Color(1, 0, 1, 1),
                    new Color(0, 1, 1, 1),
                ];

            case Palettes.RedGreenBlue:
                return
                [
                    new Color(1, 0, 0, 1),
                    new Color(0, 1, 0, 1),
                    new Color(0, 0, 1, 1),
                ];

            case Palettes.Blues:
                return
                [
                    new Color(0.968f, 0.984f, 1.000f, 1),
                    new Color(0.671f, 0.851f, 0.914f, 1),
                    new Color(0.216f, 0.529f, 0.754f, 1),
                    new Color(0.031f, 0.188f, 0.420f, 1),
                ];

            case Palettes.Greens:
                return
                [
                    new Color(0.968f, 0.988f, 0.960f, 1),
                    new Color(0.678f, 0.866f, 0.556f, 1),
                    new Color(0.216f, 0.628f, 0.333f, 1),
                    new Color(0.000f, 0.392f, 0.000f, 1),
                ];

            case Palettes.Reds:
                return
                [
                    new Color(1.000f, 0.960f, 0.941f, 1),
                    new Color(0.988f, 0.573f, 0.447f, 1),
                    new Color(0.890f, 0.102f, 0.110f, 1),
                    new Color(0.500f, 0.000f, 0.000f, 1),
                ];

            case Palettes.Purples:
                return
                [
                    new Color(0.988f, 0.984f, 0.992f, 1),
                    new Color(0.737f, 0.741f, 0.863f, 1),
                    new Color(0.502f, 0.490f, 0.729f, 1),
                    new Color(0.247f, 0.000f, 0.490f, 1),
                ];

            case Palettes.Oranges:
                return
                [
                    new Color(1.000f, 0.960f, 0.921f, 1),
                    new Color(0.992f, 0.682f, 0.419f, 1),
                    new Color(0.906f, 0.298f, 0.235f, 1),
                    new Color(0.498f, 0.152f, 0.016f, 1),
                ];

            case Palettes.YlOrRd:
                return
                [
                    new Color(1.000f, 1.000f, 0.800f, 1),
                    new Color(0.996f, 0.698f, 0.298f, 1),
                    new Color(0.992f, 0.400f, 0.200f, 1),
                    new Color(0.800f, 0.000f, 0.000f, 1),
                ];

            case Palettes.PiYG:
                return
                [
                    new Color(0.780f, 0.090f, 0.550f, 1),
                    new Color(0.970f, 0.970f, 0.970f, 1),
                    new Color(0.200f, 0.630f, 0.170f, 1),
                ];

            case Palettes.BrBG:
                return
                [
                    new Color(0.600f, 0.400f, 0.200f, 1),
                    new Color(0.960f, 0.960f, 0.960f, 1),
                    new Color(0.200f, 0.600f, 0.600f, 1),
                ];

            case Palettes.PurpleGreen:
                return
                [
                    new Color(0.500f, 0.000f, 0.500f, 1),
                    new Color(0.950f, 0.950f, 0.950f, 1),
                    new Color(0.000f, 0.500f, 0.000f, 1),
                ];

            case Palettes.Twilight:
                return
                [
                    new Color(0.200f, 0.100f, 0.300f, 1),
                    new Color(0.400f, 0.300f, 0.600f, 1),
                    new Color(0.700f, 0.700f, 0.900f, 1),
                    new Color(0.900f, 0.600f, 0.700f, 1),
                    new Color(0.400f, 0.200f, 0.300f, 1),
                ];

            case Palettes.Phase:
                return
                [
                    new Color(1, 0, 0, 1),
                    new Color(1, 1, 0, 1),
                    new Color(0, 1, 0, 1),
                    new Color(0, 1, 1, 1),
                    new Color(0, 0, 1, 1),
                    new Color(1, 0, 1, 1),
                ];

            case Palettes.Category20:
                return
                [
                    new Color(0.121f, 0.466f, 0.705f, 1),
                    new Color(0.682f, 0.780f, 0.909f, 1),
                    new Color(1.000f, 0.498f, 0.054f, 1),
                    new Color(1.000f, 0.733f, 0.471f, 1),
                    new Color(0.172f, 0.627f, 0.172f, 1),
                    new Color(0.596f, 0.874f, 0.541f, 1),
                    new Color(0.839f, 0.153f, 0.157f, 1),
                    new Color(1.000f, 0.596f, 0.588f, 1),
                    new Color(0.580f, 0.404f, 0.741f, 1),
                    new Color(0.773f, 0.690f, 0.835f, 1),
                ];

            case Palettes.Set1:
                return
                [
                    new Color(0.894f, 0.102f, 0.110f, 1),
                    new Color(0.216f, 0.494f, 0.722f, 1),
                    new Color(0.302f, 0.686f, 0.290f, 1),
                    new Color(0.596f, 0.306f, 0.639f, 1),
                ];

            case Palettes.Set2:
                return
                [
                    new Color(0.400f, 0.760f, 0.647f, 1),
                    new Color(0.988f, 0.553f, 0.384f, 1),
                    new Color(0.553f, 0.627f, 0.796f, 1),
                    new Color(0.906f, 0.541f, 0.765f, 1),
                ];

            case Palettes.Set3:
                return
                [
                    new Color(0.553f, 0.827f, 0.780f, 1),
                    new Color(1.000f, 1.000f, 0.702f, 1),
                    new Color(0.745f, 0.729f, 0.855f, 1),
                    new Color(0.984f, 0.502f, 0.447f, 1),
                ];

            case Palettes.Pastel1:
                return
                [
                    new Color(0.984f, 0.705f, 0.682f, 1),
                    new Color(0.702f, 0.803f, 0.890f, 1),
                    new Color(0.800f, 0.922f, 0.773f, 1),
                ];

            case Palettes.Pastel2:
                return
                [
                    new Color(0.702f, 0.886f, 0.803f, 1),
                    new Color(0.992f, 0.803f, 0.674f, 1),
                    new Color(0.796f, 0.835f, 0.909f, 1),
                ];

            case Palettes.Accent:
                return
                [
                    new Color(0.498f, 0.788f, 0.498f, 1),
                    new Color(0.745f, 0.682f, 0.831f, 1),
                    new Color(0.992f, 0.753f, 0.525f, 1),
                ];

            case Palettes.Topographic:
                return
                [
                    new Color(0.000f, 0.200f, 0.600f, 1),
                    new Color(0.200f, 0.600f, 1.000f, 1),
                    new Color(0.200f, 0.700f, 0.200f, 1),
                    new Color(0.800f, 0.800f, 0.200f, 1),
                    new Color(0.500f, 0.300f, 0.100f, 1),
                ];

            case Palettes.Elevation:
                return
                [
                    new Color(0.000f, 0.300f, 0.800f, 1),
                    new Color(0.200f, 0.700f, 0.300f, 1),
                    new Color(0.900f, 0.900f, 0.500f, 1),
                    new Color(0.600f, 0.400f, 0.200f, 1),
                    new Color(1.000f, 1.000f, 1.000f, 1),
                ];

            case Palettes.NormalMap:
                return
                [
                    new Color(0.5f, 0.5f, 1.0f, 1),
                    new Color(1.0f, 0.5f, 0.5f, 1),
                    new Color(0.5f, 1.0f, 0.5f, 1),
                ];

            default:
                return
                [
                    new Color(1, 0, 0, 1),
                    new Color(0, 1, 0, 1),
                    new Color(0, 0, 1, 1),
                ];
        }
    }
}
