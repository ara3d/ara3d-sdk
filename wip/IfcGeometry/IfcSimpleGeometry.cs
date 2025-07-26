namespace Ara3D.IfcGeometry.Simple;

public class IfcSimpleGeometry
{ }

public class IfcLoop
{
    public List<IfcVector> Points { get; } 
}

public class IfcFaceBounds
{
    public IfcLoop IfcLoop;
    public bool Orientation;
}

public class IfcFace
{
    public IfcFaceBounds OuterBounds;
    public List<IfcFaceBounds> InnerBounds;
}

// IfcCartesianPoint
// IfcDirection 
public class IfcVector
{
    public double X;
    public double Y;
    public double Z;
}

public class IfcClosedShell
{
    public List<IfcFace> Faces;
}

public class IfcFacetedBrep
{
    public IfcClosedShell Shell;
}

public class IfcAxis2Placement2D
{
    public IfcVector RefDirection;
    
    // P[1]: The normalized direction of the placement X Axis.This is [1.0, 0.0] if RefDirection is omitted.
    public IfcVector P1;

    // P[2]: The normalized direction of the placement Y Axis. This is a derived attribute and is orthogonal to P[1]. If RefDirection is omitted, it defaults to[0.0, 1.0]
    public IfcVector P2;
}