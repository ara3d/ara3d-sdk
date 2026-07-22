using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
[Description("Shows or hides IFC elements by broad discipline groups (walls, doors, MEP, structure, and so on).")]
public class IfcFilter : IModifier
{
    public bool BeamAndColumns { get; set; }
    public bool RoofsAndCovering { get; set; }
    public bool Doors { get; set; }
    public bool Furniture { get; set; }
    public bool Ducts { get; set; }
    public bool MEP { get; set; }
    public bool RampsAndStairs { get; set; }
    public bool Railings { get; set; }
    public bool FloorsAndSlabs { get; set; }
    public bool WallsAndPlates { get; set; }
    public bool Windows { get; set; }

    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public bool IsVisible(InstanceStruct inst, BimData bim)
    {
        var catName = bim.GetCategoryName((EntityIndex)inst.EntityIndex);
        switch (catName)
        {
            case "IFCBEAM":
            case "IFCCOLUMN":
            case "IFCFOOTING":
                return BeamAndColumns;

            case "IFCROOF":
            case "IFCCOVERING":
                return RoofsAndCovering;

            case "IFCDOOR":
                return Doors;

            case "IFCWINDOW":
                return Windows;

            case "IFCWALL":
            case "IFCWALLSTANDARDCASE":
            case "IFCCURTAINWALLS":
            case "IFCPLATE":
                return WallsAndPlates;

            case "IFCSLAB":
                return FloorsAndSlabs;

            case "IFCFURNISHINGELEMENT":
                return Furniture;

            case "IFCFLOWSEGMENT":
            case "IFCFLOWFITTING":
                return Ducts;

            case "IFCFLOWSTORAGEDEVICE":
            case "IFCENERGYCONVERSIONDEVICE":
            case "IFCFLOWTERMINAL":
            case "IFCFLOWMOVINGDEVICE":
                return MEP;

            case "IFCRAILING":
                return Railings;

            case "IFCSTAIRS":
            case "IFCRAMPFLIGHT":
                return RampsAndStairs;
        }


        return false;
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        return model3D.Where(inst => IsVisible(inst, bimData));
    }
}