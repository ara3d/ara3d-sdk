namespace Ara3D.Bowerbird;

// TODO: use source hashes and host version for compile caching.
/// <summary>
/// Optional cache key for future compile-on-change behavior.
/// </summary>
public readonly struct CommandCompileCacheKey
{
    public CommandCompileCacheKey(IReadOnlyList<string> sourceHashes, string hostVersion)
        => (SourceHashes, HostVersion) = (sourceHashes, hostVersion);

    public IReadOnlyList<string> SourceHashes { get; }
    public string HostVersion { get; }
}
