using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Collections
{
    public static class TreeAlgorithms
    {
        public static IBinaryTree<T> CreateTree<T>(this T value, IBinaryTree<T> left, IBinaryTree<T> right)
            => new BinaryTree<T>(value, left, right);

        public static IBinaryTree<T> CreateLeaf<T>(this T value)
            => CreateTree(value, null, null);

        public static IBinaryTree<T> LeftRotation<T>(this IBinaryTree<T> tree)
            => CreateTree(
                tree.Right.Value,
                CreateTree(
                    tree.Value,
                    tree.Left,
                    tree.Right.Left),
                tree.Right.Right);

        public static IBinaryTree<T> RightRotation<T>(this IBinaryTree<T> tree)
            => CreateTree(
                tree.Left.Value,
                tree.Left.Left,
                CreateTree(
                    tree.Value,
                    tree.Left.Right,
                    tree.Right));

        public static IBinaryTree<T> ReplaceNode<T>(this IBinaryTree<T> root,
            IBinaryTree<T> source,
            IBinaryTree<T> target)
        {
            if (root == null)
                return null;
            if (root == source)
                return target;
            return new BinaryTree<T>(
                root.Value,
                root.Left.ReplaceNode(source, target),
                root.Right.ReplaceNode(source, target));
        }

        public static IBinaryTree<T> Map<T>(this IBinaryTree<T> root,
            Func<T, T> transform)
        {
            if (root == null)
                return null;
            return new BinaryTree<T>(
                transform(root.Value),
                root.Left.Map(transform),
                root.Right.Map(transform));
        }

        public static IBinaryTree<T> DeleteLeftmostNode<T>(
            this IBinaryTree<T> self)
            => self.Left == null
                ? self.Right
                : CreateTree(self.Value,
                    self.Left.DeleteLeftmostNode(),
                    self.Right);

        public static IBinaryTree<T> DeleteRightmostNode<T>(
            this IBinaryTree<T> self)
            => self.Right == null
                ? self.Left
                : CreateTree(self.Value,
                    self.Left,
                    self.Right.DeleteRightmostNode());

        public static IBinaryTree<T> DeleteRoot<T>(
            this IBinaryTree<T> root)
        {
            if (root.Right == null)
                return root.Left;

            var next = root.Right.GetLeftmostNode();
            var right = root.Right.DeleteRightmostNode();
            return CreateTree(next.Value, root.Left, right);
        }

        public static int Count<T>(this ITree<T> self)
            => self == null ? 0 : self.Subtrees.Sum(Count) + 1;

        /*
        public static IBinaryTree<T> Filter<T>(this IBinaryTree<T> root, Func<T, bool> predicate)
        {
            if (root == null)
                return null;
            if (predicate(root.Value))
                return new BinaryTree<T>(
                    root.Value,
                    root.Left.Filter(predicate),
                    root.Right.Filter(predicate));
    
            var next = root.Right.GetLeftmostNode();
            if (next )
    
            return new BinaryTree<T>(
                    root.Value,
                    root.Left.Filter(predicate),
                    root.Right.Filter(predicate));
        }
        */

        public static IBinaryTree<T> Insert<T>(
            this IBinaryTree<T> tree,
            T value,
            Func<T, T, int> compare)
        {
            if (tree == null)
                return CreateLeaf(value);

            var c = compare(value, tree.Value);

            return c >= 0
                ? CreateTree(tree.Value, tree.Left, tree.Right.Insert(value, compare))
                : CreateTree(tree.Value, tree.Left.Insert(value, compare), tree.Right);
        }

        public static void Visit<T>(
            this ITree<T> self,
            Action<ITree<T>, int> visitAction,
            int depth = 0)
        {
            visitAction(self, depth);
            foreach (var c in self.Subtrees)
                if (c != null)
                    Visit(c, visitAction, depth + 1);
        }

        public static void OutputTreeNode<T>(ITree<T> tree, int depth)
        {
            var indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}{tree.Value}");
        }

        public static void OutputTree<T>(ITree<T> tree)
        {
            Console.WriteLine($"Tree with {tree.Count()} nodes");
            Visit(tree, OutputTreeNode);
        }

        public static double AverageDepth<T>(this ITree<T> tree)
        {
            var sum = 0;
            var cnt = 0;
            Visit(tree, (_, depth) =>
            {
                cnt++;
                sum += depth;
            });
            return (double)sum / cnt;
        }

        public static int MaxDepth<T>(this ITree<T> tree)
        {
            var max = 0;
            Visit(tree, (_, depth) => { max = Math.Max(depth, max); });
            return max;
        }

        public static IBinaryTree<T> GetLeftmostNode<T>(
            this IBinaryTree<T> root)
        {
            while (true)
            {
                if (root?.Left == null) return root;
                root = root.Left;
            }
        }

        public static IBinaryTree<T> GetRightmostNode<T>(
            this IBinaryTree<T> root)
        {
            while (true)
            {
                if (root?.Right == null) return root;
                root = root.Right;
            }
        }

        public static IBinaryTree<T> ReplaceValue<T>(
            this IBinaryTree<T> root, T value)
            => CreateTree(value, root.Left, root.Right);

        public static IBinaryTree<T> SwapWithLeftChild<T>(
            this IBinaryTree<T> root)
            => CreateTree(root.Left.Value,
                root.Left.ReplaceValue(root.Value),
                root.Right);

        public static IBinaryTree<T> SwapWithRightChild<T>(
            this IBinaryTree<T> root)
            => CreateTree(root.Right.Value,
                root.Left,
                root.Right.ReplaceValue(root.Value));

        public static bool IsHeap<T>(this IBinaryTree<T> tree,
            Func<T, T, int> compare)
        {
            if (tree.Left != null)
                if (compare(tree.Value, tree.Left.Value) < 0)
                    return false;
            if (tree.Right != null)
                if (compare(tree.Value, tree.Right.Value) < 0)
                    return false;
            return tree.Left.IsHeap(compare)
                   && tree.Right.IsHeap(compare);
        }

        public static IBinaryTree<T> InsertHeap<T>(
            this IBinaryTree<T> root, T value, Func<T, T, int> compare)
        {
            if (root == null)
                return CreateLeaf(value);

            if (compare(value, root.Value) > 0)
                return (root.GetHashCode() ^ value.GetHashCode()) % 2 == 0
                    ? CreateTree(value,
                        root.Left.InsertHeap(root.Value, compare),
                        root.Right)
                    : CreateTree(value,
                        root.Left,
                        root.Right.InsertHeap(root.Value, compare));

            if (root.Left == null)
                return CreateTree(root.Value, CreateLeaf(value), root.Right);

            if (root.Right == null)
                return CreateTree(root.Value, root.Left, CreateLeaf(value));

            if (compare(root.Left.Value, root.Right.Value) >= 0)
                return CreateTree(root.Value, root.Left.InsertHeap(value, compare), root.Right);

            return CreateTree(root.Value, root.Left, root.Right.InsertHeap(value, compare));
        }

        public static IBinaryTree<T> ExtractHeap<T>(
            this IBinaryTree<T> root,
            Func<T, T, int> compare)
        {
            if (root.Left == null) return root.Right;
            if (root.Right == null) return root.Left;

            if (compare(root.Left.Value, root.Right.Value) >= 0)
                return CreateTree(
                    root.Left.Value,
                    root.Left.ExtractHeap(compare),
                    root.Right);

            return CreateTree(
                root.Right.Value,
                root.Left,
                root.Right.ExtractHeap(compare));
        }
    
        public static IEnumerable<T> DepthFirstSearch<T>(
            T root,
            Func<T, IEnumerable<T>> getChildren)
        {
            var s = EmptyList<T>.Default.Push(root);
            while (!s.IsEmpty)
            {
                var current = s.Peek();
                s = s.Pop();
                foreach (var child in getChildren(current))
                    s.Push(child);
                yield return current;
            }
        }
    }
}