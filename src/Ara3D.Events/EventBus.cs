namespace Ara3D.Events;

/// <summary>
/// Thread‑safe, lock‑free (copy‑on‑write) event bus.
/// * Subscribers are stored via <see cref="WeakReference{T}"/> and purged lazily.
/// * Publish is fire‑and‑forget; each handler runs on the thread‑pool.
/// </summary>
public sealed class EventBus : IEventBus
{
    private readonly IEventErrorHandler _errorHandler;

    public EventBus(IEventErrorHandler errorHandler)
        => _errorHandler = errorHandler;

    public void Publish<T>(T evt) where T : IEvent
        => Subscriptions<T>.Publish(evt);

    public void Subscribe<T>(ISubscriber<T> subscriber) where T : IEvent
        => Subscriptions<T>.Add(_errorHandler, subscriber);

    public void Unsubscribe<T>(ISubscriber<T> subscriber) where T : IEvent
        => Subscriptions<T>.Remove(subscriber);

    private static class Subscriptions<T> where T : IEvent
    {
        // copy-on-write array of typed wrappers
        private static volatile SubscriberWrapper<T>[] _subs = [];
        private static readonly object _sync = new();

        public static void Add(IEventErrorHandler handler, ISubscriber<T> sub)
        {
            var wrapper = new SubscriberWrapper<T>(handler, sub);
            lock (_sync)
            {
                var old = _subs;
                var next = new SubscriberWrapper<T>[old.Length + 1];
                Array.Copy(old, next, old.Length);
                next[old.Length] = wrapper;
                _subs = next;
            }
        }

        public static void Remove(ISubscriber<T> subscriber)
        {
            lock (_sync)
            {
                var old = _subs;
                var idx = Array.FindIndex(old, w => w.Matches(subscriber));
                if (idx < 0) return;
                var next = new SubscriberWrapper<T>[old.Length - 1];
                if (idx > 0) Array.Copy(old, 0, next, 0, idx);
                if (idx < old.Length - 1)
                    Array.Copy(old, idx + 1, next, idx, old.Length - idx - 1);
                _subs = next;
            }
        }

        public static void Publish(T evt)
        {
            var list = _subs;  // volatile read
            var deadCount = 0;

            for (var i = 0; i < list.Length; i++)
            {
                if (!list[i].TryInvoke(evt))
                    deadCount++;
            }

            // optional: prune dead subscribers
            if (deadCount > 0)
            {
                lock (_sync)
                {
                    var live = Array.FindAll(_subs, w => w.IsAlive);
                    _subs = live;
                }
            }
        }
    }

    // typed, non-boxing wrapper
    private sealed class SubscriberWrapper<T> where T : IEvent
    {
        private readonly WeakReference<ISubscriber<T>> _weak;
        private readonly IEventErrorHandler _errorHandler;

        public SubscriberWrapper(IEventErrorHandler handler, ISubscriber<T> target)
        {
            _errorHandler = handler;
            _weak = new(target);
        }

        public bool IsAlive => _weak.TryGetTarget(out _);

        public bool Matches(ISubscriber<T> target)
            => _weak.TryGetTarget(out var sub)
            && ReferenceEquals(sub, target);

        public bool TryInvoke(T evt)
        {
            if (!_weak.TryGetTarget(out var sub))
                return false;

            try
            {
                sub.OnEvent(evt);
            }
            catch (Exception ex)
            {
                _errorHandler.OnError(sub, evt, ex);
            }
            return true;
        }
    }
}
