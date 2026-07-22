namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Convert)]
[Description("Extrudes a line path upward into a vertical wall surface.")]
public class ExtrudePath : IModifier
{
    [Range(0f, 10f)] public float Height { get; set; } = 1;
    [Range(1, 100)] public int Count { get; set; } = 1;
    
    public QuadMesh3D Eval(LineMesh3D lineMesh, EvalContext context)
        => lineMesh.Extrude(Vector3.UnitZ * Height, Count);
}

[Category(Cat.Convert)]
[Description("Replaces each input line segment with a box-section beam.")]
public class LinesToBoxes : IModifier
{
    [Range(0f, 2f)] public float Thickness = 0.1f;
    [Range(0f, 10f)] public float Height = 2;
    [Range(0f, 10f)] public float Radius = 2;
    [Range(2, 20)] public int Count = 5;

    public IModel3D Eval(LineMesh3D lines)
    {
        var mesh = PlatonicSolids.TriangulatedCube;
        var transforms = new List<Matrix4x4>();
        foreach (var line in lines.Lines)
        {
            var transform = line.ToBoxTransform(Thickness, Height);
            transforms.Add(transform);
        }

        return Model3D.Create(mesh, Material.Default, transforms);
    }
}