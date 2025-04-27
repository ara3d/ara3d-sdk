using Ara3D.Mathematics;
using System;
using System.Diagnostics;
using Ara3D.Collections;

namespace Ara3D.Geometry
{
    public class KdTree<T> 
        : IBounded
        where T : IBounded
    {
        public int Axis => Depth % 3;
        public int Depth { get; }
        public KdTree<T> Left { get; }
        public KdTree<T> Right { get; }
        public AABox Bounds { get; }
        public readonly IArray<T> Items;
        public bool IsSplit => Left != null || Right != null;

        public const int MinNodesForSplit = 8;

        public KdTree(IArray<T> items, int depth = 0)
        {
            Depth = depth;
            Bounds = items.GetBounds();
            var n = items.Count;
            if (n < MinNodesForSplit)
            {
                Items = items;
            }
            else
            {
                var center = Bounds.Center.GetComponent(Axis);
                // NOTE: the split strategy is to take the center of the bounds. 
                // If data is distributed unevenly, this might not be the most efficient approach. 
                var (left, right) = items.Split(i => i.Bounds.Min.GetComponent(Axis) < center);
                Left = new KdTree<T>(left, Depth + 1);
                Right = new KdTree<T>(right, Depth + 1);
                Items = LinqArray.Empty<T>();
                Debug.Assert(Left.Items.Count + Right.Items.Count == n);
            }
        }
        
        // TODO: I am not happy with this interface yet.
        public bool Visit(Func<AABox, bool> intersects, Func<T, bool> visitAndContinue)
        {
            if (!intersects(Bounds))
                return true;

            if (IsSplit)
            {
                if (!Left.Visit(intersects, visitAndContinue))
                    return false;
                if (!Right.Visit(intersects, visitAndContinue))
                    return false;
            }
            else
            {
                foreach (var item in Items.ToEnumerable())
                {
                    if (!visitAndContinue(item))
                        return false;
                }
            }

            return true;
        }
    }
}
