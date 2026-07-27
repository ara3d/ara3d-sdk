namespace Ara3D.Ifc.Tests;

/// <summary>
/// Entity-level changeset between two IFC files, keyed by STEP instance id. An id present in both
/// files but with a different entity type counts as deleted *and* added: the original instance no
/// longer exists, a different one merely reuses its number. Same type but different raw text is a
/// change.
/// </summary>
public sealed class IfcDiff
{
    public readonly IReadOnlyList<int> Added;
    public readonly IReadOnlyList<int> Deleted;
    public readonly IReadOnlyList<int> Changed;

    public IfcDiff(IReadOnlyList<int> added, IReadOnlyList<int> deleted, IReadOnlyList<int> changed)
    {
        Added = added;
        Deleted = deleted;
        Changed = changed;
    }

    public bool IsEmpty
        => Added.Count == 0 && Deleted.Count == 0 && Changed.Count == 0;

    public int Count
        => Added.Count + Deleted.Count + Changed.Count;

    public static IfcDiff Compare(IfcSourceFile before, IfcSourceFile after)
    {
        var added = new List<int>();
        var deleted = new List<int>();
        var changed = new List<int>();

        foreach (var a in before.Spans)
        {
            var b = after.GetSpan(a.Id);
            if (b == null)
                deleted.Add(a.Id);
            else if (!string.Equals(a.TypeName, b.Value.TypeName, StringComparison.OrdinalIgnoreCase))
            {
                deleted.Add(a.Id);
                added.Add(a.Id);
            }
            else if (!string.Equals(before.GetText(a), after.GetText(b.Value), StringComparison.Ordinal))
                changed.Add(a.Id);
        }

        foreach (var b in after.Spans)
            if (!before.Contains(b.Id))
                added.Add(b.Id);

        added.Sort();
        deleted.Sort();
        changed.Sort();
        return new IfcDiff(added, deleted, changed);
    }

    public override string ToString()
        => $"added: {Summarize(Added)}, deleted: {Summarize(Deleted)}, changed: {Summarize(Changed)}";

    private static string Summarize(IReadOnlyList<int> ids)
    {
        if (ids.Count == 0)
            return "none";
        var shown = Math.Min(ids.Count, 8);
        var text = string.Join(",", ids.Take(shown).Select(i => $"#{i}"));
        return ids.Count > shown ? $"{ids.Count} ({text},...)" : $"{ids.Count} ({text})";
    }
}
