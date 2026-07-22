namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Colors the instance under the mouse in a model, using the GPU pick pass only:
/// ctx.Services.ViewportInput.HoverObjectId is the index of the hovered instance in the model's
/// instance list (-1 over the background), so hover costs nothing per frame regardless of scene
/// size — no rays, no spatial structures. Per-instance data such as EntityIndex can be read from
/// the hovered instance. [PointerTracking] re-evaluates this node as the pointer moves. On hosts
/// without a viewport the model passes through unchanged.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[PointerTracking]
[Description("Highlights the instance currently under the mouse, using the GPU pick pass and re-evaluating as the pointer moves.")]
public class ColorHoveredObject : IModifier
{
    public Vector3 HighlightColor = new(1f, 0.3f, 0.1f);

    public int HoveredInstance { get; private set; } = -1;
    public int HoveredEntity { get; private set; } = -1;

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        var hover = ctx.Services.ViewportInput?.HoverObjectId ?? -1;
        HoveredInstance = hover >= 0 && hover < model.Instances.Count ? hover : -1;
        HoveredEntity = HoveredInstance >= 0 ? model.Instances[HoveredInstance].EntityIndex : -1;
        ctx.Services.RefreshUI(this);
        if (HoveredInstance < 0)
            return model;

        var instances = model.Instances.ToList();
        var instance = instances[HoveredInstance];
        instance.Color = new Color(HighlightColor.X, HighlightColor.Y, HighlightColor.Z, 1);
        instances[HoveredInstance] = instance;
        return new Model3D(model.Meshes, instances);
    }
}
