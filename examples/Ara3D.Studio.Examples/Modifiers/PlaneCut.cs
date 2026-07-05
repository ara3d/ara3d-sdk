namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Cuts a triangle mesh with a horizontal plane, keeping geometry at or below the plane.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class MeshHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.Bounds;
        if (Height == 0) return ([], []);
        var z = bounds.Min.Z.Lerp(bounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);
        return TriangleMeshPlaneCut.CutBelow(mesh, plane);
    }
}

/// <summary>
/// Cuts a model with a horizontal plane, keeping geometry at or below the plane.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class ModelHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public IModel3D Eval(IModel3D model)
    {
        var mb = new Model3DBuilder();
        var totalBounds = model.GetBounds();
        var instanceBounds = model.GetInstanceBounds();
        var z = totalBounds.Min.Z.Lerp(totalBounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);

        var meshRemap = new Dictionary<int, int>();
        for (int i = 0; i < model.Instances.Count; i++)
        {
            var inst = model.Instances[i];
            if (inst.MeshIndex < 0)
            {
                mb.AddInstance(inst);
                continue;
            }

            var bounds = instanceBounds[i];
            
            // Completly skip the bounds 
            if (bounds.Min.Z >= z)
                continue;

            if (bounds.Max.Z <= z)
                mb.AddInstanceAndRemapMesh(inst, model, meshRemap);
            else
            {
                var mesh = model.GetTransformedMesh(inst);
                var cutMesh = TriangleMeshPlaneCut.CutBelow(mesh, plane);

                // NOTE: we end up baking the instance transform into the mesh 
                // This may or may not be desirable, but it is easier 
                mb.AddInstance(cutMesh, inst.Material);
            }
        }

        return mb.Build();
    }
}

internal static class TriangleMeshPlaneCut
{
    private const float Eps = 1e-5f;

    // TODO: is this really the best name? 
    public static TriangleMesh3D CutBelow(this TriangleMesh3D mesh, Plane plane)
    {
        var builder = new TriangleMesh3DBuilder();

        foreach (var face in mesh.FaceIndices)
        {
            foreach (var tri in ClipBelow(mesh.Triangle(face), plane, Eps))
            {
                var i0 = builder.Points.Count;
                builder.Points.Add(tri.A);
                var i1 = builder.Points.Count;
                builder.Points.Add(tri.B);
                var i2 = builder.Points.Count;
                builder.Points.Add(tri.C);
                builder.Faces.Add((i0, i1, i2));
            }
        }

        return builder.ToTriangleMesh3D();
    }

    public static List<Triangle3D> ClipBelow(this Triangle3D tri, Plane plane, float eps)
    {
        var polygon = ClipPolygonBelow([tri.A, tri.B, tri.C], plane, eps);
        return FanTriangulate(polygon);
    }

    public static List<Point3D> ClipPolygonBelow(this IReadOnlyList<Point3D> polygon, Plane plane, float eps)
    {
        if (polygon.Count < 3)
            return [];

        var output = new List<Point3D>();

        for (var i = 0; i < polygon.Count; i++)
        {
            var start = polygon[i];
            var end = polygon[(i + 1) % polygon.Count];
            var ds = plane.DotCoordinate(start);
            var de = plane.DotCoordinate(end);

            if (ds <= eps)
                output.Add(start);

            if (ds * de < 0)
                output.Add(IntersectEdge(start, ds, end, de));
        }

        return output;
    }

    public static Point3D IntersectEdge(Point3D a, float da, Point3D b, float db)
    {
        var t = da / (da - db);
        return a.Lerp(b, t);
    }

    public static List<Triangle3D> FanTriangulate(this IReadOnlyList<Point3D> polygon)
    {
        if (polygon.Count < 3)
            return [];

        if (polygon.Count == 3)
            return [new Triangle3D(polygon[0], polygon[1], polygon[2])];

        var triangles = new List<Triangle3D>();
        var v0 = polygon[0];
        for (var i = 1; i < polygon.Count - 1; i++)
            triangles.Add(new Triangle3D(v0, polygon[i], polygon[i + 1]));

        return triangles;
    }
}
