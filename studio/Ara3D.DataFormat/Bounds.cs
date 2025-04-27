using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ara3D.Data
{
    public readonly struct Bounds : IEquatable<Bounds>
    {
        public readonly Vector3 Min;
        public readonly Vector3 Max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds(Vector3 min, Vector3 max) => (Min, Max) = (min, max);

        /// <summary>
        /// Returns an array of the eight corners of this bounding box.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3[] GetCorners()
        {
            return new[]
            {
                new Vector3(Min.X, Min.Y, Min.Z),
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Min.X, Max.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Max.X, Max.Y, Min.Z),
                new Vector3(Max.X, Max.Y, Max.Z)
            };
        }

        /// <summary>
        /// Applies a transform to this bounding box and returns the axis-aligned Bounds that encapsulate
        /// all transformed corners. This may enlarge the Bounds if the rotation skews it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds ApplyTransform(in Transform t)
        {
            var corners = GetCorners();

            // Initialize min/max to the first transformed corner
            var firstCorner = t.Apply(corners[0]);
            var min = firstCorner;
            var max = firstCorner;

            // Transform each corner and expand min/max
            for (var i = 1; i < corners.Length; i++)
            {
                var transformed = t.Apply(corners[i]);
                min = Vector3.Min(min, transformed);
                max = Vector3.Max(max, transformed);
            }

            return new Bounds(min, max);
        }

        /// <summary>
        /// Extent of the bounding box = (Max - Min).
        /// </summary>
        public Vector3 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Max - Min;
        }

        /// <summary>
        /// Half of the extent of the bounding box = (Max - Min) / 2
        /// </summary>
        public Vector3 HalfSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size / 2;
        }

        /// <summary>
        /// Center of the bounding box = (Min + Max) / 2.
        /// </summary>
        public Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Min + Max) * 0.5f;
        }

        /// <summary>
        /// Returns a bounding box that encloses both this box and the other.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds Union(in Bounds other)
        {
            var min = Vector3.Min(Min, other.Min);
            var max = Vector3.Max(Max, other.Max);
            return new Bounds(min, max);
        }

        /// <summary>
        /// Returns a bounding box that is the intersection of this box and the other.
        /// If they do not overlap, the result can be invalid (Min > Max).
        /// Check validity via IsValid.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds Intersect(in Bounds other)
        {
            var min = Vector3.Max(Min, other.Min);
            var max = Vector3.Min(Max, other.Max);
            return new Bounds(min, max);
        }

        /// <summary>
        /// Expands the bounding box on all sides by the given amount.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds Expand(Vector3 amount)
        {
            var min = Min - amount;
            var max = Max + amount;
            return new Bounds(min, max);
        }

        /// <summary>
        /// Returns whether the given point is inside this bounding box (inclusive).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vector3 point)
        {
            return point.X >= Min.X && point.X <= Max.X
                                    && point.Y >= Min.Y && point.Y <= Max.Y
                                    && point.Z >= Min.Z && point.Z <= Max.Z;
        }

        /// <summary>
        /// Checks if this bounding box intersects with another bounding box.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(in Bounds other)
        {
            return (Max.X >= other.Min.X && Min.X <= other.Max.X) &&
                   (Max.Y >= other.Min.Y && Min.Y <= other.Max.Y) &&
                   (Max.Z >= other.Min.Z && Min.Z <= other.Max.Z);
        }

        /// <summary>
        /// Returns true if Min <= Max in all components.
        /// Useful for checking if Intersect(...) returned a valid box.
        /// </summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Min.X <= Max.X) && (Min.Y <= Max.Y) && (Min.Z <= Max.Z);
        }

        /// <summary>
        /// Returns the volume of the bounding box (Extent.X * Extent.Y * Extent.Z).
        /// </summary>
        public float Volume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var size = Size;
                return size.X * size.Y * size.Z;
            }
        }

        /// <summary>
        /// Grows this bounding box so that it contains the given point.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bounds Encapsulate(Vector3 point)
        {
            var min = Vector3.Min(Min, point);
            var max = Vector3.Max(Max, point);
            return new Bounds(min, max);
        }

        /// <summary>
        /// The radius of a bounding sphere. 
        /// </summary>
        /// <returns></returns>
        public float Radius
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get 
            {
                var halfSize = HalfSize;
                return Math.Max(Math.Max(halfSize.X, halfSize.Y), halfSize.Z);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? other)
            => other is Bounds b && Min == b.Min && Max == b.Max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => HashCode.Combine(Min, Max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Bounds b)
            => Min == b.Min && Max == b.Max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(Bounds a, Bounds b) 
            => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Bounds a, Bounds b) 
            => !a.Equals(b);
    }
}