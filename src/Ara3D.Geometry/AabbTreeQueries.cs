namespace Ara3D.Geometry;


public delegate bool AabbShouldVisit(in Bounds3D bounds, out float priority);
public delegate bool AabbVisitItem(int index, in Bounds3D bounds);

public readonly struct LambdaAabbTreeQuery : IAabbTreeQuery
{
    private readonly AabbShouldVisit _shouldVisit;
    private readonly AabbVisitItem _visit;
    private readonly Func<bool>? _shouldStop;

    public bool ShouldStop => _shouldStop?.Invoke() ?? false;

    public LambdaAabbTreeQuery(
        AabbShouldVisit shouldVisit,
        AabbVisitItem visit,
        Func<bool>? shouldStop = null)
    {
        _shouldVisit = shouldVisit ?? throw new ArgumentNullException(nameof(shouldVisit));
        _visit = visit ?? throw new ArgumentNullException(nameof(visit));
        _shouldStop = shouldStop;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
        => _shouldVisit(in bounds, out priority);

    public bool Visit(int index, in Bounds3D bounds)
        => _visit(index, in bounds);
}

public struct PointContainmentQuery : IAabbTreeQuery
{
    public Point3D Point;
    public List<int> Results;
    public bool StopOnFirst;

    public bool ShouldStop => StopOnFirst && Results.Count > 0;

    public PointContainmentQuery(Point3D point, List<int> results, bool stopOnFirst)
    {
        Point = point;
        Results = results ?? throw new ArgumentNullException(nameof(results));
        StopOnFirst = stopOnFirst;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = 0;
        return bounds.Contains(Point);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Contains(Point))
            return false;

        Results.Add(index);
        return true;
    }
}

public struct BoundsOverlapAnyQuery : IAabbTreeQuery
{
    public Bounds3D Bounds;
    public bool Found;

    public bool ShouldStop => Found;

    public BoundsOverlapAnyQuery(Bounds3D bounds)
    {
        Bounds = bounds;
        Found = false;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = CenterDistanceSquared(Bounds, bounds);
        return bounds.Intersects(Bounds);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Intersects(Bounds))
            return false;

        Found = true;
        return true;
    }

    private static float CenterDistanceSquared(in Bounds3D a, in Bounds3D b)
    {
        var d = a.Center.Vector3 - b.Center.Vector3;
        return d.LengthSquared();
    }
}

public struct BoundsOverlapQuery : IAabbTreeQuery
{
    public Bounds3D Bounds;
    public List<int> Results;
    public bool StopOnFirst;

    public bool ShouldStop => StopOnFirst && Results.Count > 0;

    public BoundsOverlapQuery(Bounds3D bounds, List<int> results, bool stopOnFirst)
    {
        Bounds = bounds;
        Results = results ?? throw new ArgumentNullException(nameof(results));
        StopOnFirst = stopOnFirst;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = CenterDistanceSquared(Bounds, bounds);
        return bounds.Intersects(Bounds);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Intersects(Bounds))
            return false;

        Results.Add(index);
        return true;
    }

    private static float CenterDistanceSquared(in Bounds3D a, in Bounds3D b)
    {
        var d = a.Center.Vector3 - b.Center.Vector3;
        return d.LengthSquared();
    }
}

public struct RayBoundsQuery : IAabbTreeQuery
{
    public Ray3D Ray;
    public List<int> Results;
    public float MaxDistance;
    public bool StopOnFirst;

    public bool ShouldStop => StopOnFirst && Results.Count > 0;

    public RayBoundsQuery(
        Ray3D ray,
        List<int> results,
        float maxDistance,
        bool stopOnFirst)
    {
        Ray = ray;
        Results = results ?? throw new ArgumentNullException(nameof(results));
        MaxDistance = maxDistance;
        StopOnFirst = stopOnFirst;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = float.PositiveInfinity;

        if (!Ray.Intersects(bounds, out var interval))
            return false;

        if (interval.End < 0)
            return false;

        if (interval.Start > MaxDistance)
            return false;

        priority = MathF.Max(0, interval.Start);
        return true;
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!ShouldVisit(in bounds, out _))
            return false;

        Results.Add(index);
        return true;
    }
}

public struct RayClosestHitQuery<T> : IAabbTreeQuery
{
    public IReadOnlyList<T> Values;
    public Ray3D Ray;
    public RayItemIntersection<T> Intersects;
    public float BestDistance;
    public int BestIndex;
    public float Tolerance;

    public bool ShouldStop => false;

    public RayClosestHitQuery(
        IReadOnlyList<T> values,
        Ray3D ray,
        RayItemIntersection<T> intersects,
        float maxDistance,
        float tolerance)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
        Ray = ray;
        Intersects = intersects ?? throw new ArgumentNullException(nameof(intersects));
        BestDistance = maxDistance;
        BestIndex = -1;
        Tolerance = tolerance;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = float.PositiveInfinity;

        if (!Ray.Intersects(bounds, out var interval))
            return false;

        if (interval.End < 0)
            return false;

        if (interval.Start > BestDistance)
            return false;

        priority = MathF.Max(0, interval.Start);
        return true;
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!ShouldVisit(in bounds, out _))
            return false;

        var best = BestDistance;

        if (!Intersects(Values[index], Ray, ref best, Tolerance))
            return false;

        if (best < BestDistance)
        {
            BestDistance = best;
            BestIndex = index;
        }

        return true;
    }
}

public struct BoundsExactAnyQuery<T> : IAabbTreeQuery
{
    public IReadOnlyList<T> Values;
    public Bounds3D Bounds;
    public BoundsItemIntersection<T> ExactIntersects;
    public float Tolerance;
    public bool Found;

    public bool ShouldStop => Found;

    public BoundsExactAnyQuery(
        IReadOnlyList<T> values,
        Bounds3D bounds,
        BoundsItemIntersection<T> exactIntersects,
        float tolerance)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
        Bounds = bounds;
        ExactIntersects = exactIntersects ?? throw new ArgumentNullException(nameof(exactIntersects));
        Tolerance = tolerance;
        Found = false;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = 0;
        return bounds.Intersects(Bounds);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Intersects(Bounds))
            return false;

        if (!ExactIntersects(Values[index], in Bounds, Tolerance))
            return false;

        Found = true;
        return true;
    }
}

public struct TriangleAnyIntersectionQuery : IAabbTreeQuery
{
    public Triangle3D Triangle;
    public Bounds3D Bounds;
    public IReadOnlyList<Triangle3D> OtherTriangles;
    public float Tolerance;
    public bool Found;

    public bool ShouldStop => Found;

    public TriangleAnyIntersectionQuery(
        Triangle3D triangle,
        Bounds3D bounds,
        IReadOnlyList<Triangle3D> otherTriangles,
        float tolerance)
    {
        Triangle = triangle;
        Bounds = bounds;
        OtherTriangles = otherTriangles;
        Tolerance = tolerance;
        Found = false;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = 0;
        return bounds.Intersects(Bounds);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Intersects(Bounds))
            return false;

        if (!Triangle.Intersects(OtherTriangles[index], Tolerance))
            return false;

        Found = true;
        return true;
    }
}

public struct TrianglePairsQuery : IAabbTreeQuery
{
    public int TriangleIndex;
    public Triangle3D Triangle;
    public Bounds3D Bounds;
    public IReadOnlyList<Triangle3D> OtherTriangles;
    public List<(int A, int B)> Results;
    public bool ExactTest;
    public float Tolerance;

    public bool ShouldStop => false;

    public TrianglePairsQuery(
        int triangleIndex,
        Triangle3D triangle,
        Bounds3D bounds,
        IReadOnlyList<Triangle3D> otherTriangles,
        List<(int A, int B)> results,
        bool exactTest,
        float tolerance)
    {
        TriangleIndex = triangleIndex;
        Triangle = triangle;
        Bounds = bounds;
        OtherTriangles = otherTriangles;
        Results = results;
        ExactTest = exactTest;
        Tolerance = tolerance;
    }

    public bool ShouldVisit(in Bounds3D bounds, out float priority)
    {
        priority = 0;
        return bounds.Intersects(Bounds);
    }

    public bool Visit(int index, in Bounds3D bounds)
    {
        if (!bounds.Intersects(Bounds))
            return false;

        if (ExactTest && !Triangle.Intersects(OtherTriangles[index], Tolerance))
            return false;

        Results.Add((TriangleIndex, index));
        return true;
    }
}

public delegate bool RayItemIntersection<T>(
    T item,
    Ray3D ray,
    ref float bestDistance,
    float tolerance);

public delegate bool BoundsItemIntersection<T>(
    T item,
    in Bounds3D bounds,
    float tolerance);

public static class AabbTreeExtensions
{
    public static bool Traverse(
        this AabbTree tree,
        AabbShouldVisit shouldVisit,
        AabbVisitItem visit,
        Func<bool>? shouldStop = null)
    {
        var query = new LambdaAabbTreeQuery(shouldVisit, visit, shouldStop);
        return tree.Traverse(ref query);
    }

    public static AabbTree ToAabbTree(
        this IReadOnlyList<Bounds3D> bounds,
        int maxItemsPerLeaf = AabbTree.DefaultMaxItemsPerLeaf)
        => new(bounds, maxItemsPerLeaf);

    public static AabbTree ToAabbTree<T>(
        this IReadOnlyList<T> values,
        Func<T, Bounds3D> getBounds,
        int maxItemsPerLeaf = AabbTree.DefaultMaxItemsPerLeaf)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (getBounds == null)
            throw new ArgumentNullException(nameof(getBounds));

        var bounds = new Bounds3D[values.Count];

        for (var i = 0; i < values.Count; i++)
            bounds[i] = getBounds(values[i]);

        return new AabbTree(bounds, maxItemsPerLeaf);
    }

    public static AabbTree ToAabbTree<T>(
        this IReadOnlyList<T> values,
        IReadOnlyList<Bounds3D> bounds,
        int maxItemsPerLeaf = AabbTree.DefaultMaxItemsPerLeaf)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (bounds == null)
            throw new ArgumentNullException(nameof(bounds));

        if (values.Count != bounds.Count)
            throw new ArgumentException("The value and bounds collections must have the same count.");

        return new AabbTree(bounds, maxItemsPerLeaf);
    }

    public static List<int> QueryPoint(this AabbTree tree, Point3D point)
    {
        var result = new List<int>();
        var query = new PointContainmentQuery(point, result, stopOnFirst: false);
        tree.Traverse(ref query);
        return result;
    }

    public static bool ContainsAny(this AabbTree tree, Point3D point)
    {
        var result = new List<int>(capacity: 1);
        var query = new PointContainmentQuery(point, result, stopOnFirst: true);
        return tree.Traverse(ref query);
    }

    public static List<int> QueryOverlaps(this AabbTree tree, Bounds3D bounds)
    {
        var result = new List<int>();
        var query = new BoundsOverlapQuery(bounds, result, stopOnFirst: false);
        tree.Traverse(ref query);
        return result;
    }

    public static bool OverlapsAny(this AabbTree tree, Bounds3D bounds)
    {
        var query = new BoundsOverlapAnyQuery(bounds);
        return tree.Traverse(ref query);
    }

    public static bool OverlapsAny(
        this AabbTree tree,
        IReadOnlyList<Bounds3D> bounds)
    {
        if (tree == null)
            throw new ArgumentNullException(nameof(tree));

        if (bounds == null)
            throw new ArgumentNullException(nameof(bounds));

        for (var i = 0; i < bounds.Count; i++)
        {
            if (tree.OverlapsAny(bounds[i]))
                return true;
        }

        return false;
    }

    public static List<int> QueryRayBounds(
        this AabbTree tree,
        Ray3D ray,
        float maxDistance = float.PositiveInfinity)
    {
        var result = new List<int>();
        var query = new RayBoundsQuery(ray, result, maxDistance, stopOnFirst: false);
        tree.Traverse(ref query);
        return result;
    }

    public static bool RayHitsAnyBounds(
        this AabbTree tree,
        Ray3D ray,
        float maxDistance = float.PositiveInfinity)
    {
        var result = new List<int>(capacity: 1);
        var query = new RayBoundsQuery(ray, result, maxDistance, stopOnFirst: true);
        return tree.Traverse(ref query);
    }

    public static bool RayClosestHit<T>(
        this AabbTree tree,
        IReadOnlyList<T> values,
        Ray3D ray,
        RayItemIntersection<T> itemIntersects,
        out int index,
        out float distance,
        float maxDistance = float.PositiveInfinity,
        float tolerance = 1e-5f)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (itemIntersects == null)
            throw new ArgumentNullException(nameof(itemIntersects));

        if (values.Count != tree.Count)
            throw new ArgumentException("The value collection must have the same count as the AABB tree.");

        var query = new RayClosestHitQuery<T>(
            values,
            ray,
            itemIntersects,
            maxDistance,
            tolerance);

        var hit = tree.Traverse(ref query);

        index = query.BestIndex;
        distance = query.BestDistance;

        return hit;
    }

    public static bool IntersectsAny<T>(
        this AabbTree tree,
        IReadOnlyList<T> values,
        Bounds3D bounds,
        BoundsItemIntersection<T> exactIntersects,
        float tolerance = 1e-5f)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));

        if (exactIntersects == null)
            throw new ArgumentNullException(nameof(exactIntersects));

        if (values.Count != tree.Count)
            throw new ArgumentException("The value collection must have the same count as the AABB tree.");

        var query = new BoundsExactAnyQuery<T>(
            values,
            bounds,
            exactIntersects,
            tolerance);

        return tree.Traverse(ref query);
    }

    /// <summary>
    /// Basic broad-phase mesh/mesh overlap.
    /// This assumes Triangle3D.Bounds() and Triangle3D.Intersects(...) exist.
    /// </summary>
    public static bool IntersectsAnyTriangle(
        this AabbTree treeA,
        IReadOnlyList<Triangle3D> trianglesA,
        AabbTree treeB,
        IReadOnlyList<Triangle3D> trianglesB,
        float tolerance = 1e-5f)
    {
        if (trianglesA == null)
            throw new ArgumentNullException(nameof(trianglesA));

        if (trianglesB == null)
            throw new ArgumentNullException(nameof(trianglesB));

        if (trianglesA.Count != treeA.Count)
            throw new ArgumentException("trianglesA count must match treeA count.");

        if (trianglesB.Count != treeB.Count)
            throw new ArgumentException("trianglesB count must match treeB count.");

        // Iterate the smaller triangle set and query the larger tree.
        if (trianglesA.Count > trianglesB.Count)
            return treeB.IntersectsAnyTriangle(trianglesB, treeA, trianglesA, tolerance);

        for (var i = 0; i < trianglesA.Count; i++)
        {
            var triA = trianglesA[i];
            var boundsA = triA.Bounds();

            var query = new TriangleAnyIntersectionQuery(
                triA,
                boundsA,
                trianglesB,
                tolerance);

            if (treeB.Traverse(ref query))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns candidate overlapping triangle index pairs.
    /// This is broad-phase only unless you pass exactTest = true.
    /// </summary>
    public static List<(int A, int B)> TriangleIntersectionPairs(
        this AabbTree treeA,
        IReadOnlyList<Triangle3D> trianglesA,
        AabbTree treeB,
        IReadOnlyList<Triangle3D> trianglesB,
        bool exactTest = true,
        float tolerance = 1e-5f)
    {
        if (trianglesA.Count != treeA.Count)
            throw new ArgumentException("trianglesA count must match treeA count.");

        if (trianglesB.Count != treeB.Count)
            throw new ArgumentException("trianglesB count must match treeB count.");

        var result = new List<(int A, int B)>();

        for (var i = 0; i < trianglesA.Count; i++)
        {
            var triA = trianglesA[i];
            var boundsA = triA.Bounds();

            var query = new TrianglePairsQuery(
                i,
                triA,
                boundsA,
                trianglesB,
                result,
                exactTest,
                tolerance);

            treeB.Traverse(ref query);
        }

        return result;
    }

    private static float PriorityByCenterDistance(in Bounds3D bounds, Point3D point)
    {
        var d = bounds.Center.Vector3 - point.Vector3;
        return d.LengthSquared();
    }
}