using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ara3D.Data
{
    public readonly struct Transform
    {
        public readonly Vector3 Translation;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform(Vector3 translation, Quaternion rotation, Vector3 scale) =>
            (Translation, Rotation, Scale) = (translation, rotation, scale);

        /// <summary>
        /// Applies this transform to the given input point.
        /// Order: scale → rotate → translate.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Apply(Vector3 input)
        {
            // 1) Scale
            var scaled = input * Scale;
            // 2) Rotate
            var rotated = Vector3.Transform(scaled, Rotation);
            // 3) Translate
            var translated = rotated + Translation;
            return translated;
        }

        /// <summary>
        /// Returns the identity transform (no translation, no rotation, no scaling).
        /// </summary>
        public static Transform Identity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(Vector3.Zero, Quaternion.Identity, Vector3.One);
        }

        /// <summary>
        /// Returns the inverse of this transform.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform Inverse()
        {
            // Invert scale
            var invScale = new Vector3(
                Scale.X != 0 ? 1f / Scale.X : 0f,
                Scale.Y != 0 ? 1f / Scale.Y : 0f,
                Scale.Z != 0 ? 1f / Scale.Z : 0f
            );

            // Invert rotation
            var invRotation = Quaternion.Inverse(Rotation);

            // Compute the inverted translation:
            // first translate by -Translation, then rotate by invRotation, then scale
            var invTranslation = Vector3.Transform(-Translation, invRotation) * invScale;

            return new Transform(invTranslation, invRotation, invScale);
        }

        /// <summary>
        /// Concatenates this transform with another.
        /// The resulting transform is "this" followed by "other".
        /// 
        /// i.e. if we do: result = TransformA.Compose(TransformB),
        /// then for any vector v, 
        /// result.Apply(v) = TransformB.Apply( TransformA.Apply(v) ).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform Compose(in Transform other)
        {
            // Combined scale
            var combinedScale = Scale * other.Scale;

            // Combined rotation
            var combinedRotation = Rotation * other.Rotation;

            // Combined translation
            // Apply "this" to other's translation
            var combinedTranslation = Apply(other.Translation);

            return new Transform(combinedTranslation, combinedRotation, combinedScale);
        }

        /// <summary>
        /// Linearly interpolates between two transforms.
        /// Scales and translations are lerped, rotations are slerped.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform Lerp(in Transform a, in Transform b, float t)
        {
            var translation = Vector3.Lerp(a.Translation, b.Translation, t);
            var rotation = Quaternion.Slerp(a.Rotation, b.Rotation, t);
            var scale = Vector3.Lerp(a.Scale, b.Scale, t);
            return new Transform(translation, rotation, scale);
        }

        /// <summary>
        /// Returns a transform that translates by the specified amount.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FromTranslation(Vector3 translation)
            => new(translation, Quaternion.Identity, Vector3.One);

        /// <summary>
        /// Returns a transform that rotates by the specified Quaternion.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FromRotation(Quaternion rotation)
            => new(Vector3.Zero, rotation, Vector3.One);

        /// <summary>
        /// Returns a transform that scales by the specified amount.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform FromScale(Vector3 scale)
            => new(Vector3.Zero, Quaternion.Identity, scale);
    }
}