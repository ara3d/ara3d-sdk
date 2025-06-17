using Ara3D.PropKit;

namespace Ara3D.Studio.Samples;

public class ParametricSurfaceDemo : IModelGenerator
{
    [Options(nameof(SurfaceNames))] public int Surface;
    [Range(0f, 1f)] public float Red = 0.2f;
    [Range(0f, 1f)] public float Green = 0.8f;
    [Range(0f, 1f)] public float Blue = 0.1f;
    [Range(0f, 1f)] public float Alpha = 1f;
    [Range(0f, 1f)] public float Metallic = 0f;
    [Range(0f, 1f)] public float Roughness = 0.5f;
    public bool ClosedU = false;
    public bool ClosedV = false;
    [Range(2, 256)] public int GridSize = 24;

    public Dictionary<string, ParametricSurface> SurfaceLookup { get; }
    public List<string> SurfaceNames { get; }

    public ParametricSurfaceDemo()
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

    public Material Material =>
        new((Red, Green, Blue, Alpha), Metallic, Roughness);

    public Model3D Eval(EvalContext context)
    {
        var mesh = GetSurface(Surface)
            .WithClosedUV(ClosedU, ClosedV)
            .Triangulate(GridSize, GridSize);
        return new Element(mesh, Material);
    }
}