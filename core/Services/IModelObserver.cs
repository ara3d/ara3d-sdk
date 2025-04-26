using Ara3D.Domo;
using System.Collections.Generic;

namespace Ara3D.Services
{
    public class RepositoryChangedEvent : IEvent
    {
        public RepositoryChangedEvent(RepositoryChangeArgs args) => Args = args;
        public RepositoryChangeArgs Args { get; }
    }

    public class ModelChangedEvent<T> : RepositoryChangedEvent
    {
        public ModelChangedEvent(RepositoryChangeArgs args) : base(args) { }

        /// <summary>
        /// The current state of models in the repository 
        /// </summary>
        public IReadOnlyList<IModel<T>> Models => ((IRepository<T>)Args.Repository).GetModels();

        /// <summary>
        /// This is the model that was added, changed, or deleted. 
        /// </summary>
        public IModel<T> Model => (IModel<T>)Args.Repository.GetModel(Args.ModelId);
    }

    public interface IModelObserver<T> : ISubscriber<ModelChangedEvent<T>>
    { }
}