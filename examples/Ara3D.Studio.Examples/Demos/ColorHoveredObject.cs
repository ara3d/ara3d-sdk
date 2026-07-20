namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Colors the object under the mouse in a model, using the GPU pick pass only: the host's
/// object-id render target yields ctx.Services.ViewportInput.HoverObjectId (the EntityIndex of
/// the hovered instance, -1 over the background), so hover costs nothing per frame regardless of
/// scene size — no rays, no spatial structures. All instances sharing the hovered entity are
/// re-tinted (a BIM object is often several instances). [PointerTracking] re-evaluates this node
/// as the pointer moves. On hosts without a viewport the model passes through unchanged.
/// Note: instances with EntityIndex -1 (plain generated geometry) are never highlighted.
/// </summary>
[Category(nameof(Categories.Demos))]
[PointerTracking]
public class ColorHoveredObject : IModifier
{
    public Vector3 HighlightColor = new(1f, 0.3f, 0.1f);

    public int HoveredEntity { get; private set; } = -1;

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        HoveredEntity = ctx.Services.ViewportInput?.HoverObjectId ?? -1;
        ctx.Services.RefreshUI(this);
        if (HoveredEntity < 0)
            return model;

        var highlight = new Color(HighlightColor.X, HighlightColor.Y, HighlightColor.Z, 1);
        var instances = model.Instances.ToList();
        var hitAny = false;
        for (var i = 0; i < instances.Count; i++)
        {
            if (instances[i].EntityIndex != HoveredEntity)
                continue;
            var instance = instances[i];
            instance.Color = highlight;
            instances[i] = instance;
            hitAny = true;
        }
        return hitAny ? new Model3D(model.Meshes, instances) : model;
    }
}
