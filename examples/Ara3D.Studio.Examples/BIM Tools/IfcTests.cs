using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
[Description("Scratch harness for experimenting with IFC category queries; currently passes the model through unchanged.")]
public class IfcTests: IModifier
{
    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public IModel3D Eval(IModel3D model, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        var catName = "IFCFLOWTERMINAL";
        //return model.Where(inst => inst.EntityIndex >= 0 && entities[inst.EntityIndex].Category == catName);
        return model;   
    }
}