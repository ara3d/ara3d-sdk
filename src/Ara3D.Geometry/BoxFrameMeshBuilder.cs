namespace Ara3D.Geometry;

public class BoxFrameMeshBuilder
{
    public readonly QuadMesh3D Mesh;
    public readonly float FrameRatio;

    public BoxFrameMeshBuilder(float frameRatio = 0.02f)
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
            vertices.Add((getPos(x), getPos(y), getPos(z)));

        Mesh = new QuadMesh3D(vertices, shape.GetQuadFaces());
    }

    private float getPos(int i)
        => i switch
        {
            0 => -0.5f,
            1 => -0.5f + FrameRatio,
            2 => 0.5f - FrameRatio,
            3 => 0.5f,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static QuadMesh3D CreateBoxFrameMesh(float frameRatio = 0.02f)
        => new BoxFrameMeshBuilder(frameRatio).Mesh;

    public static QuadMesh3D DefaultBoxFrame 
        => CreateBoxFrameMesh();
}