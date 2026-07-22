using System.Globalization;
using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
public class MakeDoorsTransparent : IModifier
{
    private BimData _data;
    private BimObjectModel _model;

    [Range(0f, 1f)] public float Alpha = 0.1f;
    
    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public bool IsDoor(InstanceStruct i)
    {
        if (_model == null) return false;
        if (i.EntityIndex < 0) return false;
        var cat = _model.Entities[i.EntityIndex].Category;
        if (cat == null) return false;
        return cat.StartsWith("door", true, CultureInfo.InvariantCulture);
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData != _data)
        {
            _data = bimData;
            _model = new BimObjectModel(_data, true);
        }

        var meshes = model3D.Meshes;
        var instances = model3D.Instances.Map(i => IsDoor(i) ? i.WithAlpha(Alpha) : i);
        return new Model3D(meshes, instances);
    }
}