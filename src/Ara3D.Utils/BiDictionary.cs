using System;
using System.Collections;
using System.Collections.Generic;

namespace Ara3D.Utils
{
    public class BiDictionary<TKey, TValue> 
        : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly Dictionary<TKey, TValue> _forward = new();
        private readonly Dictionary<TValue, TKey> _reverse = new();

        public int Count => _forward.Count;

        public IEnumerable<TKey> Keys => _forward.Keys;
        public IEnumerable<TValue> Values => _forward.Values;

        public TValue this[TKey key]
        {
            get => _forward[key];
            set => AddOrUpdate(key, value);
        }

        public TKey GetKey(TValue value) 
            => _reverse[value];
        
        public bool TryGetValue(TKey key, out TValue value) 
            => _forward.TryGetValue(key, out value!);
       
        public bool TryGetKey(TValue value, out TKey key) 
            => _reverse.TryGetValue(value, out key!);

        public void Add(TKey key, TValue value)
        {
            if (_forward.ContainsKey(key))
                throw new ArgumentException($"Duplicate key: {key}");
            if (_reverse.ContainsKey(value))
                throw new ArgumentException($"Duplicate value: {value}");
            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            if (_forward.TryGetValue(key, out var oldValue))
                _reverse.Remove(oldValue);
            if (_reverse.TryGetValue(value, out var oldKey))
                _forward.Remove(oldKey);
            _forward[key] = value;
            _reverse[value] = key;
        }

        public bool RemoveByKey(TKey key)
        {
            if (!_forward.Remove(key, out var value))
                return false;
            _reverse.Remove(value);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if (!_reverse.Remove(value, out var key))
                return false;
            _forward.Remove(key);
            return true;
        }

        public bool ContainsKey(TKey key) 
            => _forward.ContainsKey(key);
        
        public bool ContainsValue(TValue value) 
            => _reverse.ContainsKey(value);

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() 
            => _forward.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
    }
}
