using System;

namespace Ara3D.Collections
{
    public static class MapExtensions
    {
        public static IMap<T1, T3> Select<T1, T2, T3>(this IMap<T1, T2> self, Func<T2, T3> func)
            => new Map<T1, T3>(x => func(self[x]));
    }
}