using System.Globalization;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Mcp;

/// <summary>A parameter is one named value carried by an element. IFC stores properties and
/// quantities the same way — a named set reached through IfcRelDefinesByProperties — so this index
/// covers both, which is what a user asking about "parameters" means.
///
/// The point of it is direction. <see cref="IfcPropData"/> answers "what does element N carry", so
/// every cross-model question — which elements are load bearing, what fire ratings exist, Height
/// for all walls — costs a walk of every element with a token unwrap per value, and the unwrap is
/// the expensive part. This inverts that once per session: value text and its numeric reading are
/// resolved a single time, then keyed by parameter, so a query costs a dictionary hit and a walk of
/// the matches instead of a walk of the model.
///
/// It adds no file read. <see cref="IfcSession.Properties"/> has already scanned the file; this is
/// a second pass over data that is already in memory.</summary>
public sealed class IfcParameterIndex
{
    public const int SampleCount = 3;

    private readonly Dictionary<IfcParamKey, IfcParamValue[]> _values;
    private readonly Dictionary<IfcParamKey, IfcParameterInfo> _info;
    private readonly Dictionary<string, IfcParamKey[]> _byName;
    private readonly Dictionary<int, string> _elementTypes;
    private readonly IfcParamKey[] _keys;

    public IfcParameterIndex(IfcSession session)
    {
        _elementTypes = [];
        var buckets = Accumulate(session, _elementTypes);
        _values = new Dictionary<IfcParamKey, IfcParamValue[]>(buckets.Count, IfcParamKey.IgnoreCase.Instance);
        _info = new Dictionary<IfcParamKey, IfcParameterInfo>(buckets.Count, IfcParamKey.IgnoreCase.Instance);

        foreach (var pair in buckets)
        {
            var values = pair.Value.Values.ToArray();
            Array.Sort(values, (a, b) => a.ElementId.CompareTo(b.ElementId));
            _values[pair.Key] = values;
            _info[pair.Key] = Describe(pair.Key, pair.Value, values);
        }

        _keys = SortKeys(_values.Keys);
        _byName = GroupByName(_keys);
    }

    public int ParameterCount
        => _keys.Length;

    public int ElementCount
        => _elementTypes.Count;

    /// <summary>Every parameter in the model, ordered by set then name so paging is stable.</summary>
    public IReadOnlyList<IfcParamKey> Keys
        => _keys;

    public string ElementType(int elementId)
        => _elementTypes.TryGetValue(elementId, out var type) ? type : "";

    public IfcParameterInfo Info(IfcParamKey key)
        => _info[key];

    public IReadOnlyList<IfcParamValue> Values(IfcParamKey key)
        => _values.TryGetValue(key, out var values) ? values : [];

    /// <summary>The keys a caller's token names. A token may be qualified with its set —
    /// <c>Pset_WallCommon.LoadBearing</c> — or bare, in which case every set carrying that name
    /// matches, because the same name legitimately appears in several sets.</summary>
    public IReadOnlyList<IfcParamKey> Resolve(string token, string? propertySet)
    {
        var (set, name) = Split(token, propertySet);
        if (set == null)
            return _byName.TryGetValue(name, out var keys) ? keys : [];

        var key = new IfcParamKey(set, name);
        return _values.ContainsKey(key) ? [key] : [];
    }

    private static (string? Set, string Name) Split(string token, string? propertySet)
    {
        var dot = token.LastIndexOf('.');
        return dot > 0 && propertySet == null
            ? (token[..dot], token[(dot + 1)..])
            : (propertySet, token);
    }

    private static Dictionary<IfcParamKey, Bucket> Accumulate(IfcSession session, Dictionary<int, string> types)
    {
        var data = session.Properties;
        var document = session.File.Document;
        var buckets = new Dictionary<IfcParamKey, Bucket>(IfcParamKey.IgnoreCase.Instance);

        foreach (var entity in session.Resolver.GetEntities())
        {
            if (!data.ObjectToPropSets.TryGetValue(entity.Id, out var setIds))
                continue;

            types[entity.Id] = entity.GetEntityName();
            foreach (var setId in setIds)
                // A relation can point at a set kind IfcPropData does not parse; skip it rather than throw.
                if (data.PropSets.TryGetValue(setId, out var set))
                    foreach (var value in data.GetProperties(set))
                        Record(buckets, set, value, entity.Id, document);
        }

        return buckets;
    }

    private static void Record(
        Dictionary<IfcParamKey, Bucket> buckets,
        IfcPropSet set,
        IfcPropValue value,
        int elementId,
        StepDocument document)
    {
        var key = new IfcParamKey(set.Name, value.Name);
        if (!buckets.TryGetValue(key, out var bucket))
            buckets[key] = bucket = new Bucket(value.Kind == IfcPropKind.Quantity, MeasureOf(value));

        var text = value.GetValueText(document);
        bucket.Values.Add(new IfcParamValue(elementId, text, AsNumber(text)));
    }

    /// <summary>A quantity carries a bare number, so its own entity name is the only measure type
    /// on offer — the same fallback <c>ifc_properties</c> makes.</summary>
    private static string MeasureOf(IfcPropValue value)
    {
        var measure = value.GetMeasureType();
        return measure.Length == 0 ? value.EntityName : measure;
    }

    private static IfcParameterInfo Describe(IfcParamKey key, Bucket bucket, IfcParamValue[] values)
    {
        var distinct = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var elements = new HashSet<int>();
        var samples = new List<string>();
        var numeric = 0;
        var min = double.MaxValue;
        var max = double.MinValue;

        foreach (var value in values)
        {
            elements.Add(value.ElementId);
            if (distinct.Add(value.Text) && value.Text.Length > 0 && samples.Count < SampleCount)
                samples.Add(value.Text);

            if (value.Number is not { } number)
                continue;

            numeric++;
            min = Math.Min(min, number);
            max = Math.Max(max, number);
        }

        return new IfcParameterInfo(
            key.PropertySet,
            key.Name,
            bucket.IsQuantity,
            bucket.Measure,
            elements.Count,
            distinct.Count,
            numeric,
            numeric > 0 ? min : null,
            numeric > 0 ? max : null,
            samples);
    }

    private static IfcParamKey[] SortKeys(IEnumerable<IfcParamKey> keys)
    {
        var sorted = keys.ToArray();
        Array.Sort(sorted, (a, b) =>
        {
            var set = string.Compare(a.PropertySet, b.PropertySet, StringComparison.OrdinalIgnoreCase);
            return set != 0 ? set : string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        });
        return sorted;
    }

    private static Dictionary<string, IfcParamKey[]> GroupByName(IReadOnlyList<IfcParamKey> keys)
    {
        var groups = new Dictionary<string, List<IfcParamKey>>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in keys)
        {
            if (!groups.TryGetValue(key.Name, out var list))
                groups[key.Name] = list = [];
            list.Add(key);
        }

        var result = new Dictionary<string, IfcParamKey[]>(groups.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var group in groups)
            result[group.Key] = group.Value.ToArray();
        return result;
    }

    /// <summary>IFC writes reals with an invariant decimal point, so the numeric reading is
    /// culture-independent. Booleans are logicals (<c>.T.</c>/<c>.F.</c>) and stay text.</summary>
    public static double? AsNumber(string text)
        => double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            ? number
            : null;

    private sealed class Bucket(bool isQuantity, string measure)
    {
        public readonly List<IfcParamValue> Values = [];
        public readonly bool IsQuantity = isQuantity;
        public readonly string Measure = measure;
    }
}
