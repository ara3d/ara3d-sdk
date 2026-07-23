namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Inverts the colors of a Bitmap2D. Demonstrates a Bitmap2D -> Bitmap2D modifier; the result is still a bitmap, drawn as a colored quad grid.")]
public class BitmapInvertModifier : IModifier
{
    public Bitmap2D Eval(Bitmap2D bitmap)
    {
        var pixels = new Vector3[bitmap.Pixels.Count];
        for (var i = 0; i < pixels.Length; i++)
        {
            var c = bitmap.Pixels[i];
            pixels[i] = new Vector3(1f - c.X, 1f - c.Y, 1f - c.Z);
        }
        return new Bitmap2D(bitmap.Width, bitmap.Height, pixels);
    }
}
