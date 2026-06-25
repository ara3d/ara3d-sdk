using Ara3D.IO.StepParser;

namespace Ara3D.IfcLoader;

public class IfcEntityResolver
{
    public StepDocument Document { get; }
    public Dictionary<int, IfcEntity> EntityLookup { get; } = new();
    
    public IfcEntityResolver(StepDocument d)
    {
        Document = d;

        // TODO: this should be filtered, so that we don't create IfcEntity for everything. 
        foreach (var def in Document.Definitions)
        {
            //var entityName = def.NameToken.ToString();
            var attributes = def.AttributesToken.AsList(Document);
            var entity = new IfcEntity(def.Id, def, attributes, Document);
            EntityLookup.Add(def.Id, entity);
        }
    }

    public IfcEntity GetEntity(int id)
        => EntityLookup[id];

    public IfcEntity GetEntityOrDefault(int id)
        => EntityLookup.GetValueOrDefault(id);

    public IEnumerable<IfcEntity> GetEntities()
        => EntityLookup.Values;
}