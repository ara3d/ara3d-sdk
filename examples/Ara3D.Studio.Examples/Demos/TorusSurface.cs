namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.Surfaces)]
[Description("A torus as a parametric surface, sampled into a mesh by the default surface renderer.")]
public class TorusSurface : IGenerator
{
    [Range(0.5f, 10f)] public float MainRadius = 3f;
    [Range(0.1f, 5f)] public float TubeRadius = 1f;

    public ParametricSurface Eval(EvalContext context)
        => new(uv =>
        {
            var u = uv.X.Turns;
            var v = uv.Y.Turns;
            var d = (Number)MainRadius + v.Cos * (Number)TubeRadius;
            var x = d * u.Cos;
            var y = d * u.Sin;
            var z = v.Sin * (Number)TubeRadius;
            return new Vector3(x, y, z);
        }, true, true);
}
