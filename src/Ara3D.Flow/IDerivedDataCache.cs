using Ara3D.Geometry;

namespace Ara3D.Studio.API;

/// <summary>
/// Host-controlled access to the derived-data cache (tracker studio-045/048). The host owns
/// the instance so cache policy — size budgets, second-level content-keyed caching, telemetry —
/// can evolve without changing script code. Scripts reach it via <see cref="EvalContext.DerivedData"/>.
/// </summary>
public interface IDerivedDataCache
{
    T GetOrCompute<T>(object owner, DerivedKey key, Func<T> compute);
}

public static class DerivedDataCacheExtensions
{
    public static T GetOrCompute<T>(this IDerivedDataCache cache, object owner, string name, Func<T> compute)
        => cache.GetOrCompute(owner, new DerivedKey(name), compute);
}

/// <summary>
/// Default host implementation: a view over a <see cref="DerivedDataCache"/> instance,
/// the process-wide <see cref="DerivedDataCache.Default"/> unless one is provided — so data
/// cached through the context and through the mesh accessors (e.g. DerivedTopology) is shared.
/// </summary>
public sealed class SharedDerivedDataCache : IDerivedDataCache
{
    private readonly DerivedDataCache _cache;

    public SharedDerivedDataCache(DerivedDataCache? cache = null)
        => _cache = cache ?? DerivedDataCache.Default;

    public T GetOrCompute<T>(object owner, DerivedKey key, Func<T> compute)
        => _cache.GetOrCompute(owner, key, compute);
}
