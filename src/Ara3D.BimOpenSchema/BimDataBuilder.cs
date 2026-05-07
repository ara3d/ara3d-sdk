using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Ara3D.Geometry;

namespace Ara3D.BimOpenSchema;

// This is a helper class for incrementally constructing a BIMData object without repeating objects. 
// TODO: this should not implement "IBimData" directly. 
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

    // TODO: it is very awkward that this is not a BimGeometryBuilder
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
    public const DescriptorIndex InvalidDescriptorIndex = (DescriptorIndex)(-1);
    public const NumberIndex InvalidNumberIndex = (NumberIndex)(-1);

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

    public void AddBimData(IBimData bd, string title, string path)
    {
        var numEntities = _entities.Count;
        var numDocs = _documents.Count;
        var numPoints = _points.Count;
        var numParameters = _parameters.Count;
        var numDescriptors = _descriptors.Count;
        var numNumbers = _numbers.Count;

        //========================================
        // Helper functions

        StringIndex NewStr(StringIndex i) =>
            i == InvalidStringIndex ? InvalidStringIndex : AddString(bd.Strings[(int)i]);
        
        EntityIndex NewEntity(EntityIndex i) 
            => i == InvalidEntityIndex ? InvalidEntityIndex : numEntities + i;

        DescriptorIndex NewDesc(DescriptorIndex i)
            => i == InvalidDescriptorIndex ? InvalidDescriptorIndex : numDescriptors + i;

        NumberIndex NewNumber(NumberIndex i)
            => i == InvalidNumberIndex ? InvalidNumberIndex : numNumbers + i;

        //========================================

        foreach (var e in bd.Entities)
            _entities.Add(new(e.LocalId, NewStr(e.GlobalId), (DocumentIndex)numDocs, NewStr(e.Name), NewEntity(e.Category),
                NewEntity(e.Type)));

        foreach (var d in bd.Descriptors)
            _descriptors.Add(new(NewStr(d.Name), NewStr(d.Units), NewStr(d.Group), d.Type));

        foreach (var p in bd.Points)
            _points.Add(new Point(p.X, p.Y, p.Z));

        foreach (var x in bd.Numbers)
            _numbers.Add(x);

        foreach (var r in bd.Relations)
            _relations.Add(new(NewEntity(r.EntityA), NewEntity(r.EntityB), r.RelationType));

        foreach (var p in bd.Parameters)
        {
            var val = p.Value;
            var desc = bd.Descriptors[(int)p.Descriptor];
            if (desc.Type == ParameterType.Entity)
            {
                val = (int)NewEntity((EntityIndex)val);
            }
            else if (desc.Type == ParameterType.Point)
            {
                if (val >= 0) val = val + numPoints; 
            }
            else if (desc.Type == ParameterType.String)
            {
                val = (int)NewStr((StringIndex)val);
            }
            else if (desc.Type == ParameterType.Number)
            {
                val = (int)NewNumber((NumberIndex)val);
            }
            var p2 = new Parameter(NewEntity(p.Entity), NewDesc(p.Descriptor), val);
            _parameters.Add(p2);
        }

        // TEMP: right now we add the document 
        _documents.Add(new(AddString(title), AddString(path)));
    }
}

