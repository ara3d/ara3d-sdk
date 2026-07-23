namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Samples a sphere distance field into a ScalarGrid3D of stored values. The grid flows through the graph and the default renderer draws a box at every cell inside the surface (value <= iso).")]
public class SphereScalarGridDemo : IGenerator
{
    [Range(4, 48)] public int Resolution = 20;
    [Range(1f, 10f)] public float Extent = 6f;
    [Range(0.5f, 8f)] public float Radius = 4f;

    public ScalarGrid3D Eval(EvalContext context)
    {
        var r = Resolution;
        var bounds = new Bounds3D(new Point3D(-Extent, -Extent, -Extent), new Point3D(Extent, Extent, Extent));
        var dims = new Integer3(r, r, r);
        // Reuse VoxelizedField for cell-centre geometry while sampling into stored values.
        var cells = new VoxelizedField(bounds, dims, _ => 0f);
        var values = new float[r * r * r];
        for (var k = 0; k < r; k++)
        for (var j = 0; j < r; j++)
        for (var i = 0; i < r; i++)
            values[i + j * r + k * r * r] = cells.GetVoxelCenter(i, j, k).Vector3.Length - Radius;
        return new ScalarGrid3D(bounds, dims, values);
    }
}
