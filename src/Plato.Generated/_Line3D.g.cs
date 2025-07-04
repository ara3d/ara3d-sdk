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
    public partial struct Line3D: IDeformable3D<Line3D>, IInterval<Point3D>, IArrayLike<Line3D, Point3D>
    {
        // Fields
        [DataMember] public readonly Point3D A;
        [DataMember] public readonly Point3D B;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Line3D WithA(Point3D a) => new Line3D(a, B);
        [MethodImpl(AggressiveInlining)] public Line3D WithB(Point3D b) => new Line3D(A, b);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Line3D(Point3D a, Point3D b) { A = a; B = b; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Line3D Create(Point3D a, Point3D b) => new Line3D(a, b);

        // Static default implementation
        public static readonly Line3D Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Point3D, Point3D)(Line3D self) => (self.A, self.B);
        [MethodImpl(AggressiveInlining)] public static implicit operator Line3D((Point3D, Point3D) value) => new Line3D(value.Item1, value.Item2);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Point3D a, out Point3D b) { a = A; b = B;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Line3D other) => A.Equals(other.A) && B.Equals(other.B);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Line3D other) => !A.Equals(other.A) && B.Equals(other.B);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Line3D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(A, B);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"A\" = {A}, \"B\" = {B} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // IArrayLike predefined functions
        public Integer NumComponents { [MethodImpl(AggressiveInlining)] get => 2; }
        public IReadOnlyList<Point3D> Components { [MethodImpl(AggressiveInlining)] get => Intrinsics.MakeArray<Point3D>(A, B); }
        [MethodImpl(AggressiveInlining)] public static Line3D CreateFromComponents(IReadOnlyList<Point3D> numbers) => new Line3D(numbers[0], numbers[1]);

        [MethodImpl(AggressiveInlining)] public static Line3D CreateFromComponent(Point3D x) => new Line3D(x, x);

        // Implemented interface functions
        public Number Length { [MethodImpl(AggressiveInlining)] get  => this.B.Subtract(this.A).Length; } 
public Vector3 Direction { [MethodImpl(AggressiveInlining)] get  => this.B.Subtract(this.A); } 
public Ray3D Ray3D { [MethodImpl(AggressiveInlining)] get  => (this.A, this.Direction.Normalize); } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Ray3D(Line3D x) => x.Ray3D;
        // AMBIGUOUS FUNCTIONS 3
        /* Geometry_14.Reverse(x: Line3D): Line3D [Library]; */
        /* IInterval_16.Reverse(x: IInterval<$T>): IInterval<$T> [Library]; */
        /* ArrayLibrary_3.Reverse(xs: IArrayLike<$T>): IArrayLike<$T> [Library]; */
        public Line3D Reverse { [MethodImpl(AggressiveInlining)] get  => (this.B, this.A); } 
public Bounds3D Bounds3D { [MethodImpl(AggressiveInlining)] get  => (this.A.Min(this.B), this.A.Max(this.B)); } 
[MethodImpl(AggressiveInlining)]  public static implicit operator Bounds3D(Line3D x) => x.Bounds3D;
        public Point3D Start { [MethodImpl(AggressiveInlining)] get  => this.A; } 
public Point3D End { [MethodImpl(AggressiveInlining)] get  => this.B; } 
// AMBIGUOUS FUNCTIONS 2
        /* Geometry_14.Center(x: Line3D): Point3D [Library]; */
        /* IInterval_16.Center(x: IInterval<$T>): $T [Library]; */
        public Point3D Center { [MethodImpl(AggressiveInlining)] get  => this.A.Average(this.B); } 
[MethodImpl(AggressiveInlining)] public Point3D Eval(Number t) => this.A.Lerp(this.B, t);
public IReadOnlyList<Point3D> Points { [MethodImpl(AggressiveInlining)] get  => Intrinsics.MakeArray(this.A, this.B); } 
[MethodImpl(AggressiveInlining)] public Line3D Deform(System.Func<Point3D, Point3D> f) => (f.Invoke(this.A), f.Invoke(this.B));
[MethodImpl(AggressiveInlining)] public Line3D Transform(Transform3D t){
            var _var59 = t;
            return this.Deform((p)  => p.Vector3.Transform(_var59.Matrix));
        }

[MethodImpl(AggressiveInlining)] public Line3D Scale(Vector3 v) => this.Transform(new Scaling3D(v));
[MethodImpl(AggressiveInlining)] public Line3D Scale(Number s) => this.Scale((s, s, s));
[MethodImpl(AggressiveInlining)] public Line3D ScaleX(Number s) => this.Scale((s, ((Integer)1), ((Integer)1)));
[MethodImpl(AggressiveInlining)] public Line3D ScaleY(Number s) => this.Scale((((Integer)1), s, ((Integer)1)));
[MethodImpl(AggressiveInlining)] public Line3D ScaleZ(Number s) => this.Scale((((Integer)1), ((Integer)1), s));
[MethodImpl(AggressiveInlining)] public Line3D Rotate(Quaternion q) => this.Transform(new Rotation3D(q));
[MethodImpl(AggressiveInlining)] public Line3D RotateX(Angle a) => this.Rotate(a.RotateX);
[MethodImpl(AggressiveInlining)] public Line3D RotateY(Angle a) => this.Rotate(a.RotateY);
[MethodImpl(AggressiveInlining)] public Line3D RotateZ(Angle a) => this.Rotate(a.RotateZ);
[MethodImpl(AggressiveInlining)] public Line3D Translate(Vector3 v) => this.Transform(new Translation3D(v));
[MethodImpl(AggressiveInlining)] public Line3D TranslateX(Number s) => this.Translate(s.XVector3);
[MethodImpl(AggressiveInlining)] public Line3D TranslateY(Number s) => this.Translate(s.YVector3);
[MethodImpl(AggressiveInlining)] public Line3D TranslateZ(Number s) => this.Translate(s.ZVector3);
public Point3D Size { [MethodImpl(AggressiveInlining)] get  => this.End.Subtract(this.Start); } 
[MethodImpl(AggressiveInlining)] public Point3D Lerp(Number amount) => this.Start.Lerp(this.End, amount);
[MethodImpl(AggressiveInlining)] public Boolean Contains(Point3D value) => value.Between(this.Start, this.End);
[MethodImpl(AggressiveInlining)] public Boolean Contains(Line3D y) => this.Contains(y.Start).And(this.Contains(y.End));
[MethodImpl(AggressiveInlining)] public Boolean Overlaps(Line3D y) => this.Contains(y.Start).Or(this.Contains(y.End).Or(y.Contains(this.Start).Or(y.Contains(this.End))));
[MethodImpl(AggressiveInlining)] public Tuple2<Line3D, Line3D> SplitAt(Number t) => (this.Left(t), this.Right(t));
public Tuple2<Line3D, Line3D> Split { [MethodImpl(AggressiveInlining)] get  => this.SplitAt(((Number)0.5)); } 
[MethodImpl(AggressiveInlining)] public Line3D Left(Number t) => (this.Start, this.Lerp(t));
[MethodImpl(AggressiveInlining)] public Line3D Right(Number t) => (this.Lerp(t), this.End);
[MethodImpl(AggressiveInlining)] public Line3D MoveTo(Point3D v) => (v, v.Add(this.Size));
public Line3D LeftHalf { [MethodImpl(AggressiveInlining)] get  => this.Left(((Number)0.5)); } 
public Line3D RightHalf { [MethodImpl(AggressiveInlining)] get  => this.Right(((Number)0.5)); } 
[MethodImpl(AggressiveInlining)] public Line3D Recenter(Point3D c) => (c.Subtract(this.Size.Half), c.Add(this.Size.Half));
[MethodImpl(AggressiveInlining)] public Line3D Clamp(Line3D y) => (this.Clamp(y.Start), this.Clamp(y.End));
[MethodImpl(AggressiveInlining)] public Point3D Clamp(Point3D value) => value.Clamp(this.Start, this.End);
[MethodImpl(AggressiveInlining)] public IReadOnlyList<Point3D> LinearSpace(Integer count){
            var _var60 = this;
            return count.LinearSpace.Map((x)  => _var60.Lerp(x));
        }

[MethodImpl(AggressiveInlining)] public IReadOnlyList<Point3D> LinearSpaceExclusive(Integer count){
            var _var61 = this;
            return count.LinearSpaceExclusive.Map((x)  => _var61.Lerp(x));
        }

[MethodImpl(AggressiveInlining)] public Line3D Subdivide(Number start, Number end) => (this.Lerp(start), this.Lerp(end));
[MethodImpl(AggressiveInlining)] public Line3D Subdivide(NumberInterval subInterval) => this.Subdivide(subInterval.Start, subInterval.End);
[MethodImpl(AggressiveInlining)] public IReadOnlyList<Line3D> Subdivide(Integer count){
            var _var62 = this;
            return count.Intervals.Map((i)  => _var62.Subdivide(i));
        }

[MethodImpl(AggressiveInlining)] public Point3D At(Integer n) => this.Components[n];
public Point3D this[Integer n] { [MethodImpl(AggressiveInlining)]  get => At(n); }
        [MethodImpl(AggressiveInlining)] public Line3D MapComponents(System.Func<Point3D, Point3D> f) => Line3D.CreateFromComponents(this.Components.Map(f));
[MethodImpl(AggressiveInlining)] public Line3D ZipComponents(Line3D b, System.Func<Point3D, Point3D, Point3D> f) => Line3D.CreateFromComponents(this.Components.Zip(b.Components, f));
[MethodImpl(AggressiveInlining)] public Line3D ZipComponents(Line3D b, Line3D c, System.Func<Point3D, Point3D, Point3D, Point3D> f) => Line3D.CreateFromComponents(this.Components.Zip(b.Components, c.Components, f));
[MethodImpl(AggressiveInlining)] public Boolean AllZipComponents(Line3D b, System.Func<Point3D, Point3D, Boolean> f) => this.Components.Zip(b.Components, f).All((x)  => x);
[MethodImpl(AggressiveInlining)] public Boolean AllZipComponents(Line3D b, Line3D c, System.Func<Point3D, Point3D, Point3D, Boolean> f) => this.Components.Zip(b.Components, c.Components, f).All((x)  => x);
[MethodImpl(AggressiveInlining)] public Boolean AnyZipComponents(Line3D b, System.Func<Point3D, Point3D, Boolean> f) => this.Components.Zip(b.Components, f).Any((x)  => x);
[MethodImpl(AggressiveInlining)] public Boolean AnyZipComponents(Line3D b, Line3D c, System.Func<Point3D, Point3D, Point3D, Boolean> f) => this.Components.Zip(b.Components, c.Components, f).Any((x)  => x);
[MethodImpl(AggressiveInlining)] public Boolean AllComponents(System.Func<Point3D, Boolean> predicate) => this.Components.All(predicate);
[MethodImpl(AggressiveInlining)] public Boolean AnyComponent(System.Func<Point3D, Boolean> predicate) => this.Components.Any(predicate);

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class Line3DExtensions
    {
    }
}
