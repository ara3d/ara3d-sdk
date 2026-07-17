using Color = Ara3D.Geometry.Color;
using Material = Ara3D.Models.Material;

namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Shatters a mesh into Voronoi fragments — the Voronoi idea applied to real 3D geometry.
/// Seed points are scattered through the mesh's bounding box, optionally evened out with a few
/// Lloyd/k-means passes over the triangle centroids, then every triangle is assigned to its nearest
/// seed (the defining Voronoi property: "closest site wins"). Each seed's triangles become one
/// colored fragment; Explode pushes the fragments radially apart so the partition is visible.
/// A 3D companion to the flat <c>VoronoiDiagramDemo</c> generator (ara3d-040).
/// </summary>
[Category(nameof(Categories.Meshes))]
public class VoronoiShatter : IModifier
{
    [Range(2, 300)] public int Fragments = 24;
    public int Seed;
    [Range(0, 8)] public int Relaxation = 2;
    [Range(0f, 2f)] public float Explode = 0.25f;

    static Vector3 FaceCentroid(TriangleMesh3D mesh, Integer3 f)
        => (mesh.Points[f.A].Vector3 + mesh.Points[f.B].Vector3 + mesh.Points[f.C].Vector3) * (1f / 3f);

    static int Nearest(IReadOnlyList<Vector3> seeds, Vector3 p)
    {
        var best = 0;
        var bestD = float.MaxValue;
        for (var i = 0; i < seeds.Count; ++i)
        {
            var d = (float)(seeds[i] - p).LengthSquared();
            if (d < bestD) { bestD = d; best = i; }
        }

        return best;
    }

    public IModel3D Eval(TriangleMesh3D mesh)
    {
        var faces = mesh.FaceIndices;
        if (faces.Count == 0)
            return new Model3DBuilder().Build();

        var bounds = mesh.Bounds;
        Vector3 lo = bounds.Min.Vector3, hi = bounds.Max.Vector3;
        var size = (float)bounds.Size.Length;

        var centroids = new Vector3[faces.Count];
        for (var i = 0; i < faces.Count; ++i)
            centroids[i] = FaceCentroid(mesh, faces[i]);

        var random = new Random(Seed);
        var seeds = new List<Vector3>(Fragments);
        for (var i = 0; i < Fragments; ++i)
            seeds.Add(new Vector3(
                (float)lo.X + random.NextSingle() * ((float)hi.X - (float)lo.X),
                (float)lo.Y + random.NextSingle() * ((float)hi.Y - (float)lo.Y),
                (float)lo.Z + random.NextSingle() * ((float)hi.Z - (float)lo.Z)));

        // Lloyd / k-means over the triangle centroids: pull the seeds onto the actual geometry so
        // fragments come out evenly sized instead of clumped where the random seeds happened to land.
        var assignment = new int[faces.Count];
        for (var iter = 0; iter <= Relaxation; ++iter)
        {
            for (var i = 0; i < centroids.Length; ++i)
                assignment[i] = Nearest(seeds, centroids[i]);

            if (iter == Relaxation)
                break;

            var sum = new Vector3[seeds.Count];
            var count = new int[seeds.Count];
            for (var i = 0; i < centroids.Length; ++i)
            {
                sum[assignment[i]] += centroids[i];
                count[assignment[i]]++;
            }

            for (var i = 0; i < seeds.Count; ++i)
                if (count[i] > 0)
                    seeds[i] = sum[i] * (1f / count[i]);
        }

        var center = bounds.Center.Vector3;
        var groups = new List<Integer3>[seeds.Count];
        for (var i = 0; i < faces.Count; ++i)
            (groups[assignment[i]] ??= new List<Integer3>()).Add(faces[i]);

        var mb = new Model3DBuilder();
        for (var g = 0; g < groups.Length; ++g)
        {
            if (groups[g] == null)
                continue;

            var fragment = SubMesh(mesh, groups[g]);
            var dir = seeds[g] - center;
            var len = (float)dir.Length;
            var offset = len > 1e-5f ? dir * (1f / len) * (Explode * size * 0.5f) : Vector3.Zero;
            var color = Demos.DelaunayTriangulationDemo.Pastel(g / (float)Math.Max(seeds.Count, 1));
            mb.AddInstance(fragment.Translate(offset), new Material(color, 0f, 0.6f));
        }

        return mb.Build();
    }

    static TriangleMesh3D SubMesh(TriangleMesh3D mesh, IReadOnlyList<Integer3> faces)
    {
        var map = new Dictionary<int, int>();
        var points = new List<Point3D>();
        var indices = new List<Integer3>(faces.Count);

        int Remap(int i)
        {
            if (map.TryGetValue(i, out var j))
                return j;

            j = points.Count;
            points.Add(mesh.Points[i]);
            map[i] = j;
            return j;
        }

        foreach (var f in faces)
            indices.Add(new Integer3(Remap(f.A), Remap(f.B), Remap(f.C)));

        return new TriangleMesh3D(points, indices);
    }
}
