namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Scales a 2D profile about the origin. Demonstrates a Profile2D -> Profile2D modifier; the result is still a profile, drawn as a closed polyline.")]
public class ProfileScaleModifier : IModifier
{
    [Range(0.1f, 5f)] public float Scale = 1.5f;

    public Profile2D Eval(Profile2D profile)
        => profile.Map(v => new Vector2(v.X * Scale, v.Y * Scale));
}
