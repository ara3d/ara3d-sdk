namespace Ara3D.Collections
{

    public class TreeFromArray<T> : IBinaryTree<T>
    {
        public int RootIndex { get; }
        public T[] Values { get; }

        public TreeFromArray(T[] values, int rootIndex)
            => (Values, RootIndex) = (values, rootIndex);

        public T Value => Values[RootIndex];

        public ISequence<ITree<T>> Subtrees
            => new ITree<T>[] { Left, Right }.ToIArray();

        public IBinaryTree<T> GetSubtree(int index)
            => index >= Values.Length
                ? null
                : new TreeFromArray<T>(Values, index);

        public IBinaryTree<T> Left
            => GetSubtree(RootIndex * 2);

        public IBinaryTree<T> Right
            => GetSubtree(RootIndex * 2 + 1);
    }
}