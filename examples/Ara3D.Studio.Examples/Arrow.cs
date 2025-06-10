namespace Ara3D.Studio.Samples;

public class Arrow : IModelGenerator
{
    public float ShaftWidth;
    public float ShaftHeight;
    public float TipWidth;
    public float TipHeight;
    
    public float TotalHeight => ShaftHeight + TipHeight;

    public Model3D Eval(EvalContext context)
    {
        var halfOutLine = new Point2D[]
        {
            (0, 0),
            (ShaftWidth / 2, 0),
            (ShaftWidth / 2, ShaftHeight),
            (TipWidth / 2, ShaftHeight),
            (0, TotalHeight),
        };
        
        // Make the points as if we are drawing up.
        var points3D = halfOutLine.Map(p => new Point3D(p.X, 0, p.Y));

        var grid = points3D.Extrude(ShaftHeight);
        return grid.Triangulate();
    }
}