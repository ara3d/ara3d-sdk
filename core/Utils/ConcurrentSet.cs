using System.Collections.Concurrent;
using System.Linq;

namespace Ara3D.Utils
{
    /// <summary>
    /// A very simple concurrent set used by the Event bus.
    /// Surprisingly absent from C#
    /// </summary>
    public class ConcurrentSet<T>
    {
        private readonly ConcurrentDictionary<T, bool> _dictionary = new ConcurrentDictionary<T, bool>();

        public void Add(T x)
            => _dictionary.TryAdd(x, true);

        public void Remove(T x)
            => _dictionary.TryRemove(x, out _);

        public T[] Values 
            => _dictionary.Keys.ToArray();
    }
}