namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Colors the instance under the mouse in a model. [PointerTracking] re-evaluates this node as
/// the pointer or camera moves; the world-space cursor ray from ctx.Services.ViewportInput is
/// tested against each instance (bounds prefilter, then triangles transformed to world space so
/// hit distances are comparable across differently-scaled instances) and the nearest hit
/// instance is re-tinted. On hosts without a viewport the model passes through unchanged.
/// </summary>
[Category(nameof(Categories.Demos))]
[PointerTracking]
public class ColorHoveredObject : IModifier
{
    public Vector3 HighlightColor = new(1f, 0.3f, 0.1f);

    public int HoveredInstance { get; private set; } = -1;

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        HoveredInstance = FindHoveredInstance(model, ctx.Services.ViewportInput);
        ctx.Services.RefreshUI(this);
        if (HoveredInstance < 0)
            return model;
        var instances = model.Instances.ToList();
        var hovered = instances[HoveredInstance];
        hovered.Color = new Color(HighlightColor.X, HighlightColor.Y, HighlightColor.Z, 1);
        instances[HoveredInstance] = hovered;
        return new Model3D(model.Meshes, instances);
    }

    public static int FindHoveredInstance(IModel3D model, IViewportInput input)
    {
        if (input == null)
            return -1;
        var ray = input.CursorRay;
        var bounds = model.GetInstanceBounds();
        var best = -1;
        var bestDistance = float.MaxValue;
        for (var i = 0; i < model.Instances.Count; i++)
        {
            if (!ray.Intersects(bounds[i]))
                continue;
            var instance = model.Instances[i];
            var matrix = instance.Matrix4x4;
            var triangles = model.Meshes[instance.MeshIndex].Triangles;
            for (var j = 0; j < triangles.Count; j++)
            {
                var t = triangles[j];
                var world = new Triangle3D(
                    t.A.Vector3.Transform(matrix),
                    t.B.Vector3.Transform(matrix),
                    t.C.Vector3.Transform(matrix));
                if (ray.Intersects(world, out var distance) && distance < bestDistance)
                {
                    bestDistance = distance;
                    best = i;
                }
            }
        }
        return best;
    }
}
