using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Geometry
{
    public static class PerlinNoise
    {
        // Permutation table. The array is duplicated so we don't need modulus operations.
        public static readonly int[] _perm =
        [
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
            8,99,37,240,21,10,23,190, 6,148,247,120,234,75,0,26,
            197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,
            56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,
            27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,
            92,41,55,46,245,40,244,102,143,54, 65,25,63,161,1,216,
            80,73,209,76,132,187,208, 89,18,169,200,196,135,130,116,188,
            159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,
            58,17,182,189,28,42,223,183,170,213,119,248,152, 2,44,154,
            163,70,221,153,101,155,167, 43,172,9,129,22,39,253,19,98,
            108,110,79,113,224,232,178,185,112,104,218,246,97,228,251,34,
            242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,
            239,107,49,192,214,31,181,199,106,157,184, 84,204,176,115,121,
            50,45,127,  4,150,254,138,236,205,93,222,114, 67,29,24,72,
            243,141,128,195,78,66,215,61,156,180,

            // repeat
            151,160,137,91,90,15,
            131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
            8,99,37,240,21,10,23,190, 6,148,247,120,234,75,0,26,
            197,62,94,252,219,203,117,35,11,32,57,177,33,88,237,149,
            56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,
            27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,
            92,41,55,46,245,40,244,102,143,54, 65,25,63,161,1,216,
            80,73,209,76,132,187,208, 89,18,169,200,196,135,130,116,188,
            159,86,164,100,109,198,173,186,3,64,52,217,226,250,124,123,
            5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,
            58,17,182,189,28,42,223,183,170,213,119,248,152, 2,44,154,
            163,70,221,153,101,155,167, 43,172,9,129,22,39,253,19,98,
            108,110,79,113,224,232,178,185,112,104,218,246,97,228,251,34,
            242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,
            239,107,49,192,214,31,181,199,106,157,184, 84,204,176,115,121,
            50,45,127,  4,150,254,138,236,205,93,222,114, 67,29,24,72,
            243,141,128,195,78,66,215,61,156,180
        ];

        // Fade function as defined by Ken Perlin. This eases coordinate values
        // so they will “ease” towards integral values. This smooths the final output.
        public static float Fade(float t)
            => t * t * t * (t * (t * 6 - 15) + 10);

        // Linear interpolation
        public static float Lerp(float t, float a, float b)
            => a + t * (b - a);

        // Convert lower 4 bits of hash code into 12 gradient directions.
        public static float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15;
            var u = h < 8 ? x : y;
            var v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        /// <summary>
        /// Generates 3D Perlin noise in [-1,1] at the given point.
        /// </summary>
        public static float Noise(Vector3 point)
        {
            var floor = point.Floor;

            // Find unit cube that contains point
            var X = (int)floor.X & 255;
            var Y = (int)floor.Y & 255;
            var Z = (int)floor.Z & 255;

            // Find relative x, y, z of point in cube
            float x = point.X - floor.X;
            float y = point.Y - floor.Y;
            float z = point.Z - floor.Z;

            // Compute fade curves for each of x, y, z
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);

            // Hash coordinates of the 8 cube corners
            var A = _perm[X] + Y;
            var AA = _perm[A] + Z;
            var AB = _perm[A + 1] + Z;
            var B = _perm[X + 1] + Y;
            var BA = _perm[B] + Z;
            var BB = _perm[B + 1] + Z;

            // And add blended results from 8 corners of cube
            var res = Lerp(w,
                Lerp(v,
                    Lerp(u, Grad(_perm[AA], x, y, z),
                             Grad(_perm[BA], x - 1, y, z)),
                    Lerp(u, Grad(_perm[AB], x, y - 1, z),
                             Grad(_perm[BB], x - 1, y - 1, z))
                ),
                Lerp(v,
                    Lerp(u, Grad(_perm[AA + 1], x, y, z - 1),
                             Grad(_perm[BA + 1], x - 1, y, z - 1)),
                    Lerp(u, Grad(_perm[AB + 1], x, y - 1, z - 1),
                             Grad(_perm[BB + 1], x - 1, y - 1, z - 1))
                )
            );

            // Scale result to [-1,1]
            return res;
        }

        /// <summary>
        /// Generates 2D Perlin noise in [-1,1] by embedding into the Z=0 plane.
        /// </summary>
        public static float Noise(Vector2 point)
            => Noise(point.To3D);
    }
}
