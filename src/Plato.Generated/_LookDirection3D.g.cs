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
    public partial struct LookDirection3D: IRotation3D
    {
        // Fields
        [DataMember] public readonly Vector3 Direction;

        // With functions 
        [MethodImpl(AggressiveInlining)] public LookDirection3D WithDirection(Vector3 direction) => new LookDirection3D(direction);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public LookDirection3D(Vector3 direction) { Direction = direction; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static LookDirection3D Create(Vector3 direction) => new LookDirection3D(direction);

        // Static default implementation
        public static readonly LookDirection3D Default = default;

        // Implicit converters to/from single field
        [MethodImpl(AggressiveInlining)] public static implicit operator Vector3(LookDirection3D self) => self.Direction;
        [MethodImpl(AggressiveInlining)] public static implicit operator LookDirection3D(Vector3 value) => new LookDirection3D(value);

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(LookDirection3D other) => Direction.Equals(other.Direction);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(LookDirection3D other) => !Direction.Equals(other.Direction);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is LookDirection3D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Direction);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Direction\" = {Direction} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        // AMBIGUOUS FUNCTIONS 2
        /* Transforms_20.Matrix(r: LookDirection3D): Matrix4x4 [Library]; */
        /* Transforms_20.Matrix(r: IRotation3D): Matrix4x4 [Library]; */
        public Matrix4x4 Matrix { [MethodImpl(AggressiveInlining)] get  => Ara3D.Geometry.Matrix4x4.CreateWorld(((Number)0), this.Direction, Constants.ZAxis3D); } 
public static LookDirection3D Identity { [MethodImpl(AggressiveInlining)] get  => Ara3D.Geometry.Vector3.UnitY; } 
[MethodImpl(AggressiveInlining)] public Point3D Multiply(Point3D v) => this.TransformPoint(v);
[MethodImpl(AggressiveInlining)]  public static Point3D operator *(LookDirection3D x, Point3D v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Vector3 Multiply(Vector3 v) => this.TransformNormal(v);
[MethodImpl(AggressiveInlining)]  public static Vector3 operator *(LookDirection3D x, Vector3 v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Transform3D Multiply(Matrix4x4 m) => this.Compose(m);
[MethodImpl(AggressiveInlining)]  public static Transform3D operator *(LookDirection3D x, Matrix4x4 m) => x.Multiply(m);
        public Transform3D Transform3D { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Transform3D(LookDirection3D x) => x.Transform3D;
        [MethodImpl(AggressiveInlining)] public Point3D TransformPoint(Point3D v) => v.Vector3.Transform(this.Matrix);
[MethodImpl(AggressiveInlining)] public Vector3 TransformNormal(Vector3 v) => v.TransformNormal(this.Matrix);
public Matrix4x4 Matrix4x4 { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Matrix4x4(LookDirection3D t) => t.Matrix4x4;
        public Transform3D Invert { [MethodImpl(AggressiveInlining)] get  => this.Matrix.Invert; } 
[MethodImpl(AggressiveInlining)] public Transform3D Compose(Matrix4x4 m) => this.Matrix.Multiply(m);

        // Unimplemented interface functions
        public Quaternion Quaternion => throw new NotImplementedException();
[MethodImpl(AggressiveInlining)]  public static implicit operator Quaternion(LookDirection3D x) => x.Quaternion;
    }
    // Extension methods for the type
    public static class LookDirection3DExtensions
    {
        [MethodImpl(AggressiveInlining)] public static Quaternion Quaternion(this LookDirection3D x) => x.Quaternion;
    }
}
