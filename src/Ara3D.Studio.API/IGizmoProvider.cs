using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// A script (generator or modifier) that wants interactive viewport handles implements
/// this and returns pure-data elements. The host discovers the capability by probing the
/// selected node's evaluator (like <see cref="IAnimated"/>) and owns everything else:
/// projecting the primitives, drawing through the overlay pass, hit-testing, easing the
/// hover/active style channel, solving the drag named by each element's constraint, and
/// writing the edited value back through the property system (inspector sync and re-eval
/// are automatic). Elements are re-read whenever the selection or the script changes, so
/// hot reload needs no extra work.
///
/// Coordinate convention: primitive positions are gizmo-local — the origin is the gizmo
/// anchor, the axes are world axes, and 1.0 is one "arm length" (a constant on-screen
/// size chosen by the host, ~78 px at the anchor's distance). Drag constraints are in
/// world units (distances) and radians (angles); use <see cref="GizmoBinding.Gain"/> to
/// convert (e.g. radians to degrees).
/// </summary>
public interface IGizmoProvider
{
    IReadOnlyList<GizmoElement> GetGizmoElements();

    /// <summary>Where the gizmo sits. OutputBounds (default) follows the edited object —
    /// right for transforms. InputBounds stays on the unmodified input — right for
    /// destructive modifiers (e.g. a plane cut) whose output shrinks under the tool.
    /// A method (not a property) so PropKit never shows it in the inspector.</summary>
    GizmoAnchor GetGizmoAnchor() => GizmoAnchor.OutputBounds;

    /// <summary>An extra world-space offset added to the bounds anchor, so the gizmo can
    /// ride a feature that the edited value moves rather than staying at the bounds center.
    /// The default (zero) keeps the gizmo on the bounds. Example: a plane cut returns
    /// <c>Normal * Offset</c> so the handle tracks the plane as you slide it. A method (not
    /// a property) so PropKit never shows it in the inspector.</summary>
    Vector3 GetGizmoAnchorOffset() => default;
}

public enum GizmoAnchor
{
    OutputBounds,
    InputBounds,
}

//==
// Visual primitives (positions in gizmo-local units; see IGizmoProvider remarks).

public abstract record GizmoPrimitive;

/// <summary>Thick anti-aliased line segment.</summary>
public sealed record GizmoLine(Vector3 A, Vector3 B, float WidthPx = 3f) : GizmoPrimitive;

/// <summary>Screen-space triangular arrow head at Tip, pointing along Direction.</summary>
public sealed record GizmoArrowHead(Vector3 Tip, Vector3 Direction, float SizePx = 13f) : GizmoPrimitive;

/// <summary>Screen-facing marker of constant pixel size.</summary>
public sealed record GizmoMarker(Vector3 Position, float SizePx = 8f,
    GizmoMarkerKind Kind = GizmoMarkerKind.Disc) : GizmoPrimitive;

public enum GizmoMarkerKind
{
    Disc,
    Square,
}

/// <summary>World-space circle of the given local-unit radius about Center, in the plane
/// perpendicular to Normal; projected (so it foreshortens to an ellipse).</summary>
public sealed record GizmoRing(Vector3 Center, Vector3 Normal, float Radius = 1f,
    float WidthPx = 3f) : GizmoPrimitive;

/// <summary>Filled and/or outlined quadrilateral (corners in gizmo-local space).</summary>
public sealed record GizmoQuad(Vector3 A, Vector3 B, Vector3 C, Vector3 D,
    float FillOpacity = 0.26f, float OutlineWidthPx = 1.4f) : GizmoPrimitive;

//==
// Drag constraints: what motion a drag on the element produces.

public abstract record GizmoConstraint;

/// <summary>Drag along a world axis; delta is a world-space distance.</summary>
public sealed record AxisDrag(Vector3 Direction) : GizmoConstraint;

/// <summary>Drag in the plane with the given world normal; delta is a world vector.</summary>
public sealed record PlaneDrag(Vector3 Normal) : GizmoConstraint;

/// <summary>Drag in the camera-facing plane; delta is a world vector.</summary>
public sealed record ViewPlaneDrag : GizmoConstraint;

/// <summary>Rotate about the world axis through the anchor; delta is a signed angle in
/// radians (counter-clockwise looking down the axis).</summary>
public sealed record AngleDrag(Vector3 Axis) : GizmoConstraint;

//==
// Binding: which evaluator member the drag edits.

/// <summary>
/// Names a public float or Vector3 field/property on the evaluator. The host applies
/// <c>base + delta * Gain</c>: scalar delta to a float member directly, scalar delta to a
/// Vector3 member along the constraint direction, vector delta to a Vector3 member
/// component-wise. Use Gain for unit conversion (e.g. <see cref="Degrees"/>).
/// </summary>
public sealed record GizmoBinding(string Name, float Gain = 1f)
{
    public static GizmoBinding Degrees(string name)
        => new(name, 180f / MathF.PI);
}

//==
// Style: per-state appearance; the host eases between states over TransitionSeconds.

public sealed record GizmoStyle(Vector4 Color, float Opacity = 1f, float Scale = 1f)
{
    public GizmoStyle Dimmed(float opacity)
        => this with { Opacity = opacity };

    public GizmoStyle Emphasized(float scale = 1.25f)
        => this with { Scale = scale };
}

/// <summary>
/// One interactive (or display-only) piece of a gizmo. Key gives the element a stable
/// identity for hover/active state across frames. Constraint and Binding may be null for
/// display-only elements. Hover styling eases in/out over TransitionSeconds.
/// </summary>
public sealed record GizmoElement(
    string Key,
    IReadOnlyList<GizmoPrimitive> Primitives,
    GizmoConstraint Constraint = null,
    GizmoBinding Binding = null,
    GizmoStyle Idle = null,
    GizmoStyle Hover = null,
    GizmoStyle Active = null,
    float TransitionSeconds = 0.12f,
    float HitTolerancePx = 9f)
{
    public bool IsInteractive => Constraint != null && Binding != null;
}

//==
// Standard gizmos, built from the same public vocabulary available to plugins.

public static class GizmoElements
{
    public static readonly Vector4 XColor = new(0.94f, 0.30f, 0.36f, 1f);
    public static readonly Vector4 YColor = new(0.46f, 0.83f, 0.32f, 1f);
    public static readonly Vector4 ZColor = new(0.30f, 0.56f, 0.98f, 1f);
    public static readonly Vector4 FreeColor = new(0.92f, 0.92f, 0.95f, 1f);

    public static Vector4 AxisColor(int axis)
        => axis == 0 ? XColor : axis == 1 ? YColor : ZColor;

    public static Vector3 AxisDirection(int axis)
        => axis == 0 ? Vector3.UnitX : axis == 1 ? Vector3.UnitY : Vector3.UnitZ;

    public static GizmoStyle Style(Vector4 color, float opacity = 0.95f)
        => new(color, opacity);

    private static (GizmoStyle Idle, GizmoStyle Hover, GizmoStyle Active) States(Vector4 color)
        => (new(color, 0.92f), new(color, 1f, 1.18f), new(color, 1f, 1.28f));

    /// <summary>Axis arrow (shaft + head) dragging the binding along the axis.</summary>
    public static GizmoElement AxisArrow(string key, int axis, GizmoBinding binding)
    {
        var d = AxisDirection(axis);
        var (idle, hover, active) = States(AxisColor(axis));
        return new(key,
            [new GizmoLine(d * 0.13f, d, 3f), new GizmoArrowHead(d, d)],
            new AxisDrag(d), binding, idle, hover, active);
    }

    /// <summary>Plane quad tucked into the origin corner, dragging in that plane.</summary>
    public static GizmoElement PlaneQuad(string key, int normalAxis, GizmoBinding binding)
    {
        var u = AxisDirection((normalAxis + 1) % 3);
        var v = AxisDirection((normalAxis + 2) % 3);
        var (idle, hover, active) = States(AxisColor(normalAxis));
        return new(key,
            [new GizmoQuad(default, u * 0.38f, (u + v) * 0.38f, v * 0.38f)],
            new PlaneDrag(AxisDirection(normalAxis)), binding, idle, hover, active);
    }

    /// <summary>Center free-move ball: view-plane drag of all three components.</summary>
    public static GizmoElement CenterBall(string key, GizmoBinding binding)
    {
        var (idle, hover, active) = States(FreeColor);
        return new(key, [new GizmoMarker(default, 8f)],
            new ViewPlaneDrag(), binding, idle with { Opacity = 0.55f }, hover, active);
    }

    /// <summary>Rotation ring about a world axis; scalar delta in radians times Gain.</summary>
    public static GizmoElement AxisRing(string key, int axis, GizmoBinding binding, float radius = 0.85f)
    {
        var (idle, hover, active) = States(AxisColor(axis));
        return new(key, [new GizmoRing(default, AxisDirection(axis), radius)],
            new AngleDrag(AxisDirection(axis)), binding, idle, hover, active);
    }

    /// <summary>Axis handle with a square end (scale convention).</summary>
    public static GizmoElement AxisSquareHandle(string key, int axis, GizmoBinding binding)
    {
        var d = AxisDirection(axis);
        var (idle, hover, active) = States(AxisColor(axis));
        return new(key,
            [new GizmoLine(d * 0.13f, d * 0.92f, 3f), new GizmoMarker(d * 0.92f, 7f, GizmoMarkerKind.Square)],
            new AxisDrag(d), binding, idle, hover, active);
    }

    /// <summary>Plane-scale triangle tucked into the origin corner between the plane's two
    /// axes, dragging in that plane — the triangular counterpart of the translation gizmo's
    /// <see cref="PlaneQuad"/> square. Rendered as a quad whose last corner repeats, so no
    /// new primitive is needed.</summary>
    public static GizmoElement PlaneScaleTriangle(string key, int normalAxis, GizmoBinding binding)
    {
        var u = AxisDirection((normalAxis + 1) % 3);
        var v = AxisDirection((normalAxis + 2) % 3);
        var (idle, hover, active) = States(AxisColor(normalAxis));
        return new(key,
            [new GizmoQuad(default, u * 0.45f, v * 0.45f, v * 0.45f)],
            new PlaneDrag(AxisDirection(normalAxis)), binding, idle, hover, active);
    }

    /// <summary>The standard translation gizmo: three arrows, three plane quads, center ball.</summary>
    public static IReadOnlyList<GizmoElement> Translation(string binding)
    {
        var b = new GizmoBinding(binding);
        return
        [
            PlaneQuad("plane-x", 0, b), PlaneQuad("plane-y", 1, b), PlaneQuad("plane-z", 2, b),
            AxisArrow("axis-x", 0, b), AxisArrow("axis-y", 1, b), AxisArrow("axis-z", 2, b),
            CenterBall("center", b),
        ];
    }

    /// <summary>The standard rotation gizmo: one ring per axis, each bound (in degrees)
    /// to its own float member.</summary>
    public static IReadOnlyList<GizmoElement> RotationRings(string xBinding, string yBinding, string zBinding)
        =>
        [
            AxisRing("ring-x", 0, GizmoBinding.Degrees(xBinding), 0.80f),
            AxisRing("ring-y", 1, GizmoBinding.Degrees(yBinding), 0.86f),
            AxisRing("ring-z", 2, GizmoBinding.Degrees(zBinding), 0.92f),
        ];

    /// <summary>The standard scale gizmo: square-ended axis handles, plane-scale triangles,
    /// plus a uniform center square dragging all three components along the (1,1,1)
    /// diagonal.</summary>
    public static IReadOnlyList<GizmoElement> ScaleHandles(string binding)
    {
        var b = new GizmoBinding(binding);
        var (idle, hover, active) = (new GizmoStyle(FreeColor, 0.55f),
            new GizmoStyle(FreeColor, 1f, 1.18f), new GizmoStyle(FreeColor, 1f, 1.28f));
        var diagonal = new Vector3(1f, 1f, 1f).Normalize;
        return
        [
            PlaneScaleTriangle("plane-x", 0, b), PlaneScaleTriangle("plane-y", 1, b), PlaneScaleTriangle("plane-z", 2, b),
            AxisSquareHandle("scale-x", 0, b), AxisSquareHandle("scale-y", 1, b), AxisSquareHandle("scale-z", 2, b),
            new("scale-uniform", [new GizmoMarker(default, 8f, GizmoMarkerKind.Square)],
                new AxisDrag(diagonal), b, idle, hover, active),
        ];
    }
}
