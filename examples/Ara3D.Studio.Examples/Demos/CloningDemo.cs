using Color = Ara3D.Geometry.Color;

namespace Ara3D.Studio.Samples.Demos;

public class CloningDemo : IGenerator
{
    [Range(0, 4)]
    public int Shape { get; set; }

    [Range(1, 100), DisplayName("# Rows")]
    public int NumRows { get; set; } = 5;

    [Range(1, 100), DisplayName("# Columns")]
    public int NumCols { get; set; } = 5;

    [Range(1, 100), DisplayName("# Layers")]
    public int NumLayers { get; set; } = 1;

    [Range(0f, 20f)]
    public float Spacing = 2f;

    [Range(0f, 1f)]
    public float Red = 0.2f;

    [Range(0f, 1f)]
    public float Green = 0.8f;

    [Range(0f, 1f)]
    public float Blue = 0.1f;

    [Range(0f, 1f)]
    public float Alpha = 1f;

    public Color Color => (Red, Green, Blue, Alpha);

    public IModel3D Eval(EvalContext context)
    {
        var mesh = PlatonicSolids.GetMesh(Shape);
        var matrices = new List<Matrix4x4>();
        var material = new Material(Color, 0.5f, 0.5f);

        for (var k = 0; k < NumLayers; k++)
        {
            for (var j = 0; j < NumRows; j++)
            {
                for (var i = 0; i < NumCols; i++)
                {
                    var vec = new Vector3(i,j,k) * Spacing;
                    var trans = Matrix4x4.CreateTranslation(vec);
                    matrices.Add(trans);
                }
            }
        }

        return Model3D.Create(mesh, material, matrices);
    }
}