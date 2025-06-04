using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ara3D.Collections
{
    /// <summary>
    /// Implements an IArray via a function and a count.
    /// </summary>
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        public readonly Func<int, T> Function;

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public T this[int n]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Function(n);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyList(int count, Func<int, T> function)
            => (Count, Function) = (count, function);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() 
            => new ReadOnlyListEnumerator<T>(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public ReadOnlyList<T> Default =>
            new ReadOnlyList<T>(0, _ => default(T));
    }
}