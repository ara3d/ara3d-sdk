namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Transform)]
public class GridClone : IModifier
{
    [Range(1, 100)] public int Rows = 3;
    [Range(1, 100)] public int Columns = 3;
    [Range(1, 100)] public int Layers = 3;
    [Range(0f, 20f)] public float Offset = 5f;

    public IModel3D Eval(TriangleMesh3D mesh)
    {
        var positions = new List<Point3D>();
        for (var i = 0; i < Columns; i++)
        for (var j = 0; j < Rows; j++)
        for (var k = 0; k < Layers; k++)
            positions.Add(new Point3D(i, j, k) * Offset);

        return mesh.Clone(Material.Default, positions);
    }
}