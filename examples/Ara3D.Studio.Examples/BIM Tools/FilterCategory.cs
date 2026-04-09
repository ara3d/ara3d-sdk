using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class FilterCategory : IModifier
{
    //[Options(nameof(CategoryNames))] 
    [Range(0, 80)] public int Category;
    public string CategoryName => CategoryNames?.ElementAtOrDefault(Category, "");
    public List<string> CategoryNames { get; private set; } = [];
    
    private List<StringIndex> _categoryIndices;

    private BimData _data;
    private BimObjectModel _model;

    public void RecomputeCategoryNames(BimData bimData, EvalContext context)
    {
        if (bimData == _data)
            return;
        
        _data = bimData;
        
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

    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public FilteredModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        RecomputeCategoryNames(bimData, context);

        if (Category < 0 || Category >= CategoryNames.Count)
            return model3D.Where((InstanceStruct inst) => true);

        var entities = _model.Entities;
        return model3D.Where(inst => entities[inst.EntityIndex].Category == CategoryName);
    }
}