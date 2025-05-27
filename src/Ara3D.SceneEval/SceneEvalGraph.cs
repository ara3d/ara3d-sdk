namespace Ara3D.SceneEval
{
    public class SceneEvalGraph
    {
        public IReadOnlyList<SceneEvalNode> Roots { get; private set; } = [];

        private Dictionary<SceneEvalNode, SceneEvalNode> _rootLookup = new();

        public event EventHandler GraphChanged;
        public event EventHandler GraphInvalidated;

        public List<object> Evaluate(CancellationToken src = default)
        {
            var context = new SceneEvalContext(src);
            return Roots.AsParallel().Select(r => r.Eval(context)).ToList();
        }

        public void UpdateRoots(IReadOnlyList<SceneEvalNode> newRoots)
        {
            foreach (var root in Roots)
                root.Invalidated -= RootInvalidated;
            Roots = newRoots;
            _rootLookup.Clear();
            foreach (var root in Roots)
                StoreRootLookupAndValidate(root, root);
            foreach (var root in Roots)
                root.Invalidated += RootInvalidated;
            NotifyGraphChanged();
        }

        private void StoreRootLookupAndValidate(SceneEvalNode node, SceneEvalNode root)
        {
            if (!_rootLookup.TryAdd(node, root))
                throw new Exception("Cycle detected");
            foreach (var input in node.Inputs)
                StoreRootLookupAndValidate(input, root);
        }

        public void RootInvalidated(object sender, EventArgs args)
        {
            NotifyGraphInvalidated();
        }

        public void NotifyGraphInvalidated()
        {
            GraphInvalidated?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyGraphChanged()
        {
            GraphChanged?.Invoke(this, EventArgs.Empty);
            NotifyGraphInvalidated();
        }

        public SceneEvalNode GetRoot(SceneEvalNode node)
            => !_rootLookup.TryGetValue(node, out var root)
                ? throw new Exception("Node is not part of the graph")
                : root;

        public void AddRoot(SceneEvalNode node)
        {
            var newRoots = Roots.Append(node).ToList();
            UpdateRoots(newRoots);
        }

        public void ReplaceRoot(SceneEvalNode oldRoot, SceneEvalNode newRoot)
        {
            var newRoots = Roots.Select(r => r == oldRoot ? newRoot : r).ToList();
            UpdateRoots(newRoots);
        }
    }
}
