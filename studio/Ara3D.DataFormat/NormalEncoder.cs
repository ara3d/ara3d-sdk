using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ara3D.Data
{
    public static class NormalEncoder
    {
        /// <summary>
        /// Encodes a unit normal vector into a 32-bit unsigned integer using octahedral encoding.
        /// </summary>
        /// <param name="normal">A normalized Vector3 (x² + y² + z² = 1).</param>
        /// <returns>A 32-bit unsigned integer representing the encoded normal.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static uint EncodeNormal(Vector3 normal)
        {
            // Ensure normal is normalized (if not already)
            normal = Vector3.Normalize(normal);

            var x = normal.X;
            var y = normal.Y;
            var z = normal.Z;

            // Project normal onto the octahedron
            var denom = Math.Abs(x) + Math.Abs(y) + Math.Abs(z) + 1e-10f; // small epsilon to avoid division by zero
            var ox = x / denom;
            var oy = y / denom;

            // If we are in the lower hemisphere, fold it over
            if (z < 0.0f)
            {
                var oldOx = ox;
                var oldOy = oy;
                ox = (1.0f - Math.Abs(oldOy)) * MathF.Sign(oldOx);
                oy = (1.0f - Math.Abs(oldOx)) * MathF.Sign(oldOy);
            }

            // Remap from [-1,1] to [0,1]
            var u = ox * 0.5f + 0.5f;
            var v = oy * 0.5f + 0.5f;

            // Convert to 16-bit range
            var usX = (ushort)(u * 65535.0f + 0.5f);
            var usY = (ushort)(v * 65535.0f + 0.5f);

            // Pack two 16-bit values into one 32-bit integer
            var encoded = ((uint)usX << 16) | (uint)usY;
            return encoded;
        }

        /// <summary>
        /// Decodes a 32-bit unsigned integer back into a normalized Vector3 using octahedral decoding.
        /// </summary>
        /// <param name="encoded">32-bit unsigned integer representing the encoded normal.</param>
        /// <returns>A unit-length normal vector (Vector3).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 DecodeNormal(uint encoded)
        {
            // Extract the two 16-bit values
            var usX = (ushort)(encoded >> 16);
            var usY = (ushort)(encoded & 0xFFFF);

            // Convert back to [-1,1]
            var u = (usX / 65535.0f) * 2.0f - 1.0f;
            var v = (usY / 65535.0f) * 2.0f - 1.0f;

            // Construct the initial octahedral vector
            var n = new Vector3(u, v, 1.0f - MathF.Abs(u) - MathF.Abs(v));

            // If z is negative, "unfold" the vector
            if (n.Z < 0.0f)
            {
                var oldX = n.X;
                var oldY = n.Y;

                n = new Vector3(
                    MathF.Sign(oldX) * (1.0f - MathF.Abs(oldY)),
                    MathF.Sign(oldY) * (1.0f - MathF.Abs(oldX)),
                    -n.Z);
            }

            // Normalize the vector (it should already be close to unit length)
            return Vector3.Normalize(n);
        }
    }
}