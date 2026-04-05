using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Ara3D.Studio.Services;

public class SimpleWorkItemQueue
{
    public SimpleWorkItemQueue(bool multiItem, bool threaded)
    {
        MultiItem = multiItem;
        Threaded = threaded;
    }

    public bool MultiItem { get; }
    public bool Threaded { get; }
    public ConcurrentQueue<Action> Actions { get; } = new();

    public bool TryDequeue(out Action action)
        => Actions.TryDequeue(out action);

    public void Enqueue(Action action)
    {
        if (!MultiItem)
            Actions.Clear();
        Actions.Enqueue(action);
        if (Threaded)
            Task.Run(ProcessAllPendingWork);
    }

    public void ProcessAllPendingWork()
    {
        while (TryDequeue(out var action))
        {
            action.Invoke();
        }
    }

    public bool HasWork()
        => Actions.Count > 0;
}