using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(Cat.ExperimentalBim)]
[Description("Shows only the BIM elements matching a selected category and, optionally, a selected source document.")]
public class FilterCategoryAndDocuments : IModifier
{
    public bool FilterDocument { get; set; } = true;
    [Range(0, 250)] public int DocumentIndex { get; set; }

    [Options(nameof(CategoryNames))] public int Category;
    public List<string> CategoryNames { get; private set; } = [];

    private BimData _data;
    private BimObjectModel _model;

    public MultiDictionary<string, InstanceStruct> Groups = [];

    public string GetCategory(InstanceStruct inst)
        => _model.Entities.ElementAtOrDefault(inst.EntityIndex)?.Category ?? "";

    public void RecomputeCategoryNames(BimData bimData, RenderModelData renderData, EvalContext context)
    {
        if (_data != null)
            return;

        _data = bimData;
        CategoryNames = [];
        if (_data == null)
            return;

        _model = new BimObjectModel(_data, renderData, false);

        foreach (var i in renderData.InstanceData)
            Groups.Add(GetCategory(i), i);
        CategoryNames = Groups.Keys.OrderBy(x => x).ToList();

        context.Services.RefreshUI(this);
    }

    public static int GetDocument(BimData bim, InstanceStruct inst)
        => (int)(bim.Entities.ElementAtOrDefault(inst.EntityIndex).Document);

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
        var instances = Groups.GetValueOrDefault(catName, []);

        if (FilterDocument)
        {
            instances = instances.Where(i => GetDocument(bimData, i) == DocumentIndex).ToList();
        }

        return model3D.WithInstances(instances);
    }
}