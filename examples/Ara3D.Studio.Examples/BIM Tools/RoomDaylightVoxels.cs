using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

/// <summary>
/// Voxelizes all rooms and colors voxels by distance to the nearest glazing
/// (windows / curtain panels). Naive Euclidean proxy for daylight access — not ray-traced.
/// </summary>
[Category(nameof(Categories.Buildings))]
public class RoomDaylightVoxels : IModifier
{
    [Range(0.1f, 2f)] public float VoxelSize = 0.5f;
    [Range(1f, 30f)] public float MaxDistance = 8f;
    [Range(0f, 1f)] public float OriginalTransparency = 0.2f;
    public bool HighlightGlazing = true;
    public bool UseTransparencyHeuristic;
    [Range(0f, 100f)] public float TransparencyThreshold = 50f;

    private BimData? _data;
    private BimObjectModel? _bom;

    const int MaxVoxelsPerRoom = 8000;

    static readonly Color NearDaylight = new(1f, 0.95f, 0.7f, 1f);
    static readonly Color MidDaylight = new(1f, 0.6f, 0.2f, 1f);
    static readonly Color FarDaylight = new(0.15f, 0.2f, 0.45f, 1f);
    static readonly Color GlazingHighlight = new(0.2f, 0.85f, 1f, 1f);

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null)
            return model3D;

        if (bimData != _data)
        {
            _data = bimData;
            _bom = new BimObjectModel(bimData, model3D, computeParametersAndRelations: true);
        }

        var bom = _bom!;
        var meshBounds = model3D.GetMeshBounds();

        var rooms = bom.Entities
            .Where(BimEntityHelpers.IsRoom)
            .Where(e => e.HasGeometry)
            .ToList();

        var glazingBounds = bom.Entities
            .Where(e => e.HasGeometry && BimEntityHelpers.IsLightSource(e, UseTransparencyHeuristic, TransparencyThreshold))
            .Select(e => BimEntityHelpers.GetEntityWorldBounds(e, meshBounds))
            .Where(BimEntityHelpers.HasValidBounds)
            .ToList();

        var mb = new Model3DBuilder();
        mb.AddModel(model3D.MapInstances(i => i.WithAlpha(OriginalTransparency)));

        if (HighlightGlazing)
        {
            var glazingIndices = bom.Entities
                .Where(e => e.HasGeometry && BimEntityHelpers.IsLightSource(e, UseTransparencyHeuristic, TransparencyThreshold))
                .Select(e => (int)e.Index)
                .ToHashSet();

            var glazedInstances = model3D.Instances
                .Where(i => i.EntityIndex >= 0 && glazingIndices.Contains(i.EntityIndex))
                .Select(i => i.WithMaterial(GlazingHighlight).WithAlpha(1f))
                .ToList();

            if (glazedInstances.Count > 0)
                mb.AddModel(model3D.WithInstances(glazedInstances));
        }

        var voxelMesh = PlatonicSolids.TriangulatedCube.Scale(VoxelSize * 0.9f);
        var voxelInstances = new List<(Matrix4x4, Material)>();

        foreach (var room in rooms)
        {
            var bounds = BimEntityHelpers.GetEntityWorldBounds(room, meshBounds);
            if (!BimEntityHelpers.HasValidBounds(bounds))
                continue;

            foreach (var center in VoxelCentersInBounds(bounds, VoxelSize, MaxVoxelsPerRoom))
            {
                if (!bounds.Contains(center))
                    continue;

                var dist = MinGlazingDistance(center, glazingBounds);
                var color = ColorFromDaylightDistance(dist, MaxDistance);
                voxelInstances.Add((Matrix4x4.CreateTranslation(center.Vector3), Material.Default.WithColor(color)));
            }
        }

        if (voxelInstances.Count > 0)
            mb.AddInstances(voxelMesh, voxelInstances);

        return mb.Build();
    }

    public static float DistancePointToBox(Point3D p, Bounds3D box)
    {
        if (!BimEntityHelpers.HasValidBounds(box))
            return float.MaxValue;

        var v = p.Vector3;
        var min = box.Min.Vector3;
        var max = box.Max.Vector3;

        var cx = Math.Clamp(v.X, min.X, max.X);
        var cy = Math.Clamp(v.Y, min.Y, max.Y);
        var cz = Math.Clamp(v.Z, min.Z, max.Z);

        var dx = v.X - cx;
        var dy = v.Y - cy;
        var dz = v.Z - cz;
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static IEnumerable<Point3D> VoxelCentersInBounds(Bounds3D bounds, float voxelSize, int maxVoxels = MaxVoxelsPerRoom)
    {
        if (!BimEntityHelpers.HasValidBounds(bounds) || voxelSize <= 0f)
            yield break;

        var size = bounds.Size;
        var nx = Math.Max(1, (int)Math.Ceiling(size.X / voxelSize));
        var ny = Math.Max(1, (int)Math.Ceiling(size.Y / voxelSize));
        var nz = Math.Max(1, (int)Math.Ceiling(size.Z / voxelSize));

        while (nx * ny * nz > maxVoxels)
        {
            voxelSize *= 1.25f;
            nx = Math.Max(1, (int)Math.Ceiling(size.X / voxelSize));
            ny = Math.Max(1, (int)Math.Ceiling(size.Y / voxelSize));
            nz = Math.Max(1, (int)Math.Ceiling(size.Z / voxelSize));
        }

        var half = voxelSize * 0.5f;
        for (var iz = 0; iz < nz; iz++)
        for (var iy = 0; iy < ny; iy++)
        for (var ix = 0; ix < nx; ix++)
        {
            var center = new Point3D(
                bounds.Min.X + ix * voxelSize + half,
                bounds.Min.Y + iy * voxelSize + half,
                bounds.Min.Z + iz * voxelSize + half);
            yield return center;
        }
    }

    public static float MinGlazingDistance(Point3D p, IReadOnlyList<Bounds3D> glazingBounds)
    {
        if (glazingBounds.Count == 0)
            return float.MaxValue;

        var min = float.MaxValue;
        foreach (var g in glazingBounds)
            min = Math.Min(min, DistancePointToBox(p, g));

        return min;
    }

    public static Color ColorFromDaylightDistance(float distance, float maxDistance)
    {
        var t = ColorFromDistance.RelativeDistance(distance, maxDistance);
        return ColorFromDistance.ThreePointGradient(NearDaylight, MidDaylight, FarDaylight, t);
    }
}
