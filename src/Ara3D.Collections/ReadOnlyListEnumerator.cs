using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Collections
{
    public class ReadOnlyListEnumerator<T> : IEnumerator<T>
    {
        public readonly System.Collections.Generic.IReadOnlyList<T> Array;
        public int Index = -1;

        [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyListEnumerator(System.Collections.Generic.IReadOnlyList<T> array)
            => Array = array;

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[Index];
        }

        object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => ++Index < Array.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => Index = -1;
    }
}