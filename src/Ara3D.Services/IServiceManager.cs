using Ara3D.Domo;
using System.Collections.Generic;
using Ara3D.Events;

namespace Ara3D.Services
{
    /// <summary>
    /// This is the interface that bridges services, repositories, and events.
    /// In effect this is the infrastructure of an application.
    /// For simplicity, services and repositories can be added, but never removed.
    /// </summary>
    public interface IServiceManager : IRepositoryManager
    {
        IReadOnlyList<IService> GetServices();
        void AddService(IService service);
        void AddRepository(IRepository repository);
        IEventBus EventBus { get; } 
    }
}
