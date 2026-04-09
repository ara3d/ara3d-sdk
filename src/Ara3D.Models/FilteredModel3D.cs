namespace Ara3D.Models;

public class FilteredModel3D 
{
    public IModel3D Model { get; }
    public Func<InstanceStruct, bool> Func { get; }

    public FilteredModel3D(IModel3D model, Func<InstanceStruct, bool> func = null)
        => (Model, Func) = (model, func ?? (_ => true));
}