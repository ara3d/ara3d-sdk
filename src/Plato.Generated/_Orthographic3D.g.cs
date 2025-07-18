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
    public partial struct Orthographic3D: ITransform3D
    {
        // Fields
        [DataMember] public readonly Number Width;
        [DataMember] public readonly Number Height;
        [DataMember] public readonly Number Near;
        [DataMember] public readonly Number Far;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Orthographic3D WithWidth(Number width) => new Orthographic3D(width, Height, Near, Far);
        [MethodImpl(AggressiveInlining)] public Orthographic3D WithHeight(Number height) => new Orthographic3D(Width, height, Near, Far);
        [MethodImpl(AggressiveInlining)] public Orthographic3D WithNear(Number near) => new Orthographic3D(Width, Height, near, Far);
        [MethodImpl(AggressiveInlining)] public Orthographic3D WithFar(Number far) => new Orthographic3D(Width, Height, Near, far);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Orthographic3D(Number width, Number height, Number near, Number far) { Width = width; Height = height; Near = near; Far = far; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Orthographic3D Create(Number width, Number height, Number near, Number far) => new Orthographic3D(width, height, near, far);

        // Static default implementation
        public static readonly Orthographic3D Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Number, Number, Number, Number)(Orthographic3D self) => (self.Width, self.Height, self.Near, self.Far);
        [MethodImpl(AggressiveInlining)] public static implicit operator Orthographic3D((Number, Number, Number, Number) value) => new Orthographic3D(value.Item1, value.Item2, value.Item3, value.Item4);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Number width, out Number height, out Number near, out Number far) { width = Width; height = Height; near = Near; far = Far;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Orthographic3D other) => Width.Equals(other.Width) && Height.Equals(other.Height) && Near.Equals(other.Near) && Far.Equals(other.Far);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Orthographic3D other) => !Width.Equals(other.Width) && Height.Equals(other.Height) && Near.Equals(other.Near) && Far.Equals(other.Far);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Orthographic3D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Width, Height, Near, Far);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Width\" = {Width}, \"Height\" = {Height}, \"Near\" = {Near}, \"Far\" = {Far} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        public Matrix4x4 Matrix { [MethodImpl(AggressiveInlining)] get  => Ara3D.Geometry.Matrix4x4.CreateOrthographic(this.Width, this.Height, this.Near, this.Far); } 
[MethodImpl(AggressiveInlining)] public Point3D Multiply(Point3D v) => this.TransformPoint(v);
[MethodImpl(AggressiveInlining)]  public static Point3D operator *(Orthographic3D x, Point3D v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Vector3 Multiply(Vector3 v) => this.TransformNormal(v);
[MethodImpl(AggressiveInlining)]  public static Vector3 operator *(Orthographic3D x, Vector3 v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Transform3D Multiply(Matrix4x4 m) => this.Compose(m);
[MethodImpl(AggressiveInlining)]  public static Transform3D operator *(Orthographic3D x, Matrix4x4 m) => x.Multiply(m);
        public Transform3D Transform3D { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Transform3D(Orthographic3D x) => x.Transform3D;
        [MethodImpl(AggressiveInlining)] public Point3D TransformPoint(Point3D v) => v.Vector3.Transform(this.Matrix);
[MethodImpl(AggressiveInlining)] public Vector3 TransformNormal(Vector3 v) => v.TransformNormal(this.Matrix);
public Matrix4x4 Matrix4x4 { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Matrix4x4(Orthographic3D t) => t.Matrix4x4;
        public Transform3D Invert { [MethodImpl(AggressiveInlining)] get  => this.Matrix.Invert; } 
[MethodImpl(AggressiveInlining)] public Transform3D Compose(Matrix4x4 m) => this.Matrix.Multiply(m);

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class Orthographic3DExtensions
    {
    }
}
