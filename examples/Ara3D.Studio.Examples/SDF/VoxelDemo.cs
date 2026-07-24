namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.Volumetric)]
[Description("A signed-distance-field primitive. Size is the primary radius/half-extent; Secondary is used for boxes (second half-extent) and tori (tube radius).")]
public class SdfPrimitive : IGenerator
{
    public SdfPrimitive3D Shape;
    [Range(0, 5)] public float Size = 0.2f;
    [Range(0, 5)] public float Secondary = 0.3f;

    public Sdf3D Eval()
        => SdfPrimitives.Create(Shape, Size, Secondary);
}

[Category(Cat.Convert)]
[Description("Voxelizes a signed-distance field into a voxel grid at a chosen resolution.")]
public class SdfVoxelize : IModifier
{
    [Range(0.1f, 2f)] public float BoundSize = 1.2f;
    [Range(1, 128)] public int GridSize = 16;

    public static Bounds3D UnitBounds = new(-Vector3.One.Half, Vector3.One.Half);

    public IVoxels Eval(Sdf3D sdf)
    {
        var bounds = UnitBounds.Scale(BoundSize);
        return sdf.Voxelize(bounds, (GridSize, GridSize, GridSize));
    }
}

[Category(Cat.Convert)]
[Description("Converts a voxel grid into a mesh, either by cloning cubes or by marching cubes.")]
public class VoxelsToModel3D : IModifier
{
    [Range(0, 1)] public float Threshold = 0.05f;
    [Range(0, 1.5f)] public float VoxelSize = 0.75f;
    public bool Triangulate = false;

    public IModel3D Eval(IVoxels voxels)
    {
        var gridCnt = new Vector3(voxels.NumColumns, voxels.NumRows, voxels.NumLayers);
        var _voxelSize = voxels.Bounds.Size / gridCnt;
        if (!Triangulate)
        {
            var mesh = PlatonicSolids.TriangulatedCube.Scale(_voxelSize * VoxelSize);
            var positions = voxels.Where(v => v.Value < Threshold).Select(v => v.Position).ToList();
            return mesh.Clone(Material.Default, positions);
        }
        return voxels.MarchingCubes(Threshold).ToTriangleMesh3D().ToModel3D();
    }
}


[Category(Cat.ExperimentalDemos)]
[Description("Demonstrates SDF modeling: combines primitive fields with boolean operations and meshes the result via voxelization or marching cubes.")]
public class VoxelDemo : IGenerator
{
    [Range(1, 128)] public int GridSize = 16;
    [Range(0, 1)] public float Threshold = 0.05f;

    [Range(0, 1)] public float SphereRadius = 0.5f;
    [Range(0, 1)] public float AxisRadius = 0.1f;

    [Range(0, 1.5f)] public float VoxelSize = 0.75f;
    [Range(0.1f, 5f)] public float BoundSize = 1.5f;

    public List<string> ShapeNames() =>
    [
        "Sphere", "Axes", "Sphere ∪ Axes", "Sphere ∩ Axes", "Sphere − Axes",
        "Sphere ⊕ Axes", "Axes ∪ Sphere", "Axes − Sphere", "Axes ∩ Sphere", "Axes ⊕ Sphere",
    ];

    [Options(nameof(ShapeNames))] public int Shape = 0;

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

    public IModel3D Eval()
    {
        var sdf = GetShape(Shape);
        var bounds = UnitBounds.Scale(BoundSize);
        var voxelField = sdf.Voxelize(bounds, (GridSize, GridSize, GridSize));
        var mesh = PlatonicSolids.TriangulatedCube.Scale(voxelField.VoxelSize * VoxelSize);

        if (Triangulate)
        {
            return voxelField
                .MarchingCubes(Threshold)
                .ToTriangleMesh3D()
                .ToModel3D();
        }

        var positions = voxelField.Where(v => v.Value < Threshold).Select(v => v.Position).ToList();
        return mesh.Clone(Material.Default, positions);
    }
}

