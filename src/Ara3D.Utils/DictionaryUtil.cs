using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils
{
    /// <summary>
    /// Helper functions for working with dictionary classes,
    /// in particular immutable dictionaries, and enumerable key-value pairs.
    /// Note that both IDictionary and IReadOnlyDictionary support IEnumerable&lt;KeyValuePair>.
    /// NOTE: KeyValuePair is so unfortunate, I wish it was a value tuple. 
    /// </summary>
    public static class DictionaryUtil
    {
        public static IReadOnlyDictionary<TKey, TValue> Remove<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> self, TKey key)
            => self.Where(kv => !kv.Key.Equals(key)).ToDictionary(kv => kv.Key, kv => kv.Value);

        public static IReadOnlyDictionary<TKey, TValue> Add<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> self,
            TKey key, TValue value)
            => self.Append(new KeyValuePair<TKey, TValue>(key, value)).ToDictionary(kv => kv.Key, kv => kv.Value);

        public static IReadOnlyDictionary<TKey, TValue> Update<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> self, TKey key, TValue value)
            => self.ToDictionary(kv => kv.Key, kv => kv.Key.Equals(key) ? value : kv.Value);

        public static IReadOnlyDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> self,
            TKey key, TValue value)
            => self.ContainsKey(key) ? self.Update(key, value) : self.Add(key, value);

        public static IReadOnlyDictionary<TKey, TValue> Clear<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> self)
            => new Dictionary<TKey, TValue>();

        public static IDictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> self, IEnumerable<KeyValuePair<TKey, TValue>> other)
            => self.ToDictionary().AddOrUpdate(other);

        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> other)
        {
            foreach (var kv in other)
                self[kv.Key] = kv.Value;
            return self;
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> keyValues)
            => keyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}