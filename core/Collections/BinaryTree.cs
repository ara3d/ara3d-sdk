namespace Ara3D.Collections
{
    public class BinaryTree<T> : IBinaryTree<T>
    {
        public T Value { get; }
        public ISequence<ITree<T>> Subtrees => new ITree<T>[] { Left, Right }.ToIArray();
        public IBinaryTree<T> Left { get; }
        public IBinaryTree<T> Right { get; }

        public BinaryTree(T value, IBinaryTree<T> left = null, IBinaryTree<T> right = null)
            => (Value, Left, Right) = (value, left, right);
    }
}