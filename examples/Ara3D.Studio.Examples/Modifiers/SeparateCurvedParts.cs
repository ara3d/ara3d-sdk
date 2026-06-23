using Ara3D.Studio.Samples;

[Category(nameof(Categories.Tests))]
public class SeparateCurvedParts : IModifier
{
    [Range(0, 180)] public float CreaseCutOff { get; set; } = 35f;

    public int NumGroups { get; private set; }

    public bool ApplyRandomColors { get; set; } = true;
    [Range(0, 1000)] public int Seed { get; set; }

    public static Color RandomColor(Random rng)
        => Color.Create(rng.NextSingle(), rng.NextSingle(), rng.NextSingle(), 1);

    public bool RemoveFlatSurfaces { get; set; } = true;
    [Range(0, 45)] public float MaxFlatAngleChange { get; set; } = 5f;

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        var totalGroups = 0;

        var result = model.SplitMeshes(mesh =>
        {
            mesh = mesh.WeldVertices();
            var parts = mesh.SplitByCreaseAngle(CreaseCutOff);
            totalGroups += parts.Count;
            return parts;   
        });

        if (ApplyRandomColors)
        {
            var rng = new Random(Seed);
            var coloredInstances = result.Instances.Select(inst => inst.WithColor(RandomColor(rng))).ToList();
            result = result.WithInstances(coloredInstances);
        }

        if (RemoveFlatSurfaces)
        {
            result = result.WhereMeshes(mesh => !mesh.IsMostlyFlat(MaxFlatAngleChange));
        }

        NumGroups = totalGroups;
        ctx.Application.RefreshUI(this);
        return result;
    }

}