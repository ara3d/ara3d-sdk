namespace Ara3D.Studio.Samples;

public class CenterMeshes : IModelModifier
{
    public Model3D Eval(Model3D model3D, EvalContext context)
    {
        var offsets = model3D.Meshes.Map(m => -m.Bounds.Center.Vector3);
        var meshes = model3D.Meshes.Zip(offsets, (mesh, offset) => mesh.Translate(offset));
        var r = model3D.WithMeshes(meshes);
        var transforms = offsets.Map(Matrix4x4.CreateTranslation);
        return r.WithTransforms(transforms);
    }
}