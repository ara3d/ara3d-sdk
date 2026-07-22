namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Cuts a model with an arbitrarily oriented plane, keeping geometry below it. The plane
/// passes through the input's bounds center offset along its normal; the normal is given
/// by yaw/pitch angles. The gizmo (anchored on the INPUT bounds, so it doesn't chase the
/// shrinking cut result) offers: an arrow along the normal to slide the plane, a blue
/// ring to yaw the normal about Z, a green ring to pitch it, and a translucent quad
/// showing the plane's orientation.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class PlaneCut : IModifier, IGizmoProvider
{
    [Range(-50f, 50f)] public float Offset;
    [Range(-180f, 180f)] public float NormalYaw;
    [Range(-89f, 89f)] public float NormalPitch = 35f;

    public Vector3 Normal
    {
        get
        {
            var yaw = NormalYaw.Degrees();
            var pitch = NormalPitch.Degrees();
            return new Vector3(pitch.Cos * yaw.Cos, pitch.Cos * yaw.Sin, pitch.Sin);
        }
    }

    public IModel3D Eval(IModel3D model)
    {
        var n = Normal;
        var point = model.GetBounds().Center.Vector3 + n * Offset;
        var plane = new Plane(n, -n.Dot(point));
        var mb = new Model3DBuilder();
        for (var i = 0; i < model.Instances.Count; i++)
        {
            var inst = model.Instances[i];
            if (inst.MeshIndex < 0)
            {
                mb.AddInstance(inst);
                continue;
            }
            var cut = model.GetTransformedMesh(inst).ClipBelow(plane);
            mb.AddInstance(cut, inst.Material);
        }
        return mb.Build();
    }

    public GizmoAnchor GetGizmoAnchor() => GizmoAnchor.InputBounds;

    public IReadOnlyList<GizmoElement> GetGizmoElements()
    {
        var n = Normal;
        var yaw = NormalYaw.Degrees();
        var pitchAxis = new Vector3(yaw.Sin, -yaw.Cos, 0f);

        var u = n.Cross(Vector3.UnitZ.Cross(n).Length > 1e-4f ? Vector3.UnitZ : Vector3.UnitX).Normalize;
        var v = n.Cross(u);

        var arrowColor = GizmoElements.FreeColor;
        return
        [
            // Orientation cue: translucent quad in the cut plane (display-only).
            new("plane-cue",
                [new GizmoQuad((u + v) * -0.5f, (u - v) * 0.5f, (u + v) * 0.5f, (v - u) * 0.5f, 0.12f)],
                Idle: new GizmoStyle(arrowColor, 0.8f)),
            // Slide the plane along its normal.
            new("offset",
                [new GizmoLine(n * 0.15f, n, 3f), new GizmoArrowHead(n, n)],
                new AxisDrag(n), new GizmoBinding(nameof(Offset)),
                new GizmoStyle(arrowColor, 0.92f),
                new GizmoStyle(arrowColor, 1f, 1.18f),
                new GizmoStyle(arrowColor, 1f, 1.28f)),
            GizmoElements.AxisRing("yaw", 2, GizmoBinding.Degrees(nameof(NormalYaw)), 0.55f),
            // Pitch: rotate the normal about the horizontal axis perpendicular to it.
            new("pitch",
                [new GizmoRing(default, pitchAxis, 0.68f)],
                new AngleDrag(pitchAxis), GizmoBinding.Degrees(nameof(NormalPitch)),
                new GizmoStyle(GizmoElements.YColor, 0.92f),
                new GizmoStyle(GizmoElements.YColor, 1f, 1.18f),
                new GizmoStyle(GizmoElements.YColor, 1f, 1.28f)),
        ];
    }
}

/// <summary>
/// Cuts a triangle mesh with a horizontal plane, keeping geometry at or below the plane.
/// </summary>
[Category(nameof(Categories.Meshes))]
public class MeshHorizontalSlice : IModifier
{
    [Range(0f, 1f)] public float Height { get; set; } = 0.5f;

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var bounds = mesh.DerivedBounds();
        if (Height == 0) return ([], []);
        var z = bounds.Min.Z.Lerp(bounds.Max.Z, Height);
        var plane = new Plane(Vector3.UnitZ, -z);
        return mesh.ClipBelow(plane);
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
            
            if (bounds.Min.Z >= z)
                continue;

            if (bounds.Max.Z <= z)
                mb.AddInstanceAndRemapMesh(inst, model, meshRemap);
            else
            {
                var mesh = model.GetTransformedMesh(inst);
                var cutMesh = mesh.ClipBelow(plane);
                mb.AddInstance(cutMesh, inst.Material);
            }
        }

        return mb.Build();
    }
}
