using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.BimOpenSchema;

// This is a helper class for incrementally constructing a BIMData object without repeating objects. 
public class BimDataBuilder 
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

    public BimData Build()
    {
        return new BimData()
        {
            Manifest = Manifest,
            Descriptors = _descriptors.ToArray(),
            Documents = _documents.ToArray(),
            Diagnostics = _diagnostics.ToArray(),
            Entities = _entities.ToArray(),
            Strings = _strings.ToArray(),
            Geometry = Geometry,
            Numbers = _numbers.ToArray(),
            Parameters = _parameters.ToArray(),
            Points = _points.ToArray(),
            Relations = _relations.ToArray(),
        };
    }


    public string Get(StringIndex index) => _strings[(int)index];
    public Entity Get(EntityIndex index) => _entities[(int)index];
    public Document Get(DocumentIndex index) => _documents[(int)index];
    public Point Get(PointIndex index) => _points[(int)index];
    public float Get(NumberIndex index) => _numbers[(int)index];
    public Parameter Get(ParameterIndex index) => _parameters[(int)index];
    public EntityRelation Get(RelationIndex index) => _relations[(int)index];
    public ParameterDescriptor Get(DescriptorIndex index) => _descriptors[(int)index];

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
        // The new index must come from the list, not the dictionary: AddBimData appends
        // items to the lists that may be duplicates, so the dictionary can be smaller.
        var r = list.Count;
        d.Add(val, r);
        list.Add(val);
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
        => AddParameter(e, pi, AddDescriptor(name, units, group, ParameterType.Point));

    public void AddDiagnostic(DiagnosticType type, string msg, DocumentIndex di, EntityIndex e)
        => _diagnostics.Add(new(type, di, e, AddString(msg)));

    /// <summary>
    /// Adds all data from the given IBimData, re-assigning every entity to a single
    /// new document created from the given title and path.
    /// </summary>
    public void AddBimData(IBimData bd, string title, string path)
        => AddBimData(bd, title, path, preserveDocuments: false);

    /// <summary>
    /// Adds all data from the given IBimData, preserving its document table
    /// (documents are appended and entity document indices remapped).
    /// </summary>
    public void AddBimData(IBimData bd)
        => AddBimData(bd, null, null, preserveDocuments: true);

    private void AddBimData(IBimData bd, string title, string path, bool preserveDocuments)
    {
        var numEntities = _entities.Count;
        var numPoints = _points.Count;
        var numDescriptors = _descriptors.Count;
        var numNumbers = _numbers.Count;
        var numStrings = _strings.Count;

        // The string table is copied positionally, so string indices are remapped by a
        // fixed offset. When the builder is empty, all indices of the added data are
        // preserved verbatim, making the copy lossless.
        foreach (var s in bd.Strings)
        {
            var s2 = s ?? "";
            _stringLookup.TryAdd(s2, _strings.Count);
            _strings.Add(s2);
        }

        //========================================
        // Helper functions

        StringIndex NewStr(StringIndex i) =>
            i == InvalidStringIndex ? InvalidStringIndex : numStrings + i;

        EntityIndex NewEntity(EntityIndex i)
            => i == InvalidEntityIndex ? InvalidEntityIndex : numEntities + i;

        DescriptorIndex NewDesc(DescriptorIndex i)
            => i == InvalidDescriptorIndex ? InvalidDescriptorIndex : numDescriptors + i;

        NumberIndex NewNumber(NumberIndex i)
            => i == InvalidNumberIndex ? InvalidNumberIndex : numNumbers + i;

        //========================================

        Func<DocumentIndex, DocumentIndex> documentMapper;
        if (preserveDocuments)
        {
            var numDocs = _documents.Count;
            foreach (var d in bd.Documents)
            {
                var d2 = new Document(NewStr(d.Title), NewStr(d.Path));
                _documentLookup.TryAdd(d2, _documents.Count);
                _documents.Add(d2);
            }
            documentMapper = i => i == InvalidDocumentIndex ? InvalidDocumentIndex : numDocs + i;
        }
        else
        {
            var docIndex = (DocumentIndex)_documents.Count;
            var doc = new Document(AddString(title), AddString(path));
            _documentLookup.TryAdd(doc, _documents.Count);
            _documents.Add(doc);
            documentMapper = _ => docIndex;
        }

        foreach (var e in bd.Entities)
            _entities.Add(new(e.LocalId, NewStr(e.GlobalId), documentMapper(e.Document), NewStr(e.Name), NewEntity(e.Category),
                NewEntity(e.Type)));

        // Items are appended positionally (the parameter remapping below relies on fixed
        // offsets), so duplicates are kept. The lookup dictionaries are still updated
        // (first occurrence wins) so that Add* calls made after AddBimData return valid indices.
        foreach (var d in bd.Descriptors)
        {
            var d2 = new ParameterDescriptor(NewStr(d.Name), NewStr(d.Units), NewStr(d.Group), d.Type);
            _descriptorLookup.TryAdd(d2, _descriptors.Count);
            _descriptors.Add(d2);
        }

        foreach (var p in bd.Points)
        {
            var p2 = new Point(p.X, p.Y, p.Z);
            _pointLookup.TryAdd(p2, _points.Count);
            _points.Add(p2);
        }

        foreach (var x in bd.Numbers)
        {
            _numberLookup.TryAdd(x, _numbers.Count);
            _numbers.Add(x);
        }

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

        foreach (var d in bd.Diagnostics)
            _diagnostics.Add(new(d.Type, documentMapper(d.Document), NewEntity(d.Entity), NewStr(d.Message)));
    }
}

