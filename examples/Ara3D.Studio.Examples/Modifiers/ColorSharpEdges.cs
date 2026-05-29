namespace Ara3D.Studio.Samples.Modifiers;

/*
public class SeparateAtSharpCorners : IModifier
{
    [Range(0, 360)] public int AngleTolerance = 95;

    public IModel3D Eval(TriangleMesh3D mesh, EvalContext context)
    {
        var topology = mesh.GetTopology();

    }
}
*/

public class ColorSharpEdges : IModifier
{
    public static Angle HalfTurn = 180.Degrees();

    public static Angle DistanceFromPlane(Angle a)
    {
        // TODO: I am not sure about this math
        return Math.Abs((HalfTurn - a).Value);
    }

    public static Angle VertexAngle(TopoVertex v)
    {
        var angles = v.EdgeAngles.Select(DistanceFromPlane).ToList();
        if (angles.Count == 0)
            return HalfTurn;
        // TODO: there is a bug because Angles are not IComparable.
        return angles.Select(v => v.Value).Max();
    }

    public Vector3 ColorA = new(0, 1, 0);
    public Vector3 ColorB = new (1, 0, 0);

    public float ToFloat01(Angle a)
        => Math.Clamp(a.Value / HalfTurn.Value, 0, 1);

    public Vector3 GetColor(Angle a)
        => ColorA.Lerp(ColorB, ToFloat01(a));

    public Vector3 GetColor(TopoVertex v)
        => GetColor(VertexAngle(v));

    public ColoredTriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext context)
    {
        var topology = mesh.GetTopology();
        var colors = topology.GetVertices().Select(GetColor);
        return mesh.ToColored(colors);
    }
}