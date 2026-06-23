using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class FilterCategoryWithUI : IModifier
{
    public Action ShowWindow => ShowWindowImpl;

    public void ShowWindowImpl()
    {
        MultiSelectListWindow.Show(
            CategoryNames,
            onSelectionChanged: UpdateSelection,
            title: "Categories");
    }

    public void UpdateSelection(IReadOnlyList<string> sel)
    {
        _app?.Invalidate(this);
        _selectedNames.Clear();
        foreach (var name in sel)
            _selectedNames.Add(name);
    }

    public List<string> CategoryNames { get; private set; } = [];
    private readonly HashSet<string> _selectedNames = new();
    private BimData? _data;
    private BimObjectModel? _model;
    private IHostApplication _app;

    public void RecomputeCategoryNames(BimData bimData, EvalContext context)
    {
        _app = context.Application;

        if (bimData == _data)
            return;

        _data = bimData;

        context.Application.Invalidate(this);

        if (_data == null)
        {
            CategoryNames = [];
            return;
        }

        _model = new BimObjectModel(_data, true);

        CategoryNames = _model
            .Entities
            .Where(e => e.HasGeometry)
            .Select(e => e.Category)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }
    
    public bool IsSelected(InstanceStruct inst)
    {
        if (_selectedNames.Count == 0) return true;
        var ei = inst.EntityIndex;
        if (ei < 0) return false;
        var cat = _model.Entities[ei].Category;
        return _selectedNames.Contains(cat);
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null) return model3D;
        RecomputeCategoryNames(bimData, context);
        return model3D.Where(IsSelected);
    }
}