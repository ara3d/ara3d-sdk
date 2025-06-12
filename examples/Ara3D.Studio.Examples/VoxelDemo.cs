namespace Ara3D.Studio.Samples;

public class VoxelDemo : IModelGenerator
{
    [Range(1, 128)] public int GridSize = 16;
    [Range(0, 1)] public float Threshold = 0.05f;

    public static Bounds3D UnitBounds = new(
        -Vector3.One.Half,
        Vector3.One.Half);

    public Number SDF(Point3D p)
        => p.Vector3.Length - 0.5f;

    public Model3D Eval(EvalContext context)
    {
        var bounds = UnitBounds;
        var voxelField = new VoxelizedField(UnitBounds, GridSize, GridSize, GridSize, SDF);
        var mesh = PlatonicSolids.TriangulatedCube.Scale(voxelField.VoxelSize * 0.8f);
        var positions = voxelField.Where(v => v.Value < Threshold).Select(v => v.Position).ToList();
        return mesh.Clone(Material.Default, positions);
    }
}