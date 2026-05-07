using System.Collections.Generic;

namespace Ara3D.Utils;

public class CountedSet<T> : Dictionary<T, int>
{
    public bool Contains(T x)
        => ContainsKey(x);

    public int IndexOf(T x)
    {
        if (TryGetValue(x, out var tmp))
            return tmp;
        return -1;
    }

    public void Add(T key)
    {
        if (TryGetValue(key, out var val))
            this[key] = val + 1; else
            Add(key, 0);
    }
}