namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Grid-snap picking driven by the real mouse. [PointerTracking] re-evaluates this node as the
/// pointer (or camera) moves; the cursor ray comes from ctx.Services.ViewportInput and is
/// intersected with the grid plane (Z=0). The blue sphere follows the mouse; the green marker is
/// where a snapping pick lands (nearest intersection or cell center); the hovered cell lights up.
/// Left-click (without dragging — drags orbit the camera) commits a point (yellow). On hosts
/// without a viewport, or with AnimateCursor on, a slider/animation-driven cursor stands in.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[Animated]
[PointerTracking]
public class GridSnapPick : IGenerator
{
    [Range(2, 30)] public int Cells = 10;
    [Range(0.25f, 5f)] public float CellSize = 1f;
    [Range(0f, 1f)] public float CursorU = 0.37f;
    [Range(0f, 1f)] public float CursorV = 0.62f;
    public bool AnimateCursor;
    public bool SnapToIntersections = true;
    [Range(0.05f, 1f)] public float MarkerSize = 0.3f;
    public bool ClearPoints { get; set; }

    public float SnappedX { get; private set; }
    public float SnappedY { get; private set; }
    public int CommittedCount { get; private set; }

    private readonly List<Point3D> _committed = new();
    private float _cx;
    private float _cy;

    public IModel3D Eval(EvalContext ctx)
    {
        var extent = Cells * CellSize;
        var half = extent / 2f;
        var input = ctx.Services.ViewportInput;

        if (input != null && !AnimateCursor)
        {
            var ray = input.CursorRay;
            float oz = ray.Origin.Z, dz = ray.Direction.Z;
            if (MathF.Abs(dz) > 1e-6f)
            {
                var t = -oz / dz;
                if (t > 0)
                {
                    _cx = ray.Origin.X + ray.Direction.X * t;
                    _cy = ray.Origin.Y + ray.Direction.Y * t;
                }
            }
        }
        else
        {
            var time = (float)ctx.AnimationTime;
            var u = AnimateCursor ? 0.5f + 0.45f * MathF.Sin(time * 0.6f) : CursorU;
            var v = AnimateCursor ? 0.5f + 0.45f * MathF.Sin(time * 1.0f + 1.3f) : CursorV;
            _cx = u * extent - half;
            _cy = v * extent - half;
        }

        var sx = SnapToIntersections
            ? MathF.Round((_cx + half) / CellSize) * CellSize - half
            : (Col(_cx, half) + 0.5f) * CellSize - half;
        var sy = SnapToIntersections
            ? MathF.Round((_cy + half) / CellSize) * CellSize - half
            : (Col(_cy, half) + 0.5f) * CellSize - half;

        if (ClearPoints)
        {
            _committed.Clear();
            ClearPoints = false;
        }
        if (input != null && input.ConsumePrimaryClick())
            _committed.Add(new Point3D(sx, sy, 0f));

        SnappedX = sx;
        SnappedY = sy;
        CommittedCount = _committed.Count;
        ctx.Services.RefreshUI(this);

        var hoverCol = Col(_cx, half);
        var hoverRow = Col(_cy, half);
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
                .Clone(Material.Default.WithColor((0.2f, 0.55f, 1f, 1f)), new[] { new Point3D(_cx, _cy, cursorHeight) }),
            PlatonicSolids.Octahedron.Scale(MarkerSize)
                .Clone(Material.Default.WithColor((0.1f, 0.9f, 0.4f, 1f)), new[] { new Point3D(sx, sy, 0.1f) }),
            post.Clone(Material.Default.WithColor((0.1f, 0.9f, 0.4f, 0.6f)), new[] { new Point3D(sx, sy, 0f) }),
        };
        if (_committed.Count > 0)
            parts.Add(PlatonicSolids.Octahedron.Scale(MarkerSize)
                .Clone(Material.Default.WithColor((1f, 0.85f, 0.1f, 1f)), _committed.ToList()));
        return parts.Merge();
    }

    private int Col(float x, float half)
        => Math.Clamp((int)MathF.Floor((x + half) / CellSize), 0, Cells - 1);
}
