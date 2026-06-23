using Ara3D.BimOpenSchema;

namespace Ara3D.Studio.Samples.Lakehouse;

public class LHRoom
{
    public LHDoc Doc { get; set; }
    public EntityIndex EntityIndex { get; set; }
    public Entity Entity { get; set; }
    public string Name { get; set; }
    public TriangleMesh3D Mesh { get; set; }
    public InstanceStruct Instance { get; set; }
    public Bounds3D Bounds { get; set; }
    public IModel3D Model { get; set; }
    public List<Parameter> Parameters { get; set; } = [];
    public HashSet<InstanceStruct> Members { get; set; } = [];
}