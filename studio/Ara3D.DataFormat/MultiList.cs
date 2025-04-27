using System.Collections;
using System.Runtime.CompilerServices;

namespace Ara3D.Data
{


    public interface IMultiList<T0, T1> : IReadOnlyList<(T0, T1)>
    {
        IReadOnlyList<T0> Items0 { get; }
        IReadOnlyList<T1> Items1 { get; }
    }

    public class MultiList<T0, T1> : IMultiList<T0, T1>
    {
        public IReadOnlyList<T0> Items0 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
        }
        public IReadOnlyList<T1> Items1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
        }
        public int Count 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get; 
        }
        
        public (T0, T1) this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Items0[index], Items1[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MultiList(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1)
        {
            if (items0.Count != items1.Count)
                throw new Exception("Both lists must have the same length");
            Items0 = items0;
            Items1 = items1;
            Count = items0.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(T0, T1)> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();        
    }

    public interface IMultiList<T0, T1, T2> : IReadOnlyList<(T0, T1, T2)>
    {
        IReadOnlyList<T0> Items0 { get; }
        IReadOnlyList<T1> Items1 { get; }
        IReadOnlyList<T2> Items2 { get; }
    }

    public class MultiList<T0, T1, T2> : IMultiList<T0, T1, T2>
    {
        public IReadOnlyList<T0> Items0
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public IReadOnlyList<T1> Items1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public IReadOnlyList<T2> Items2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public (T0, T1, T2) this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Items0[index], Items1[index], Items2[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MultiList(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1, IReadOnlyList<T2> items2)
        {
            if (items0.Count != items1.Count)
                throw new Exception("All lists must have the same length");
            if (items0.Count != items2.Count)
                throw new Exception("All lists must have the same length");
            Items0 = items0;
            Items1 = items1;
            Items2 = items2;
            Count = items0.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(T0, T1, T2)> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IMultiList<T0, T1, T2, T3> : IReadOnlyList<(T0, T1, T2, T3)>
    {
        IReadOnlyList<T0> Items0 { get; }
        IReadOnlyList<T1> Items1 { get; }
        IReadOnlyList<T2> Items2 { get; }
        IReadOnlyList<T3> Items3 { get; }
    }

    public class MultiList<T0, T1, T2, T3> : IMultiList<T0, T1, T2, T3>
    {
        public IReadOnlyList<T0> Items0
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public IReadOnlyList<T1> Items1
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public IReadOnlyList<T2> Items2
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public IReadOnlyList<T3> Items3
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public (T0, T1, T2, T3) this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Items0[index], Items1[index], Items2[index], Items3[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MultiList(IReadOnlyList<T0> items0, IReadOnlyList<T1> items1, IReadOnlyList<T2> items2, IReadOnlyList<T3> items3)
        {
            if (items0.Count != items1.Count)
                throw new Exception("All lists must have the same length");
            if (items0.Count != items2.Count)
                throw new Exception("All lists must have the same length");
            if (items0.Count != items3.Count)
                throw new Exception("All lists must have the same length");
            Items0 = items0;
            Items1 = items1;
            Items2 = items2;
            Items3 = items3;
            Count = items0.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<(T0, T1, T2, T3)> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}