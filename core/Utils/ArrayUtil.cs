using System.Collections.Generic;

namespace Ara3D.Utils
{
    public static class ArrayUtil
    {
        public static IList<T> Swap<T>(this IList<T> self, int i, int j)
        {
            (self[i], self[j]) = (self[j], self[i]);
            return self;
        }
    }
}