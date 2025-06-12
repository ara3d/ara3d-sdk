namespace Ara3D.Studio.Samples;

public class VoxelDemo : IModelGenerator
{
    [Range(1, 128)] public int GridSize = 16;
    [Range(0, 1)] public float Threshold = 0.05f;

    [Range(0, 1)] public float SphereRadius = 0.5f;
    [Range(0, 1)] public float AxisRadius = 0.1f;

    [Range(0, 1.5f)] public float VoxelSize = 0.75f;
    [Range(0.1f, 5f)] public float BoundSize = 1.5f;

    [Range(0, 9)] public int Shape = 0;

    public bool Triangulate = false;

    public static Bounds3D UnitBounds = new(
        -Vector3.One.Half,
        Vector3.One.Half);

    public Sdf3D GetShape(int n)
    {
        var sphereSdf = SdfPrimitives.Sphere(SphereRadius);
        var axisSdf = SdfPrimitives.AllAxis(AxisRadius);

        switch (n)
        {
            case 0: return sphereSdf;
            case 1: return axisSdf;
            case 2: return sphereSdf.Union(axisSdf);
            case 3: return sphereSdf.Intersection(axisSdf);
            case 4: return sphereSdf.Subtract(axisSdf);
            case 5: return sphereSdf.XOr(axisSdf);
            case 6: return axisSdf.Union(sphereSdf);
            case 7: return axisSdf.Subtract(sphereSdf);
            case 8: return axisSdf.Intersection(sphereSdf);
            case 9: return axisSdf.XOr(sphereSdf);
        }

        throw new InvalidOperationException();
    }

    public Model3D Eval(EvalContext context)
    {
        var sdf = GetShape(Shape);
        var bounds = UnitBounds.Scale(BoundSize);
        var voxelField = sdf.Voxelize(bounds, (GridSize, GridSize, GridSize));
        var mesh = PlatonicSolids.TriangulatedCube.Scale(voxelField.VoxelSize * VoxelSize);

        if (Triangulate)
        {
            return voxelField.MarchingCubes(Threshold).ToTriangleMesh3D();
        }

        var positions = voxelField.Where(v => v.Value < Threshold).Select(v => v.Position).ToList();
        return mesh.Clone(Material.Default, positions);
    }
}

