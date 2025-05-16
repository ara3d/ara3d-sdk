using System.Collections;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Plato
{
    public readonly struct Array2D<T> : IArray2D<T>
    {
        public Integer Count { get; }
        public Integer NumColumns { get; }
        public Integer NumRows { get; }
        public readonly Func<Integer, Integer, T> Func;
        
        [MethodImpl(AggressiveInlining)]
        public Array2D(Integer numColumns, Integer numRows, Func<Integer, Integer, T> func)
        {
            Func = func;
            NumColumns = numColumns;
            NumRows = numRows;
            Count = numColumns * numRows;
        }

        [MethodImpl(AggressiveInlining)]
        public T At(Integer col, Integer row)
            => this[col, row];

        [MethodImpl(AggressiveInlining)]
        public T At(Integer index)
            => Func(index / NumColumns, index % NumColumns);

        T IReadOnlyList<T>.this[int index]
        {
            [MethodImpl(AggressiveInlining)]
            get => At(index);
        }

        T IArray<T>.this[Integer index]
        {
            [MethodImpl(AggressiveInlining)]
            get => Func(index / NumColumns, index % NumColumns);
        }

        public T this[Integer col, Integer row]
        {
            [MethodImpl(AggressiveInlining)]
            get => Func(col, row);
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