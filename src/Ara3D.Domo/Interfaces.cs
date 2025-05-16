using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;

namespace Ara3D.Domo
{
    /// <summary>
    /// Represents the kind of change that happens when a change event is triggered 
    /// </summary>
    public enum RepositoryChangeType
    {
        RepositoryAdded,
        RepositoryDeleted,
        ModelAdded,
        ModelRemoved,
        ModelUpdated,
    }

    /// <summary>
    /// Contains information provided when a change event is triggered. 
    /// </summary>
    public class RepositoryChangeArgs : EventArgs
    {
        public IRepository Repository { get; set; }
        public Guid ModelId { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public RepositoryChangeType ChangeType { get; set; }
    }

    /// <summary>
    /// Manages a collection of repositories.
    /// When disposed, all repositories are deleted (disposed).
    /// Provides hooks for responding to changes to repositories. 
    /// </summary>
    public interface IRepositoryManager
        : IDisposable
    {
        /// <summary>
        /// Adds a repository to the store. The RepositoryManager is now
        /// responsible for disposing the repository
        /// </summary>
        IRepository AddRepository(IRepository repository);

        /// <summary>
        /// Geta a shallow copy of all of the repositories managed
        /// by the store at the current moment. 
        /// </summary>
        IReadOnlyList<IRepository> GetRepositories();

        /// <summary>
        /// Removes the specified repository from the store, and disposes it. 
        /// </summary>
        void DeleteRepository(IRepository repository);

        /// <summary>
        /// Retrieves a repository based on the type.  
        /// </summary>
        IRepository GetRepository(Type type);

        /// <summary>
        /// Called after a change to a repository 
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanged;
    }

    /// <summary>
    /// A repository is a container for either zero or more domain models (an IAggregateRepository)
    /// or a single domain model (ISingletonRepository).
    /// A repository is responsible for managing the actual state of the domain model, and
    /// supports Create, GetModel, Update, and Delete (CRUD) operations. 
    /// Repositories are stored in a Value Store. A Repository's Guid is a compile-time constant that
    /// defines its identity across processes, and versions. This is useful for serialization
    /// of repositories, and having different versions of a repsitory. 
    /// When disposed, all domain models are disposed.
    /// </summary>
    public interface IRepository 
        : IDisposable, INotifyCollectionChanged
    {
        /// <summary>
        /// The type of the model objects stored in in this particular repository 
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Returns the model stored in the repository
        /// </summary>
        IModel GetModel(Guid modelId);

        /// <summary>
        /// Returns the value stored in the repository. 
        /// </summary>
        object GetValue(Guid modelId);

        /// <summary>
        /// Call this function to attempt a change in the state of particular repository. 
        /// </summary>
        bool Update(Guid modelId, Func<object, object> updateFunc);

        /// <summary>
        /// Returns true if the state is valid, or false otherwise.
        /// </summary>
        bool Validate(object state);

        /// <summary>
        /// Creates a new domain model given the existing state and adds it to the repository.
        /// </summary>
        IModel Add(object state);

        /// <summary>
        /// Deletes the specified domain model.  
        /// </summary>
        void Delete(Guid modelId);

        /// <summary>
        /// Returns all of the managed domain models at the current moment in time. 
        /// </summary>
        IReadOnlyList<IModel> GetModels();

        /// <summary>
        /// Returns true if the model exists, or false otherwise
        /// </summary>
        bool ModelExists(Guid modelId);

        /// <summary>
        /// Called after a change to a repository 
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanged;

        /// <summary>
        /// Removes all of the models from the repository. 
        /// </summary>
        void Clear();

        /// <summary>
        /// Return the number of models
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns a default value
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Returns true if the 
        /// </summary>
        bool IsSingleton { get; }
    }

    /// <summary>
    /// Strongly typed repository. The T object can be any C# type, but it is strongly recommended to be immutable.
    /// Reference between data models should be created via ModelReference classes.
    /// </summary>
    public interface IRepository<TValue>
        : IRepository
    {
        /// <summary>
        /// Returns the model stored in the repository. 
        /// </summary>
        new IModel<TValue> GetModel(Guid modelId);

        /// <summary>
        /// Returns the value stored in the repository. 
        /// </summary>
        new TValue GetValue(Guid modelId);

        /// <summary>
        /// Updates the value for the given model ID
        /// </summary>
        bool Update(Guid modelId, Func<TValue,TValue> updateFunc);

        /// <summary>
        /// Creates a valid version of the value.
        /// </summary>
        TValue ForceValid(TValue value);

        /// <summary>
        /// Returns true if the value is valid or not. 
        /// </summary>
        bool Validate(TValue value);

        /// <summary>
        /// Creates a new valid value.
        /// </summary>
        TValue Create();

        /// <summary>
        /// Adds a new value to the repository and returns the model.
        /// The value is forced to valid.
        /// </summary>
        IModel<TValue> Add(TValue value = default);

        /// <summary>
        /// Returns a list of models 
        /// </summary>
        new IReadOnlyList<IModel<TValue>> GetModels();

        /// <summary>
        /// Returns the default value to construct for values
        /// </summary>
        new TValue DefaultValue { get; }
    }

    /// <summary>
    /// An aggregate repository manages a collection of domain models. 
    /// </summary>
    public interface IAggregateRepository<T> : 
        IRepository<T>
    {
    }

    /// <summary>
    /// In a singleton repository, the Guid of the DomainModel is the same Guid as that of the Repository.
    /// To be informed of changes to an underlying data model subscriptions should be made to the model
    /// itself.
    /// </summary>
    public interface ISingletonRepository<T> 
        : IRepository<T>
    {
        /// <summary>
        /// The domain model associated with the repository 
        /// </summary>
        IModel<T> Model { get; }

        /// <summary>
        /// The value associated with the model.
        /// </summary>
        T Value { get; set; }
    }

    /// <summary>
    /// A Model is a reference to a state within a repository. The value is immutable,
    /// but can be replaced with a new one, triggering a PropertyChanged
    /// event. Model also implement ICustomTypeDescriptor which simplify data binding in Data Grids.
    /// Models support IDynamicMetaObjectProvider, allowing them to be used in a dynamic context. 
    /// The parameter name will always be String.Empty.
    /// This allows Views or View Models to respond to changes in a domain model. 
    /// Domain models can refer 
    /// (a guid) to identify the model across different states.
    /// The state type (T) can be any C# type but is strongly recommended to be immutable.
    /// If the state changes the INotifyPropertyChanged will always be triggered, with a null
    /// parameter name.
    /// This enables domain models to support data binding to views or view models as desired.
    /// When Disposed all events handlers are removed. 
    /// </summary>
    public interface IModel :
        INotifyPropertyChanged, IDisposable, ICustomTypeDescriptor, IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Represents this particular domain model. Is persistent, and does not change
        /// if the underlying value changed. Useful for creating serializable references 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The underlying value or entity of the model. The actual value
        /// is stored in a repository using the Guid as a key. 
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The type of the value or entity.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// The in-memory backing storage for the model values
        /// </summary>
        IRepository Repository { get; }

        /// <summary>
        /// Called by the repository to identify when changes happen.
        /// This invokes the INotifyPropertyChanged.PropertyChanged event
        /// with the parameter name set to string.Empty.
        /// </summary>
        void TriggerChangeNotification();   
    }

    /// <summary>
    /// Type safe model. The type parameter can be a class or struct. It is recommended that the
    /// type parameter is type-safe.
    /// Do not derive your classes from this class. 
    /// </summary>
    public interface IModel<TValue> 
        : IModel
    {
        /// <summary>
        /// The current value associated with the model.
        /// Setting the value, updates the repository. 
        /// </summary>
        new TValue Value { get; set; }
        
        /// <summary>
        /// The repository that houses this value. 
        /// </summary>
        new IRepository<TValue> Repository { get; }
    }
}
