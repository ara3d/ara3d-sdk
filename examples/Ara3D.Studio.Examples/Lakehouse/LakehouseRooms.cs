using Ara3D.BimOpenSchema;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ara3D.Studio.Samples.Lakehouse;

public class LakehouseRooms : IGenerator
{
    public int Entities => Project?.BimData?.Entities.Length ?? 0;
    public int Documents => Project?.BimData?.Documents.Length ?? 0;
    public int ParameterTypes => Project?.BimData?.Descriptors.Length ?? 0;
    public int ParameterValues => Project?.BimData?.Parameters.Length ?? 0;
    public int Meshes => Project?.Model?.Meshes.Count ?? 0;
    public int Instances => Project?.Model?.Instances.Count ?? 0;
    public LHProject Project { get; private set; }

    [Options(nameof(FileNames))] public int File;

    public bool FilterRooms { get; set; } = false;

    [Options(nameof(RoomNames))] public int Room;

    public Action ShowRoomData => ShowRoomDataImpl;

    public void ShowRoomDataImpl()
    {
        if (currentDoc == null)
        {
            MessageBox.Show("No document currently active");
            return;
        }
        var props = currentDoc.GetRoomProperties();
        DataTableWindow.CreateAndShow(props);
    }

    private int _prevFile;

    public List<string> FileNames { get; private set; } = [];
    public List<string> RoomNames { get; private set; } = [];
    public List<string> Categories { get; private set; } = [];

    private LHDocument currentDoc => Project?.Documents.ElementAtOrDefault(File);
    private LHRoom currentRoom => currentDoc?.Rooms.ElementAtOrDefault(Room);

    private int _oldFile = -1;

    private static DirectoryPath _dataFolder = new(@"C:\data\nxt-bld\lakehouse-test");
    private static FilePath _lakehouseFile = _dataFolder.RelativeFile("manifest.lh");

    private IHostApplication _app;

    public List<Entity> RoomEntities = [];

    public void Init(IHostApplication app)
    {
        _app = app;
        _ = LoadInitialAssetAsync();
    }

    private async Task LoadInitialAssetAsync()
    {
        try
        {
            _app.Logger.Log("Starting load of LakeHouse");
            var asset = await _app.LoadAssetAsync(_lakehouseFile, false);
            var bimData = asset.Attachments[0] as BimData;
            var renderModel = asset.Attachments[1] as RenderModelData;
            _app.Logger.Log("Completed loading phase");
            _app.Logger.Log("Starting initialization");
            Project =new LHProject(bimData, renderModel);
            _app.Logger.Log("Completed initialization");
            FileNames = Project.Documents.Select(f => f.Title).ToList();
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

        if (currentRoom == null)
            return currentDoc.Model;

        return currentRoom.Model;
    }
}