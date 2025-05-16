using Ara3D.Domo;
using Ara3D.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Ara3D.Services
{
    public interface ISingletonModelBackedService<T> : IService, INotifyPropertyChanged
    {
        ISingletonRepository<T> Repository { get; }
        IModel<T> Model { get; }
    }

    public interface IAggregateModelBackedService<T> : IService, INotifyCollectionChanged
    {
        IAggregateRepository<T> Repository { get; }
        IReadOnlyList<IModel<T>> Models { get; }
    }

    /// <summary>
    /// A service with a singleton repository.
    /// </summary>
    public class SingletonModelBackedService<TModel> : BaseService, ISingletonModelBackedService<TModel>
        where TModel : new()
    {
        public SingletonModelBackedService(IServiceManager app)
            : base(app)
        {
            Repository = new SingletonRepository<TModel>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        protected virtual void OnRepositoryChanged(object sender, RepositoryChangeArgs e)
        { }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Model.PropertyChanged += value;
            remove => Model.PropertyChanged -= value;
        }

        public ISingletonRepository<TModel> Repository { get; }
        public IModel<TModel> Model => Repository.Model;

        public TModel Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }
    }

    /// <summary>
    /// A service with an aggregate repository 
    /// </summary>
    public class AggregateModelBackedService<TModel> : BaseService, IAggregateModelBackedService<TModel>
        where TModel : new()
    {
        public AggregateModelBackedService(IServiceManager app)
            : base(app)
        {
            Repository = new AggregateRepository<TModel>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Repository.CollectionChanged += value;
            remove => Repository.CollectionChanged -= value;
        }

        protected virtual void OnRepositoryChanged(object sender, RepositoryChangeArgs e)
        { }

        public IAggregateRepository<TModel> Repository { get; }
        public IReadOnlyList<IModel<TModel>> Models => Repository.GetModels();
    }

    public class BaseService : IService
    {
        public Dictionary<string, INamedCommand> CommandDictionary = new Dictionary<string, INamedCommand>();

        public INamedCommand RegisterCommand(Delegate execute, Delegate canExecute, IRepository repository)
        {
            var r = new NamedCommand(execute, canExecute);
            CommandDictionary.Add(r.Name, r);

            if (repository != null)
                repository.RepositoryChanged += (_1, _2) => r.NotifyCanExecuteChanged();

            return r;
        }

        public INamedCommand GetCommand(string name)
            => CommandDictionary[name];

        public IReadOnlyList<INamedCommand> Commands 
            => CommandDictionary.Values.ToList();

        protected BaseService(IServiceManager app)
        {
            App = app;
            app.AddService(this);
        }

        public IServiceManager App { get; }

        public virtual void Dispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
            Disposing = null;
        }

        public event EventHandler Disposing;
    }
}