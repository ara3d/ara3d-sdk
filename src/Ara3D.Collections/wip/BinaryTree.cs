﻿using System.Collections.Generic;

namespace Ara3D.Collections.wip
{
    public class BinaryTree<T> : IBinaryTree<T>
    {
        public T Value { get; }
        public IReadOnlyList<ITree<T>> Subtrees => new[] { Left, Right };
        public IBinaryTree<T> Left { get; }
        public IBinaryTree<T> Right { get; }

        public BinaryTree(T value, IBinaryTree<T> left = null, IBinaryTree<T> right = null)
            => (Value, Left, Right) = (value, left, right);
    }
}