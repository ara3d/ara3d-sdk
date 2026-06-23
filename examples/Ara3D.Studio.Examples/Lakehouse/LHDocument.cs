using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.Lakehouse;

public class LHDocument
{
    public BimData BimData => Project.BimData;
    public LHProject Project { get; set; }
    public List<LHRoom> Rooms { get; set; } = [];
    public IModel3D Model { get; set; }

    public DocumentIndex DocumentIndex { get; set; }
    public Document Document { get; set; }
    public string Path { get; set; }
    public string Title { get; set; }

    public LHDocument(LHProject project, DocumentIndex docIndex)
    {
        Project = project;
        DocumentIndex = docIndex;
        Document = BimData.Get(DocumentIndex) ?? default;
        Path = BimData.Get(Document.Path);
        Title = BimData.Get(Document.Title);
    }

    public IDataTable ToDataTable()
    {
        var builder = new DataTableBuilder("Rooms");

        var vals = new List<IDictionary<string, string>>();
        
        foreach (var row in BimData.

        builder.AddColumn("Name", typeof(string));
        builder.AddColumn("EntityIndex", typeof(EntityIndex));
        builder.AddColumn("Entity", typeof(Entity));
        builder.AddColumn("Mesh", typeof(TriangleMesh3D));
        builder.AddColumn("Instance", typeof(InstanceStruct));
        builder.AddColumn("Bounds", typeof(Bounds3D));
        builder.AddColumn("Model", typeof(IModel3D));
        builder.AddColumn("Parameters", typeof(List<Parameter>));
        foreach (var room in Rooms)
        {
            builder.AddRow(room.Name, room.EntityIndex, room.Entity, room.Mesh, room.Instance, room.Bounds, room.Model, room.Parameters);
        }
        return builder.Build();
    }
}