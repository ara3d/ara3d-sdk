using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// A 2D raster of RGB colors (tracker studio-168) — an image / heightmap source. Pixels are
/// row-major, each an RGB <see cref="Vector3"/> in [0,1]. Its default rendering draws the image
/// as a flat colored quad grid on the XY plane (one colored vertex per pixel), so it is visible
/// without any texture-mapping support in the renderer.
/// </summary>
public sealed class Bitmap2D
{
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<Vector3> Pixels { get; }

    public Bitmap2D(int width, int height, IReadOnlyList<Vector3> pixels)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
    }

    public Vector3 Get(int x, int y)
        => Pixels[Math.Clamp(y, 0, Height - 1) * Width + Math.Clamp(x, 0, Width - 1)];

    /// <summary>Nearest-pixel sample from normalized coordinates u,v in [0,1].</summary>
    public Vector3 SampleUV(float u, float v)
        => Get((int)(u * (Width - 1) + 0.5f), (int)(v * (Height - 1) + 0.5f));
}
