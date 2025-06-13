using Ara3D.Logging;
using Ara3D.Models;
using Ara3D.SceneEval;

namespace Ara3D.Studio.API;

public class ModelPipeline
{
    private readonly List<IModelModifier> _modifiers = new();

    public IReadOnlyList<IModelModifier> Modifiers => _modifiers;

    public void AddModifier(IModelModifier modifier)
    {
        _modifiers.Add(modifier);
    }

    public void SetModifiers(IEnumerable<IModelModifier> modifiers)
    {
        _modifiers.Clear();
        _modifiers.AddRange(modifiers);
    }

    public Model3D Evaluate(Model3D model)
    {
        var context = new EvalContext(CancellationToken.None, 0, Logger.Console);
        foreach (var modifier in _modifiers)
            model = modifier.Eval(model, context);
        return model;
    }
}