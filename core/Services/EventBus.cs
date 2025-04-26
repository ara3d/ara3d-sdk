using System;
using System.Collections.Concurrent;
using Ara3D.Utils;

namespace Ara3D.Services
{
    /// <summary>
    /// Event types should derive from this class. An event is a data packet
    /// broadcast for anyone listening.  
    /// </summary>
    public interface IEvent
    { }

    /// <summary>
    /// Listeners are notified when an event they have subscribed to has been sent.
    /// Filtering of events is done by the bus, only subscribed events are ever sent.
    /// Listeners inform when they are being disposed, so that they can be detached.  
    /// </summary>
    public interface ISubscriber<in T> 
    {
        void OnEvent(T evt);
    }

    /// <summary>
    /// Used to subscribe to, and publish events between services.
    /// This decouples event publishers from subscribers.
    /// Subscribers are removed automatically when no-longer used.
    /// </summary>
    public interface IEventBus
    {
        void Publish<T>(T evt) where T : IEvent;
        void Subscribe<T>(ISubscriber<T> subscriber) where T : IEvent;
    }

    /// <summary>
    /// The event bus provides a type-safe and loosely coupled way for events (notifications/message) to
    /// be propagated to observers. The synchronizer object is used to assure that notifications happen on
    /// the correct thread (e.g. the correct UI thread). 
    /// </summary>
    public class EventBus : IEventBus
    {
        public readonly ConcurrentDictionary<Type, ConcurrentSet<WeakReference>> Subscribers 
            = new ConcurrentDictionary<Type, ConcurrentSet<WeakReference>>();

        private readonly Synchronizer _synchronizer;

        public EventBus(Synchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public void Publish<T>(T evt) where T: IEvent
        {
            if (!Subscribers.TryGetValue(typeof(T), out var subscribers)) 
                return;
            RemoveDeadReferences(subscribers);
            foreach (var s in subscribers.Values)
                if (s.Target is ISubscriber<T> subscriber)
                    _synchronizer.Invoke(() => subscriber.OnEvent(evt));
        }

        public void Subscribe<T>(ISubscriber<T> subscriber) where T : IEvent
        {
            var subscribers = Subscribers.GetOrAdd(typeof(T), _ => new ConcurrentSet<WeakReference>());
            RemoveDeadReferences(subscribers);
            subscribers.Add(new WeakReference(subscriber));
        }

        private static void RemoveDeadReferences(ConcurrentSet<WeakReference> set)
        {
            foreach (var weakReference in set.Values)
                if (!weakReference.IsAlive)
                    set.Remove(weakReference);
        }
    }
}
