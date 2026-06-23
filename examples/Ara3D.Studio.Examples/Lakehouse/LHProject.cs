using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.Lakehouse;

public class LHProject
{
    public BimData BimData { get; }
    public RenderModelData RenderData { get; }
    public IModel3D Model { get; }
    public List<LHDocument> Documents { get; } = [];
    public MultiDictionary<EntityIndex, Parameter> Parameters = [];
    public Dictionary<DescriptorIndex, ParameterDescriptor> Descriptors = [];
    public Dictionary<EntityIndex, Bounds3D> Bounds { get; } = [];
    public Dictionary<EntityIndex, LHRoom> Rooms { get; } = [];

    public bool IsRoom(InstanceStruct inst)
    {
        var catName = BimData.CategoryName(inst);
        return catName == "IFCSPACE" || catName == "Room";
    }

    public LHProject(BimData bimData, RenderModelData renderData)
    {
        BimData = bimData;
        RenderData = renderData;
        Model = RenderData.ToModel3D();
        Parameters = bimData.Parameters.GroupBy(p => p.Entity).ToMultiDictionary();
        foreach (var d in bimData.DocumentIndices())
        {
            var doc = new LHDocument(this, d);
            Documents.Add(doc);
        }

        // Compute Bounds
        var n = RenderData.InstanceCount;
        for (var i = 0; i < n; i++)
        {
            var inst = RenderData.InstanceData[i];

            var entityIndex = (EntityIndex)inst.EntityIndex;
            if (entityIndex < 0)
                continue;

            var instBounds = RenderData.InstanceBoundsData[i];
            if (!Bounds.TryAdd(entityIndex, instBounds))
                Bounds[entityIndex] = Bounds[entityIndex].Include(instBounds);
        }

        // Compute the models for each document 
        foreach (var group in RenderData.Instances.GroupBy(inst => BimData.DocumentIndex(inst)))
        {
            var docIndex = group.Key;
            var lhDoc = Documents[(int)docIndex];
            lhDoc.Model = Model.WithInstances(group).RemoveUnusedMeshes();
        }

        // Create rooms entities
        for (var i = 0; i < Model.Instances.Count; i++)
        {
            var inst = Model.Instances[i];
            if (inst.MeshIndex < 0 || inst.EntityIndex < 0)
                continue;

            if (!IsRoom(inst)) 
                continue;
            
            var entity = BimData.Entity(inst);
            if (!entity.HasValue)
                continue;
            var entityIndex = (EntityIndex)inst.EntityIndex;

            var docIndex = entity.Value.Document;
            var lhDoc = Documents[(int)docIndex];

            var room = new LHRoom()
            {
                EntityIndex = entityIndex,
                Document = lhDoc,
                Entity = entity.Value,
                Bounds = Bounds.GetValueOrDefault(entityIndex, Bounds3D.Empty),
                Instance = inst,
                Mesh = Model.Meshes[inst.MeshIndex],
                Name = BimData.Name(entity)
            };

            Rooms.Add(room.EntityIndex, room);
            room.Document.Rooms.Add(room);
        }

        // Figure out which entities belong in which rooms
        foreach (var doc in Documents)
        {
            var model = doc.Model;
            for (var i = 0; i < model.Instances.Count; i++)
            {
                var inst = model.Instances[i];
                if (inst.MeshIndex < 0 || inst.EntityIndex < 0)
                    continue;

                // Skip rooms and spaces
                if (IsRoom(inst)) 
                    continue;

                if (!Bounds.TryGetValue((EntityIndex)inst.EntityIndex, out var bounds))
                    continue;

                foreach (var room in doc.Rooms)
                    if (room.Bounds.Intersects(bounds))
                        room.Members.Add(inst);
            }

            // Compute the model for each room. 
            foreach (var room in doc.Rooms)
                room.Model = model.WithInstances(room.Members.ToList()).RemoveUnusedMeshes();
        }
    }

    public IDictionary<string, string> GetParameterData(EntityIndex ei)
    {
        var r = new Dictionary<string, string>();
        var parameters = Parameters.GetValueOrDefault(ei, []);
        foreach (var param in parameters)
        {
            var name = BimData.ParameterName(param);
            var value = BimData.ParameterValue(param);
            r[name] = param.Value;
        }
        return r;
    }
}