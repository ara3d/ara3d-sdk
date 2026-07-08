namespace Ara3D.Ifc.Mesher.Approach1;

public enum GeometrySupportStatus
{
    Supported,
    Approximate,
    Unsupported,
}

public sealed class MeshingDiagnostics
{
    readonly Dictionary<string, int> _counts = new(StringComparer.Ordinal);
    readonly Dictionary<string, GeometrySupportStatus> _status = new(StringComparer.Ordinal);
    readonly List<string> _messages = [];

    public IReadOnlyDictionary<string, int> EntityCounts => _counts;
    public IReadOnlyDictionary<string, GeometrySupportStatus> EntityStatus => _status;
    public IReadOnlyList<string> Messages => _messages;

    public void Record(string entityName, GeometrySupportStatus status, string? message = null)
    {
        _counts[entityName] = _counts.GetValueOrDefault(entityName) + 1;
        if (!_status.TryGetValue(entityName, out var existing) || status < existing)
            _status[entityName] = status;
        if (!string.IsNullOrEmpty(message))
            _messages.Add($"{entityName}: {message}");
    }

    public void RecordUnsupported(string entityName, string? reason = null)
        => Record(entityName, GeometrySupportStatus.Unsupported, reason);

    public void RecordApproximate(string entityName, string? reason = null)
        => Record(entityName, GeometrySupportStatus.Approximate, reason);

    public void RecordSupported(string entityName)
        => Record(entityName, GeometrySupportStatus.Supported);
}
