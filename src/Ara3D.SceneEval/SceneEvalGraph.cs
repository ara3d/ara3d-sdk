namespace Ara3D.SceneEval
{
    public class SceneEvalGraph
    {
        public IReadOnlyList<SceneEvalNode> Roots { get; private set; } = [];
        public Dictionary<SceneEvalNode, SceneEvalNode> RootLookup { get; } = [];

        public IEnumerable<SceneEvalNode> GetAllNodes()
            => Roots.SelectMany(x => x.GetAllNodes());

        public event EventHandler GraphInvalidated;
        public event EventHandler GraphChanged;

        public void UpdateRoots(IReadOnlyList<SceneEvalNode> newRoots)
        {
            foreach (var root in Roots)
                root.Invalidated -= NotifyGraphInvalidated;
            Roots = newRoots;
            RootLookup.Clear();
            foreach (var root in Roots)
                Validate(root, root);
            foreach (var root in Roots)
                root.Invalidated += NotifyGraphInvalidated;
            NotifyGraphChanged();
        }

        public void NotifyGraphChanged()
        {
            GraphChanged?.Invoke(this, EventArgs.Empty);
            GraphInvalidated?.Invoke(this, EventArgs.Empty);
        }

        private void Validate(SceneEvalNode node, SceneEvalNode root)
        {
            if (!RootLookup.TryAdd(node, root))
                throw new Exception("Cycle found");
            foreach (var input in node.Inputs)
                Validate(input, root);
        }

        public void NotifyGraphInvalidated(object sender, EventArgs args)
        {
            GraphInvalidated?.Invoke(sender, EventArgs.Empty);
        }

        public SceneEvalNode GetRoot(SceneEvalNode node)
            => !RootLookup.TryGetValue(node, out var root)
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

        public void RemoveRoot(SceneEvalNode node)
        {
            var newRoots = Roots.Where(r => r != node).ToList();
            UpdateRoots(newRoots);
        }
    }
}
