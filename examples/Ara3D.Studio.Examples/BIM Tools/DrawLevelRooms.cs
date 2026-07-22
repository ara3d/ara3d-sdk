using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

/// <summary>
/// Shows IFCSPACE room geometry for a selected building level (IFCBUILDINGSTOREY).
/// </summary>
[Category(Cat.ExperimentalBim)]
public class DrawLevelRooms : IModifier
{
    public List<string> LevelNames { get; private set; } = [];

    private int _level;

    [Options(nameof(LevelNames))]
    public int Level
    {
        get => LevelNames.Count == 0 ? 0 : int.Clamp(_level, 0, LevelNames.Count - 1);
        set => _level = value;
    }

    [Range(0f, 1f)] public float RoomAlpha = 0.35f;
    public Color RoomColor = new(0.2f, 0.6f, 1f, 1f);

    private BimData? _data;
    private BimObjectModel? _bom;
    private List<EntityModel> _levelEntities = [];

    const string LevelCategory = "IFCBUILDINGSTOREY";

    void RecomputeLevels(BimData bimData, IModel3D model3D, EvalContext context)
    {
        if (_data == bimData)
            return;

        _data = bimData;
        _bom = new BimObjectModel(bimData, model3D, computeParametersAndRelations: true);

        _levelEntities = _bom.Entities
            .Where(e => e.IsNotTypeOrCategory && e.Category == LevelCategory)
            .OrderBy(LevelSortKey)
            .ThenBy(e => e.Name)
            .ToList();

        LevelNames = _levelEntities.Select(e => e.Name).ToList();
        context.Services.RefreshUI(this);
    }

    static float LevelSortKey(EntityModel level)
        => level.Instances.Count > 0 ? level.Instances[0].Matrix4x4.Translation.Z : float.MaxValue;

    static bool IsLinkedToLevel(EntityModel room, EntityModel level)
    {
        foreach (var r in room.OutgoingRelations)
        {
            if ((r.RelationType == RelationType.ContainedIn || r.RelationType == RelationType.MemberOf)
                && r.Target.Index == level.Index)
                return true;
        }
        return false;
    }

    Material RoomMaterial => RoomColor.WithA(RoomAlpha);

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null)
            return model3D;

        RecomputeLevels(bimData, model3D, context);

        if (_levelEntities.Count == 0)
            return new Model3D(model3D.Meshes, []);

        var selectedLevel = _levelEntities[Level];
        var material = RoomMaterial;

        var roomInstances = _bom!.Entities
            .Where(e => e.IsRoomEntity())
            .Where(e => IsLinkedToLevel(e, selectedLevel))
            .SelectMany(e => e.Instances)
            .Select(i => i.WithVisibility(true).WithMaterial(material))
            .ToList();

        return model3D.WithInstances(roomInstances);
    }
}
