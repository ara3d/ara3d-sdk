namespace Ara3D.Studio.Samples.Demos;

public static class PolygonTriangulator
{

    public static List<Integer3> Triangulate(IReadOnlyList<Vector2> polygon)
    {
        if (polygon == null)
            throw new ArgumentNullException(nameof(polygon));

        if (polygon.Count < 3)
            throw new ArgumentException("Polygon must have at least 3 points.", nameof(polygon));

        // Build index list
        var indices = new List<int>(polygon.Count);
        for (var i = 0; i < polygon.Count; i++)
            indices.Add(i);

        // Ensure CCW winding for the ear clipping logic
        if (SignedArea(polygon) < 0)
            indices.Reverse();

        var result = new List<Integer3>();

        var guard = 0;
        while (indices.Count > 3)
        {
            var earFound = false;

            for (var i = 0; i < indices.Count; i++)
            {
                var prevIndex = indices[(i - 1 + indices.Count) % indices.Count];
                var currIndex = indices[i];
                var nextIndex = indices[(i + 1) % indices.Count];

                var a = polygon[prevIndex];
                var b = polygon[currIndex];
                var c = polygon[nextIndex];

                if (!IsConvex(a, b, c))
                    continue;

                var containsOtherPoint = false;
                for (var j = 0; j < indices.Count; j++)
                {
                    var testIndex = indices[j];
                    if (testIndex == prevIndex || testIndex == currIndex || testIndex == nextIndex)
                        continue;

                    if (PointInTriangle(polygon[testIndex], a, b, c))
                    {
                        containsOtherPoint = true;
                        break;
                    }
                }

                if (containsOtherPoint)
                    continue;

                // Found an ear
                result.Add((prevIndex, currIndex, nextIndex));
                indices.RemoveAt(i);
                earFound = true;
                break;
            }

            if (!earFound)
                throw new InvalidOperationException(
                    "Failed to triangulate polygon. It may be self-intersecting, degenerate, or contain collinear issues.");

            // Safety guard against infinite loops in malformed input
            guard++;
            if (guard > polygon.Count * polygon.Count)
                throw new InvalidOperationException("Triangulation aborted due to unexpected input.");
        }

        // Final triangle
        result.Add((indices[0], indices[1], indices[2]));
        return result;
    }

    private static float SignedArea(IReadOnlyList<Vector2> polygon)
    {
        var area = 0f;
        for (var i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            area += a.X * b.Y - b.X * a.Y;
        }
        return area * 0.5f;
    }

    private static bool IsConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        return Cross(b - a, c - b) > 0f;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        // Works for CCW triangles
        var c1 = Cross(b - a, p - a);
        var c2 = Cross(c - b, p - b);
        var c3 = Cross(a - c, p - c);

        const float epsilon = 1e-6f;
        return c1 >= -epsilon && c2 >= -epsilon && c3 >= -epsilon;
    }
}