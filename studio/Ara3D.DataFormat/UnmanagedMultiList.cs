using System.Runtime.CompilerServices;

namespace Ara3D.Data
{
    public class UnmanagedMultiList<T0, T1>
        where T0 : unmanaged
        where T1 : unmanaged
    {
        public readonly UnmanagedList<T0> Items0 = new();
        public readonly UnmanagedList<T1> Items1 = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T0 x, T1 y)
        {
            Items0.Add(x);
            Items1.Add(y);
        }

        public void Dispose()
        {
            Items0.Dispose();
            Items1.Dispose();
        }

        public (T0, T1) this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Items0[index], Items1[index]);
        }

        public long Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Items0.Count;
        }
    }
}