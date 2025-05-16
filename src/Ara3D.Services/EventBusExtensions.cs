using Ara3D.Domo;
using System;

namespace Ara3D.Services
{
    public static class EventBusExtensions
    {
        public static void AddRepositoryAsPublisher(this IEventBus bus, IRepository repo)
        {
            var t = repo.ValueType;
            var m = typeof(EventBusExtensions).GetMethod(nameof(AddTypedRepositoryAsPublisher));
            var gm = m.MakeGenericMethod(t);
            gm.Invoke(m, new object[] { bus, repo });
        }

        // Invoked via Reflection. 
        // ReSharper disable once UnusedMember.Global
        public static void AddTypedRepositoryAsPublisher<T>(this IEventBus bus, IRepository<T> repo)
        {
            repo.RepositoryChanged += (_, args) => bus.Publish(new ModelChangedEvent<T>(args));
        }

        /// <summary>
        /// Given a dynamic object, which potentially implements many ISubscriber interfaces,
        /// connect up to it, and inform it when the object happens first.  
        /// </summary>
        public static void AddSubscriberUsingReflection(this IEventBus bus, object subscriber)
        {
            foreach (var iface in subscriber.GetType().GetInterfaces())
            {
                if (!iface.IsGenericType)
                    continue;
                if (iface.GetGenericTypeDefinition() != typeof(ISubscriber<>))
                    continue;

                var modelType = iface.GenericTypeArguments[0];
                var constructedSubscribeMethod = bus.GetType().GetMethod("Subscribe")?.MakeGenericMethod(modelType);
                if (constructedSubscribeMethod == null)
                    throw new Exception("Internal error: could not find or construct subscriber method");

                constructedSubscribeMethod.Invoke(bus, new[] { subscriber });
            }
        }
    }
}