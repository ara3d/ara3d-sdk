namespace Ara3D.Events;

public interface ISubscriber
{ }

/// <summary>
/// Listeners are notified when an event they have subscribed to has been sent.
/// Filtering of events is done by the bus, only subscribed events are ever sent.
/// </summary>
public interface ISubscriber<in T> : ISubscriber
{
    void OnEvent(T evt);
}