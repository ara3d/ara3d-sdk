using System.Diagnostics;
using System.Threading.Channels;

namespace Ara3D.WorkItems
{
    public class WorkItemQueue : IWorkItemQueue
    {
        public string Name { get; }
        public IWorkItemListener Listener;

        private bool _disposed;

        // It is worth considering that maybe this might be improved by being replaced with a Blocking Collection. 
        private readonly Channel<WorkItem> _channel;
        private readonly Thread? _thread;
        private CancellationTokenSource _cts = new();

        public WorkItemQueue(string name, IWorkItemListener listener, ThreadPriority priority, bool threaded, int capacity)
        {
            Name = name;
            Listener = listener;
            _channel = capacity > 0
                ? Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(capacity)
                {
                    SingleReader = true,
                    FullMode = BoundedChannelFullMode.DropOldest
                })
                : Channel.CreateUnbounded<WorkItem>(new UnboundedChannelOptions
                {
                    SingleReader = true
                });

            if (threaded)
            {
                _thread = new Thread(RunLoop)
                {
                    IsBackground = true,
                    Name = name,
                    Priority = priority
                };
                _thread.Start();
            }
        }

        public void Enqueue(WorkItem item)
        {
            if (!_channel.Writer.TryWrite(item))
                throw new InvalidOperationException($"Failed to enqueue work item {item.Name}");
        }

        public void ClearAllPendingWork()
        {
            var reader = _channel.Reader;
            // TryRead returns false as soon as the channel is empty.
            while (reader.TryRead(out _)) ;
        }

        public void CancelCurrentAndClearPending()
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ClearAllPendingWork();
        }

        public void ProcessAllPendingWork()
        {
            var reader = _channel.Reader;
            var token = _cts.Token;
            while (reader.TryRead(out var work))
            {
                try
                {
                    try
                    {
                        Listener?.OnWorkStarted(this, work);
                    }
                    catch
                    {
                        Debug.Assert(false, "Listener OnWorkStarted should never throw an error");
                    }

                    work.Action(token);

                    try
                    {
                        Listener?.OnWorkCompleted(this, work);
                    }
                    catch
                    {
                        Debug.Assert(false, "Listener OnWorkCompleted should never thrown an error");
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        Listener?.OnWorkError(this, work, ex);
                    }
                    catch
                    {
                        Debug.Assert(false, "Listener OnWorkError should never throw an error");
                    }
                }
            }
        }

        private void RunLoop(object? _)
        {
            var reader = _channel.Reader;
            var token = _cts.Token;

            // Loop until the channel is completed 
            try
            {
                while (true)
                {
                    try
                    {
                        // Block until there's data or the channel is completed
                        var moreData = reader.WaitToReadAsync(token).AsTask().GetAwaiter().GetResult();
                        if (!moreData)
                        {
                            // When there is no more data, the channel is completed and we leave the loop 
                            break;
                        }

                        ProcessAllPendingWork();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, "Intenal error in WorkItemQueue, failed to handle error.");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            CancelCurrentAndClearPending();
            _cts.Dispose();
            _thread?.Join();
        }
    }
}
