namespace Ara3D.Events;

public class Subscriber<T> : ISubscriber<T>
    where T : IEvent
{
    private EventBus _bus { get; }
    private Action<T> _action { get; }
    private bool _once { get; }

    public Subscriber(EventBus bus, Action<T> action, bool once)
    {
        _bus = bus;
        _action = action;
        _once = once;
    }

    public void OnEvent(T evt)
    {
        if (_once) _bus.Unsubscribe(this);
        _action.Invoke(evt);
    }

}