using Ara3D.Collections;
using Ara3D.Models;

namespace Ara3D.BimOpenSchema;

public class BimModel3D 
{
    public BimModel3D(BimObjectModel model)
    {
        ObjectModel = model;
        RenderModelData = new RenderModelData(3);
        RenderModelData.Update(model.Model3D);
    }

    public RenderModelData RenderModelData { get; private set; }
    public BimObjectModel ObjectModel { get; }

    public static BimModel3D Create(BimObjectModel model)
        => new(model);

    public static BimModel3D Create(IBimData data, bool computeParametersAndRelations)
        => new(new BimObjectModel(data, data.Geometry.ToModel3D(), computeParametersAndRelations));
    
    public EntityModel GetEntityModel(InstanceStruct inst)
        => ObjectModel.Entities.ElementAtOrDefault(inst.EntityIndex);
}