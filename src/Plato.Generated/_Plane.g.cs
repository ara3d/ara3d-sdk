// Autogenerated file: DO NOT EDIT
// Created on 2025-06-07 6:14:52 PM

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using Ara3D.Collections;

namespace Ara3D.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public partial struct Plane: IValue
    {
        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Plane Create(Vector3 normal, Number d) => new Plane(normal, d);

        // Static default implementation
        public static readonly Plane Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Vector3, Number)(Plane self) => (self.Normal, self.D);
        [MethodImpl(AggressiveInlining)] public static implicit operator Plane((Vector3, Number) value) => new Plane(value.Item1, value.Item2);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Vector3 normal, out Number d) { normal = Normal; d = D;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Plane other) => Value.Equals(other.Value);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Plane other) => !Value.Equals(other.Value);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Plane other ? Equals(other) : false;
        [MethodImpl(AggressiveInlining)] public static Boolean operator==(Plane a, Plane b) => a.Equals(b);
        [MethodImpl(AggressiveInlining)] public static Boolean operator!=(Plane a, Plane b) => !a.Equals(b);
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Value.GetHashCode();
        [MethodImpl(AggressiveInlining)] public override string ToString() => Value.ToString();

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class PlaneExtensions
    {
        [MethodImpl(AggressiveInlining)] public static Vector3 Normal(this Plane self) => self.Normal;
        [MethodImpl(AggressiveInlining)] public static Vector3 Normal(this System.Numerics.Plane self) => ((Plane)self).Normal;
        [MethodImpl(AggressiveInlining)] public static Number D(this Plane self) => self.D;
        [MethodImpl(AggressiveInlining)] public static Number D(this System.Numerics.Plane self) => ((Plane)self).D;
        [MethodImpl(AggressiveInlining)] public static Plane WithNormal(this Plane self, Vector3 normal) => self.WithNormal(normal);
        [MethodImpl(AggressiveInlining)] public static Plane WithNormal(this System.Numerics.Plane self, Vector3 normal) => ((Plane)self).WithNormal(normal);
        [MethodImpl(AggressiveInlining)] public static Plane WithD(this Plane self, Number d) => self.WithD(d);
        [MethodImpl(AggressiveInlining)] public static Plane WithD(this System.Numerics.Plane self, Number d) => ((Plane)self).WithD(d);
        [MethodImpl(AggressiveInlining)] public static Plane Normalize(this Plane self) => self.Normalize;
        [MethodImpl(AggressiveInlining)] public static Plane Normalize(this System.Numerics.Plane self) => ((Plane)self).Normalize;
        [MethodImpl(AggressiveInlining)] public static Number Dot(this Plane self, Vector4 value) => self.Dot(value);
        [MethodImpl(AggressiveInlining)] public static Number Dot(this System.Numerics.Plane self, Vector4 value) => ((Plane)self).Dot(value);
        [MethodImpl(AggressiveInlining)] public static Number DotCoordinate(this Plane self, Vector3 value) => self.DotCoordinate(value);
        [MethodImpl(AggressiveInlining)] public static Number DotCoordinate(this System.Numerics.Plane self, Vector3 value) => ((Plane)self).DotCoordinate(value);
        [MethodImpl(AggressiveInlining)] public static Number DotNormal(this Plane self, Vector3 value) => self.DotNormal(value);
        [MethodImpl(AggressiveInlining)] public static Number DotNormal(this System.Numerics.Plane self, Vector3 value) => ((Plane)self).DotNormal(value);
        [MethodImpl(AggressiveInlining)] public static Plane Transform(this Plane self, Quaternion rotation) => self.Transform(rotation);
        [MethodImpl(AggressiveInlining)] public static Plane Transform(this System.Numerics.Plane self, Quaternion rotation) => ((Plane)self).Transform(rotation);
        [MethodImpl(AggressiveInlining)] public static Plane Transform(this Plane self, Matrix4x4 matrix) => self.Transform(matrix);
        [MethodImpl(AggressiveInlining)] public static Plane Transform(this System.Numerics.Plane self, Matrix4x4 matrix) => ((Plane)self).Transform(matrix);
    }
}
