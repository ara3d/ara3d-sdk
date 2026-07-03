using System.Collections.Generic;
using Ara3D.Events;

namespace Ara3D.Services
{
    /// <summary>
    /// Application infrastructure: services and an event bus.
    /// </summary>
    public interface IServiceManager
    {
        IReadOnlyList<IService> GetServices();
        void AddService(IService service);
        IEventBus EventBus { get; }
    }
}
