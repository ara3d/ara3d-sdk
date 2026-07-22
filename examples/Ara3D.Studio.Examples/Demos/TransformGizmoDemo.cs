namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// The classic translate/rotate/scale gizmo rendered as parameter-driven geometry. Studio scripts
/// cannot see the mouse yet (in-canvas widgets are tracker issue studio-001), so the sliders stand
/// in for dragging: move them and the gizmo frame and model respond live — the same two-way sync a
/// real viewport manipulator would drive from the other end. The gizmo shows the local frame after
/// rotation, the way Blender/Maya/Unity draw it in "local" orientation mode.
/// </summary>
[Category(Cat.ExperimentalDemos)]
public class TransformGizmo : IModifier
{
    [Range(-20f, 20f)] public float PositionX = 0f;
    [Range(-20f, 20f)] public float PositionY = 0f;
    [Range(-20f, 20f)] public float PositionZ = 0f;
    [Range(-180f, 180f)] public float RotateX = 0f;
    [Range(-180f, 180f)] public float RotateY = 0f;
    [Range(-180f, 180f)] public float RotateZ = 30f;
    [Range(0.1f, 4f)] public float Scale = 1f;
    [Range(0.2f, 20f)] public float GizmoSize = 2f;
    public bool ShowRings = true;
    public bool ShowModel = true;

    private const float DegToRad = MathF.PI / 180f;

    public IModel3D Eval(IModel3D input)
    {
        var rotation =
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, RotateZ * DegToRad) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, RotateY * DegToRad) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, RotateX * DegToRad);
        var position = new Vector3(PositionX, PositionY, PositionZ);
        var transform = new TRSTransform3D(position, rotation, new Vector3(Scale, Scale, Scale));

        var parts = new List<IModel3D> { BuildGizmo(new Pose3D(position, rotation)) };
        if (ShowModel)
            parts.Add(input.Transform(transform));
        return parts.Merge();
    }

    public IModel3D BuildGizmo(Matrix4x4 frame)
    {
        var arrow = Primitives.DefaultUpArrow().Triangulate().Scale(GizmoSize);
        var center = PlatonicSolids.Octahedron.Scale(GizmoSize * 0.12f);
        var parts = new List<IModel3D>
        {
            arrow.Clone(AxisMaterial(0), new[] { Axis(Vector3.UnitX) * frame }),
            arrow.Clone(AxisMaterial(1), new[] { Axis(Vector3.UnitY) * frame }),
            arrow.Clone(AxisMaterial(2), new[] { Axis(Vector3.UnitZ) * frame }),
            center.Clone(Material.Default.WithColor((1f, 0.85f, 0.1f, 1f)), new[] { frame }),
        };
        if (ShowRings)
        {
            var ring = RingMesh(GizmoSize * 0.75f, GizmoSize * 0.02f);
            parts.Add(ring.Clone(AxisMaterial(0), new[] { Axis(Vector3.UnitX) * frame }));
            parts.Add(ring.Clone(AxisMaterial(1), new[] { Axis(Vector3.UnitY) * frame }));
            parts.Add(ring.Clone(AxisMaterial(2), new[] { Axis(Vector3.UnitZ) * frame }));
        }
        return parts.Merge();
    }

    private static Matrix4x4 Axis(Vector3 dir)
        => new Pose3D(Vector3.Zero, dir.AlignZAxisWith());

    private static Material AxisMaterial(int axis)
        => Material.Default.WithColor(axis switch
        {
            0 => (0.9f, 0.15f, 0.15f, 1f),
            1 => (0.15f, 0.75f, 0.15f, 1f),
            _ => (0.2f, 0.4f, 1f, 1f),
        });

    // Torus around Z; the cross-section is closed by repeating its first point
    // (Revolve only closes the sweep direction).
    private static TriangleMesh3D RingMesh(float radius, float tube)
    {
        const int sides = 12;
        var section = new Point3D[sides + 1];
        for (var i = 0; i <= sides; i++)
        {
            var a = i * 2f * MathF.PI / sides;
            section[i] = new Point3D(radius + tube * MathF.Cos(a), 0f, tube * MathF.Sin(a));
        }
        return section.Revolve(Vector3.UnitZ, 48).Triangulate();
    }
}
