namespace Ara3D.WorkItems;

public readonly struct WorkItem
{
    private static long _NextId;
    public long Id { get; }
    public readonly string Name;
    public readonly Action<CancellationToken> Action;

    public WorkItem(string name, Action<CancellationToken> action)
    {
        Id = Interlocked.Increment(ref _NextId);
        Name = name;
        Action = action;
    }

    public override string ToString()
        => $"[{Id}] {Name}";
}