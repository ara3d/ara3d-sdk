using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// A script (generator or modifier) that wants interactive viewport handles implements this
/// and returns pure-data descriptors. The host discovers the capability by probing the
/// selected node's evaluator (like <see cref="IAnimated"/>) and owns everything else:
/// drawing through the overlay pass, hit-testing, drag capture, and writing the edited
/// value back through the property system (so the inspector stays in sync and the node
/// re-evaluates live). Descriptors are re-read whenever the selection or the script
/// changes, so hot reload needs no extra work.
/// </summary>
public interface IGizmoProvider
{
    IReadOnlyList<GizmoHandle> GetHandles();
}

public enum GizmoHandleKind
{
    /// <summary>Axis arrow: dragging moves the bound value along <see cref="GizmoHandle.Direction"/>.</summary>
    Arrow,

    /// <summary>Plane quad: dragging moves the bound value in the plane whose normal is
    /// <see cref="GizmoHandle.Direction"/>.</summary>
    Plane,

    /// <summary>Free-move ball at the gizmo center: dragging moves the bound value in the
    /// camera-facing plane (all three axes). <see cref="GizmoHandle.Direction"/> is ignored.</summary>
    Ball,
}

/// <summary>
/// One interactive handle. <paramref name="Binding"/> names a public Vector3 field or
/// property on the evaluator; the host adds the drag's world-space delta to it.
/// Directions are in world space.
/// </summary>
public readonly record struct GizmoHandle(GizmoHandleKind Kind, string Binding, Vector3 Direction);

public static class GizmoHandles
{
    /// <summary>The standard translation gizmo: three axis arrows, three plane quads,
    /// and a center free-move ball, all editing the same Vector3 parameter.</summary>
    public static IReadOnlyList<GizmoHandle> Translation(string binding)
        => [
            new(GizmoHandleKind.Arrow, binding, Vector3.UnitX),
            new(GizmoHandleKind.Arrow, binding, Vector3.UnitY),
            new(GizmoHandleKind.Arrow, binding, Vector3.UnitZ),
            new(GizmoHandleKind.Plane, binding, Vector3.UnitX),
            new(GizmoHandleKind.Plane, binding, Vector3.UnitY),
            new(GizmoHandleKind.Plane, binding, Vector3.UnitZ),
            new(GizmoHandleKind.Ball, binding, default),
        ];
}
