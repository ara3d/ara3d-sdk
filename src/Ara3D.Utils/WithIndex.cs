using System.Runtime.CompilerServices;

namespace Ara3D.Utils;

public readonly record struct WithIndex<T>(T Value, int Index)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out T value, out int index) 
        => (value, index) = (Value, Index);
}