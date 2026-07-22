namespace Ara3D.Studio.Samples.Selection;

/// <summary>
/// Topologically dilates (positive steps) or erodes (negative steps) the current
/// face selection across shared edges.
/// </summary>
[Category(Cat.Select)]
[Description("Grows (positive steps) or shrinks (negative steps) the current face selection across shared edges.")]
public class SelectGrowShrink : IModifier
{
    [Range(-10, 10)] public int Steps = 1;

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var selection = ctx.GetFaceSelection(mesh);
        if (selection == null || Steps == 0)
            return ctx.Input;
        var topology = mesh.DerivedTopology();
        var mask = selection.ToMask();
        for (var step = 0; step < Math.Abs(Steps); step++)
            mask = Steps > 0 ? Dilate(topology, mask) : Erode(topology, mask);
        return ctx.SelectFaces(mesh, SelectionCombine.Replace,
            SelectionSet.FromMask(FlowAttribute.AttributeDomain.Face, mask));
    }

    private static bool[] Dilate(Topology topology, bool[] mask)
        => MapNeighbors(topology, mask, (self, anyNeighbor) => self || anyNeighbor);

    private static bool[] Erode(Topology topology, bool[] mask)
        => MapNeighbors(topology, InvertMask(mask), (self, anyNeighbor) => !(self || anyNeighbor));

    private static bool[] InvertMask(bool[] mask)
    {
        var result = new bool[mask.Length];
        for (var i = 0; i < mask.Length; i++)
            result[i] = !mask[i];
        return result;
    }

    // For each face: op(mask[face], any edge-neighbor has mask set)
    private static bool[] MapNeighbors(Topology topology, bool[] mask, Func<bool, bool, bool> op)
    {
        var result = new bool[mask.Length];
        for (var f = 0; f < mask.Length; f++)
        {
            var anyNeighbor = false;
            foreach (var he in topology.GetHalfEdgeIds((FaceId)f))
            {
                if (topology.TryGetTwin(he, out var twin) && mask[(int)topology.GetAssociatedFaceId(twin)])
                {
                    anyNeighbor = true;
                    break;
                }
            }
            result[f] = op(mask[f], anyNeighbor);
        }
        return result;
    }
}
