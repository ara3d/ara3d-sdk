using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils
{
    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
                base.Add(key, new List<TValue>());
            this[key].Add(value);
        }

        public MultiDictionary(IEnumerable<(TKey, TValue)> keyValues)
        : base(keyValues
            .GroupBy(kv => kv.Item1)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(kv => kv.Item2).ToList()))
        {
        }

        public MultiDictionary()
        { }
    }
}