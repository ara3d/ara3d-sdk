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
    public partial struct Cone: ISolid
    {
        // Fields
        [DataMember] public readonly Number Height;
        [DataMember] public readonly Number Radius;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Cone WithHeight(Number height) => new Cone(height, Radius);
        [MethodImpl(AggressiveInlining)] public Cone WithRadius(Number radius) => new Cone(Height, radius);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Cone(Number height, Number radius) { Height = height; Radius = radius; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Cone Create(Number height, Number radius) => new Cone(height, radius);

        // Static default implementation
        public static readonly Cone Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Number, Number)(Cone self) => (self.Height, self.Radius);
        [MethodImpl(AggressiveInlining)] public static implicit operator Cone((Number, Number) value) => new Cone(value.Item1, value.Item2);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Number height, out Number radius) { height = Height; radius = Radius;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Cone other) => Height.Equals(other.Height) && Radius.Equals(other.Radius);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Cone other) => !Height.Equals(other.Height) && Radius.Equals(other.Radius);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Cone other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Height, Radius);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Height\" = {Height}, \"Radius\" = {Radius} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions

        // Unimplemented interface functions
        [MethodImpl(AggressiveInlining)] public Point3D Eval(Vector2 t) => throw new NotImplementedException();
public Boolean ClosedX => throw new NotImplementedException();
public Boolean ClosedY => throw new NotImplementedException();
}
    // Extension methods for the type
    public static class ConeExtensions
    {
        [MethodImpl(AggressiveInlining)] public static Point3D Eval(this Cone x, Vector2 t) => x.Eval(t);
        [MethodImpl(AggressiveInlining)] public static Boolean ClosedX(this Cone x) => x.ClosedX;
        [MethodImpl(AggressiveInlining)] public static Boolean ClosedY(this Cone x) => x.ClosedY;
    }
}
