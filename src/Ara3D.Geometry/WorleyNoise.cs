
using Ara3D.Geometry;

public static class WorleyNoise
{
    // Simple 2D integer hash (x,y) → 32-bit int
    private static int Hash(int x, int y)
    {
        unchecked
        {
            var h = x * 374761393 + y * 668265263;
            h = (h ^ (h >> 13)) * 1274126177;
            return h;
        }
    }

    // Simple 3D integer hash (x,y,z) → 32-bit int
    private static int Hash(int x, int y, int z)
    {
        unchecked
        {
            const int C1 = 374761393;
            const int C2 = 668265263;
            // cast the oversized literal down into an int (wraps around in two’s-complement)
            const int C3 = unchecked((int)3266489917);

            var h = x * C1
                    + y * C2
                    + z * C3;        // now all operands are int, so result is int

            h = (h ^ (h >> 13)) * 1274126177;
            return h;
        }
    }

    // Converts a 32-bit hash into a float in [0,1)
    private static float HashToFloat(int h)
    {
        // Take upper 23 bits, divide by 2^23
        return ((h >> 9) & 0x007FFFFF) / (float)(1 << 23);
    }

    /// <summary>
    /// 2D Worley noise: distance to nearest feature point, in [0, √2/2] roughly.
    /// </summary>
    public static float Noise(Vector2 pos)
    {
        // Which cell we’re in
        var xi = (int)MathF.Floor(pos.X);
        var yi = (int)MathF.Floor(pos.Y);
        var minDist2 = float.MaxValue;

        // Check neighboring cells
        for (var oy = -1; oy <= 1; oy++)
            for (var ox = -1; ox <= 1; ox++)
            {
                var cx = xi + ox;
                var cy = yi + oy;

                // Generate one pseudo-random point in cell [cx,cy]
                var h = Hash(cx, cy);
                var fx = HashToFloat(h);
                var fy = HashToFloat(h * 15731); // different scramble for y

                var featurePoint = new Vector2(cx + fx, cy + fy);

                // Compute squared distance
                var delta = pos - featurePoint;
                float d2 = delta.LengthSquared();

                if (d2 < minDist2)
                    minDist2 = d2;
            }

        // Return Euclidean distance
        return MathF.Sqrt(minDist2);
    }

    /// <summary>
    /// 3D Worley noise: distance to nearest feature point.
    /// </summary>
    public static float Noise(Vector3 pos)
    {
        var xi = (int)MathF.Floor(pos.X);
        var yi = (int)MathF.Floor(pos.Y);
        var zi = (int)MathF.Floor(pos.Z);
        var minDist2 = float.MaxValue;

        for (var oz = -1; oz <= 1; oz++)
            for (var oy = -1; oy <= 1; oy++)
                for (var ox = -1; ox <= 1; ox++)
                {
                    var cx = xi + ox;
                    var cy = yi + oy;
                    var cz = zi + oz;

                    var h = Hash(cx, cy, cz);
                    var fx = HashToFloat(h);
                    var fy = HashToFloat(h * 15731);
                    var fz = HashToFloat(h * 789221);

                    var featurePoint = new Vector3(cx + fx, cy + fy, cz + fz);

                    var delta = pos - featurePoint;
                    float d2 = delta.LengthSquared();

                    if (d2 < minDist2)
                        minDist2 = d2;
                }

        return MathF.Sqrt(minDist2);
    }
}