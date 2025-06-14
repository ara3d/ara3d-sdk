﻿using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Collections.wip
{
    /// <summary>
    /// Lookup table: mapping from a key to some value.
    /// </summary>
    public interface ILookup<TKey, TValue>
    {
        IReadOnlyList<TKey> Keys { get; }
        IReadOnlyList<TValue> Values { get; }
        bool Contains(TKey key);
        TValue this[TKey key] { get; }
    }

    public class EmptyLookup<TKey, TValue> : ILookup<TKey, TValue>
    {
        public IReadOnlyList<TKey> Keys => LinqArray.Empty<TKey>();
        public IReadOnlyList<TValue> Values => LinqArray.Empty<TValue>();
        public bool Contains(TKey key) => false;
        public TValue this[TKey key] => default;
    }

    public class LookupFromDictionary<TKey, TValue> : ILookup<TKey, TValue>
    {
        public IDictionary<TKey, TValue> Dictionary;
        private TValue _default;

        public LookupFromDictionary(IDictionary<TKey, TValue> d = null, TValue defaultValue = default)
        {
            Dictionary = d ?? new Dictionary<TKey, TValue>();
            _default = defaultValue;
            Keys = d.Keys.ToList();
            Values = d.Values.ToList();
        }

        public IReadOnlyList<TKey> Keys { get; }
        public IReadOnlyList<TValue> Values { get; }
        public TValue this[TKey key] => Contains(key) ? Dictionary[key] : _default;
        public bool Contains(TKey key) => Dictionary.ContainsKey(key);
    }

    public class LookupFromArray<TValue> : ILookup<int, TValue>
    {
        private IReadOnlyList<TValue> _readOnlyList;

        public LookupFromArray(IReadOnlyList<TValue> xs)
        {
            _readOnlyList = xs;
            Keys = _readOnlyList.Indices();
            Values = _readOnlyList;
        }

        public IReadOnlyList<int> Keys { get; }
        public IReadOnlyList<TValue> Values { get; }
        public TValue this[int key] => _readOnlyList[key];
        public bool Contains(int key) => key >= 0 && key <= _readOnlyList.Count;
    }

    public static class LookupExtensions
    {
        public static ILookup<TKey, TValue> ToLookup<TKey, TValue>(this IDictionary<TKey, TValue> d, TValue defaultValue = default)
            => new LookupFromDictionary<TKey, TValue>(d, defaultValue);

        public static TValue GetOrDefault<TKey, TValue>(this ILookup<TKey, TValue> lookup, TKey key)
            => lookup.Contains(key) ? lookup[key] : default;

    }
}
