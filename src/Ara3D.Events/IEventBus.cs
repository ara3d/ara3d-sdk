namespace Ara3D.Events;

/// <summary>
/// Used to subscribe to, and publish events between services.
/// This decouples event publishers from subscribers.
/// Subscribers are removed automatically when no-longer used,
/// because internally it uses WeakReference
/// </summary>
public interface IEventBus
{
    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(ISubscriber<T> subscriber) where T : IEvent;
    void Unsubscribe<T>(ISubscriber<T> subscriber) where T : IEvent;
}