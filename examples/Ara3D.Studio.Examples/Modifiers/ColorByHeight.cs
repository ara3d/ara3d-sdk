namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Colors vertices by height (Z), but carries the result as a Color *channel* on the
/// FlowObject instead of returning a colored-mesh wrapper: the geometry passes through as
/// a bare TriangleMesh3D and the host materializes the color at publish time (see
/// FlowColorExtensions.ApplyColorChannel). This is the channel form of VertexNormalColor —
/// it demonstrates that any geometry flowing through can carry an optional color channel,
/// so downstream modifiers still see a plain mesh while the viewport still draws it colored.
/// </summary>
[Category(Cat.Color)]
[Description("Colors vertices by height and carries it as a Color channel on the FlowObject, leaving the geometry a bare mesh.")]
public class ColorByHeight : IModifier
{
    public Vector3 LowColor = new(0.1f, 0.2f, 0.8f);
    public Vector3 HighColor = new(1f, 0.9f, 0.2f);

    public FlowObject Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var points = mesh.Points;
        var minZ = float.MaxValue;
        var maxZ = float.MinValue;
        for (var i = 0; i < points.Count; i++)
        {
            var z = (float)points[i].Vector3.Z;
            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }
        var range = maxZ - minZ;

        var colors = new Vector3[points.Count];
        for (var i = 0; i < points.Count; i++)
        {
            var t = range > 0f ? ((float)points[i].Vector3.Z - minZ) / range : 0f;
            colors[i] = LowColor.Lerp(HighColor, t);
        }

        return ctx.Input
            .WithNewContent(mesh, FlowAttribute.AttributeDomainMask.All)
            .WithColors(colors);
    }
}
