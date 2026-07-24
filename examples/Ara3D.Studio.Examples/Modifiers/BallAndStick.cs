namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Convert)]
[Description("Converts a mesh into a ball-and-stick model, with spheres at vertices and tubes along edges.")]
public class BallAndStick : IModifier
{
    [Range(3, 64)] public int Sides = 16;
    [Range(0f, 1f)] public float EdgeRadius = 0.1f;
    [Range(0f, 1f)] public float PointRadius = 0.2f;
    public bool IncludePoints { get; set; } = true;
    public bool IncludeEdges { get; set; } = true;

    public IModel3D Eval(TriangleMesh3D mesh)
    {
        var mb = new Model3DBuilder();

        if (IncludeEdges)
            mb = mb.AddCylinders(mesh.GetLines(), EdgeRadius, Sides);

        if (IncludePoints)
            mb = mb.AddSpheres(mesh.Points, PointRadius);

        return mb.Build();
    }
}
