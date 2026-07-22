namespace Ara3D.Studio.Samples;

/// <summary>
/// This is a class LSystem
/// https://en.wikipedia.org/wiki/L-system
/// </summary>
public class LSystem
{
    public string Axiom { get; }
    public IReadOnlyList<(string, string)> Rules { get; }

    public LSystem(string axiom, IReadOnlyList<(string, string)> rules)
    {
        Axiom = axiom;
        Rules = rules;
    }

    public string ApplyProduction(string current, ref int index)
    {
        // To simplify code we are going to only look at the string from the current index
        current = current[index..];

        // Try matching each rule at the current index
        foreach (var (input, output) in Rules)
        {
            Debug.Assert(input.Length > 0);

            if (current.StartsWith(input))
            {
                index += input.Length;
                return output;
            }
        }

        // According to L-Systems, if no rule matches, we use the "Identity" rule
        // In practice this means, just return the first character if no rules match 
        index++;
        return current[0].ToString();
    }

    public string Eval(string current)
    {
        var sb = new StringBuilder();
        var i = 0;
        while (i < current.Length)
        {
            var tmp = i;
            sb.Append(ApplyProduction(current, ref i));
            Debug.Assert(tmp < i);
        }

        return sb.ToString();
    }

    public string Eval(int iterations, string axiom = null)
    {
        var r = axiom ?? Axiom;
        for (var i = 0; i < iterations; i++)
        {
            r = Eval(r);
        }
        return r;
    }
}

[Category(Cat.Fields)]
public class LSystemFractal : IGenerator
{
    [Range(1, 10)] public int Iterations = 3;
    [Range(0f, 1f)] public float InitialLength = 0.12f;
    [Range(0f, 1f)] public float Ratio = 0.6f;
    [Range(0f, 10f)] public float Size = 0.5f;

    public LSystem LSystem = new LSystem("0", [("1", "11"), ("0", "1[0]0")]);

    public Line2D CreateLine(Vector2 pos, float angle, float length)
    {
        var radians = MathF.PI / 180f * angle;
        var dir = new Vector2(MathF.Cos(radians), MathF.Sin(radians));
        var end = pos + dir * length;
        return new Line2D(pos, end);
    }

    public List<Line2D> ConstructLines(string s)
    {
        var lines = new List<Line2D>();

        Stack<Vector2> positions = new();
        Stack<int> angles = new();
        Stack<float> lengths = new();

        var pos = Vector2.Zero;
        var angle = 0;
        var length = InitialLength;

        foreach (var c in s)
        {
            switch (c)
            {
                case '[':
                    positions.Push(pos);
                    angles.Push(angle);
                    lengths.Push(length);
                    angle -= 45;
                    length *= Ratio;
                    break;

                case '0':
                    lines.Add(CreateLine(pos, angle, length));
                    break;

                case '1':
                    var line = CreateLine(pos, angle, length);
                    lines.Add(line);
                    pos = line.B;
                    break;

                case ']':
                    pos = positions.Peek();
                    angle = angles.Peek();
                    length = lengths.Peek();
                    angle += 45;
                    length *= Ratio;

                    positions.Pop();
                    angles.Pop();
                    lengths.Pop();
                    break;

                default:
                    throw new Exception($"Unrecognized character `{c}`");
            }
        }

        return lines;
    }

    public static Matrix4x4 GetBoxTransform(Line3D line, float thickness = 1f)
    {
        float len = line.Length;
        if (len <= 0f || float.IsNaN(len))
            return Matrix4x4.CreateScale(0f) * Matrix4x4.CreateTranslation(line.Center);

        Vector3 dir = line.Direction.Normalize;
        var q = QuaternionBetween(Vector3.UnitX, dir);

        // Scale along local X by length; Y/Z by thickness.
        var S = Matrix4x4.CreateScale(len, thickness, thickness);
        var R = Matrix4x4.CreateFromQuaternion(q);
        var T = Matrix4x4.CreateTranslation(line.Center);

        // Apply S, then R, then T
        return S * R * T;
    }

    // Shortest-arc rotation from 'from' to 'to'
    private static Quaternion QuaternionBetween(in Vector3 from, in Vector3 to)
    {
        Vector3 f = from.Normalize;
        Vector3 t = to.Normalize;
        float d = Math.Clamp(Vector3.Dot(f, t), -1f, 1f);

        if (d > 0.999999f)
            return Quaternion.Identity;

        if (d < -0.999999f)
        {
            // 180°: pick an arbitrary orthogonal axis
            Vector3 axis = Vector3.Cross(f, Vector3.UnitY);
            if (axis.LengthSquared() < 1e-8f)
                axis = Vector3.Cross(f, Vector3.UnitZ);
            axis = axis.Normalize;
            return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
        }

        Vector3 axisN = Vector3.Cross(f, t).Normalize;
        float angle = MathF.Acos(d);
        return Quaternion.CreateFromAxisAngle(axisN, angle);
    }

    public IModel3D Eval(EvalContext context)
    {
        var s = LSystem.Eval(Iterations);
        var lines = ConstructLines(s);
        var mb = new Model3DBuilder();
        mb.Meshes.Add(PlatonicSolids.TriangulatedCube);

        foreach (var line in lines)
        {
            var line3d = new Line3D(line.A.To3D(), line.B.To3D());
            var transform = line3d.ToBoxTransform(Size, Size);
            mb.AddInstance(0, transform);
        }

        return mb.Build();
    }
}