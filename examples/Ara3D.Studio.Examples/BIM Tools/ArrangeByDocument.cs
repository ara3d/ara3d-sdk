using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class ArrangeByDocument : IModifier
{
    [Range(0, 100)] public float Distance { get; set; } = 50;
    [Range(1, 100)] public int NumColumns { get; set; } = 10;

    private Dictionary<int, int> entityIndexToDocIndex = new();

    public void RecomputeDocuments(EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        for (var i = 0; i < bimData.Entities.Length; i++)
        {
            var docIndex = bimData.Entities[i].Document;    
            if (docIndex >= 0)
                entityIndexToDocIndex.Add(i, (int)docIndex);
        }
    }

    public Vector3 GetOffset(InstanceStruct inst)
    {
        if (inst.MeshIndex < 0) 
            return Vector3.Zero;
        if (!entityIndexToDocIndex.TryGetValue(inst.EntityIndex, out var docIndex))
            return Vector3.Zero;

        var x = (docIndex % NumColumns) * Distance;
        var y = (docIndex / NumColumns) * Distance;
        return (x, y, 0);
    }

    public InstanceStruct Offset(InstanceStruct inst)
        => inst.WithMatrix(inst.Matrix4x4 * Matrix4x4.CreateTranslation(GetOffset(inst)));
    

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        if (entityIndexToDocIndex.Count == 0)
            RecomputeDocuments(context);
        return model3D.WithInstances(Offset);
    }
}