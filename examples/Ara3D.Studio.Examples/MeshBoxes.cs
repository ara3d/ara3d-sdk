namespace Ara3D.Studio.Samples;

public class MeshBoxes : IModelModifier
{
    public static TriangleMesh3D ToMesh(Bounds3D bounds)
        => PlatonicSolids.TriangulatedCube.Scale(bounds.Size).Translate(bounds.Center);

    public Model3D Eval(Model3D model, EvalContext context)
    {
        var boundsAsMeshes = model.Meshes.Select(m => ToMesh(m.Bounds));
        return model.WithMeshes(boundsAsMeshes);
    }
}