using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.BIM_Tools;

[Category(nameof(Categories.Buildings))]
public class FilterParameters : IModifier
{
    public Action ShowWindow => ShowWindowImpl;

    public void ShowWindowImpl()
    {
        MultiSelectListWindow.Show(
            _parameterNames.OrderBy(x => x),
            onSelectionChanged: UpdateSelection,
            title: "Parameter Names");
    }

    public string GetDescriptorName(DescriptorIndex index)
    {
        if (_data == null) return null;
        if (index < 0) return null;
        return GetDescriptorName(_data.Descriptors[(int)index]);
    }

    public string GetDescriptorName(ParameterDescriptor desc)
    {
        if (_data != null && desc.Name >= 0)
            return _data.Strings[(int)desc.Name];
        return null;
    }

    public void UpdateSelection(IReadOnlyList<string> sel)
    {
        _selectedNames.Clear();
        _selectedDescriptors.Clear();
        _selectedEntities.Clear();

        foreach (var name in sel)
            _selectedNames.Add(name);

        for (var i=0; i < _data.Descriptors.Length; i++)
        {
            var pd = _data.Descriptors[i];
            if (_selectedNames.Contains(GetDescriptorName(pd)))
                _selectedDescriptors.Add(i);
        }

        foreach (var p in _data.Parameters)
            if (_selectedDescriptors.Contains((int)p.Descriptor))
                _selectedEntities.Add(p.Entity);

        // NOTE: very important, only do this at the end!
        _app?.Invalidate(this);
    }

    private readonly HashSet<string> _parameterNames = [];
    private readonly HashSet<string> _selectedNames = new();
    private readonly HashSet<int> _selectedDescriptors = new();
    private readonly HashSet<DescriptorIndex> _descriptors = new();
    private readonly HashSet<EntityIndex> _selectedEntities = new();
    private readonly HashSet<EntityIndex> _geometricEntities = new();

    private BimData? _data;
    private IEvalServices _app;

    public void RecomputeParameterNamesIfNeeded(BimData bimData, EvalContext context)
    {
        _app = context.Services;

        if (bimData == _data)
            return;
        _data = bimData;

        _parameterNames.Clear();
        _descriptors.Clear();
        _geometricEntities.Clear();

        if (_data == null)
            return;

        // Get the list of entities that are actually referenced by instances with geometry 
        var g = bimData.Geometry;
        for (var i = 0; i < g.GetNumInstances(); i++)
        {
            var mi = g.InstanceMeshIndex[i];
            var ei = g.InstanceEntityIndex[i];
            if (mi >= 0 && ei >= 0)_geometricEntities.Add((EntityIndex)ei);
        }

        // Get only the descriptors that are used by actual parameters
        foreach (var p in _data.Parameters)
            if (_geometricEntities.Contains(p.Entity))
                _descriptors.Add(p.Descriptor);

        // Create the list of names, from the descriptors which are linked to geometric elements
        foreach (var pd in _descriptors)
        { 
            var name = GetDescriptorName(pd);
            if (name == null) 
                continue;
            _parameterNames.Add(name);
        }
    }

    public bool IsSelected(InstanceStruct inst)
    {
        if (_selectedNames.Count == 0) return true;
        var ei = (EntityIndex)inst.EntityIndex;
        return _selectedEntities.Contains(ei);
    }

    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var bimData = context.Input.GetAttachment<BimData>();
        if (bimData == null) return model3D.Where((InstanceStruct _) => true);
        RecomputeParameterNamesIfNeeded(bimData, context);
        return model3D.Where(IsSelected);
    }
}