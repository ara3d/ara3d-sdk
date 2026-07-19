namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Grid-snap picking as it would feel with a real cursor. Scripts cannot see the mouse yet
/// (in-canvas widgets are tracker issue studio-001), so the "cursor" is either two sliders or an
/// animated point gliding over the grid. The blue sphere is the free cursor; the green marker is
/// where a snapping pick would land (nearest intersection or cell center); the hovered cell
/// lights up. SnappedX/SnappedY read back the picked coordinate, panel-synced every frame.
/// </summary>
[Category(nameof(Categories.Demos))]
[Animated]
public class GridSnapPick : IGenerator
{
    [Range(2, 30)] public int Cells = 10;
    [Range(0.25f, 5f)] public float CellSize = 1f;
    [Range(0f, 1f)] public float CursorU = 0.37f;
    [Range(0f, 1f)] public float CursorV = 0.62f;
    public bool AnimateCursor = true;
    public bool SnapToIntersections = true;
    [Range(0.05f, 1f)] public float MarkerSize = 0.3f;

    public float SnappedX { get; private set; }
    public float SnappedY { get; private set; }

    public IModel3D Eval(EvalContext ctx)
    {
        var extent = Cells * CellSize;
        var half = extent / 2f;
        var t = (float)ctx.AnimationTime;
        var u = AnimateCursor ? 0.5f + 0.45f * MathF.Sin(t * 0.6f) : CursorU;
        var v = AnimateCursor ? 0.5f + 0.45f * MathF.Sin(t * 1.0f + 1.3f) : CursorV;
        var cx = u * extent - half;
        var cy = v * extent - half;

        var sx = SnapToIntersections
            ? MathF.Round((cx + half) / CellSize) * CellSize - half
            : (Col(cx, half) + 0.5f) * CellSize - half;
        var sy = SnapToIntersections
            ? MathF.Round((cy + half) / CellSize) * CellSize - half
            : (Col(cy, half) + 0.5f) * CellSize - half;
        SnappedX = sx;
        SnappedY = sy;
        ctx.Services.RefreshUI(this);

        var hoverCol = Col(cx, half);
        var hoverRow = Col(cy, half);
        var tileHalf = CellSize * 0.45f;
        var tile = new Bounds3D(
            new Point3D(-tileHalf, -tileHalf, -CellSize * 0.05f),
            new Point3D(tileHalf, tileHalf, 0f)).ToMesh().Triangulate();

        var centers = new List<Point3D>();
        for (var i = 0; i < Cells; i++)
        for (var j = 0; j < Cells; j++)
            if (i != hoverCol || j != hoverRow)
                centers.Add(new Point3D((i + 0.5f) * CellSize - half, (j + 0.5f) * CellSize - half, 0f));
        var hoverCenter = new Point3D((hoverCol + 0.5f) * CellSize - half, (hoverRow + 0.5f) * CellSize - half, CellSize * 0.05f);

        var cursorHeight = CellSize * 0.75f;
        var postHalf = CellSize * 0.02f;
        var post = new Bounds3D(
            new Point3D(-postHalf, -postHalf, 0f),
            new Point3D(postHalf, postHalf, cursorHeight)).ToMesh().Triangulate();

        var parts = new List<IModel3D>
        {
            tile.Clone(Material.Default.WithColor((0.45f, 0.47f, 0.52f, 1f)), centers),
            tile.Clone(Material.Default.WithColor((1f, 0.6f, 0.1f, 1f)), new[] { hoverCenter }),
            PlatonicSolids.Icosahedron.Scale(MarkerSize * 0.5f)
                .Clone(Material.Default.WithColor((0.2f, 0.55f, 1f, 1f)), new[] { new Point3D(cx, cy, cursorHeight) }),
            PlatonicSolids.Octahedron.Scale(MarkerSize)
                .Clone(Material.Default.WithColor((0.1f, 0.9f, 0.4f, 1f)), new[] { new Point3D(sx, sy, 0.1f) }),
            post.Clone(Material.Default.WithColor((0.1f, 0.9f, 0.4f, 0.6f)), new[] { new Point3D(sx, sy, 0f) }),
        };
        return parts.Merge();
    }

    private int Col(float x, float half)
        => Math.Clamp((int)MathF.Floor((x + half) / CellSize), 0, Cells - 1);
}
