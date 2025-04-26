using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Utils
{
    public static class TreeUtil
    {
        /// <summary>
        /// Uses a DepthFirstTraversal to enumerate a tree. 
        /// </summary>
        public static IEnumerable<T> SelfAndDescendants<T>(this T self) where T : ITree<T>
            => DepthFirstTraversal(self, x => x.Children);

        /// <summary>
        /// Enumerate a forest (collection of trees) each using Depth First Traversal. 
        /// </summary>
        public static IEnumerable<T> AllDescendants<T>(this IEnumerable<T> self) where T : ITree<T>
            => self.SelectMany(SelfAndDescendants);

        /// <summary>
        /// Generic depth first traversal. Improved answer over:
        /// https://stackoverflow.com/questions/5804844/implementing-depth-first-search-into-c-sharp-using-list-and-stack
        /// </summary>
        public static IEnumerable<T> DepthFirstTraversal<T>(T root, Func<T, IEnumerable<T>> childGen,
            HashSet<T> visited = null)
            => DepthFirstTraversal(Enumerable.Repeat(root, 1), childGen, visited);

        /// <summary>
        /// Generic depth first traversal. Improved answer over:
        /// https://stackoverflow.com/questions/5804844/implementing-depth-first-search-into-c-sharp-using-list-and-stack
        /// </summary>
        public static IEnumerable<T> DepthFirstTraversal<T>(this IEnumerable<T> roots, Func<T, IEnumerable<T>> childGen,
            HashSet<T> visited = null)
        {
            var stk = new Stack<T>();
            foreach (var root in roots)
                stk.Push(root);
            visited = visited ?? new HashSet<T>();
            while (stk.Count > 0)
            {
                var current = stk.Pop();
                if (!visited.Add(current))
                    continue;
                yield return current;
                var children = childGen(current);
                if (children != null)
                {
                    foreach (var x in children)
                    {
                        if (!visited.Contains(x))
                            stk.Push(x);
                    }
                }
            }
        }

        /// <summary>
        /// Generic breadth first traversal.
        /// </summary>
        public static IEnumerable<T> BreadthFirstTraversal<T>(T root, Func<T, IEnumerable<T>> childGen,
            HashSet<T> visited = null)
            => BreadthFirstTraversal(new[] { root }, childGen, visited);

        /// <summary>
        /// Generic breadth first traversal.
        /// </summary>
        public static IEnumerable<T> BreadthFirstTraversal<T>(this IEnumerable<T> roots,
            Func<T, IEnumerable<T>> childGen, HashSet<T> visited = null)
        {
            var q = new Queue<T>();
            foreach (var root in roots)
                q.Enqueue(root);
            visited = visited ?? new HashSet<T>();
            while (q.Count > 0)
            {
                var current = q.Dequeue();
                if (!visited.Add(current))
                    continue;
                yield return current;
                var children = childGen(current);
                if (children != null)
                {
                    foreach (var x in children)
                    {
                        if (!visited.Contains(x))
                            q.Enqueue(x);
                    }
                }
            }
        }
    }
}