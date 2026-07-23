namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a 3D grid of transforms as a TransformList (complements the ring scatter). The default renderer instances a marker at each transform.")]
public class GridScatterDemo : IGenerator
{
    [Range(1, 30)] public int NumX = 5;
    [Range(1, 30)] public int NumY = 5;
    [Range(1, 30)] public int NumZ = 3;
    [Range(0.5f, 10f)] public float Spacing = 2f;

    public TransformList Eval(EvalContext context)
    {
        var matrices = new List<Matrix4x4>(NumX * NumY * NumZ);
        for (var z = 0; z < NumZ; z++)
        for (var y = 0; y < NumY; y++)
        for (var x = 0; x < NumX; x++)
            matrices.Add(Matrix4x4.CreateTranslation(new Vector3(x * Spacing, y * Spacing, z * Spacing)));
        return new TransformList(matrices);
    }
}
