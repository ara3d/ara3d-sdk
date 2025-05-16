using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Domo
{
    public static class DomoExtensions
    {
        public static IRepository<T> GetRepository<T>(this IRepositoryManager store)
            => (IRepository<T>)store.GetRepository(typeof(T));

        public static IAggregateRepository<T> GetAggregateRepository<T>(this IRepositoryManager store)
            => (IAggregateRepository<T>)store.GetRepository(typeof(T));

        public static ISingletonRepository<T> GetSingletonRepository<T>(this IRepositoryManager store)
            => (ISingletonRepository<T>)store.GetRepository(typeof(T));

        public static void DeleteAllRepositories(this IRepositoryManager store)
        {
            foreach (var r in store.GetRepositories())
                store.DeleteRepository(r);
        }

        public static void OnModelAdded<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangeType.ModelAdded)
                {
                    var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                    action.Invoke(model);
                }
            };

        public static void OnModelRemoved<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangeType.ModelRemoved)
                {
                    var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                    action.Invoke(model);
                }
            };

        public static void OnModelUpdated<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangeType.ModelUpdated)
                {
                    var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                    action.Invoke(model);
                }
            };

        public static void OnModelChanged<T>(this IRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                action.Invoke(model);
            };

        public static void OnModelsChanged<T>(this IRepository<T> repository, Action<IReadOnlyList<IModel<T>>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                var models = (IReadOnlyList<IModel<T>>)args.Repository.GetModels();
                action.Invoke(models);
            };

        public static IRepository<T> AddTypedRepository<T>(this IRepositoryManager store, IRepository<T> repository) where T: new()
            => (IRepository<T>)store.AddRepository(repository);

        public static IRepository<T> CreateAggregateRepository<T>() where T: new()
            => new AggregateRepository<T>();

        public static IRepository<T> CreateSingletonRepository<T>(T value = default) where T : new()
            => new SingletonRepository<T>(value);

        public static ISingletonRepository<T> AddSingletonRepository<T>(this IRepositoryManager store, T value = default) where T : new()
            => (ISingletonRepository<T>)store.AddTypedRepository<T>(CreateSingletonRepository(value));

        public static IAggregateRepository<T> AddAggregateRepository<T>(this IRepositoryManager store) where T: new()
            => (IAggregateRepository<T>)store.AddTypedRepository(CreateAggregateRepository<T>());

        public static bool Update<T>(this IModel<T> model, Func<T, T> updateFunc)
            => model.Repository.Update(model.Id, updateFunc);

        public static bool Update<T>(this ISingletonRepository<T> repo, Func<T, T> updateFunc)
            => repo.Model.Update(updateFunc);

        public static string ToDebugString(this IModel model)
            => model == null ? "null" : $"{model.Id} {model.Value}";

        public static string GetTypeName(this object x)
            => x?.GetType().Name;

        public static IReadOnlyDictionary<Guid, object> GetModelDictionary(this IRepository r)
            => r.GetModels().ToDictionary(m => m.Id, m => m.Value);

        public static IModel<T> Add<T>(this IRepository<T> repo, T value)
            => repo.Add(value);

        public static IEnumerable<T> GetValues<T>(this IRepository<T> repo)
            => repo.GetModels().Select(m => m.Value);

        public static IEnumerable<object> GetValues(this IRepository repo)
            => repo.GetModels().Select(m => m.Value);

        public static IModel GetSingleModel(this IRepository repo)
            => repo.GetModels()[0];

        public static IModel<T> GetSingleModel<T>(this IRepository<T> repo)
            => repo.GetModels()[0];

        /// <summary>
        /// Returns a copy of the values as mutable dynamic that you can set in a
        /// thread-safe and immutable manner. A copy of the the model is created, and
        /// if a value is changed, then it is committed to the repo. 
        /// </summary>
        public static IReadOnlyList<dynamic> GetDynamicModels(this IRepository repo) 
            => repo.GetModels().ToList();

        /// <summary>
        /// For use with singleton repositories. Returns a copy of the first value as a
        /// mutable dynamic that you can set in a thread-safe and immutable manner.
        /// A copy of the the model is created, and if a value is changed, then it is committed
        /// to the repo. 
        /// </summary>
        public static dynamic GetDynamicModel(this IRepository repo)
            => repo.GetDynamicModels()[0];

        /// <summary>
        /// Returns a model, that you can update dynamically.
        /// Under the hood setting properties commits a copy of the model to
        /// the repository. You can bind this directly to a WPF control if you want. 
        /// </summary>
        public static dynamic AsDynamic(this IModel model)
            => model;

        /// <summary>
        /// Creates a copy of the existing model 
        /// </summary>
        public static IModel<T> Clone<T>(this IModel<T> self)
            => self.Repository.Add(self.Value);

        /// <summary>
        /// Deletes the model from its repository
        /// </summary>
        public static void Delete(this IModel self)
            => self.Repository.Delete(self.Id);
    }
}   