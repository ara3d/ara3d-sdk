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
    public partial struct Transform3D: ITransform3D
    {
        // Fields
        [DataMember] public readonly Matrix4x4 Matrix;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Transform3D WithMatrix(Matrix4x4 matrix) => new Transform3D(matrix);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Transform3D(Matrix4x4 matrix) { Matrix = matrix; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Transform3D Create(Matrix4x4 matrix) => new Transform3D(matrix);

        // Static default implementation
        public static readonly Transform3D Default = default;

        // Implicit converters to/from single field
        [MethodImpl(AggressiveInlining)] public static implicit operator Matrix4x4(Transform3D self) => self.Matrix;
        [MethodImpl(AggressiveInlining)] public static implicit operator Transform3D(Matrix4x4 value) => new Transform3D(value);

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Transform3D other) => Matrix.Equals(other.Matrix);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Transform3D other) => !Matrix.Equals(other.Matrix);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Transform3D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Matrix);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Matrix\" = {Matrix} }}";

        // Explicit implementation of interfaces by forwarding properties to fields
        Matrix4x4 ITransform3D.Matrix { [MethodImpl(AggressiveInlining)] get => Matrix; }

        // Implemented interface functions
        public static Transform3D Identity { [MethodImpl(AggressiveInlining)] get  => Ara3D.Geometry.Matrix4x4.Identity; } 
[MethodImpl(AggressiveInlining)] public Point3D Multiply(Point3D v) => this.TransformPoint(v);
[MethodImpl(AggressiveInlining)]  public static Point3D operator *(Transform3D x, Point3D v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Vector3 Multiply(Vector3 v) => this.TransformNormal(v);
[MethodImpl(AggressiveInlining)]  public static Vector3 operator *(Transform3D x, Vector3 v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Transform3D Multiply(Matrix4x4 m) => this.Compose(m);
[MethodImpl(AggressiveInlining)]  public static Transform3D operator *(Transform3D x, Matrix4x4 m) => x.Multiply(m);
        [MethodImpl(AggressiveInlining)] public Point3D TransformPoint(Point3D v) => v.Vector3.Transform(this.Matrix);
[MethodImpl(AggressiveInlining)] public Vector3 TransformNormal(Vector3 v) => v.TransformNormal(this.Matrix);
public Matrix4x4 Matrix4x4 { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
public Transform3D Invert { [MethodImpl(AggressiveInlining)] get  => this.Matrix.Invert; } 
[MethodImpl(AggressiveInlining)] public Transform3D Compose(Matrix4x4 m) => this.Matrix.Multiply(m);

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class Transform3DExtensions
    {
    }
}
