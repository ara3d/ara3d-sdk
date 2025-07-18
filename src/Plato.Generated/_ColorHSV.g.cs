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
    public partial struct ColorHSV: ICoordinate
    {
        // Fields
        [DataMember] public readonly Angle Hue;
        [DataMember] public readonly Number S;
        [DataMember] public readonly Number V;

        // With functions 
        [MethodImpl(AggressiveInlining)] public ColorHSV WithHue(Angle hue) => new ColorHSV(hue, S, V);
        [MethodImpl(AggressiveInlining)] public ColorHSV WithS(Number s) => new ColorHSV(Hue, s, V);
        [MethodImpl(AggressiveInlining)] public ColorHSV WithV(Number v) => new ColorHSV(Hue, S, v);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public ColorHSV(Angle hue, Number s, Number v) { Hue = hue; S = s; V = v; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static ColorHSV Create(Angle hue, Number s, Number v) => new ColorHSV(hue, s, v);

        // Static default implementation
        public static readonly ColorHSV Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Angle, Number, Number)(ColorHSV self) => (self.Hue, self.S, self.V);
        [MethodImpl(AggressiveInlining)] public static implicit operator ColorHSV((Angle, Number, Number) value) => new ColorHSV(value.Item1, value.Item2, value.Item3);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Angle hue, out Number s, out Number v) { hue = Hue; s = S; v = V;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(ColorHSV other) => Hue.Equals(other.Hue) && S.Equals(other.S) && V.Equals(other.V);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(ColorHSV other) => !Hue.Equals(other.Hue) && S.Equals(other.S) && V.Equals(other.V);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is ColorHSV other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Hue, S, V);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Hue\" = {Hue}, \"S\" = {S}, \"V\" = {V} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class ColorHSVExtensions
    {
    }
}
