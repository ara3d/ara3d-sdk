using System.Collections.ObjectModel;

namespace Ara3D.SceneEval
{
    public class SceneEvalGraph
    {
        public ObservableCollection<SceneEvalNode> Roots { get; } = [];
        
        public IEnumerable<SceneEvalNode> GetAllNodes()
            => Roots.SelectMany(x => x.GetAllNodes());

        public SceneEvalNode GetRoot(SceneEvalNode node)
            => Roots.FirstOrDefault(r => r.GetAllNodes().Any(n => n == node));

        public event EventHandler GraphInvalidated;
        public event EventHandler GraphChanged;

        public SceneEvalGraph()
        {
            Roots.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (SceneEvalNode node in e.NewItems)
                        node.Invalidated += NotifyGraphInvalidated;
                if (e.OldItems != null)
                    foreach (SceneEvalNode node in e.OldItems)
                        node.Invalidated -= NotifyGraphInvalidated;
                NotifyGraphChanged();
            };
        }

        public void NotifyGraphChanged()
        {
            GraphChanged?.Invoke(this, EventArgs.Empty);
            GraphInvalidated?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceRoot(SceneEvalNode root, SceneEvalNode node)
        {
            Roots.Add(node);
            Roots.Remove(root);
        }

        public void NotifyGraphInvalidated(object sender, EventArgs args)
        {
            GraphInvalidated?.Invoke(sender, EventArgs.Empty);
        }
    }
}
