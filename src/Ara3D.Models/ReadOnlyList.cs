using System.Collections;
using System.Runtime.CompilerServices;
using Plato;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Ara3D.Models
{
    [method: MethodImpl(AggressiveInlining)]
    public class ReadOnlyList<T>(Integer count, Func<Integer, T> func) : IReadOnlyList<T>
    {
        public readonly Func<Integer, T> Func = func;
        public readonly int Count = count;

        [MethodImpl(AggressiveInlining)]
        public T At(Integer index)
            => Func(index);

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(AggressiveInlining)]
            get => At(index);
        }

        public T this[Integer index]
        {
            [MethodImpl(AggressiveInlining)]
            get => Func(index);
        }

        int IReadOnlyCollection<T>.Count
        {
            [MethodImpl(AggressiveInlining)]
            get => Count;
        }

        [MethodImpl(AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
            => new ReadOnlyListEnumerator<T>(this);

        [MethodImpl(AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}