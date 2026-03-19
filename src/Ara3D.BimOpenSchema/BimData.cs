using System.Collections.Generic;

namespace Ara3D.BimOpenSchema;

public class BimData : IBimData
{
    public Manifest Manifest { get; set; } = new();
    public IReadOnlyList<Diagnostic> Diagnostics { get; set; } = [];
    public IReadOnlyList<ParameterDescriptor> Descriptors { get; set; } = [];
    public IReadOnlyList<Parameter> Parameters { get; set; } = [];
    public IReadOnlyList<float> Numbers { get; set; } = [];
    public IReadOnlyList<Document> Documents { get; set; } = [];
    public IReadOnlyList<Entity> Entities { get; set; } = [];
    public IReadOnlyList<string> Strings { get; set; } = [];
    public IReadOnlyList<Point> Points { get; set; } = [];
    public IReadOnlyList<EntityRelation> Relations { get; set; } = [];
    public BimGeometry Geometry { get; set; }
}