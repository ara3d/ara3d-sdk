using Ara3D.BimOpenSchema;using BimPoint = Ara3D.BimOpenSchema.Point;

namespace Ara3D.Studio.Samples.BIM_Tools;

/// <summary>
/// Derives a 2D wall plan as LineMesh3D segments on the XY plane at each wall's bottom Z, with gaps at doors.
/// Includes basic walls, curtain walls, curtain panels, and mullions.
/// </summary>[Category(Cat.ExperimentalBim)]
public class WallPlanDiagram : IModifier
{
    [Range(0.01f, 0.5f)] public float MinWallLength = 0.1f;
    [Range(0.1f, 2f)] public float ProximityTolerance = 0.5f;
    [Range(0f, 0.2f)] public float GapMargin = 0.02f;

    public LineMesh3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null)
            return LineMesh3D.Default;

        var bom = new BimObjectModel(bimData, model3D, computeParametersAndRelations: true);
        return Build(model3D, bom, MinWallLength, ProximityTolerance, GapMargin);
    }

    public static LineMesh3D Build(
        IModel3D model,
        BimObjectModel bom,
        float minWallLength = 0.1f,
        float proximityTolerance = 0.5f,
        float gapMargin = 0.02f)
    {
        var meshBounds = model.GetMeshBounds();
        var walls = bom.Entities.Where(e => e.HasGeometry && BimEntityHelpers.IsWall(e)).ToList();
        var doors = bom.Entities.Where(e => e.HasGeometry && BimEntityHelpers.IsDoor(e)).ToList();

        var wallCenterlines = walls.ToDictionary(
            w => w.Index,
            w => (GetWallCenterline(w, meshBounds), (float)BimEntityHelpers.GetEntityWorldBounds(w, meshBounds).Min.Z));
        var doorsByWall = MapDoorsToWalls(doors, walls, wallCenterlines, meshBounds, proximityTolerance);

        var points = new List<Point3D>();
        var indices = new List<Integer2>();

        foreach (var wall in walls)
        {
            if (!wallCenterlines.TryGetValue(wall.Index, out var wallLine))
                continue;

            var (centerline, bottomZ) = wallLine;

            var wallLen = (float)centerline.Length;
            if (wallLen < minWallLength)
                continue;

            var gaps = new List<(float Start, float End)>();
            if (doorsByWall.TryGetValue(wall.Index, out var wallDoors))
            {
                foreach (var door in wallDoors)
                {
                    var doorBounds = BimEntityHelpers.GetEntityWorldBounds(door, meshBounds);
                    if (doorBounds.Min.X > doorBounds.Max.X)
                        continue;
                    gaps.Add(GetDoorGapOnWall(centerline, doorBounds, gapMargin));
                }
            }

            foreach (var segment in SplitAtGaps(centerline, gaps, minWallLength, bottomZ))
                AddSegment(points, indices, segment);
        }

        return points.Count == 0 ? LineMesh3D.Default : new LineMesh3D(points, indices);
    }

    public static bool IsWallEntity(EntityModel entity) => BimEntityHelpers.IsWall(entity);

    public static bool IsDoorEntity(EntityModel entity) => BimEntityHelpers.IsDoor(entity);

    static Line3D GetWallCenterline(EntityModel wall, IReadOnlyList<Bounds3D> meshBounds)
    {
        var bounds = BimEntityHelpers.GetEntityWorldBounds(wall, meshBounds);        var bottomZ = (float)bounds.Min.Z;

        var start = GetParameterAsPoint(wall, CommonRevitParameters.ElementLocationStartPoint);
        var end = GetParameterAsPoint(wall, CommonRevitParameters.ElementLocationEndPoint);
        if (start != null && end != null)
        {
            var a = ToPoint3D(start.Value);
            var b = ToPoint3D(end.Value);
            if (a.Subtract(b).Length > 1e-6f)
            {
                return new Line3D(
                    new Point3D(a.X, a.Y, bottomZ),
                    new Point3D(b.X, b.Y, bottomZ));
            }
        }

        return CenterlineFromBounds(bounds);
    }

    static BimPoint? GetParameterAsPoint(EntityModel entity, string paramName)
    {
        foreach (var p in entity.Data.Parameters)
        {
            if (p.Entity != entity.Index)
                continue;
            var desc = entity.Model.Get(p.Descriptor);
            if (desc == null || desc.Name != paramName || desc.ParameterType != ParameterType.Point)
                continue;
            return entity.Data.Get((PointIndex)p.Value);
        }

        return null;
    }

    static Point3D ToPoint3D(BimPoint p) => new(p.X, p.Y, p.Z);

    static Line3D CenterlineFromBounds(Bounds3D bounds)    {
        var min = bounds.Min;
        var max = bounds.Max;
        var size = bounds.Size;
        var z = min.Z;

        if (size.X >= size.Y)
        {
            var y = (min.Y + max.Y) * 0.5f;
            return new Line3D(new Point3D(min.X, y, z), new Point3D(max.X, y, z));
        }

        var x = (min.X + max.X) * 0.5f;
        return new Line3D(new Point3D(x, min.Y, z), new Point3D(x, max.Y, z));
    }

    static Dictionary<EntityIndex, List<EntityModel>> MapDoorsToWalls(
        List<EntityModel> doors,
        List<EntityModel> walls,
        Dictionary<EntityIndex, (Line3D Line, float BottomZ)> wallCenterlines,
        IReadOnlyList<Bounds3D> meshBounds,
        float proximityTolerance)
    {
        var result = new Dictionary<EntityIndex, List<EntityModel>>();

        foreach (var door in doors)
        {
            var host = GetHostWall(door);
            if (host == null || !wallCenterlines.ContainsKey(host.Index))
                host = FindHostWallByProximity(door, walls, wallCenterlines, meshBounds, proximityTolerance);

            if (host == null)
                continue;

            if (!result.TryGetValue(host.Index, out var list))
            {
                list = [];
                result[host.Index] = list;
            }

            list.Add(door);
        }

        return result;
    }

    static EntityModel? GetHostWall(EntityModel door)
    {
        foreach (var rel in door.OutgoingRelations)
        {
            if (rel.RelationType == RelationType.HostedBy)
                return rel.Target;

            if (rel.RelationType != RelationType.Fills)
                continue;

            foreach (var rel2 in rel.Target.OutgoingRelations)
            {
                if (rel2.RelationType == RelationType.Voids)
                    return rel2.Target;
            }
        }

        return null;
    }

    static EntityModel? FindHostWallByProximity(
        EntityModel door,
        IReadOnlyList<EntityModel> walls,
        Dictionary<EntityIndex, (Line3D Line, float BottomZ)> wallCenterlines,
        IReadOnlyList<Bounds3D> meshBounds,
        float tolerance)
    {
        var doorCenter = BimEntityHelpers.GetEntityWorldBounds(door, meshBounds).Center.Vector3;
        EntityModel? bestWall = null;
        var bestDist = tolerance;

        foreach (var wall in walls)
        {
            if (!wallCenterlines.TryGetValue(wall.Index, out var wallLine))
                continue;

            var line = wallLine.Line;

            var dir = line.Direction;
            var len = (float)line.Length;
            if (len < 1e-6f)
                continue;

            var unitDir = dir / len;
            var dist = (float)doorCenter.DistanceToLine(line.A.Vector3, unitDir);
            if (dist >= bestDist)
                continue;

            var t = (float)GeometryUtil.SignedDistanceAlongLine(doorCenter, line.A.Vector3, unitDir);
            if (t < -tolerance || t > len + tolerance)
                continue;

            bestDist = dist;
            bestWall = wall;
        }

        return bestWall;
    }

    static (float Start, float End) GetDoorGapOnWall(Line3D wallLine, Bounds3D doorBounds, float margin)
    {
        var dir = wallLine.Direction;
        var len = (float)wallLine.Length;
        if (len < 1e-6f)
            return (0, 0);

        var unitDir = dir / len;
        var origin = wallLine.A.Vector3;

        var minT = float.MaxValue;
        var maxT = float.MinValue;
        foreach (var corner in doorBounds.Corners)
        {
            var t = (float)GeometryUtil.SignedDistanceAlongLine(corner.Vector3, origin, unitDir);
            minT = Math.Min(minT, t);
            maxT = Math.Max(maxT, t);
        }

        return (Math.Max(0, minT - margin), Math.Min(len, maxT + margin));
    }

    static IEnumerable<Line3D> SplitAtGaps(Line3D line, List<(float Start, float End)> gaps, float minLength, float bottomZ)
    {
        if (gaps.Count == 0)
        {
            if ((float)line.Length >= minLength)
                yield return FlattenToBottomZ(line, bottomZ);
            yield break;
        }

        gaps.Sort((a, b) => a.Start.CompareTo(b.Start));
        var merged = new List<(float Start, float End)>();
        foreach (var gap in gaps)
        {
            if (gap.End <= gap.Start)
                continue;
            if (merged.Count == 0 || gap.Start > merged[^1].End)
                merged.Add(gap);
            else
                merged[^1] = (merged[^1].Start, Math.Max(merged[^1].End, gap.End));
        }

        if (merged.Count == 0)
        {
            if ((float)line.Length >= minLength)
                yield return FlattenToBottomZ(line, bottomZ);
            yield break;
        }

        var len = (float)line.Length;
        var unitDir = line.Direction / len;
        var origin = line.A.Vector3;

        var pos = 0f;
        foreach (var gap in merged)
        {
            if (gap.Start - pos >= minLength)
            {
                yield return MakeFlatSegment(
                    origin + unitDir * pos,
                    origin + unitDir * gap.Start,
                    bottomZ);
            }

            pos = Math.Max(pos, gap.End);
        }

        if (len - pos >= minLength)
        {
            yield return MakeFlatSegment(
                origin + unitDir * pos,
                origin + unitDir * len,
                bottomZ);
        }
    }

    static Line3D FlattenToBottomZ(Line3D line, float bottomZ)
        => new(
            new Point3D(line.A.X, line.A.Y, bottomZ),
            new Point3D(line.B.X, line.B.Y, bottomZ));

    static Line3D MakeFlatSegment(Vector3 a, Vector3 b, float z)
        => new(new Point3D(a.X, a.Y, z), new Point3D(b.X, b.Y, z));

    static void AddSegment(List<Point3D> points, List<Integer2> indices, Line3D segment)
    {
        var i0 = points.Count;
        points.Add(segment.A);
        points.Add(segment.B);
        indices.Add((i0, i0 + 1));
    }
}
