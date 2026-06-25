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

    public IDataTable GetRoomProperties()
    {
        var rows = new List<IDictionary<string, string>>();

        foreach (var room in Rooms)
        {
            var d = Project.GetParameterData(room.EntityIndex);
            d.Add("Entity Name", room.Name);
            d.Add("Entity Index", room.EntityIndex.ToString());
            d.Add("Entity", room.Entity.ToString());
            d.Add("Local ID", room.Entity.LocalId.ToString());
            d.Add("Global ID", BimData.Get(room.Entity.GlobalId));
            rows.Add(d);
        }

        return rows.ToDataTable();
    }
}