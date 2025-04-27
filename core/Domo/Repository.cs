using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Ara3D.Domo
{
    public abstract class Repository<T> : IRepository<T>
        where T: new()
    {
        protected Repository(T value)   
        {
            DefaultValue = value == null ? new T() : value;
        }

        public event EventHandler<RepositoryChangeArgs> RepositoryChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Type ValueType
            => typeof(T);

        public T DefaultValue
        { get; }

        object IRepository.DefaultValue 
            => DefaultValue;

        private IDictionary<Guid, (T, Model<T>)> _dict = new ConcurrentDictionary<Guid, (T, Model<T>)>();

        public void Dispose()
        {
            RepositoryChanged = null;
            CollectionChanged = null;
            Clear();
            _dict = null;
        }

        public void Clear()
        {
            foreach (var v in _dict.Keys.ToArray())
            {
                Delete(v);
            }

            _dict.Clear();
        }

        IModel IRepository.GetModel(Guid modelId)
            => GetModel(modelId);

        object IRepository.GetValue(Guid modelId)
            => GetModel(modelId);

        public void NotifyRepositoryChanged(RepositoryChangeType type, Guid modelId, object newValue, object oldValue)
        {
            var args = new RepositoryChangeArgs
            {
                ChangeType = type,
                ModelId = modelId,
                NewValue = newValue,
                OldValue = oldValue,
                Repository = this,
            };
            RepositoryChanged?.Invoke(this, args);
            switch (type)
            {
                case RepositoryChangeType.ModelAdded:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, oldValue));
                    break;
                case RepositoryChangeType.ModelRemoved:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValue));
                    break;
                case RepositoryChangeType.ModelUpdated:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public bool Update(Guid modelId, Func<T, T> updateFunc)
        {
            var model = GetModel(modelId);
            var oldValue = model.Value;
            var newValue = updateFunc(oldValue);
            if (oldValue.Equals(newValue))
            {
                // When there is no difference in the values there is no need to trigger a change
                return false;
            }
            if (!Validate(newValue))
            {
                // Value is invalid
                return false;
            }
            _dict[modelId] = (newValue, _dict[modelId].Item2);
            model.TriggerChangeNotification();
            NotifyRepositoryChanged(RepositoryChangeType.ModelUpdated, modelId, oldValue, newValue);
            return true;
        }

        public virtual T Create()
            => ForceValid(new T());

        public virtual T ForceValid(T state)
            => state == null ? Create() : state;

        public virtual bool Validate(T state)
            => state != null;

        public bool Validate(object state)
            => Validate((T)state);

        public IModel<T> Add(T state = default)
        {
            var id = Guid.NewGuid();
            state = ForceValid(state);
            if (IsSingleton && _dict.Count != 0)
                throw new Exception("Singleton repository cannot have more than one model");
            var model = new Model<T>(id, this);
            _dict.Add(id, (state, model));
            NotifyRepositoryChanged(RepositoryChangeType.ModelAdded, id, model.Value, null);
            return model;
        }

        public IReadOnlyList<IModel<T>> GetModels()
            => _dict.Values.Select(x => x.Item2).ToList();

        public IModel<T> GetModel(Guid modelId)
            => _dict[modelId].Item2;

        public T GetValue(Guid modelId)
            => _dict[modelId].Item1;

        public bool Update(Guid modelId, Func<object, object> updateFunc)
            => Update(modelId, x => (T)updateFunc(x));

        public IModel Add(object state)
            => Add((T)state);

        public virtual void Delete(Guid id)
        {
            var oldValue = _dict[id].Item1;
            if (IsSingleton)
                throw new Exception("Cannot remove model from Singleton repository");
            _dict[id].Item2.Dispose();
            _dict.Remove(id);
            NotifyRepositoryChanged(RepositoryChangeType.ModelRemoved, id, null, oldValue);
        }

        public bool ModelExists(Guid id)
            => _dict.ContainsKey(id);

        IReadOnlyList<IModel> IRepository.GetModels()
            => GetModels();

        public abstract bool IsSingleton { get; }

        public int Count 
            => _dict.Count;

        public void SetModelValues(IReadOnlyList<T> values)
        {
            // TODO: allow more sophisticated value updating (e.g., using function)
            // TODO: provide a proper bulk notification (only notify once) 
            // TODO: if something fails, roll-back the whole transition (e.g., validate everything first)

            var ids = _dict.Keys.ToList();
            var n = Math.Min(values.Count, ids.Count);
            var i = 0;
            for (; i < n; ++i)
            {
                var i1 = i;
                if (!Update(ids[i], _ => values[i1]))
                    throw new Exception("Updating values failed");
            }

            while (i < values.Count)
            {
                Debug.Assert(i >= ids.Count);
                Add(values[i]);
                i++;
            }

            while (i < ids.Count)
            {
                Debug.Assert(i >= values.Count);
                Delete(ids[i]);
                i++;
            }
        }
    }

    public class AggregateRepository<TModel> : Repository<TModel>, IAggregateRepository<TModel>
        where TModel : new()
    {
        public AggregateRepository(TModel value = default)
            : base(value)
        { }

        public override bool IsSingleton => false;

        public IReadOnlyList<TModel> Values
        {
            get => GetModels().Select(m => m.Value).ToList();
            set => SetModelValues(value);
        }
    }

    public class SingletonRepository<T> : Repository<T>, ISingletonRepository<T>
        where T : new()
    {
        public SingletonRepository(T value = default)
            : base(value)
            => Model = Add(DefaultValue);
        
        public override bool IsSingleton => true;

        public IModel<T> Model { get; }

        public T Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }
    } 
}