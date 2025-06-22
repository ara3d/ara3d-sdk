namespace Ara3D.WorkItems;

/// <summary>
/// Use channels to store work items in a queue for processing
/// on specific threads. Some contain their own thread.
/// </summary>
public interface IWorkItemQueue : IDisposable
{
    string Name { get; }
    void Enqueue(WorkItem item);
    void ProcessAllPendingWork();
    void ClearAllPendingWork();
    void CancelCurrentAndClearPending();
}