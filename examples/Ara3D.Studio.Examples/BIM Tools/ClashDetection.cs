namespace Ara3D.Studio.Samples.BIM_Tools
{
    [Category(nameof(Categories.Buildings))]
    public class ClashDetection : IModifier
    {
        [Range(0.001, 0.1)] public float FrameSize { get; set; } = 0.1f;
        public bool KeepOriginal = true;
        public bool DrawAabbTree = false;
        public bool DrawObjectBounds = true;
        public bool SkipClashes = true;
        public bool SkipNonClashes = false;
        [Range(0f, 5f)] public float VolumeTolerance = 1f;

        private float _oldVolumeTolerance;

        private AabbTree _tree;
        private bool[] _clashes; 

        public void ComputeAabbTree(IModel3D input)
        {
            if (_tree != null) return;
            var bounds = input.GetInstanceBounds();
            _tree = new AabbTree(bounds);
            _clashes = new bool[bounds.Count];
            for (int i = 0; i < bounds.Count; i++)
                _clashes[i] = OverlapsAny(_tree, bounds[i], i);
        }

        public bool OverlapsAny(AabbTree tree, Bounds3D bounds, int skipIndex)
        {
            var found = false;
            return tree.Traverse(
                shouldVisit: (in Bounds3D nodeBounds, out float priority) =>
                {
                    priority = 0;
                    return nodeBounds.OverlapVolume(bounds) >= VolumeTolerance;
                },
                visit: (int index, in Bounds3D itemBounds) =>
                {
                    if (index == skipIndex)
                        return false;

                    if (!itemBounds.Intersects(bounds))
                        return false;

                    found = true;
                    return true;
                },
                shouldStop: () => found);
        }

        
        public IModel3D Eval(IModel3D input)
        {
            if (VolumeTolerance != _oldVolumeTolerance)
            {
                _oldVolumeTolerance = VolumeTolerance;
                _tree = null;
            }

            ComputeAabbTree(input);
            var mb = new Model3DBuilder();
            var frameMesh = new BoxFrameMeshBuilder(FrameSize).Mesh.Triangulate();
            if (_tree != null)
            {
                if (DrawObjectBounds)
                {
                    var bs = _tree.GetAllBounds();
                    for (var index = 0; index < bs.Count; index++)
                    {
                        if (SkipNonClashes && !_clashes[index])
                            continue;
                        if (SkipClashes && _clashes[index])
                            continue;
                        var b = bs[index];
                        // TODO: color based on whether it "conflicts" or not. 
                        mb.AddInstance(frameMesh.FitToBounds(b));
                    }
                }

                if (DrawAabbTree)
                {
                    foreach (var b in _tree.GetAllNodeBounds())
                    {
                        // TODO: color based on whether it "conflicts" or not. 
                        mb.AddInstance(frameMesh.FitToBounds(b));
                    }
                }
            }

            if (KeepOriginal)    
                mb.AddModel(input);
            return mb.Build();
        }
    }
}
