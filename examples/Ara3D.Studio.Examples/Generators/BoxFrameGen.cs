namespace Ara3D.Studio.Samples.Generators;

public class BoxFrameGen : IGenerator
{
    [Range(0f, 10f)] public float SizeX = 1;
    [Range(0f, 10f)] public float SizeY = 1;
    [Range(0f, 10f)] public float SizeZ = 1;
    [Range(0f, 0.5f)] public float FrameRatio = 0.1f;

    public QuadMesh3D Eval()
        => new BoxFrameMeshBuilder(FrameRatio).Mesh.Scale((SizeX, SizeY, SizeZ));
}