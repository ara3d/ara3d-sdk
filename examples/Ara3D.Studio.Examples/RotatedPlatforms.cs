namespace Ara3D.Studio.Samples;

public class RotatedPlatforms : IModelGenerator
{
    [Range(1, 200)] public int Count = 8;
    [Range(0.01, 100)] public float Width = 10;
    [Range(0.01, 100)] public float Depth = 10;
    [Range(0.01, 100)] public float Height = 0.5f;
    [Range(0.01, 100)] public float Spacing = 4;
    [Range(-10, 10)] public float RotationInTurns = 0.5f;

    public Model3D Eval(EvalContext context)
    {
        var rotationPerFloor = RotationInTurns.Turns() / Count;
        var mesh = PlatonicSolids.TriangulatedCube;
        var dim = new Vector3(Width, Depth, Height);
        mesh = mesh.Scale(dim);

        var elements = new List<Element>();
        foreach (var i in new IntegerRange(0, Count))
        {
            var angle = rotationPerFloor * i;
            var position = Vector3.UnitZ * ((Spacing * i) + Height);
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, angle);
            var transform = new Pose3D(position, rotation);
            var element = new Element(mesh, Material.Default, transform);
            elements.Add(element);
        }

        return Model3D.Create(elements);
    }
}