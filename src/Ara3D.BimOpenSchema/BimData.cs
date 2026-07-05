namespace Ara3D.BimOpenSchema;

public class BimData : IBimData
{
    public Manifest Manifest { get; set; } = new();
    public Diagnostic[] Diagnostics { get; set; } = [];
    public ParameterDescriptor[] Descriptors { get; set; } = [];
    public Parameter[] Parameters { get; set; } = [];
    public float[] Numbers { get; set; } = [];
    public Document[] Documents { get; set; } = [];
    public Entity[] Entities { get; set; } = [];
    public string[] Strings { get; set; } = [];
    public Point[] Points { get; set; } = [];
    public EntityRelation[] Relations { get; set; } = [];
    public BimGeometry Geometry { get; set; }
}