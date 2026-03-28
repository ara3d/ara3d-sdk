using Color = Ara3D.Geometry.Color;

namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class PipesDemo : IGenerator, IAnimated
{
    [Range(10, 500)] public int GridCount = 40;
    [Range(1, 50)] public int PipeCount = 5;
    [Range(10, 1000)] public int MsecPerStep= 250;
    [Range(0, 1000)] public int RandomSeed = 567;
    [Range(10, 1000)] public int MaxSteps = 400;
    [Range(0, 1f)] public float ProbabilityOfTurning = 0.2f;
    [Range(0f, 20f)] public float GridElementSize = 1f;
    [Range(0f, 5f)] public float PipeRadius = 0.2f;

    public int CurStep => elapsedTimeInMsec / MsecPerStep;

    private int prevStep;
    private double startTime;
    private double currentTime;
    private int elapsedTimeInMsec => (int)((currentTime - startTime) * 1000.0);
    private Random rng;

    private bool[,,] occupied;
    private List<List<Integer3>> pipes;
    private List<Geometry.Color> colors;

    public void Reset(EvalContext context)
    {
        rng = new Random(RandomSeed);
        startTime = context.AnimationTime;
        prevStep = 0;
        occupied = new bool[GridCount, GridCount, GridCount];
        pipes = new List<List<Integer3>>();
        colors = new List<Color>();
        for (int i = 0; i < PipeCount; i++)
        {
            pipes.Add(new List<Integer3>());
            colors.Add(RandomPipeColor(rng));
        }
    }

    public Integer3 RandomSpawn(Random rng)
    {
        var a = rng.Next(GridCount);
        var b = rng.Next(GridCount);
        var c = rng.Next(GridCount);
        return (a, b, c);
    }

    public Integer3 ToDir(int dirIndex)
    {
        switch (dirIndex % 6)
        {
            case 0: return (1, 0, 0);
            case 1: return (-1, 0, 0);
            case 2: return (0, 1, 0);
            case 3: return (0, -1, 0);
            case 4: return (0, 0, +1);
            case 5: return (0, 0, -1);
        }

        return (0, 0, 0);
    }

    public bool IsOccupied(Integer3 i)
        => i.A < 0 || i.A >= GridCount ||
           i.B < 0 || i.B >= GridCount ||
           i.C < 0 || i.C >= GridCount ||
           occupied[i.A, i.B, i.C];

    public void SetOccupied(Integer3 i)
        => occupied[i.A, i.B, i.C] = true;

    public Integer3 Add(Integer3 a, Integer3 b)
        => (a.A + b.A, a.B + b.B, a.C + b.C);

    public bool IsValidDir(Integer3 i, Integer3 dir)
        => !dir.Equals(Integer3.Default) && !dir.Equals(Invalid) && !IsOccupied(Add(i, dir));

    public Integer3 RandomDir(Integer3 curr, Random rng)
    {
        var dirIndex = rng.Next(6);
        for (var i = 0; i < 6; i++)
        {
            var dir = ToDir(dirIndex + i);
            if (IsValidDir(curr, dir))
                return dir;
        }
        return Invalid;
            
    }

    public Integer3 RandomNext(Integer3 curr, Random rng)
    {
        var dir = RandomDir(curr, rng);
        if (dir.Equals(Invalid))
            return Invalid;
        return Add(curr, dir);
    }

    public Integer3 RandomNext(Integer3 curr, Integer3 prev, Random rng)
    {
        var dir = (curr.A - prev.A, curr.B - prev.B, curr.C - prev.C);

        if (rng.NextDouble() < ProbabilityOfTurning || !IsValidDir(curr, dir))
            return RandomNext(curr, rng);

        return Add(curr, dir);
    }

    public static Integer3 Invalid = (-1,-1,-1);

    public void AddToPipe(List<Integer3> pipe, Integer3 i)
    {
        if (IsOccupied(i))
            return;
        SetOccupied(i);
        pipe.Add(i);
    }

    public void NextStep(EvalContext context, Random rng)
    {
        prevStep = CurStep;
        foreach (var p in pipes)
        {
            if (p.Count == 0)
            {
                AddToPipe(p, RandomSpawn(rng));
            }
            else if (p.Count == 1)
            {
                AddToPipe(p, RandomNext(p[0], rng));
            }
            else
            {
                AddToPipe(p, RandomNext(p[^1], p[^2], rng));
            }
        }
    }

    private static Color RandomPipeColor(Random rng)
    {
        return new Color(
            0.3f + 0.7f * (float)rng.NextDouble(),
            0.3f + 0.7f * (float)rng.NextDouble(),
            0.3f + 0.7f * (float)rng.NextDouble(),
            1f);
    }

    public Point3D ToPoint(Integer3 i)
        => (i.A * GridElementSize, i.B * GridElementSize, i.C * GridElementSize);

    public Integer3 GetDirection(Integer3 from, Integer3 to)
        => (to.A - from.A, to.B - from.B, to.C - from.C);

    public TriangleMesh3D Cylinder(Number radius)
    {
        var poly = new RegularPolygon(Point2D.Zero, 32).ToPolyLine3D().Scale(radius / 2f);
        var mesh = poly.Points.Extrude(1);
        return mesh.Triangulate();
    }

    public TriangleMesh3D Elbow(Number radius)
    {
        var poly = new RegularPolygon(Point2D.Zero, 32).ToPolyLine3D().Scale(radius / 2f);
        var rows = new List<IReadOnlyList<Point3D>>();
        for (var i = 0; i <= 32; i++)
        {
            var angle = (i / 32f / 4).Turns();
            var offset = Vector3.UnitX / 2f;
            var tmp = poly.Translate(-offset).RotateY(angle).Translate(offset);
            rows.Add(tmp.Points);
        }
        var grid = rows.RowsToArray().ToQuadGrid3D(true, false);
        return grid.Triangulate();
    }

    public Matrix4x4 ElbowTransform(Integer3 dirPrev, Integer3 dirNext, Point3D junction)
    {
        // Convert grid dirs to normalized world vectors
        var z = new Vector3(dirPrev.A, dirPrev.B, dirPrev.C).Normalize;
        var x = new Vector3(dirNext.A, dirNext.B, dirNext.C).Normalize;

        // Right-handed basis: y = z x x   (because we want columns [x y z])
        var y = z.NormalizedCross(x);

        // If something went wrong (parallel), fall back
        if (y.LengthSquared() < 0.5f)
            y = Vector3.UnitY;

        var m = new Matrix4x4(
            x.X, x.Y, x.Z, 0,
            y.X, y.Y, y.Z, 0,
            z.X, z.Y, z.Z, 0,
            (float)junction.X, (float)junction.Y, (float)junction.Z, 1);

        return m;
    }

    public Integer3 GetDirectionInto(List<Integer3> points, int index)
    {
        if (index <= 0) return Invalid;
        if (index >= points.Count) return Invalid;
        return GetDirection(points[index - 1], points[index]);
    }

    public Integer3 GetDirectionOut(List<Integer3> points, int index)
    {
        if (index < 0) return Invalid;
        if (index >= points.Count - 1) return Invalid;
        return GetDirection(points[index], points[index+1]);
    }

    public bool IsElbowAt(List<Integer3> points, int index)
    {
        if (index < 0 || index >= points.Count) return true;
        return !GetDirectionInto(points, index).Equals(GetDirectionOut(points, index));
    }

    public Model3D Eval(EvalContext context)
    {
        currentTime = context.AnimationTime;
        if (occupied == null 
            || occupied.Length != GridCount * GridCount * GridCount
            || pipes == null 
            || pipes.Count != PipeCount 
            || CurStep >= MaxSteps)
        {
            Reset(context);
        }
        else if (CurStep != prevStep)
        {
            NextStep(context, rng);
        }

        var cyl = Cylinder(PipeRadius);
        var elbow = Elbow(PipeRadius);
        var instances = new List<InstanceStruct>();

        var cylIndex = 0;
        var elbowIndex = 1;


        for (var pi = 0; pi < pipes.Count; pi++)
        {
            var p = pipes[pi];
            var mat = Material.Default.WithColor(colors[pi]);

            for (var i = 1; i < p.Count; i++)
            {
                // 1) Draw the cylinder segment (possibly trimmed)
                var line = new Line3D(ToPoint(p[i - 1]), ToPoint(p[i]));
                var mid = line.Center;

                if (IsElbowAt(p, i - 1))
                    line = (mid, line.B);   // start trimmed
                if (IsElbowAt(p, i))
                    line = (line.A, mid);   // end trimmed

                if (line.Length >= 1e-5f)
                {
                    var tr = line.AlignZAxisTransform();
                    instances.Add(new InstanceStruct(0, tr, cylIndex, mat, 0));
                }

                // 2) Separately add the elbow (does not replace the cylinder)
                if (i < p.Count - 1 && IsElbowAt(p, i))
                {
                    var dirPrev = GetDirectionInto(p, i);
                    var dirNext = GetDirectionOut(p, i);
                    var offset = ToPoint(dirPrev).Vector3 * 0.5f;
                    var junction = ToPoint(p[i]).Vector3 - offset;

                    var elbowTr = ElbowTransform(dirPrev, dirNext, junction);
                    instances.Add(new InstanceStruct(0, elbowTr, elbowIndex, mat, 0));
                }
            }
        }

        return new Model3D([cyl, elbow], instances);
    }
}