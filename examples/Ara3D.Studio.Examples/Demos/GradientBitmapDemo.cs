namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a color bitmap (a red/green gradient with a checker overlay). The Bitmap2D flows through the graph and is drawn as a flat colored quad grid on the XY plane by the default bitmap renderer.")]
public class GradientBitmapDemo : IGenerator
{
    [Range(2, 128)] public int Width = 32;
    [Range(2, 128)] public int Height = 32;
    [Range(1, 16)] public int Checks = 4;

    public Bitmap2D Eval(EvalContext context)
    {
        var pixels = new Vector3[Width * Height];
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            var u = x / (float)(Width - 1);
            var v = y / (float)(Height - 1);
            var checker = ((x * Checks / Width) + (y * Checks / Height)) % 2 == 0 ? 1f : 0.3f;
            pixels[y * Width + x] = new Vector3(u * checker, v * checker, 0.5f * checker);
        }
        return new Bitmap2D(Width, Height, pixels);
    }
}
