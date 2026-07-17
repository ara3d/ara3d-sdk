using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Ara3D.Geometry;

/// <summary>
/// Identifies a piece of derived data within an owner's cache: a name plus an optional
/// secondary dependency compared by reference (e.g. the face-index list when the owner
/// is the point list and the derived value depends on both).
/// </summary>
public readonly struct DerivedKey : IEquatable<DerivedKey>
{
    public readonly string Name;
    public readonly object? Dependency;

    public DerivedKey(string name, object? dependency = null)
    {
        Name = name;
        Dependency = dependency;
    }

    public bool Equals(DerivedKey other)
        => Name == other.Name && ReferenceEquals(Dependency, other.Dependency);

    public override bool Equals(object? obj)
        => obj is DerivedKey key && Equals(key);

    public override int GetHashCode()
        => HashCode.Combine(Name, Dependency == null ? 0 : RuntimeHelpers.GetHashCode(Dependency));
}

/// <summary>
/// Cache of data derived on demand from immutable payloads (tracker studio-045/048).
/// Entries are keyed weakly on an owner reference (e.g. a mesh's point list) so they are
/// collected with the owner; because payloads are never mutated in place, cached values can
/// never go stale and no invalidation protocol exists. Each entry is computed at most once,
/// even under concurrent access. <see cref="Default"/> is the process-wide instance used by
/// the mesh accessors; hosts may construct their own instances to control policy.
/// </summary>
public sealed class DerivedDataCache
{
    public static readonly DerivedDataCache Default = new();

    private readonly ConditionalWeakTable<object, ConcurrentDictionary<DerivedKey, Lazy<object>>> _cache = new();

    public T GetOrCompute<T>(object owner, DerivedKey key, Func<T> compute)
        => (T)_cache.GetOrCreateValue(owner)
            .GetOrAdd(key, static (_, f) => new Lazy<object>(() => f()!, LazyThreadSafetyMode.ExecutionAndPublication), compute)
            .Value;

    public T GetOrCompute<T>(object owner, string name, Func<T> compute)
        => GetOrCompute(owner, new DerivedKey(name), compute);
}
