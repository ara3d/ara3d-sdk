namespace Ara3D.Studio.Samples.Generators;

[Category(Cat.Surfaces)]
[Description("A parametric surface chosen from a built-in library, sampled over an adjustable U/V range.")]
public class SurfaceGenerators : IGenerator
{
    [Options(nameof(SurfaceNames))] public int Surface;
    [Range(-1f, 1f)] public float StartU = 0f;
    [Range(-1f, 1f)] public float StartV = 0f;
    [Range(0f, 2f)] public float RangeU = 1f;
    [Range(0f, 2f)] public float RangeV = 1f;

    public Dictionary<string, ParametricSurface> SurfaceLookup { get; }
    public List<string> SurfaceNames { get; }

    public SurfaceGenerators()
    {
        var t = typeof(SurfaceFunctions);

        SurfaceLookup = new Dictionary<string, ParametricSurface>(StringComparer.OrdinalIgnoreCase);
        foreach (var mi in t.GetMethods())
        {
            if (mi.ReturnType != typeof(Vector3) || mi.GetParameters().Length != 1 ||
                mi.GetParameters()[0].ParameterType != typeof(Vector2)) continue;
            var func = ReflectionUtils.CreateDelegate<Func<Vector2, Vector3>>(mi);
            var ps = new ParametricSurface(func, false, false);
            SurfaceLookup.Add(mi.Name.SplitCamelCase(), ps);
        }

        SurfaceNames = SurfaceLookup.Keys.OrderBy(k => k).ToList();
    }
        
    public ParametricSurface GetSurface(int n)
        => SurfaceLookup[SurfaceNames[n]];

    public ParametricSurface Eval()
        => GetSurface(Surface).SetDomain((StartU, StartV), (RangeU, RangeV));
}

