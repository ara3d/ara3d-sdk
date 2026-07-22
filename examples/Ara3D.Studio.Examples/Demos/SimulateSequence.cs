using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class SimulateSequence : IModifier
{
    public List<byte> OriginalFlags { get; private set; }
    public List<Matrix4x4> OriginalTransforms { get; private set; }
    public List<float> Groups { get; private set; }
    private BimData _data;

    [Range(0f,1f)]
    public float LerpAmount { get; set; }

    public static float CategoryToGroup(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return 0.9f; 

        switch (category.Trim().ToLowerInvariant())
        {
            // 1 - Structural foundation
            case "structural foundations":
            case "foundations":
            case "pile caps":
            case "footings":
            case "slab on grade":
            case "slab edges":
            case "structural rebar":
                return 0.05f;

            // 2 - Primary structure
            case "structural columns":
            case "structural beam systems":
            case "columns":
            case "structural framing":
            case "beams":
            case "floors":
            case "structural floors":
                return 0.1f;
            
            case "roofs":
            case "structural roof":
                return 0.2f;

            case "shaft openings":
                return 0.25f;

            case "curtain wall mullions":
                return 0.35f;

            // 4 - Vertical circulation / cores
            case "stairs":
            case "ramps":
            case "elevators":
            case "shafts":
            case "landings":
                return 0.4f;

            // 5 - MEP systems
            case "pipes":
            case "pipe accessories":
            case "pipe fittings":
            case "pipe segments":
            case "piping systems":
            case "plumbing fixtures":
            case "plumbing equipment":
            case "sprinklers":
                return 0.45f;

            case "cable trays":
            case "conduits":
            case "conduit runs":
            case "conduit fittings":
            case "electrical equipment":
            case "lighting fixtures":
            case "wires":
                return 0.5f;

            case "mechanical equipment":
            case "ducts":
            case "duct fittings":
                return 0.55f;

            // 6 - Interiors / partitions / ceilings
            case "ceilings":
            case "casework":
            case "furniture":
            case "specialty equipment":
            case "generic models":
            case "planting":
                return 0.6f;

            case "railings":
            case "handrails":
            case "top rails":
                return 0.65f;

            // 7 - Finishes / detail elements
            case "floor finishes":
            case "wall finishes":
            case "paint":
            case "materials":
            case "interior walls":
                return 0.7f;

            // 3 - Building envelope
            case "walls":
            case "wall sweeps":
            case "curtain walls":
            case "windows":
            case "doors":
            case "curtain panels":
            case "storefront":
                return 0.8f;

            // 0 - Site / context / topo
            case "topography":
            case "site":
            case "shared site":
            case "parking":
            case "roads":
            case "property lines":
                return 0.9f;

            // 8 - Annotation / non-physical
            case "levels":
            case "grids":
            case "annotations":
            case "text notes":
            case "dimensions":
            default:
                // Unknown categories appear late but not last-last
                return 0.9f;
        }
    }

    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public static string GetCategory(BimData bim, InstanceStruct inst)
        => bim.GetCategoryName((EntityIndex)inst.EntityIndex);

    public FlowObject Eval(FlowObject obj, EvalContext context)
    {
        if (obj.Content is not RenderModelData rmd)
            return obj;

        if (OriginalTransforms == null)
        {
            var bimData = context.Input.GetAttachment<BimData>();
            if (bimData == null)
                return obj;

            Groups = rmd.InstanceData.Select(i => CategoryToGroup(GetCategory(bimData, i))).ToList();
            OriginalTransforms = rmd.InstanceData.Select(i => i.Matrix4x4).ToList();
            OriginalFlags = rmd.InstanceData.Select(i => i.Flags).ToList();
        }

        for (var i=0; i < rmd.InstanceData.Count; i++)
        {
            var start = Groups[i];
            var end = start + 0.1f;

            if (LerpAmount < start)
            {
                rmd.InstanceData[i].Flags = 1;
            }
            else
            {
                rmd.InstanceData[i].Flags = OriginalFlags[i];
            }

            var dest = OriginalTransforms[i];
            var src = dest * Matrix4x4.CreateTranslation(new(0, 0, -50));

            if (LerpAmount >= end)
            {
                rmd.InstanceData[i] = rmd.InstanceData[i].WithMatrix(dest);
            }
            else
            {
                var amount = (LerpAmount - start) * 10f;
                var lerpedMatrix = src.Lerp(dest, amount);
                rmd.InstanceData[i] = rmd.InstanceData[i].WithMatrix(lerpedMatrix);
            }
        }

        return obj;
    }
}
