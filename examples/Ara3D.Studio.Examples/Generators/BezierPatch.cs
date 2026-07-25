namespace Ara3D.Studio.Samples.Generators;

/// <summary>
/// A bicubic Bézier patch: a 4 × 4 net of world-space control points, evaluated as a
/// <see cref="ParametricSurface"/> (the renderer tessellates it at the node's resolution).
/// Fields are named P{u}{v} — the first index steps along U, the second along V. The gizmo
/// anchors at the world origin so every handle sits on its actual control point: drag any of
/// the sixteen discs in the view plane to shape the patch, and the faint lines show the
/// control net. The four corners (bright, larger) are the only points the surface passes
/// through; the rest pull it like tangent and twist handles.
/// </summary>
[Category(Cat.Surfaces)]
[Description("A bicubic Bézier patch shaped by dragging its sixteen control points in the viewport.")]
public class BezierPatch : IGenerator, IGizmoProvider
{
    public Vector3 P00 = (-3f, -3f, 0f);
    public Vector3 P01 = (-3f, -1f, 0f);
    public Vector3 P02 = (-3f, 1f, 0f);
    public Vector3 P03 = (-3f, 3f, 0f);

    public Vector3 P10 = (-1f, -3f, 0f);
    public Vector3 P11 = (-1f, -1f, 3f);
    public Vector3 P12 = (-1f, 1f, 3f);
    public Vector3 P13 = (-1f, 3f, 0f);

    public Vector3 P20 = (1f, -3f, 0f);
    public Vector3 P21 = (1f, -1f, 3f);
    public Vector3 P22 = (1f, 1f, 3f);
    public Vector3 P23 = (1f, 3f, 0f);

    public Vector3 P30 = (3f, -3f, 0f);
    public Vector3 P31 = (3f, -1f, 0f);
    public Vector3 P32 = (3f, 1f, 0f);
    public Vector3 P33 = (3f, 3f, 0f);

    /// <summary>The control net, flattened row-major (index u * 4 + v) as the patch expects.
    /// A method, not a property, so it stays out of the inspector.</summary>
    public IReadOnlyList<Point3D> ControlNet()
        =>
        [
            P00, P01, P02, P03,
            P10, P11, P12, P13,
            P20, P21, P22, P23,
            P30, P31, P32, P33,
        ];

    public ParametricSurface Eval()
        => ControlNet().BicubicBezierPatch();

    public GizmoAnchor GetGizmoAnchor()
        => GizmoAnchor.WorldOrigin;

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        =>
        [
            Net(ControlNet()),
            Corner(P00, nameof(P00)), Handle(P01, nameof(P01)), Handle(P02, nameof(P02)), Corner(P03, nameof(P03)),
            Handle(P10, nameof(P10)), Handle(P11, nameof(P11)), Handle(P12, nameof(P12)), Handle(P13, nameof(P13)),
            Handle(P20, nameof(P20)), Handle(P21, nameof(P21)), Handle(P22, nameof(P22)), Handle(P23, nameof(P23)),
            Corner(P30, nameof(P30)), Handle(P31, nameof(P31)), Handle(P32, nameof(P32)), Corner(P33, nameof(P33)),
        ];

    private static readonly Vector4 NetColor = new(0.62f, 0.66f, 0.74f, 1f);
    private static readonly Vector4 HandleColor = new(0.36f, 0.62f, 0.98f, 1f);

    /// <summary>One draggable control point. The host scales a hovered element about the anchor
    /// (here the world origin), so hover reads as a brighter, fully opaque disc rather than a
    /// scale change, which would slide the handle away from its point.</summary>
    private static GizmoElement Handle(Vector3 p, string name, float sizePx = 6f, Vector4? color = null)
        => new(name,
            [new GizmoMarker(p, sizePx)],
            new ViewPlaneDrag(), new GizmoBinding(name),
            new GizmoStyle(color ?? HandleColor, 0.62f),
            new GizmoStyle(color ?? HandleColor, 1f),
            new GizmoStyle(GizmoElements.FreeColor, 1f),
            Space: GizmoSpace.World);

    /// <summary>A corner: interpolated by the surface, so it gets the emphasis.</summary>
    private static GizmoElement Corner(Vector3 p, string name)
        => Handle(p, name, 8f, GizmoElements.FreeColor);

    /// <summary>Display-only wireframe of the control net — the rows and columns of the 4 × 4
    /// grid, which read as the patch's tangent structure.</summary>
    private static GizmoElement Net(IReadOnlyList<Point3D> net)
    {
        var lines = new List<GizmoPrimitive>();
        for (var i = 0; i < 4; i++)
        for (var j = 0; j < 3; j++)
        {
            lines.Add(new GizmoLine(net[i * 4 + j], net[i * 4 + j + 1], 1.4f));
            lines.Add(new GizmoLine(net[j * 4 + i], net[(j + 1) * 4 + i], 1.4f));
        }
        return new("net", lines, Idle: new GizmoStyle(NetColor, 0.45f), Space: GizmoSpace.World);
    }
}
