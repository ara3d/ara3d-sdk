using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Ara3D.Studio.Data
{
    public static class NormalEncoder
    {
        public const float EPS = 1e-10f;
        public static readonly Vector256<float> OneVec = Vector256.Create(1.0f);
        public static readonly Vector256<float> NegOneVec = Vector256.Create(-1.0f);
        public static readonly Vector256<float> HalfVec = Vector256.Create(0.5f);
        public static readonly Vector256<float> ScaleVec = Vector256.Create(65535.0f);
        public static readonly Vector256<float> EpsVec = Vector256.Create(EPS);
        public static readonly Vector256<float> ZeroVec = Vector256<float>.Zero;
        public static readonly Vector256<float> AbsMask = Vector256.Create(unchecked((int)0x7FFFFFFF)).AsSingle();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EncodeNormals(ReadOnlySpan<Vector3> normals, Span<uint> output)
        {
            var n = normals.Length;
            if (n != output.Length)
                throw new ArgumentException("Input and output must be same length.");

            // If AVX/AVX2 isn't supported, fall back to scalar
            if (!(Avx.IsSupported && Avx2.IsSupported))
            {
                for (var j = 0; j < n; j++)
                    output[j] = EncodeNormal(normals[j]);
                return;
            }

            const int BATCH = 8;

            var i = 0;
            unsafe
            {
                fixed (Vector3* pN = normals)
                {
                    // Process in chunks of 8 normals
                    for (; i + BATCH <= n; i += BATCH)
                    {
                        // gather X/Y/Z into vectors
                        var vx = Vector256.Create(
                            pN[i + 0].X, pN[i + 1].X, pN[i + 2].X, pN[i + 3].X,
                            pN[i + 4].X, pN[i + 5].X, pN[i + 6].X, pN[i + 7].X);
                        var vy = Vector256.Create(
                            pN[i + 0].Y, pN[i + 1].Y, pN[i + 2].Y, pN[i + 3].Y,
                            pN[i + 4].Y, pN[i + 5].Y, pN[i + 6].Y, pN[i + 7].Y);
                        var vz = Vector256.Create(
                            pN[i + 0].Z, pN[i + 1].Z, pN[i + 2].Z, pN[i + 3].Z,
                            pN[i + 4].Z, pN[i + 5].Z, pN[i + 6].Z, pN[i + 7].Z);

                        // normalize
                        var lenSq = Avx.Add(
                                       Avx.Add(Avx.Multiply(vx, vx), Avx.Multiply(vy, vy)),
                                       Avx.Multiply(vz, vz));
                        var invLen = Avx.Divide(OneVec, Avx.Sqrt(lenSq));
                        vx = Avx.Multiply(vx, invLen);
                        vy = Avx.Multiply(vy, invLen);
                        vz = Avx.Multiply(vz, invLen);

                        // absolute values
                        var absX = Avx.And(vx, AbsMask);
                        var absY = Avx.And(vy, AbsMask);
                        var absZ = Avx.And(vz, AbsMask);

                        // octahedral projection
                        var denom = Avx.Add(Avx.Add(absX, absY), Avx.Add(absZ, EpsVec));
                        var ox = Avx.Divide(vx, denom);
                        var oy = Avx.Divide(vy, denom);

                        // build masks for hemisphere fold
                        var maskZ = Avx.Compare(vz, ZeroVec, FloatComparisonMode.OrderedLessThanNonSignaling);
                        var maskOxNonNeg = Avx.Compare(ox, ZeroVec, FloatComparisonMode.OrderedGreaterThanOrEqualNonSignaling);
                        var maskOyNonNeg = Avx.Compare(oy, ZeroVec, FloatComparisonMode.OrderedGreaterThanOrEqualNonSignaling);

                        // sign vectors: -1 or +1
                        var signOx = Avx.BlendVariable(NegOneVec, OneVec, maskOxNonNeg);
                        var signOy = Avx.BlendVariable(NegOneVec, OneVec, maskOyNonNeg);

                        // folded oct coords
                        var foldedOx = Avx.Multiply(Avx.Subtract(OneVec, Avx.And(oy, AbsMask)), signOx);
                        var foldedOy = Avx.Multiply(Avx.Subtract(OneVec, Avx.And(ox, AbsMask)), signOy);

                        // select folded where z<0
                        ox = Avx.BlendVariable(ox, foldedOx, maskZ);
                        oy = Avx.BlendVariable(oy, foldedOy, maskZ);

                        // remap [-1,1]→[0,1]
                        var u = Avx.Add(Avx.Multiply(ox, HalfVec), HalfVec);
                        var v = Avx.Add(Avx.Multiply(oy, HalfVec), HalfVec);

                        // to 16-bit
                        var uScaled = Avx.Add(Avx.Multiply(u, ScaleVec), HalfVec);
                        var vScaled = Avx.Add(Avx.Multiply(v, ScaleVec), HalfVec);

                        var uInt = Avx.ConvertToVector256Int32(uScaled);
                        var vInt = Avx.ConvertToVector256Int32(vScaled);

                        // extract & pack
                        for (var j = 0; j < BATCH; j++)
                        {
                            var ux = (ushort)uInt.GetElement(j);
                            var uy = (ushort)vInt.GetElement(j);
                            output[i + j] = ((uint)ux << 16) | uy;
                        }
                    }
                }
            }

            // tail scalar
            for (; i < n; i++)
                output[i] = EncodeNormal(normals[i]);
        }

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