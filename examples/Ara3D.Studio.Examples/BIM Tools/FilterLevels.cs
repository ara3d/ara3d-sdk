using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;


[Category(Cat.ExperimentalBim)]
public class FilterLevels : IModifier
{
    //[Options(nameof(LevelNames))] 
    //public int Level;
    
    public List<string> LevelNames { get; private set; }

    private int _level; 

    [ComputedRange(nameof(_numLevels))]
    public int Level
    {
        get => int.Clamp(_level, 0, LevelNames?.Count ?? 0);
        set => _level = value;
    }

    private int _numLevels => LevelNames.Count;
    private List<(string Name, float Elevation)> _levelData;

    private BimData? _data;
    private BimObjectModel _bom; 
    private IEvalServices _app;

    public void RecomputeLevels(BimData data, EvalContext context)
    {
        if (_data == data) return;
        _data = data;
        _bom = new BimObjectModel(data, true);
        _app = context.Services;
        _levelData = _bom.GetDistinctLevels().ToList();
        LevelNames = _levelData.Select(x => $"{x.Name} {x.Elevation:F2}").ToList();
        _app.RefreshUI(this);
    }

    public string CurLevelName => _levelData == null ? "" : _levelData[Level].Name;
    public float CurLevelElevation => _levelData == null ? 0f : _levelData[Level].Elevation;

    public bool FilterLevel(InstanceStruct inst)
    {
        var em = _bom.Entities.ElementAtOrDefault(inst.EntityIndex);
        if (em == null) return false;
        return em.LevelName == CurLevelName && (Math.Abs(em.Elevation - CurLevelElevation) < 0.0001);
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null) return model3D;
        RecomputeLevels(bimData, context);
        return model3D.Where(FilterLevel);
    }
}