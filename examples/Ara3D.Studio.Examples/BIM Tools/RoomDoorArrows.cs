using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

/// <summary>
/// Draws 3D arrows from each room center to its associated doors.
/// IFC: doors are not contained in IfcSpace — use spatial overlap (and BoundedBy when exported).
/// </summary>
[Category(Cat.ExperimentalBim)]
public class RoomDoorArrows : IModifier
{
    [Range(0f, 1f)] public float OriginalTransparency = 0.25f;
    [Range(0.01f, 1f)] public float ArrowScale = 0.15f;
    [Range(0.1f, 3f)] public float ProximityTolerance = 1f;
    [Range(0.05f, 1f)] public float LengthFraction = 0.35f;
    [Range(0f, 10f)] public float MaxArrowLength = 3f;
    public Color ArrowColor = new(1f, 0.5f, 0.1f, 1f);

    private BimData? _data;
    private BimObjectModel? _bom;

    Material ArrowMaterial => ArrowColor.WithA(1f);

    public IModel3D Eval(IModel3D model, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null)
            return model;

        if (bimData != _data)
        {
            _data = bimData;
            _bom = new BimObjectModel(bimData, model, computeParametersAndRelations: true);
        }

        var bom = _bom!;
        var meshBounds = model.GetMeshBounds();
        var rooms = bom.Entities.Where(BimEntityHelpers.IsRoom).ToList();
        var doors = bom.Entities.Where(BimEntityHelpers.IsDoor).Where(e => e.HasGeometry).ToList();
        var doorSet = doors.Select(d => (int)d.Index).ToHashSet();

        var roomCenters = new Dictionary<int, Point3D>();
        var roomBounds = new Dictionary<int, Bounds3D>();
        foreach (var room in rooms)
        {
            var bounds = BimEntityHelpers.GetEntityWorldBounds(room, meshBounds);
            if (!BimEntityHelpers.HasValidBounds(bounds))
                continue;
            var roomIndex = (int)room.Index;
            roomBounds[roomIndex] = bounds;
            roomCenters[roomIndex] = bounds.Center;
        }

        var doorBounds = doors.ToDictionary(
            d => (int)d.Index,
            d => BimEntityHelpers.GetEntityWorldBounds(d, meshBounds));

        var links = BuildRoomDoorLinks(
            rooms, doors, doorSet, roomBounds, doorBounds, ProximityTolerance);

        var arrowMesh = Primitives.DefaultUpArrow().Triangulate().Scale((ArrowScale, ArrowScale, 1f));
        var arrowMaterial = ArrowMaterial;

        var mb = new Model3DBuilder();
        mb.AddModel(model.MapInstances(i => i.WithAlpha(OriginalTransparency)));

        var arrowMeshIndex = mb.AddMeshWithoutInstance(arrowMesh);
        foreach (var (roomIndex, doorIndices) in links)
        {
            if (!roomCenters.TryGetValue(roomIndex, out var roomCenter))
                continue;

            foreach (var doorIndex in doorIndices)
            {
                if (!doorBounds.TryGetValue(doorIndex, out var doorBox))
                    continue;

                var doorCenter = doorBox.Center;
                var line = ShortenedArrowLine(roomCenter, doorCenter, LengthFraction, MaxArrowLength);
                if (line.Length < 1e-4f)
                    continue;

                mb.AddInstance(arrowMeshIndex, line.AlignZAxisTransform(), arrowMaterial);
            }
        }

        return mb.Build();
    }

    static Dictionary<int, List<int>> BuildRoomDoorLinks(
        List<EntityModel> rooms,
        List<EntityModel> doors,
        HashSet<int> doorSet,
        Dictionary<int, Bounds3D> roomBounds,
        Dictionary<int, Bounds3D> doorBounds,
        float proximityTolerance)
    {
        var links = new Dictionary<int, List<int>>();

        void AddLink(int roomIndex, int doorIndex)
        {
            if (!doorSet.Contains(doorIndex) || !roomBounds.ContainsKey(roomIndex))
                return;
            if (!links.TryGetValue(roomIndex, out var list))
            {
                list = [];
                links[roomIndex] = list;
            }
            if (!list.Contains(doorIndex))
                list.Add(doorIndex);
        }

        // Revit exports
        foreach (var door in doors)
        {
            var doorIndex = (int)door.Index;
            var toRoom = door.GetParameterAsEntity(CommonRevitParameters.FIToRoom);
            var fromRoom = door.GetParameterAsEntity(CommonRevitParameters.FIFromRoom);
            if (toRoom != null)
                AddLink((int)toRoom.Index, doorIndex);
            if (fromRoom != null)
                AddLink((int)fromRoom.Index, doorIndex);
        }

        // IFC IfcRelSpaceBoundary (when/if exported as BoundedBy)
        foreach (var room in rooms)
        {
            var roomIndex = (int)room.Index;
            foreach (var rel in room.OutgoingRelations)
            {
                if (rel.RelationType is not (RelationType.BoundedBy or RelationType.TraverseTo))
                    continue;
                var targetIndex = (int)rel.Target.Index;
                if (doorSet.Contains(targetIndex))
                    AddLink(roomIndex, targetIndex);
            }

            foreach (var rel in room.IncomingRelations)
            {
                if (rel.RelationType is not (RelationType.BoundedBy or RelationType.TraverseTo))
                    continue;
                var sourceIndex = (int)rel.Target.Index;
                if (doorSet.Contains(sourceIndex))
                    AddLink(roomIndex, sourceIndex);
            }
        }

        // IFC fallback: doors live in walls on space boundaries, not inside space volumes
        foreach (var room in rooms)
        {
            var roomIndex = (int)room.Index;
            if (!roomBounds.TryGetValue(roomIndex, out var bounds))
                continue;

            foreach (var door in doors)
            {
                var doorIndex = (int)door.Index;
                if (links.TryGetValue(roomIndex, out var existing) && existing.Contains(doorIndex))
                    continue;

                if (!doorBounds.TryGetValue(doorIndex, out var doorBox))
                    continue;

                if (BimEntityHelpers.SpatiallyAssociates(bounds, doorBox, proximityTolerance))
                    AddLink(roomIndex, doorIndex);
            }
        }

        return links;
    }

    /// <summary>Arrow from room center toward door, scaled by fraction and optionally capped in meters.</summary>
    static Line3D ShortenedArrowLine(Point3D from, Point3D to, float fraction, float maxLength)
    {
        var fullLine = new Line3D(from, to);
        var fullLen = (float)fullLine.Length;
        if (fullLen < 1e-4f)
            return fullLine;

        var t = Math.Clamp(fraction, 0.05f, 1f);
        var len = fullLen * t;
        if (maxLength > 0)
            len = Math.Min(len, maxLength);

        var end = from.Lerp(to, len / fullLen);
        return new Line3D(from, end);
    }
}
