using Ara3D.Collections;
namespace Ara3D.Geometry;

public interface IAabbTreeQuery
{
    /// <summary>
    /// Return true if this query should descend into a node with the given bounds.
    /// It also returns a relative priority. If one child has a lower value than another,
    /// it will be visited first. This can improve performance for some queries by
    /// visiting more promising nodes first.
    /// </summary>
    bool ShouldVisit(in Bounds3D boundsA, out float priority);

    /// <summary>
    /// Called on items in a leaf whose leaf bounds passed ShouldVisit.
    /// Note the non-leaf items do not have items. 
    /// Return true if this item matched the query.
    /// </summary>
    bool Visit(int index, in Bounds3D bounds);

    /// <summary>
    /// Return true to stop traversal early.
    /// Useful for AnyHit-style queries.
    /// </summary>
    bool ShouldStop { get; }
}


/// <summary>
/// This is a straightforward implementation of an AABB tree. It is optimized for fast traversal and
/// reasonably fast construction, but does not support dynamic updates. It is designed to be used with a
/// provided list of bounds, which allows it to be used in a variety of contexts
/// without needing to copy data into a specific structure. The tree is built by recursively splitting
/// the items along the longest axis of their centers until a maximum number of items per leaf is reached.
/// Traversal is done by recursively visiting nodes that intersect the query bounds, and can be stopped
/// early if desired.
/// </summary>
public sealed class AabbTree
{
    private readonly IReadOnlyList<Bounds3D> _bounds;
    private readonly int[] _indices;
    private Node[] _nodes;
    private int _nodeCount;

    public int Count => _indices.Length;
    public Bounds3D TotalBounds => _nodes[0].Bounds;

    public const int DefaultMaxItemsPerLeaf = 8;

    private readonly CenterComparer[] _centerComparers;

    public IReadOnlyList<Node> GetAllNodes()
        => _nodes.Take(_nodeCount);

    public IReadOnlyList<Bounds3D> GetAllBounds()
        => _bounds;

    public IReadOnlyList<Bounds3D> GetAllNodeBounds()
        => GetAllNodes().Select(n => n.Bounds);

    public struct Node
    {
        public Bounds3D Bounds;

        // Range into _indices.
        public int Start;
        public int Count;

        // Child indices into _nodes. -1 means no child.
        public int Left;
        public int Right;

        public int Axis;

        public bool IsLeaf => Left < 0 && Right < 0;
    }

    public AabbTree(IReadOnlyList<Bounds3D> bounds, int maxItemsPerLeaf = DefaultMaxItemsPerLeaf)
    {
        if (bounds.Count == 0)
            throw new ArgumentException("Cannot build an AABB tree over an empty collection.");

        if (maxItemsPerLeaf <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxItemsPerLeaf));

        _bounds = bounds;
        _indices = new int[bounds.Count];

        for (var i = 0; i < _indices.Length; i++)
            _indices[i] = i;

        _centerComparers =
        [
            new CenterComparer(_bounds, 0),
            new CenterComparer(_bounds, 1),
            new CenterComparer(_bounds, 2)
        ];

        // Worst-case full binary tree has less than 2N nodes.
        // This slightly over-allocates but avoids repeated resizing.
        _nodes = new Node[Math.Max(1, bounds.Count * 2)];
        _nodeCount = 0;
        BuildNode(0, bounds.Count, maxItemsPerLeaf);
    }

    public Bounds3D GetBounds(int index)
        => _bounds[index];
    
    public bool Traverse<TQuery>(ref TQuery query)
        where TQuery : struct, IAabbTreeQuery
    {
        if (_nodeCount == 0)
            return false;

        return query.ShouldVisit(in _nodes[0].Bounds, out _) 
               && TraverseNode(0, ref query);
    }

    private bool TraverseNode<TQuery>(int nodeIndex, ref TQuery query)
        where TQuery : struct, IAabbTreeQuery
    {
        ref readonly var node = ref _nodes[nodeIndex];

        if (node.IsLeaf)
        {
            var any = false;
            var end = node.Start + node.Count;
            
            for (var i = node.Start; i < end; i++)
            {
                var originalIndex = _indices[i];
                
                if (query.Visit(originalIndex, _bounds[originalIndex]))
                    any = true;

                if (query.ShouldStop)
                    return any;
            }

            return any;
        }

        var hit = false;
        var shouldVisitA = query.ShouldVisit(in _nodes[node.Left].Bounds, out var priorityA);
        var shouldVisitB = query.ShouldVisit(in _nodes[node.Right].Bounds, out var priorityB);

        if (!shouldVisitA)
        {
            if (!shouldVisitB)
                return false;
            return TraverseNode(node.Right, ref query);
        }

        if (!shouldVisitB)
        {
            return TraverseNode(node.Left, ref query);
        }

        if (priorityA <= priorityB)
        {
            hit |= TraverseNode(node.Left, ref query);
            if (query.ShouldStop)
                return hit;
            hit |= TraverseNode(node.Right, ref query);
        }
        else
        {
            hit |= TraverseNode(node.Right, ref query);
            if (query.ShouldStop)
                return hit;
            hit |= TraverseNode(node.Left, ref query);
        }

        return hit;
    }

    private int BuildNode(int start, int count, int maxItemsPerLeaf)
    {
        var nodeIndex = AllocateNode();
        ComputeBounds(start, count, out var nodeBounds, out var centerBounds);
        var axis = centerBounds.Size.GetLongestAxisIndex();

        _nodes[nodeIndex] = new Node
        {
            Bounds = nodeBounds,
            Start = start,
            Count = count,
            Left = -1,
            Right = -1,
            Axis = axis
        };

        if (count <= maxItemsPerLeaf)
            return nodeIndex;

        SortRangeByCenter(start, count, axis);

        var leftCount = count / 2;
        var rightCount = count - leftCount;

        var left = BuildNode(start, leftCount, maxItemsPerLeaf);
        var right = BuildNode(start + leftCount, rightCount, maxItemsPerLeaf);

        _nodes[nodeIndex].Left = left;
        _nodes[nodeIndex].Right = right;

        return nodeIndex;
    }

    private int AllocateNode()
    {
        if (_nodeCount == _nodes.Length)
        {
            // Should be rare because we preallocate roughly 2N nodes.
            // Still useful for safety with unusual construction choices.
            Array.Resize(ref _nodes, _nodes.Length * 2);
        }

        return _nodeCount++;
    }

    private void ComputeBounds(int start, int count, out Bounds3D totalBounds, out Bounds3D centerBounds)
    {
        var firstIndex = _indices[start];
        var firstCenter = _bounds[firstIndex].Center;
        centerBounds = new Bounds3D(firstCenter, firstCenter);
        totalBounds = _bounds[firstIndex];
        var end = start + count;
        for (var i = start + 1; i < end; i++)
        {
            var originalIndex = _indices[i];
            totalBounds = totalBounds.Include(_bounds[originalIndex]);
            centerBounds = centerBounds.Include(_bounds[originalIndex].Center);
        }
    }

    private void SortRangeByCenter(int start, int count, int axis)
        => Array.Sort(_indices, start, count, _centerComparers[axis]);

    private sealed class CenterComparer : IComparer<int>
    {
        private readonly IReadOnlyList<Bounds3D> _bounds;
        private readonly int _axis;

        public CenterComparer(IReadOnlyList<Bounds3D> bounds, int axis)
        {
            _bounds = bounds;
            _axis = axis;
        }

        public int Compare(int a, int b)
        {
            var ca = _bounds[a].Center.Vector3.GetComponent(_axis);
            var cb = _bounds[b].Center.Vector3.GetComponent(_axis);

            return ca.CompareTo(cb);
        }
    }
}
