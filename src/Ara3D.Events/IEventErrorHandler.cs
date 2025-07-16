namespace Ara3D.Events;

public interface IEventErrorHandler
{
    void OnError(ISubscriber sub, IEvent ev, Exception ex);
}