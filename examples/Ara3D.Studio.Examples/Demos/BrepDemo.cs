using Color = Ara3D.Geometry.Color;
using Material = Ara3D.Models.Material;

namespace Ara3D.Studio.Samples.Demos;

public enum BrepShape
{
    Box,
    Cylinder,
    Prism,
    TwistedBox,
    Compound,
}

/// <summary>
/// Demonstrates the experimental BREP sketch (see Brep.cs):
/// - Faces are tessellated independently at the chosen resolution, each with its own hue.
/// - Explode pulls faces apart along their centroids to reveal the boundary structure.
/// - Edges render as tubes sampled from their exact curves: twisting the box bends the
///   edge curves analytically, because deformation composes functions instead of moving points.
/// - Structural self-tests are visualized: vertices are green when Validate() passes and the
///   Euler characteristic is 2 per shell, red otherwise; edges are red unless shared by the
///   expected number of faces.
/// </summary>
[Category(Cat.ExperimentalDemos)]
public class BrepDemo : IGenerator
{
    public BrepShape Shape = BrepShape.TwistedBox;
    [Range(0.1f, 10f)] public float Width = 2f;
    [Range(0.1f, 10f)] public float Depth = 2f;
    [Range(0.1f, 10f)] public float Height = 3f;
    [Range(0.1f, 5f)] public float Radius = 1f;
    [Range(3, 12)] public int Sides = 6;
    [Range(0f, 1f)] public float Twist = 0.25f;
    [Range(1, 64)] public int Resolution = 12;
    [Range(0f, 3f)] public float Explode = 0f;
    public bool ShowFaces = true;
    public bool ShowEdges = true;
    public bool ShowVertices = true;
    [Range(0.005f, 0.2f)] public float EdgeRadius = 0.03f;
    [Range(0.01f, 0.5f)] public float VertexRadius = 0.08f;

    public int NumShells => Shape == BrepShape.Compound ? 2 : 1;

    public BrepSolid CreateSolid()
        => Shape switch
        {
            BrepShape.Box => BrepSolids.Box(Width, Depth, Height),
            BrepShape.Cylinder => BrepSolids.Cylinder(Radius, Height),
            BrepShape.Prism => BrepSolids.Prism(Sides, Radius, Height),
            BrepShape.TwistedBox => BrepSolids.Box(Width, Depth, Height).Deform(TwistPoint),
            _ => BrepSolids.Box(Width, Depth, Height)
                .Concat(BrepSolids.Cylinder(Radius, Height).Translate((0, 0, Height))),
        };

    public Point3D TwistPoint(Point3D p)
    {
        var angle = (Twist * p.Z / Height).Turns;
        var (c, s) = (angle.Cos, angle.Sin);
        return (p.X * c - p.Y * s, p.X * s + p.Y * c, p.Z);
    }

    public static Color Hue(float h)
        => new(
            Math.Clamp(Math.Abs(h * 6 - 3) - 1, 0f, 1f),
            Math.Clamp(2 - Math.Abs(h * 6 - 2), 0f, 1f),
            Math.Clamp(2 - Math.Abs(h * 6 - 4), 0f, 1f),
            1f);

    public static TriangleMesh3D UnitTube(float radius)
        => new RegularPolygon(Point2D.Zero, 12).ToPolyLine3D().Scale(radius)
            .Points.Extrude(1).Triangulate();

    public static TriangleMesh3D Marker(Point3D point, float radius)
        => PlatonicSolids.Icosahedron.Deform(p =>
            (p.X * radius + point.X, p.Y * radius + point.Y, p.Z * radius + point.Z));

    public Vector3 ExplodeOffset(BrepFace face, Point3D center)
    {
        var d = face.Centroid().Vector3 - center.Vector3;
        return d.Length > 1e-6f ? d.Normalize * Explode : Vector3.Default;
    }

    public void AddFaces(Model3DBuilder builder, BrepSolid solid, Point3D center)
    {
        for (var i = 0; i < solid.Faces.Count; i++)
        {
            var face = solid.Faces[i];
            var offset = ExplodeOffset(face, center);
            var mesh = face.Tessellate(Resolution)
                .Deform(p => (p.X + offset.X, p.Y + offset.Y, p.Z + offset.Z));
            var material = new Material(Hue(i / (float)solid.Faces.Count), 0f, 0.6f);
            builder.AddInstance(mesh, material);
        }
    }

    public void AddEdges(Model3DBuilder builder, BrepSolid solid)
    {
        var tube = builder.AddMeshWithoutInstance(UnitTube(EdgeRadius));
        for (var e = 0; e < solid.Edges.Count; e++)
        {
            // A boundary edge of a closed shell belongs to two faces (or one face twice, like a seam).
            var faceCount = 0;
            foreach (var f in solid.FacesOfEdge(e))
                faceCount += solid.Faces[f].EdgeUseCount(e);
            var color = faceCount == 2 ? new Color(0.9f, 0.9f, 0.9f, 1f) : new Color(1f, 0.1f, 0.1f, 1f);
            var material = new Material(color, 0.8f, 0.3f);

            var points = solid.SampleEdge(e, Resolution);
            for (var i = 1; i < points.Count; i++)
            {
                var line = new Line3D(points[i - 1], points[i]);
                if (line.Length < 1e-6f)
                    continue;
                builder.AddInstance(tube, line.AlignZAxisTransform(), material);
            }
        }
    }

    public void AddVertices(Model3DBuilder builder, BrepSolid solid)
    {
        var healthy = solid.Validate().Count == 0 && solid.EulerCharacteristic() == 2 * NumShells;
        var color = healthy ? new Color(0.1f, 0.9f, 0.2f, 1f) : new Color(1f, 0.1f, 0.1f, 1f);
        var material = new Material(color, 0.2f, 0.4f);
        foreach (var v in solid.Vertices)
            builder.AddInstance(Marker(v, VertexRadius), material);
    }

    public IModel3D Eval(EvalContext context)
    {
        var solid = CreateSolid();
        var center = solid.Centroid();
        var builder = new Model3DBuilder();
        if (ShowFaces) AddFaces(builder, solid, center);
        if (ShowEdges) AddEdges(builder, solid);
        if (ShowVertices) AddVertices(builder, solid);
        return builder.Build();
    }
}
