namespace Ara3D.Studio.Samples.Generators;

public class BoxFrameMeshBuilder
{
    public readonly QuadMesh3D Mesh;
    public readonly float FrameRatio;

    public BoxFrameMeshBuilder(float frameRatio)
    {
        FrameRatio = frameRatio;
        var shape = new CellGridBuilder3D(3, 3, 3)
            .Remove(1, 1, 1)
            .Remove(1, 1, 2)
            .Remove(1, 1, 0)
            .Remove(1, 0, 1)
            .Remove(1, 2, 1)
            .Remove(0, 1, 1)
            .Remove(2, 1, 1);

        var vertices = new List<Point3D>();
        for (var x = 0; x <= 3; x++)
        for (var y = 0; y <= 3; y++)
        for (var z = 0; z <= 3; z++)
            vertices.Add((Pos(x), Pos(y), Pos(z)));

        Mesh = new QuadMesh3D(vertices, shape.GetQuadFaces());
    }

    public float Pos(int i)
    {
        if (i == 0) return -0.5f;
        if (i == 1) return -0.5f + FrameRatio;
        if (i == 2) return 0.5f - FrameRatio;
        if (i == 3) return 0.5f;
        throw new ArgumentOutOfRangeException();
    }
}