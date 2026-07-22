namespace Ara3D.Studio.Samples;

[Category(Cat.ExperimentalDemos)]
public class GridDemo : IGenerator
{
    [Range(1, 256)] public int NumRows = 16;
    [Range(1, 256)] public int NumColumns = 16;

    public QuadGrid3D Eval()
    {
        var points = new FunctionalReadOnlyList2D<Point3D>(
            NumColumns + 1, NumRows + 1, 
            (i, j) => (i / (float)(NumColumns - 1), j / (float)(NumRows - 1), 0));
        return new QuadGrid3D(points, false, false);
    }
}