namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Fits a PCA-oriented bounding box to the input mesh and returns it as a box mesh, so the tight
/// oriented box is drawn aligned to the geometry. Reports the box's per-axis size and volume.
/// </summary>
[Category(Cat.Display)]
[Description("Fits and displays the PCA-oriented (tight) bounding box of the mesh, reporting its size and volume.")]
public class OrientedBoundingBox : IModifier
{
    public float SizeX { get; private set; }
    public float SizeY { get; private set; }
    public float SizeZ { get; private set; }
    public double Volume { get; private set; }

    public QuadMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var obb = mesh.Points.FitOrientedBox();
        SizeX = obb.Size.X;
        SizeY = obb.Size.Y;
        SizeZ = obb.Size.Z;
        Volume = obb.GetVolume();
        ctx.Services.RefreshUI(this);
        return PlatonicSolids.Cube.Transform(obb.ToMatrix());
    }
}

/// <summary>
/// Extracts every open (naked) boundary edge of the mesh and returns them as disjoint line segments,
/// the standard overlay for revealing holes and open borders. Reports the boundary edge count.
/// </summary>
[Category(Cat.Display)]
[Description("Shows the open boundary (naked) edges of the mesh as line segments, reporting how many there are.")]
public class BoundaryEdges : IModifier
{
    public int BoundaryEdgeCount { get; private set; }

    public LineMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var topology = mesh.DerivedTopology();
        var points = new List<Point3D>();
        var indices = new List<Integer2>();

        foreach (var edge in topology.GetUndirectedEdgeIds())
        {
            if (!topology.IsBoundary(edge))
                continue;
            var i0 = points.Count;
            points.Add(mesh.Points[(int)topology.GetVertexA(edge)]);
            points.Add(mesh.Points[(int)topology.GetVertexB(edge)]);
            indices.Add((i0, i0 + 1));
        }

        BoundaryEdgeCount = indices.Count;
        ctx.Services.RefreshUI(this);
        return indices.Count == 0 ? LineMesh3D.Default : new LineMesh3D(points, indices);
    }
}
