namespace Ara3D.Geometry;

public readonly struct Cylinder
{
    public Line3D Line { get; }
    public float Radius { get; }

    public Cylinder(Line3D line, float radius)
    {
        Line = line;
        Radius = radius;
    }

    public float Length() => Line.Length;
    public Cylinder WithRadius(float radius) => new(Line, radius);
    public Cylinder WithLine(Line3D line) => new(line, Radius);
    public Cylinder WithLineStart(Point3D point) => WithLine(Line.WithA(point));
    public Cylinder WithLineEnd(Point3D point) => WithLine(Line.WithB(point));
    public Cylinder Transform(Matrix4x4 m)
    {
        var newLine = Line.Transform(m);
        var scaleFactor = newLine.Length / Length();
        var newRadius = Radius * scaleFactor;
        return new(newLine, newRadius);
    }

    public Cylinder Scale(float f)
        => new(Line.Scale(f), Radius * f);
}

