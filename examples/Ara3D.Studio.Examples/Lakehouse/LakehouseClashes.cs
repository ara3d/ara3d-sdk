using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.Lakehouse;

public class Clash
{
    public int SourceEntity { get; set; }
    public int DestinationEntity { get; set; }
}

public class ClashList
{
    public List<Clash> Clashes { get; set; }
}

[Category(Cat.ExperimentalBim)]
public class LakehouseClashes : IGenerator
{
    [Options(nameof(FileNames))] public int File;

    public bool ShowClashes = true;
    [Range(0, 1)] public float NonClashTransparency = 0.6f;

    public HashSet<int> ClashedEntities = new();
    public List<string> FileNames { get; private set; } = [];
    public List<FilePath> FilePaths { get; private set; } = [];
    public List<RenderModelData> RenderModels = [];
    public List<IModel3D> Models = [];
    public List<BimData> BimDataObjects = [];

    private int _oldFile = -1;

    private static DirectoryPath _dataFolder = new DirectoryPath(@"C:\data\nxt-bld\gni-bos");
    private static DirectoryPath _clashFolder = new DirectoryPath(@"C:\data\nxt-bld\clashes");

    private IHostApplication _app;

    public void Init(IHostApplication app)
    {
        _app = app;
        FilePaths = _dataFolder.GetFiles().ToList();
        FileNames = FilePaths.Select(f => f.GetFileName()).ToList();

        Models = Enumerable.Repeat(default(IModel3D), FileNames.Count).ToList();
        RenderModels = Enumerable.Repeat(default(RenderModelData), FileNames.Count).ToList();
        BimDataObjects = Enumerable.Repeat(default(BimData), FileNames.Count).ToList();
    }

    public FlowObject Eval()
    {
        if (File < 0 || File > Models.Count - 1)
            return null;

        if (Models[File] == null)
        {
            var f = FilePaths[File];
            var asset =  _app.LoadAssetAsync(f, false).GetAwaiter().GetResult();
            var renderData = asset.Value as RenderModelData;
            if (renderData == null)
                throw new Exception("Missing Render Model data");
            var bimData = asset.Attachments.FirstOrDefault() as BimData;
            if (bimData == null)
                throw new Exception("Missing BIM data");
            RenderModels[File] = renderData;
            BimDataObjects[File] = bimData;
            Models[File] = renderData.ToModel3D();
        }

        var model = Models[File];
        if (ShowClashes)
        {
            var f = FilePaths[File];
            ClashedEntities.Clear();
            var clashFile = f.ChangeDirectoryAndExt(_clashFolder, ".json");
            if (clashFile.Exists())
            {
                var clashList = clashFile.LoadJson<ClashList>();
                foreach (var clash in clashList.Clashes)
                {
                    ClashedEntities.Add(clash.SourceEntity);
                    ClashedEntities.Add(clash.DestinationEntity);
                }
            }

            var newInstances = new List<InstanceStruct>();
            foreach (var inst in model.Instances)
            {
                if (ClashedEntities.Contains(inst.EntityIndex))
                    newInstances.Add(inst);
                else
                    newInstances.Add(inst.WithAlpha(NonClashTransparency));
            }
            model = model.WithInstances(newInstances);
        }

        var flowObject = new FlowObject(model, null, [], [BimDataObjects[File], model]);
        return flowObject;
    }
}