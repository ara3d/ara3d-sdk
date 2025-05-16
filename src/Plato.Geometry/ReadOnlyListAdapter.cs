using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Plato.Geometry
{
    public class ReadOnlyListAdapter<T> : IArray<T> 
    {
        public readonly IReadOnlyList<T> List;

        [MethodImpl(AggressiveInlining)]
        public ReadOnlyListAdapter(IReadOnlyList<T> list)
        {
            List = list;
        }

        [MethodImpl(AggressiveInlining)]
        public T At(Integer index)
            => List[index];

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(AggressiveInlining)]
            get => List[index];
        }

        T IArray<T>.this[Integer index]
        {
            [MethodImpl(AggressiveInlining)]
            get => List[index];
        }

        Integer IArray<T>.Count
        {
            [MethodImpl(AggressiveInlining)]
            get => List.Count;
        }

        int IReadOnlyCollection<T>.Count
        {
            [MethodImpl(AggressiveInlining)]
            get => List.Count;
        }

        [MethodImpl(AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
            => List.GetEnumerator();

        [MethodImpl(AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
