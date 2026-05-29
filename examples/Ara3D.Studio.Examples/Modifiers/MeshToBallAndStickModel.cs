namespace Ara3D.Studio.Samples.Modifiers;

[Category(nameof(Categories.Converters))]
public class MeshToBallAndStickModel : IModifier
{
    [Range(3, 64)] public int Sides = 16;
    [Range(0f, 1f)] public float EdgeRadius = 0.1f;
    [Range(0f, 1f)] public float PointRadius = 0.2f;
    public bool IncludePoints { get; set; } = true;
    public bool IncludeEdges { get; set; } = true;

    public static Matrix4x4 CreateLineMatrix(Line3D line)
    {
        var a = line.A.Vector3;
        var b = line.B.Vector3;

        var dz = b - a;
        var length = dz.Length();

        if (length <= float.Epsilon)
            return Matrix4x4.CreateTranslation(a);

        var z = dz / length;

        // Pick a stable reference vector that is not parallel to the line.
        var reference = MathF.Abs(Vector3.Dot(z, Vector3.UnitZ)) < 0.999f
            ? Vector3.UnitZ
            : Vector3.UnitY;

        var x = reference.NormalizedCross(z);
        var y = z.Cross(x);

        // Maps a unit-height cylinder running from local Z=0 to local Z=1
        // onto the line from A to B.
        return new Matrix4x4(
            x.X, x.Y, x.Z, 0,
            y.X, y.Y, y.Z, 0,
            z.X * length, z.Y * length, z.Z * length, 0,
            a.X, a.Y, a.Z, 1);
    }

    public IModel3D Eval(TriangleMesh3D mesh)
    {
        var edgeMesh = Sides.GetCircularPoints(EdgeRadius).Extrude(1).Triangulate();
        var pointMesh = PlatonicSolids.Icosahedron;

        var mb = new Model3DBuilder();

        var lineMeshIndex = mb.AddMesh(edgeMesh);
        var pointMeshIndex = mb.AddMesh(pointMesh);

        var lines = IncludeEdges
            ? mesh.Triangles.SelectMany(t => new[] { t.LineA, t.LineB, t.LineC })
            : [];

        var points = IncludePoints
            ? mesh.Points
            : [];

        foreach (var line in lines)
            mb.AddInstance(lineMeshIndex, CreateLineMatrix(line));

        foreach (var point in points)
        {
            var matrix =
                Matrix4x4.CreateScale(PointRadius) *
                Matrix4x4.CreateTranslation(point);

            mb.AddInstance(pointMeshIndex, matrix);
        }

        return mb.Build();
    }
}