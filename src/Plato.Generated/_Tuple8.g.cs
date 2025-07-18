// Autogenerated file: DO NOT EDIT
// Created on 2025-06-07 6:14:52 PM

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using Ara3D.Collections;

namespace Ara3D.Geometry
{
    [DataContract, StructLayout(LayoutKind.Sequential, Pack=1)]
    public partial struct Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>
    {
        // Fields
        [DataMember] public readonly T0 X0;
        [DataMember] public readonly T1 X1;
        [DataMember] public readonly T2 X2;
        [DataMember] public readonly T3 X3;
        [DataMember] public readonly T4 X4;
        [DataMember] public readonly T5 X5;
        [DataMember] public readonly T6 X6;
        [DataMember] public readonly T7 X7;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX0(T0 x0) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(x0, X1, X2, X3, X4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX1(T1 x1) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, x1, X2, X3, X4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX2(T2 x2) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, x2, X3, X4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX3(T3 x3) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, X2, x3, X4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX4(T4 x4) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, X2, X3, x4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX5(T5 x5) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, X2, X3, X4, x5, X6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX6(T6 x6) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, X2, X3, X4, X5, x6, X7);
        [MethodImpl(AggressiveInlining)] public Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> WithX7(T7 x7) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(X0, X1, X2, X3, X4, X5, X6, x7);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Tuple8(T0 x0, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7) { X0 = x0; X1 = x1; X2 = x2; X3 = x3; X4 = x4; X5 = x5; X6 = x6; X7 = x7; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> Create(T0 x0, T1 x1, T2 x2, T3 x3, T4 x4, T5 x5, T6 x6, T7 x7) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(x0, x1, x2, x3, x4, x5, x6, x7);

        // Static default implementation
        public static readonly Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (T0, T1, T2, T3, T4, T5, T6, T7)(Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> self) => (self.X0, self.X1, self.X2, self.X3, self.X4, self.X5, self.X6, self.X7);
        [MethodImpl(AggressiveInlining)] public static implicit operator Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>((T0, T1, T2, T3, T4, T5, T6, T7) value) => new Tuple8<T0, T1, T2, T3, T4, T5, T6, T7>(value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out T0 x0, out T1 x1, out T2 x2, out T3 x3, out T4 x4, out T5 x5, out T6 x6, out T7 x7) { x0 = X0; x1 = X1; x2 = X2; x3 = X3; x4 = X4; x5 = X5; x6 = X6; x7 = X7;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> other) => X0.Equals(other.X0) && X1.Equals(other.X1) && X2.Equals(other.X2) && X3.Equals(other.X3) && X4.Equals(other.X4) && X5.Equals(other.X5) && X6.Equals(other.X6) && X7.Equals(other.X7);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> other) => !X0.Equals(other.X0) && X1.Equals(other.X1) && X2.Equals(other.X2) && X3.Equals(other.X3) && X4.Equals(other.X4) && X5.Equals(other.X5) && X6.Equals(other.X6) && X7.Equals(other.X7);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Tuple8<T0, T1, T2, T3, T4, T5, T6, T7> other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(X0, X1, X2, X3, X4, X5, X6, X7);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"X0\" = {X0}, \"X1\" = {X1}, \"X2\" = {X2}, \"X3\" = {X3}, \"X4\" = {X4}, \"X5\" = {X5}, \"X6\" = {X6}, \"X7\" = {X7} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions

        // Unimplemented interface functions
    }
}
