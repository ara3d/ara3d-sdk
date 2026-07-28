using Ara3D.Utils;

namespace Ara3D.Ifc.Mcp;

/// <summary>Keeps recently used models open across tool calls, because loading an IFC file is a
/// whole-file parse and an agent asks many small questions of one model. Bounded, evicting the
/// least recently used, since each open model holds its whole file in memory.</summary>
public sealed class IfcSessionCache : IDisposable
{
    public const int DefaultCapacity = 3;

    private readonly Dictionary<FilePath, IfcSession> _sessions = [];
    private readonly List<FilePath> _order = [];
    private readonly object _lock = new();
    private readonly int _capacity;

    public IfcSessionCache(int capacity = DefaultCapacity)
    {
        if (capacity < 1)
            throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
    }

    /// <summary>Returns the open session for a file, loading it if needed. Tools call this rather
    /// than requiring an explicit open, so any tool works as the first call against a model.</summary>
    public IfcSession Get(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("An IFC file path is required.", nameof(path));

        var full = new FilePath(path);
        if (!File.Exists(full.FullPath))
            throw new FileNotFoundException($"IFC file not found: {full.FullPath}");

        lock (_lock)
        {
            if (_sessions.TryGetValue(full, out var existing))
            {
                Touch(full);
                return existing;
            }

            var session = new IfcSession(full);
            _sessions[full] = session;
            Touch(full);
            EvictWhileOverCapacity();
            return session;
        }
    }

    public bool IsOpen(string path)
    {
        lock (_lock)
            return _sessions.ContainsKey(new FilePath(path));
    }

    public bool Close(string path)
    {
        lock (_lock)
        {
            var full = new FilePath(path);
            if (!_sessions.Remove(full, out var session))
                return false;
            _order.Remove(full);
            session.Dispose();
            return true;
        }
    }

    public int CloseAll()
    {
        lock (_lock)
        {
            var count = _sessions.Count;
            foreach (var session in _sessions.Values)
                session.Dispose();
            _sessions.Clear();
            _order.Clear();
            return count;
        }
    }

    public IReadOnlyList<IfcSession> OpenSessions()
    {
        lock (_lock)
            return _order.Select(path => _sessions[path]).ToList();
    }

    private void Touch(FilePath path)
    {
        _order.Remove(path);
        _order.Add(path);
    }

    private void EvictWhileOverCapacity()
    {
        while (_order.Count > _capacity)
        {
            var oldest = _order[0];
            _order.RemoveAt(0);
            if (_sessions.Remove(oldest, out var session))
                session.Dispose();
        }
    }

    public void Dispose()
        => CloseAll();
}
