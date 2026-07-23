using Ara3D.Studio.API;

namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("Generates a ring of transforms as a TransformList. The list flows through the graph and the default renderer instances a small marker at each transform; a downstream modifier can instead instance a chosen mesh.")]
public class ScatterRingDemo : IGenerator
{
    [Range(1, 200)] public int Count = 24;
    [Range(1f, 20f)] public float Radius = 6f;

    public TransformList Eval(EvalContext context)
    {
        var matrices = new List<Matrix4x4>(Count);
        for (var i = 0; i < Count; i++)
        {
            var angle = i / (float)Count * 2f * MathF.PI;
            var pos = new Vector3(MathF.Cos(angle) * Radius, MathF.Sin(angle) * Radius, 0f);
            matrices.Add(Matrix4x4.CreateTranslation(pos));
        }
        return new TransformList(matrices);
    }
}
