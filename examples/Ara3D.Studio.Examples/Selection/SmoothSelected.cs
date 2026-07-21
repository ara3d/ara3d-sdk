namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Uniform Laplacian smoothing restricted to the vertices of the selected faces.
/// With no selection present the whole mesh is smoothed — every selection-aware
/// tool degrades to a normal tool.
/// </summary>
[Category(nameof(Categories.Selection))]
public class SmoothSelected : IModifier
{
    [Range(0, 50)] public int Iterations = 5;
    [Range(0f, 1f)] public float Strength = 0.5f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var selection = ctx.GetFaceSelection(mesh);
        var movable = MovableVertices(mesh, selection);
        var points = new Point3D[mesh.Points.Count];
        for (var i = 0; i < points.Length; i++)
            points[i] = mesh.Points[i];
        var sums = new Vector3[points.Length];
        var counts = new int[points.Length];
        for (var iteration = 0; iteration < Iterations; iteration++)
        {
            Array.Clear(sums);
            Array.Clear(counts);
            for (var f = 0; f < mesh.FaceIndices.Count; f++)
            {
                var face = mesh.FaceIndices[f];
                Accumulate(sums, counts, points, face.A, face.B, face.C);
                Accumulate(sums, counts, points, face.B, face.A, face.C);
                Accumulate(sums, counts, points, face.C, face.A, face.B);
            }
            for (var i = 0; i < points.Length; i++)
            {
                if (!movable[i] || counts[i] == 0)
                    continue;
                var average = sums[i] / counts[i];
                points[i] = points[i].Vector3.Lerp(average, Strength);
            }
        }
        return mesh.WithPoints(points);
    }

    private static bool[] MovableVertices(TriangleMesh3D mesh, SelectionSet? selection)
    {
        var movable = new bool[mesh.Points.Count];
        if (selection == null)
        {
            Array.Fill(movable, true);
            return movable;
        }
        for (var i = 0; i < selection.Indices.Count; i++)
        {
            var face = mesh.FaceIndices[selection.Indices[i]];
            movable[face.A] = true;
            movable[face.B] = true;
            movable[face.C] = true;
        }
        return movable;
    }

    private static void Accumulate(Vector3[] sums, int[] counts, Point3D[] points, int target, int n0, int n1)
    {
        sums[target] += points[n0].Vector3 + points[n1].Vector3;
        counts[target] += 2;
    }
}
