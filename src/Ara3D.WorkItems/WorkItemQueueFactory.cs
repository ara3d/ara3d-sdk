namespace Ara3D.WorkItems;

public static class WorkItemQueueFactory
{
    /// <summary>
    /// Creates a multithreaded work item single element queue. New items replace the older item.
    /// </summary>
    public static IWorkItemQueue CreateThreadedLastOnly(string name, ThreadPriority priority, IWorkItemListener listener)
        => new WorkItemQueue(name, listener, priority, true, 1);

    /// <summary>
    /// Creates a multithreaded work item queue that can hold multiple items 
    /// </summary>
    public static IWorkItemQueue CreateThreaded(string name, ThreadPriority priority, IWorkItemListener listener)
        => new WorkItemQueue(name, listener, priority, true, 0);

    /// <summary>
    /// Creates a work item multi-item queue that will process work on a SynchronizationContext (like a WPF dispatcher).
    /// </summary>
    public static IWorkItemQueue CreateSynchronized(string name, SynchronizationContext context, IWorkItemListener listener)
        => new SynchronizedWorkItemQueue(name, listener, 0, context);

    /// <summary>
    /// Creates a work item multi-item queue that does not process items on a thread. They have to be extracted manually
    /// </summary>
    public static IWorkItemQueue CreateManual(string name, IWorkItemListener listener)
        => new WorkItemQueue(name, listener, ThreadPriority.Normal, false, 0);
}