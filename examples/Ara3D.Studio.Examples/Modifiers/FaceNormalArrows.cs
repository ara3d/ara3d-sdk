namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Analyze)]
public class FaceNormalArrows : IModifier
{
    [Range(0.01f, 10f)] public float Scale = 0.1f;
    public bool ShowInputMesh = true;

    public TriangleMesh3D GetArrowMesh()
        => Primitives.DefaultUpArrow().Triangulate().Scale(Scale);

    public static Matrix4x4 AlignToFace(Triangle3D face)
        => face.Normal.AlignZAxisWith() * Matrix4x4.CreateTranslation(face.Center);

    public IModel3D ArrowsFromMesh(TriangleMesh3D mesh)
    {
        var arrowMesh = GetArrowMesh();
        var transforms = mesh.Triangles.Map(AlignToFace);
        return arrowMesh.Clone(Material.Default, transforms);
    }

    public IModel3D Eval(IModel3D input)
    {
        var arrowMesh = GetArrowMesh();
        var arrowModels = input.Meshes.Map(ArrowsFromMesh).ToList();

        var mb = new Model3DBuilder();
        mb.AddMeshWithoutInstance(arrowMesh);
        foreach (var inst in input.Instances)
        {
            if (inst.MeshIndex < 0) continue;
            var arrowModel = arrowModels[inst.MeshIndex];
            foreach (var arrowInstance in arrowModel.Instances)
            {
                mb.AddInstance(0, arrowInstance.Matrix4x4 * inst.Matrix4x4);
            }
        }

        if (ShowInputMesh)
            mb.AddModel(input);

        return mb.Build();
    }
}
