using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.BimOpenSchema;

// This is a helper class for incrementally constructing a BIMData object without repeating objects. 
public class BimDataBuilder : IBimData
{
    public Manifest Manifest { get; set; } = new();

    private readonly Dictionary<Document, int> _documentLookup = new();
    private readonly Dictionary<Point, int> _pointLookup = new();
    private readonly Dictionary<ParameterDescriptor, int> _descriptorLookup = new();
    private readonly Dictionary<string, int> _stringLookup = new();
    private readonly Dictionary<float, int> _numberLookup = new();

    private readonly List<ParameterDescriptor> _descriptors = [];
    private readonly List<Parameter> _parameters = [];
    private readonly List<Document> _documents = [];
    private readonly List<Entity> _entities = [];
    private readonly List<string> _strings = [];
    private readonly List<Point> _points = [];
    private readonly List<float> _numbers = [];
    private readonly List<EntityRelation> _relations = [];
    private readonly List<Diagnostic> _diagnostics = [];

    public IReadOnlyList<ParameterDescriptor> Descriptors => _descriptors;
    public IReadOnlyList<Parameter> Parameters => _parameters;
    public IReadOnlyList<Document> Documents => _documents;
    public IReadOnlyList<Entity> Entities => _entities;
    public IReadOnlyList<string> Strings => _strings;
    public IReadOnlyList<Point> Points => _points;
    public IReadOnlyList<float> Numbers => _numbers;
    public IReadOnlyList<EntityRelation> Relations => _relations;
    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public BimGeometry Geometry { get; set; }

    private int Add<T>(Dictionary<T, int> d, List<T> list, T val)
    {
        Debug.Assert(val != null);
        if (d.TryGetValue(val, out var index))
            return index;
        var r = d.Count;
        d.Add(val, r);
        list.Add(val);
        Debug.Assert(d.Count == list.Count);
        return r;
    }
    
    public void AddRelation(EntityIndex a, EntityIndex b, RelationType rt)
        => _relations.Add(new(a, b, rt));

    public const StringIndex InvalidStringIndex = (StringIndex)(-1);
    public const DocumentIndex InvalidDocumentIndex = (DocumentIndex)(-1);
    public const EntityIndex InvalidEntityIndex = (EntityIndex)(-1);

    public EntityIndex AddEntity(long localId, string globalId, DocumentIndex d, string name, EntityIndex category, EntityIndex type)
    {
        _entities.Add(new(localId, AddString(globalId), d, AddString(name), category, type));
        return (EntityIndex)(_entities.Count - 1);
    }

    public EntityIndex AddEntity()
    {
        _entities.Add(new(-1, InvalidStringIndex, InvalidDocumentIndex, InvalidStringIndex, InvalidEntityIndex, InvalidEntityIndex));
        return (EntityIndex)(_entities.Count - 1);
    }

    public void UpdateEntity(EntityIndex index, long localId, string globalId, DocumentIndex d, string name, EntityIndex category, EntityIndex type)
        => _entities[(int)index] = new(localId, AddString(globalId), d, AddString(name), category, type);

    public DocumentIndex AddDocument(string title, string pathName)
        => (DocumentIndex)Add(_documentLookup, _documents, new(AddString(title), AddString(pathName)));

    public PointIndex AddPoint(Point p)
        => (PointIndex)Add(_pointLookup, _points, p);

    public DescriptorIndex AddDescriptor(string name, string units, string group, ParameterType pt)
        => (DescriptorIndex)Add(_descriptorLookup, _descriptors, new(AddString(name), AddString(units), AddString(group), pt));

    public StringIndex AddString(string name)
        => (StringIndex)Add(_stringLookup, _strings, name ?? "");

    public NumberIndex AddNumber(double val)
        => (NumberIndex)Add(_numberLookup, _numbers, (float)val);

    public void AddParameter(EntityIndex e, double val, DescriptorIndex d)
        => _parameters.Add(new(e, d, (int)AddNumber(val)));

    public void AddParameter(EntityIndex e, int val, DescriptorIndex d)
        => _parameters.Add(new(e, d, val));

    public void AddParameter(EntityIndex e, EntityIndex val, DescriptorIndex d)
        => _parameters.Add(new(e, d, (int)val));

    public void AddParameter(EntityIndex e, string val, DescriptorIndex d)
        => _parameters.Add(new(e, d, (int)AddString(val)));

    public void AddParameter(EntityIndex e, PointIndex pi, DescriptorIndex d)
        => _parameters.Add(new(e, d, (int)pi));

    public void AddParameter(EntityIndex e, Point p, DescriptorIndex d)
        => _parameters.Add(new(e, d, (int)AddPoint(p)));

    public void AddParameter(EntityIndex e, double val, string name, string units, string group)
        => AddParameter(e, val, AddDescriptor(name, units, group, ParameterType.Number));

    public void AddParameter(EntityIndex e, int val, string name, string units, string group)
        => AddParameter(e, val, AddDescriptor(name, units, group, ParameterType.Int));

    public void AddParameter(EntityIndex e, EntityIndex val, string name, string units, string group)
        => AddParameter(e, val, AddDescriptor(name, units, group, ParameterType.Entity));

    public void AddParameter(EntityIndex e, string val, string name, string units, string group)
        => AddParameter(e, val, AddDescriptor(name, units, group, ParameterType.String));

    public void AddParameter(EntityIndex e, Point p, string name, string units, string group)
        => AddParameter(e, p, AddDescriptor(name, units, group, ParameterType.Point));

    public void AddParameter(EntityIndex e, PointIndex pi, string name, string units, string group)
        => AddParameter(e, pi, AddDescriptor(name, units, group, ParameterType.Int));

    public void AddDiagnostic(DiagnosticType type, string msg, DocumentIndex di, EntityIndex e)
        => _diagnostics.Add(new(type, di, e, AddString(msg)));
}

