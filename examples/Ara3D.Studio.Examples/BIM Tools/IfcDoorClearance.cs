using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
[Description("Visualizes swing/clearance zones in front of IFC doors, with adjustable wall transparency.")]
public class IfcDoorClearance : IModifier
{
    public List<string> CategoryNames { get; set; } = [];
    [Range(0, 1)] public float WallTransparency = 0.2f;
    [Range(0.001f, 0.2f)] public float FrameSize = 0.01f;
    public bool DisplayClearances = true;
    public bool IncludeOriginal = false;
    [Range(0, 1)] public float ZoneTransparency = 0.2f;
    public Color ZoneColor = new(0, 0.3f, 1, 1);

    public Material GetZoneMaterial()
        => ZoneColor.WithA(ZoneTransparency);

    private List<StringIndex> _categoryIndices;
    private BimData _data;
    private BimObjectModel _model;
    private IEvalServices _app;

    public void RecomputeCategoryNames(BimData bimData, EvalContext context)
    {
        if (bimData == _data)
            return;

        _data = bimData;

        if (_data == null)
            return;
       
        _model = new BimObjectModel(_data, true);
        
        CategoryNames = _model
            .Entities
            .Where(e => e.HasGeometry)
            .Select(e => e.Category)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        context.Services.RefreshUI(this);
    }

    public static HashSet<string> Transparent = ["IFCFLOOR", "IFCSLAB", "IFCROOF", "IFCWALL"];

    public Action ForceRecompute => RecomputeImpl;

    public void RecomputeImpl()
    {
        _data = null;
        _app?.Invalidate(this);
    }

    public string GetCategory(InstanceStruct inst)
        => _model.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public bool IsTransparent(InstanceStruct inst)
        => Transparent.Contains(GetCategory(inst));

    public bool IsDoor(InstanceStruct inst)
        => GetCategory(inst) == "IFCDOOR";

    public (InstanceStruct?, Bounds3D) GetBiggestBounds(IEnumerable<InstanceStruct> instances, IReadOnlyList<Bounds3D> meshBounds)
    {
        var bounds = Bounds3D.Empty;
        InstanceStruct? biggestInst = null;
        foreach (var inst in instances)
        {
            if (inst.MeshIndex < 0)
                continue;
            var localBounds = meshBounds[inst.MeshIndex];
            if (localBounds.Volume() > bounds.Volume())
            {
                bounds = localBounds;
                biggestInst = inst;
            }
        }

        return (biggestInst, bounds);
    }

    public void AddClearances(Model3DBuilder builder, IModel3D model)
    {
        var meshBounds = model.GetMeshBounds();
        var frameMesh = new BoxFrameMeshBuilder(FrameSize).Mesh.Triangulate();
        var zoneMesh = PlatonicSolids.TriangulatedCube;
        var doorInstances = model.Instances.Where(IsDoor);
        var doorGroups = doorInstances.GroupBy(inst => inst.EntityIndex);
        var zones = new List<Bounds3D>();
        foreach (var group in doorGroups)
        {
            // We want to get the largest door mesh untransformed (so that X / Y have predictable meaning)
            // The challenge is that door parts might have different transforms.
            // We are assuming no difference in scaling between the different parts 
            var (biggestInst, bounds) = GetBiggestBounds(group, meshBounds);
            if (biggestInst == null)
                continue;
            var width = bounds.Size.X;
            bounds = bounds.Expand((0, width, 0));
            var localFrameMesh = frameMesh.FitToBounds(bounds);
            var localZoneMesh = zoneMesh.FitToBounds(bounds);
            var matrix = biggestInst?.Matrix4x4 ?? Matrix4x4.Identity;
            builder.AddInstance(localFrameMesh, matrix);
            builder.AddInstance(localZoneMesh, matrix, GetZoneMaterial());
        }

        builder.AddBoxes(zones, GetZoneMaterial());
    }

    public IModel3D Eval(IModel3D model, EvalContext context)
    {
        _app = context.Services;
        var bimData = context.Input.GetAttachment<BimData>();
        RecomputeCategoryNames(bimData, context);
        var mb = new Model3DBuilder();
        var transparentInstances = model.Instances.Where(IsTransparent).Select(inst => inst.WithAlpha(WallTransparency));
        var transparentModel = model.WithInstances(transparentInstances);
        mb.AddModel(transparentModel);
        if (DisplayClearances)
            AddClearances(mb, model);
        if (IncludeOriginal)
            mb.AddModel(model);
        return mb.Build();
    }
}