namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class ColorFromDistance : IModifier
{
    [Range(0f, 1f)] public float X { get; set; } = 0.5f;
    [Range(0f, 1f)] public float Y { get; set; } = 0.5f;
    [Range(0f, 1f)] public float Z { get; set; } = 0.5f;

    [Range(0f, 50f)] public float Radius { get; set; }

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        var meshBounds = model.Meshes.Select(m => m.Bounds).ToList();
        
        var c1 = new Color(1, 0, 0, 1);
        var c2 = new Color(0, 1, 1, 1);
        var c3 = new Color(0.5f, 0.5f, 0.5f, 1);

        var totalBounds = model.GetBounds();
        var totalCenter = totalBounds.Lerp((X, Y, Z));

        var newInstances = new List<InstanceStruct>();
        foreach (var inst in model.Instances)
        {
            if (inst.MeshIndex < 0)
            {
                newInstances.Add(inst);
                continue;
            }

            var localCenter = meshBounds[inst.MeshIndex].Center;
            var tformCenter = localCenter.Transform(inst.Matrix4x4);

            var dist = tformCenter.Vector3.Distance(totalCenter);
            var relDist = Math.Clamp(dist / Radius, 0, 1);

            var color = relDist < 0.5f
                ? c1.Lerp(c2, relDist * 2)
                : c2.Lerp(c3, (relDist - 0.5f) * 2);

            newInstances.Add(inst.WithColor(color));
        }

        return model.WithInstances(newInstances);
    }

}