namespace Ara3D.WorkItems;

public static class WorkItemQueueExtensions
{
    public static void Enqueue(this IWorkItemQueue self, Action<CancellationToken> action)
        => self.Enqueue(action, "");

    public static void Enqueue(this IWorkItemQueue self, Action action)
        => self.Enqueue(action, "");

    public static void Enqueue(this IWorkItemQueue self, Action<CancellationToken> action, string name)
        => self.Enqueue(new WorkItem(name, action));

    public static void Enqueue(this IWorkItemQueue self, Action action, string name)
        => self.Enqueue(new WorkItem(name, _ => action()));
}