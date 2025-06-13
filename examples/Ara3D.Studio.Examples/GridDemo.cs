namespace Ara3D.Studio.Samples;

public class GridDemo : IModelGenerator
{
    [Range(1, 256)] public int NumRows = 16;
    [Range(1, 256)] public int NumColumns = 16;

    public Model3D Eval(EvalContext context)
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(
            NumColumns + 1, NumRows + 1, 
            (i, j) => (i / (float)(NumColumns - 1), j / (float)(NumRows - 1), 0));
        var grid = new QuadGrid3D(points, false, false);
        return grid.Triangulate();
    }
}