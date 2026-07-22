using Ara3D.Studio.Samples;

[Category(Cat.ExperimentalTests)]
public class SeparateRandomFaceGroups : IModifier
{
    [Range(1, 20)] public int GroupSize { get; set; } = 5;

    public int NumGroups { get; private set; }

    public IModel3D Eval(IModel3D model, EvalContext ctx)
    {
        var mesh = model.ToMesh().WeldVertices();
        var faceGroups = FaceGroups.Create(mesh, g => g.Count >= GroupSize);
        NumGroups = faceGroups.Groups.Count;
        ctx.Services.RefreshUI(this); 
        var mb = new Model3DBuilder();
        mb.AddInstances(faceGroups.Split(mesh));
        return mb.Build();
    }
}