using System.Collections.Generic;
using Ara3D.Domo;
using Ara3D.Utils;

namespace Ara3D.Services
{
    public class ServiceRegisteredEvent : IEvent
    {
        public IService Service { get; }
        public ServiceRegisteredEvent(IService service)
            => Service = service;
    }

    /// <summary>
    /// Applications needs a Synchronization Context to work. 
    /// This means that it has to be hosted in a Window: The synchronization context is null otherwise.  
    /// </summary>
    public sealed class ServiceManager : IServiceManager
    {
        private readonly List<IService> _services = new List<IService>();
        private readonly List<IRepository> _repositories = new List<IRepository>();

        public IReadOnlyList<IService> GetServices() => _services;
        public IReadOnlyList<IRepository> GetRepositories() => _repositories;
        
        public IEventBus EventBus { get; } 
        public Synchronizer Synchronizer { get; } 

        public ServiceManager()
        {
            Synchronizer = Synchronizer.Create();
            EventBus = new EventBus(Synchronizer);
        }

        public void AddService(IService service) 
        {
            _services.Add(service);
            EventBus.Publish(new ServiceRegisteredEvent(service));
        }

        public void AddRepository(IRepository repository) 
        {
            _repositories.Add(repository);
            EventBus.AddRepositoryAsPublisher(repository);
            EventBus.Publish(new RepositoryChangedEvent(new RepositoryChangeArgs()
            {
                Repository = repository,
                ChangeType = RepositoryChangeType.RepositoryAdded,
            }));
        }
    }
}
