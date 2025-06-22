namespace Ara3D.WorkItems;

public class SynchronizedWorkItemQueue : IWorkItemQueue
{
    private readonly WorkItemQueue _queue;
    private readonly SynchronizationContext _context;
        
    public SynchronizedWorkItemQueue(string name, IWorkItemListener listener, int capacity, SynchronizationContext context)
    {
        _queue = new WorkItemQueue(name, listener, ThreadPriority.Normal, false, capacity);
        _context = context;
    }

    public string Name 
        => _queue.Name;

    public void ProcessAllPendingWork()
        => _context.Post(_ => _queue.ProcessAllPendingWork(), null);

    public void Enqueue(WorkItem item) 
        => _queue.Enqueue(item);

    public void Dispose()
        => _queue.Dispose();

    public void ClearAllPendingWork()
        => _queue.ClearAllPendingWork();

    public void CancelCurrentAndClearPending()
        => _queue.CancelCurrentAndClearPending();
}