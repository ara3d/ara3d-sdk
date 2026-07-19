using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class FilterCategory : IModifier
{
    [Options(nameof(CategoryNames))] public int Category;

    public List<string> CategoryNames { get; private set; } = [];

    private string _prevCatName;
    private int _prevCatIndex;

    private BimData _data;
    private BimObjectModel _model;

    public MultiDictionary<string, InstanceStruct> Groups = [];

    public string GetCategory(InstanceStruct inst)
        => _model.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public void RecomputeCategoryNames(BimData bimData, RenderModelData renderData, EvalContext context)
    {   
        if (_data == bimData)
            return; 

        _data = bimData;
        _model = null;
        CategoryNames = [];
        Groups = [];

        if (_data == null)
            return;

        _model = new BimObjectModel(_data, renderData, false);
        foreach (var i in renderData.InstanceData)
            Groups.Add(GetCategory(i), i.WithVisibility(true));
        CategoryNames = Groups.Keys.OrderBy(x => x).ToList();

        context.Services.RefreshUI(this);
    }

    public static string GetCategory(BimObjectModel bim, InstanceStruct inst)
        => bim.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        var renderData = context.Input.GetAttachment<RenderModelData>();
    
        RecomputeCategoryNames(bimData, renderData, context);

        if (Category < 0 || Category >= CategoryNames.Count)
            return model3D.Where((InstanceStruct _) => true);

        var catName = CategoryNames.ElementAtOrDefault(Category);

        if (Category == _prevCatIndex)
        {
            if (catName != _prevCatName)
            {
                var tmp = CategoryNames.IndexOf(_prevCatName);
                if (tmp != -1)
                {
                    Category = tmp;
                    catName = _prevCatName;
                }
            }
        }

        _prevCatIndex = Category;
        _prevCatName = catName;

        var instances = Groups.GetValueOrDefault(catName, []);

        return model3D.WithInstances(instances);
    }
}