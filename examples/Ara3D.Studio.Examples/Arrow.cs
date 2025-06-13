namespace Ara3D.Studio.Samples;

public class Arrow : IModelGenerator
{
    [Range(1, 32)] public int Count = 16;
    [Range(0f, 1f)] public float ShaftWidth = 0.01f;
    [Range(0f, 1f)] public float ShaftHeight = 0.8f;
    [Range(0f, 1f)] public float TipWidth = 0.2f;
    [Range(0f, 1f)] public float TipHeight = 0.2f;

    public Model3D Eval(EvalContext context)
    {
        var TotalHeight = ShaftHeight + TipHeight;
        var halfOutLine = new Point3D[]
        {
            (0, 0, 0),
            (ShaftWidth / 2, 0, 0),
            (ShaftWidth / 2, 0, ShaftHeight),
            (TipWidth / 2, 0, ShaftHeight),
            (0, 0, TotalHeight),
        };
        
        var grid = halfOutLine.Revolve(Vector3.UnitZ, Count);
        return grid.Triangulate();
    }
}