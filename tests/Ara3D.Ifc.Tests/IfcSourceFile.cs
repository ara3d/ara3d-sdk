using Ara3D.IfcLoader;
using Ara3D.Utils;

namespace Ara3D.Ifc.Tests;

/// <summary>
/// An IFC file opened for entity-level editing: the parsed <see cref="IfcFile"/> (Ara3D.IfcLoader)
/// plus the original bytes and the byte span of every entity instance. All edits are raw byte
/// splices against <see cref="Bytes"/>, so untouched content stays byte-identical.
/// </summary>
public sealed class IfcSourceFile : IDisposable
{
    public readonly IfcFile File;
    public readonly byte[] Bytes;
    public readonly IReadOnlyList<IfcEntitySpan> Spans;

    private readonly Dictionary<int, int> _spanIndexById;

    private IfcSourceFile(IfcFile file, byte[] bytes)
    {
        File = file;
        Bytes = bytes;
        Spans = IfcEntitySpans.Compute(file.Document);
        _spanIndexById = new Dictionary<int, int>(Spans.Count);
        for (var i = 0; i < Spans.Count; i++)
            _spanIndexById.Add(Spans[i].Id, i);
    }

    public static IfcSourceFile Load(FilePath filePath)
        => new(IfcFile.Load(filePath, includeGeometry: false), System.IO.File.ReadAllBytes(filePath));

    public int Count
        => Spans.Count;

    public bool Contains(int id)
        => _spanIndexById.ContainsKey(id);

    public IfcEntitySpan? GetSpan(int id)
        => _spanIndexById.TryGetValue(id, out var i) ? Spans[i] : null;

    /// <summary>Raw source text of one entity, '#' through ';'.</summary>
    public string GetText(IfcEntitySpan span)
        => System.Text.Encoding.Latin1.GetString(Bytes, span.Begin, span.Length);

    public int MaxId
    {
        get
        {
            var max = 0;
            for (var i = 0; i < Spans.Count; i++)
                if (Spans[i].Id > max)
                    max = Spans[i].Id;
            return max;
        }
    }

    public int FirstIdOfType(string typeName)
    {
        for (var i = 0; i < Spans.Count; i++)
            if (string.Equals(Spans[i].TypeName, typeName, StringComparison.OrdinalIgnoreCase))
                return Spans[i].Id;
        return -1;
    }

    /// <summary>Maps IfcRoot GlobalId to entity id for every instance whose first attribute is a string.</summary>
    public Dictionary<string, int> GlobalIdToEntityId()
    {
        var r = new Dictionary<string, int>();
        foreach (var e in File.EntityResolver.GetEntities())
        {
            if (e.Attributes.Count == 0 || !e.Attributes[0].IsString)
                continue;
            var guid = e.GetString(0);
            if (guid.Length == IfcGuid.Length)
                r.TryAdd(guid, e.Id);
        }
        return r;
    }

    public void Dispose()
        => File.Dispose();
}
