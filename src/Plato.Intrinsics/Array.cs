using System.Collections;
using System.Runtime.CompilerServices;

using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Plato
{
    public readonly struct Array<T> : IArray<T>
    {
        public readonly Func<Integer, T> Func;
        public readonly Integer Count;

        [MethodImpl(AggressiveInlining)]
        public Array(Integer count, Func<Integer, T> func)
        {
            Func = func;
            Count = count;
        }

        [MethodImpl(AggressiveInlining)]
        public T At(Integer index)
            => Func(index);

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(AggressiveInlining)]
            get => At(index);
        }

        T IArray<T>.this[Integer index]
        {
            [MethodImpl(AggressiveInlining)]
            get => Func(index);
        }

        Integer IArray<T>.Count
        {
            [MethodImpl(AggressiveInlining)]
            get => Count;
        }

        int IReadOnlyCollection<T>.Count
        {
            [MethodImpl(AggressiveInlining)]
            get => Count;
        }

        [MethodImpl(AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() 
            => new ArrayEnumerator<T>(this);

        [MethodImpl(AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() 
             => GetEnumerator();
    }
}