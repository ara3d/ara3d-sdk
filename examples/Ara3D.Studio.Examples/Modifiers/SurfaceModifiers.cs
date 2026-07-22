namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Surface)]
[Description("Samples a parametric surface into a quad mesh at the given resolution.")]
public class SurfaceToMesh : IModifier
{
    [Range(2, 256)] public int GridSize = 64;

    public QuadGrid3D Eval(ParametricSurface surface)
        => surface.ToQuadGrid(GridSize, GridSize);
}

[Category(Cat.Surface)]
[Description("Remaps the U/V domain of a parametric surface, cropping or offsetting the sampled region.")]
public class SurfaceRemap : IModifier
{
    [Range(-1f, 1f)] public float StartU = 0f;
    [Range(-1f, 1f)] public float StartV = 0f;
    [Range(0f, 2f)] public float RangeU = 1f;
    [Range(0f, 2f)] public float RangeV = 1f;

    public ParametricSurface Eval(ParametricSurface surface)
        => surface.SetDomain((StartU, StartV), (RangeU, RangeV));
}

[Category(Cat.Surface)]
[Description("Tiles a parametric surface by repeating its U/V domain.")]
public class SurfaceRepeat : IModifier
{
    [Range(0f, 10f)] public float RepeatU = 3f;
    [Range(0f, 10f)] public float RepeatV = 3f;

    public ParametricSurface Eval(ParametricSurface surface)
        => surface.Repeat(RepeatU, RepeatV);
}
