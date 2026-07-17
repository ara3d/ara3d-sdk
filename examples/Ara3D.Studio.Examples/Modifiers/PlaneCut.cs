namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Cuts a triangle mesh with a horizontal plane, keeping geometry at or below the plane.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class MeshHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        if (Height == 0) return ([], []);
        var z = bounds.Min.Z.Lerp(bounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);
        return mesh.ClipBelow(plane);
    }
}

/// <summary>
/// Cuts a model with a horizontal plane, keeping geometry at or below the plane.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class ModelHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public IModel3D Eval(IModel3D model)
    {
        var mb = new Model3DBuilder();
        var totalBounds = model.GetBounds();
        var instanceBounds = model.GetInstanceBounds();
        var z = totalBounds.Min.Z.Lerp(totalBounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);

        var meshRemap = new Dictionary<int, int>();
        for (int i = 0; i < model.Instances.Count; i++)
        {
            var inst = model.Instances[i];
            if (inst.MeshIndex < 0)
            {
                mb.AddInstance(inst);
                continue;
            }

            var bounds = instanceBounds[i];
            
            if (bounds.Min.Z >= z)
                continue;

            if (bounds.Max.Z <= z)
                mb.AddInstanceAndRemapMesh(inst, model, meshRemap);
            else
            {
                var mesh = model.GetTransformedMesh(inst);
                var cutMesh = mesh.ClipBelow(plane);
                mb.AddInstance(cutMesh, inst.Material);
            }
        }

        return mb.Build();
    }
}
