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
    public partial struct Pose3D: IRigidTransform3D
    {
        // Fields
        [DataMember] public readonly Translation3D Translation;
        [DataMember] public readonly Rotation3D Rotation;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Pose3D WithTranslation(Translation3D translation) => new Pose3D(translation, Rotation);
        [MethodImpl(AggressiveInlining)] public Pose3D WithRotation(Rotation3D rotation) => new Pose3D(Translation, rotation);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Pose3D(Translation3D translation, Rotation3D rotation) { Translation = translation; Rotation = rotation; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Pose3D Create(Translation3D translation, Rotation3D rotation) => new Pose3D(translation, rotation);

        // Static default implementation
        public static readonly Pose3D Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Translation3D, Rotation3D)(Pose3D self) => (self.Translation, self.Rotation);
        [MethodImpl(AggressiveInlining)] public static implicit operator Pose3D((Translation3D, Rotation3D) value) => new Pose3D(value.Item1, value.Item2);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Translation3D translation, out Rotation3D rotation) { translation = Translation; rotation = Rotation;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Pose3D other) => Translation.Equals(other.Translation) && Rotation.Equals(other.Rotation);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Pose3D other) => !Translation.Equals(other.Translation) && Rotation.Equals(other.Rotation);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Pose3D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Translation, Rotation);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Translation\" = {Translation}, \"Rotation\" = {Rotation} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        public Matrix4x4 Matrix { [MethodImpl(AggressiveInlining)] get  => this.Translation.Matrix.Multiply(this.Rotation.Matrix); } 
public static Pose3D Identity { [MethodImpl(AggressiveInlining)] get  => (Ara3D.Geometry.Translation3D.Identity, Ara3D.Geometry.Rotation3D.Identity); } 
[MethodImpl(AggressiveInlining)] public Point3D Multiply(Point3D v) => this.TransformPoint(v);
[MethodImpl(AggressiveInlining)]  public static Point3D operator *(Pose3D x, Point3D v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Vector3 Multiply(Vector3 v) => this.TransformNormal(v);
[MethodImpl(AggressiveInlining)]  public static Vector3 operator *(Pose3D x, Vector3 v) => x.Multiply(v);
        [MethodImpl(AggressiveInlining)] public Transform3D Multiply(Matrix4x4 m) => this.Compose(m);
[MethodImpl(AggressiveInlining)]  public static Transform3D operator *(Pose3D x, Matrix4x4 m) => x.Multiply(m);
        public Transform3D Transform3D { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Transform3D(Pose3D x) => x.Transform3D;
        [MethodImpl(AggressiveInlining)] public Point3D TransformPoint(Point3D v) => v.Vector3.Transform(this.Matrix);
[MethodImpl(AggressiveInlining)] public Vector3 TransformNormal(Vector3 v) => v.TransformNormal(this.Matrix);
public Matrix4x4 Matrix4x4 { [MethodImpl(AggressiveInlining)] get  => this.Matrix; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Matrix4x4(Pose3D t) => t.Matrix4x4;
        public Transform3D Invert { [MethodImpl(AggressiveInlining)] get  => this.Matrix.Invert; } 
[MethodImpl(AggressiveInlining)] public Transform3D Compose(Matrix4x4 m) => this.Matrix.Multiply(m);

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class Pose3DExtensions
    {
    }
}
