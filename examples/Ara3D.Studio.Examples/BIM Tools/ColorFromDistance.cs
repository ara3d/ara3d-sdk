namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
[Description("Colors model elements by their distance from an adjustable spherical probe point.")]
public class ColorFromDistance : IModifier
{
    [Range(0f, 1f)] public float X { get; set; } = 0.5f;
    [Range(0f, 1f)] public float Y { get; set; } = 0.5f;
    [Range(0f, 1f)] public float Z { get; set; } = 0.5f;

    [Range(0.001f, 50f)] public float Radius { get; set; } = 10f;

    public Action RecomputeBounds 
        => RecomputeBoundsImpl;

    private IModel3D _model;
    private List<Bounds3D> _meshBounds;
    private Bounds3D _totalBounds;
    private Point3D _targetPoint;

    public void RecomputeBoundsImpl()
    {
        if (_model == null)
            return;
        _totalBounds = _model.GetBounds();
        _meshBounds = _model.Meshes.Select(m => m.Bounds).ToList();
    }

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        if (_model == null)
        {
            _model = model;
            RecomputeBoundsImpl();
        }

        _targetPoint = _totalBounds.Lerp((X, Y, Z));

        var c1 = new Color(1.00f, 0.28f, 0.12f, 1); // warm orange-red
        var c2 = new Color(0.10f, 0.55f, 1.00f, 1); // clear blue
        var c3 = new Color(0.55f, 0.55f, 0.55f, 1); // neutral gray

        var newInstances = model.Instances
            .Select(inst => ColorInstanceFromDistance(inst, _meshBounds, _targetPoint, Radius, c1, c2, c3))
            .ToList();

        return model.WithInstances(newInstances);
    }

    public static InstanceStruct ColorInstanceFromDistance(
        InstanceStruct inst,
        IReadOnlyList<Bounds3D> meshBounds,
        Point3D targetPoint,
        float radius,
        Color nearColor,
        Color midColor,
        Color farColor)
    {
        if (!TryGetInstanceWorldCenter(inst, meshBounds, out var center))
            return inst;

        var dist = center.Vector3.Distance(targetPoint.Vector3);
        var relDist = RelativeDistance(dist, radius);
        var color = ThreePointGradient(nearColor, midColor, farColor, relDist);

        return inst.WithColor(color);
    }

    public static bool TryGetInstanceWorldCenter(
        InstanceStruct inst,
        IReadOnlyList<Bounds3D> meshBounds,
        out Point3D center)
    {
        center = default;

        if (inst.MeshIndex < 0 || inst.MeshIndex >= meshBounds.Count)
            return false;

        var localCenter = meshBounds[inst.MeshIndex].Center;
        center = localCenter.Transform(inst.Matrix4x4);
        return true;
    }

    public static float RelativeDistance(float distance, float radius)
    {
        if (radius <= 0f)
            return 1f;

        return Math.Clamp(distance / radius, 0f, 1f);
    }

    public static Color ThreePointGradient(Color c1, Color c2, Color c3, float t)
    {
        t = Math.Clamp(t, 0f, 1f);

        return t < 0.5f
            ? c1.Lerp(c2, t * 2f)
            : c2.Lerp(c3, (t - 0.5f) * 2f);
    }
}