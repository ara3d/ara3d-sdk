using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// An immutable set of selected component indices (faces or vertices) within a
/// specific domain of a piece of geometry. Indices are sorted and distinct.
/// DomainCount is the component count at creation time; a selection is only
/// meaningful against content whose component count still matches (see Matches).
/// </summary>
public sealed class SelectionSet
{
    public FlowAttribute.AttributeDomain Domain { get; }
    public int DomainCount { get; }
    public IReadOnlyList<int> Indices => _indices;
    internal readonly int[] _indices;

    public int Count => _indices.Length;
    public bool IsEmpty => _indices.Length == 0;

    private SelectionSet(FlowAttribute.AttributeDomain domain, int[] sortedDistinct, int domainCount)
    {
        Domain = domain;
        _indices = sortedDistinct;
        DomainCount = domainCount;
    }

    public static SelectionSet Create(FlowAttribute.AttributeDomain domain, IReadOnlyList<int> indices, int domainCount)
    {
        if (domainCount < 0)
            throw new ArgumentOutOfRangeException(nameof(domainCount));
        var mask = new bool[domainCount];
        var count = 0;
        for (var i = 0; i < indices.Count; i++)
        {
            var index = indices[i];
            if (index < 0 || index >= domainCount)
                throw new ArgumentOutOfRangeException(nameof(indices), $"Index {index} outside domain of size {domainCount}");
            if (!mask[index])
            {
                mask[index] = true;
                count++;
            }
        }
        return new(domain, MaskToIndices(mask, count), domainCount);
    }

    public static SelectionSet FromMask(FlowAttribute.AttributeDomain domain, bool[] mask)
        => new(domain, MaskToIndices(mask, CountTrue(mask)), mask.Length);

    public static SelectionSet Empty(FlowAttribute.AttributeDomain domain, int domainCount)
        => new(domain, [], domainCount);

    public static SelectionSet All(FlowAttribute.AttributeDomain domain, int domainCount)
    {
        var indices = new int[domainCount];
        for (var i = 0; i < domainCount; i++)
            indices[i] = i;
        return new(domain, indices, domainCount);
    }

    public bool[] ToMask()
        => _indices.ToMask(DomainCount);

    public bool Contains(int index)
        => Array.BinarySearch(_indices, index) >= 0;

    public SelectionSet Invert()
        => new(Domain, _indices.Complement(DomainCount), DomainCount);

    public SelectionSet Union(SelectionSet other)
        => Combine(other, (a, b) => a | b);

    public SelectionSet Intersect(SelectionSet other)
        => Combine(other, (a, b) => a & b);

    public SelectionSet Subtract(SelectionSet other)
        => Combine(other, (a, b) => a & !b);

    private SelectionSet Combine(SelectionSet other, Func<bool, bool, bool> op)
    {
        if (other.Domain != Domain || other.DomainCount != DomainCount)
            throw new ArgumentException($"Selection domains do not match: {Domain}/{DomainCount} vs {other.Domain}/{other.DomainCount}");
        var a = ToMask();
        var b = other.ToMask();
        for (var i = 0; i < a.Length; i++)
            a[i] = op(a[i], b[i]);
        return FromMask(Domain, a);
    }

    /// <summary>
    /// True when this selection can be applied to content with the given component count.
    /// A cheap validity check, not a content-identity proof.
    /// </summary>
    public bool Matches(int domainCount)
        => DomainCount == domainCount;

    private static int[] MaskToIndices(bool[] mask, int count)
    {
        var result = new int[count];
        var j = 0;
        for (var i = 0; i < mask.Length; i++)
            if (mask[i])
                result[j++] = i;
        return result;
    }

    private static int CountTrue(bool[] mask)
    {
        var count = 0;
        for (var i = 0; i < mask.Length; i++)
            if (mask[i])
                count++;
        return count;
    }
}
