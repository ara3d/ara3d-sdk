using Ara3D.BimOpenSchema;
using System.Threading.Tasks;

namespace Ara3D.Studio.Samples.Lakehouse;

public class LakehouseRooms : IGenerator
{
    public int Entities => BimData?.Entities.Length ?? 0;
    public int Documents => BimData?.Documents.Length ?? 0;
    public int ParameterTypes => BimData?.Descriptors.Length ?? 0;
    public int ParameterValues => BimData?.Parameters.Length ?? 0;
    public int Meshes => Model?.Meshes.Count ?? 0;
    public int Instances => Model?.Instances.Count ?? 0;
    public LHProject Project { get; private set; }

    [Options(nameof(FileNames))] public int File;

    public bool FilterRooms { get; set; } = false;

    [Options(nameof(RoomNames))] public int Room;

    public Action ShowRoomData => ShowRoomDataImpl;

    public void ShowRoomDataImpl()
    {
        var builder = new DataTableBuilder("Rooms");
        var d = new Dictionary<string, string>();
        var roomLookup = RoomData.ToDictionary(rc => rc.RoomEntityIndex, rc => rc);
        var parameters = new Dictionary<int, ParameterDescriptor>();
    }

    private int _prevFile;

    public List<string> FileNames { get; private set; } = [];
    public List<string> RoomNames { get; private set; } = [];
    public List<string> Categories { get; private set; } = [];

    private int _oldFile = -1;

    private static DirectoryPath _dataFolder = new(@"C:\data\nxt-bld\lakehouse-test");
    private static FilePath _lakehouseFile = _dataFolder.RelativeFile("manifest.lh");

    private IHostApplication _app;

    public List<Entity> RoomEntities = [];

    public Entity? GetEntity(InstanceStruct i)
        => i.EntityIndex < 0 ? null : BimData.Entities[i.EntityIndex];

    public int GetDocument(InstanceStruct i)
        => (int?)(GetEntity(i)?.Document) ?? -1;

    public void Init(IHostApplication app)
    {
        _app = app;
        _ = LoadInitialAssetAsync();
    }

    
    private async Task LoadInitialAssetAsync()
    {
        try
        {
            var asset = await _app.LoadAssetAsync(_lakehouseFile, false);
            var bimData = asset.Attachments[0] as BimData;
            var renderModel = asset.Attachments[1] as RenderModelData;
            Project = new LHProject(bimData, renderModel);

            _app.RefreshUI(this);
            _app.Invalidate(this);
        }
        catch (Exception ex)
        {
            _app.Logger.LogError(ex);
        }
    }

    public IModel3D Eval()
    {
        var emptyModel = new Model3D([], []);
        var currentDoc = Project.Documents.ElementAtOrDefault(File);
        if (currentDoc == null)
            return emptyModel;

        if (_prevFile != File)
        {
            RoomNames = currentDoc.Rooms.Select(rd => rd.Name).ToList();
            _prevFile = File;
            _app.RefreshUI(this);
        }

        if (!FilterRooms)
            return currentDoc.Model;

        if (Room < 0 || Room >= RoomData.Count)
            return currentDoc.Model;

        return RoomData[Room].Model;
    }
}