using System.Reflection;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcGeometry
{
    public abstract record IfcClass();

    public abstract record IfcRepresentationItem()
        : IfcClass();

    public abstract record IfcGeometricRepresentationItem()
        : IfcRepresentationItem();

    public abstract record IfcTopologicalRepresentationItem()
        : IfcRepresentationItem();

    public record IfcCartesianPoint(List<double> Coordinates)
        : IfcGeometricRepresentationItem();

    public record IfcDirection(List<double> DirectionRatios)
        : IfcGeometricRepresentationItem();

    public record IfcVector(IfcDirection Orientation, double Magnitude)
        : IfcGeometricRepresentationItem();

    public record IfcAxis1Placement(IfcCartesianPoint Location, IfcDirection Axis)
        : IfcGeometricRepresentationItem();

    public abstract record IfcPlacement(IfcCartesianPoint Location)
        : IfcGeometricRepresentationItem();

    public record IfcAxis2Placement2D(IfcCartesianPoint Location, IfcDirection? RefDirection = null)
        : IfcPlacement(Location);

    public record IfcAxis2Placement3D(
        IfcCartesianPoint Location,
        IfcDirection? Axis = null,
        IfcDirection? RefDirection = null
    ) : IfcPlacement(Location);

    public abstract record IfcCurve()
        : IfcGeometricRepresentationItem();

    public abstract record IfcBoundedCurve()
        : IfcCurve();

    public record IfcPolyline(List<IfcCartesianPoint> Points)
        : IfcBoundedCurve();

    public record IfcLine(IfcCartesianPoint Pnt, IfcVector Dir)
        : IfcCurve();

    public abstract record IfcConic(IfcAxis2Placement2D Position)
        : IfcCurve();

    public record IfcCircle(IfcAxis2Placement2D Position, double Radius)
        : IfcConic(Position);

    public record IfcEllipse(IfcAxis2Placement2D Position, double SemiMajorAxis, double SemiMinorAxis)
        : IfcConic(Position);

    public record IfcTrimmedCurve(
        IfcCurve BasisCurve,
        List<double>? Trim1,
        List<double>? Trim2,
        bool SenseAgreement,
        string? MasterRepresentation
    ) : IfcBoundedCurve();

    public abstract record IfcSurface() : IfcGeometricRepresentationItem();

    public abstract record IfcElementarySurface(IfcAxis2Placement3D Position) : IfcSurface();

    public record IfcPlane(IfcAxis2Placement3D Position)
        : IfcElementarySurface(Position);

    public record IfcCylindricalSurface(IfcAxis2Placement3D Position, double Radius)
        : IfcElementarySurface(Position);

    public record IfcSurfaceOfLinearExtrusion(
        IfcProfileDef SweptCurve,
        IfcDirection ExtrudedDirection,
        double Depth
    ) : IfcSurface();

    public record IfcSurfaceOfRevolution(
        IfcProfileDef SweptCurve,
        IfcAxis1Placement AxisPosition
    ) : IfcSurface();

    public abstract record IfcSolidModel()
        : IfcGeometricRepresentationItem();

    public abstract record IfcSweptAreaSolid(IfcProfileDef SweptArea, IfcAxis2Placement3D? Position = null)
        : IfcSolidModel();

    public record IfcExtrudedAreaSolid(
        IfcProfileDef SweptArea,
        IfcAxis2Placement3D? Position,
        IfcDirection ExtrudedDirection,
        double Depth
    ) : IfcSweptAreaSolid(SweptArea, Position);

    public record IfcRevolvedAreaSolid(
        IfcProfileDef SweptArea,
        IfcAxis2Placement3D? Position,
        IfcAxis1Placement Axis,
        double Angle
    ) : IfcSweptAreaSolid(SweptArea, Position);

    public abstract record IfcProfileDef(string? ProfileName)
        : IfcClass();

    public abstract record IfcParameterizedProfileDef(string? ProfileName)
        : IfcProfileDef(ProfileName);

    public record IfcRectangleProfileDef(string? ProfileName, double XDim, double YDim)
        : IfcParameterizedProfileDef(ProfileName);

    public record IfcCircleProfileDef(string? ProfileName, double Radius)
        : IfcParameterizedProfileDef(ProfileName);

    public abstract record IfcBooleanResult(
        string Operation,
        IfcSolidModel FirstOperand,
        IfcSolidModel SecondOperand
    ) : IfcSolidModel();

    public record IfcBooleanClippingResult(
        string Operation,
        IfcSolidModel FirstOperand,
        IfcSolidModel SecondOperand
    ) : IfcBooleanResult(Operation, FirstOperand, SecondOperand);

    public abstract record IfcManifoldSolidBrep(IfcClosedShell Outer)
        : IfcSolidModel();

    public record IfcFacetedBrep(IfcClosedShell Outer) : IfcManifoldSolidBrep(Outer);

    public record IfcAdvancedBrep(IfcClosedShell Outer)
        : IfcManifoldSolidBrep(Outer);

    public record IfcShellBasedSurfaceModel(List<IfcOpenShell> SbsmBoundary)
        : IfcGeometricRepresentationItem();

    public record IfcFaceBasedSurfaceModel(List<IfcConnectedFaceSet> FbsmFaces)
        : IfcGeometricRepresentationItem();

    public abstract record IfcTessellatedItem()
        : IfcGeometricRepresentationItem();

    public record IfcPolygonalFaceSet(
        IfcCartesianPointList3D Coordinates,
        bool Closed,
        List<IfcIndexedPolygonalFace> Faces
    ) : IfcTessellatedItem();

    public record IfcCartesianPointList3D(List<List<double>> CoordList) : IfcClass();

    public record IfcIndexedPolygonalFace(List<int> CoordIndex) : IfcClass();

    public abstract record IfcTopologicalRepresentationItem2DOr3D()
        : IfcTopologicalRepresentationItem(); // (Not an official IFC entity, just a placeholder if needed)

    public abstract record IfcVertex()
        : IfcTopologicalRepresentationItem();

    public record IfcVertexPoint(IfcPointOrVertexPoint? VertexGeometry)
        : IfcVertex(); // In IFC, IfcPointOrVertexPoint is a select. Here as nullable?

    public abstract record IfcEdge(IfcVertex EdgeStart, IfcVertex EdgeEnd)
        : IfcTopologicalRepresentationItem();

    public record IfcEdgeCurve(
        IfcVertex EdgeStart,
        IfcVertex EdgeEnd,
        IfcCurve EdgeGeometry,
        bool SameSense
    ) : IfcEdge(EdgeStart, EdgeEnd);

    public record IfcOrientedEdge(
        IfcEdge EdgeElement,
        bool Orientation
    ) : IfcEdge(EdgeElement.EdgeStart, EdgeElement.EdgeEnd);

    public record IfcPath(List<IfcOrientedEdge> EdgeList)
        : IfcTopologicalRepresentationItem();

    public abstract record IfcLoop()
        : IfcTopologicalRepresentationItem();

    public record IfcEdgeLoop(List<IfcOrientedEdge> EdgeList)
        : IfcLoop();

    public record IfcVertexLoop(IfcVertex LoopVertex)
        : IfcLoop();

    public abstract record IfcFaceBound(IfcLoop Bound, bool Orientation)
        : IfcTopologicalRepresentationItem();

    public record IfcFaceOuterBound(IfcLoop Bound, bool Orientation)
        : IfcFaceBound(Bound, Orientation);

    public record IfcFace(List<IfcFaceBound> Bounds)
        : IfcTopologicalRepresentationItem();

    public record IfcFaceSurface(List<IfcFaceBound> Bounds, IfcSurface FaceSurface, bool SameSense)
        : IfcFace(Bounds);

    public record IfcConnectedFaceSet(List<IfcFace> CfsFaces)
        : IfcTopologicalRepresentationItem();

    public record IfcClosedShell(List<IfcFace> CfsFaces)
        : IfcConnectedFaceSet(CfsFaces);

    public record IfcOpenShell(List<IfcFace> CfsFaces)
        : IfcConnectedFaceSet(CfsFaces);

    // In IFC, IfcPointOrVertexPoint and other SELECT types are not formal entities. 
    // You may adapt those to your own usage or code-generation strategy as needed.
    public record IfcPointOrVertexPoint(IfcCartesianPoint? Point, IfcVertex? Vertex)
        : IfcClass();

    public class IfcFactory
    {
        public Dictionary<uint, StepInstance> StepInstances  = new Dictionary<uint, StepInstance>();
        public Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public Dictionary<uint, IfcClass> IfcInstances = new Dictionary<uint, IfcClass>();

        public IfcFactory()
        {
            Types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Name.StartsWith("Ifc"))
                .ToDictionary(t => t.Name.ToUpperInvariant(), t => t);
        }

        public void AddValue(StepInstance inst)
            => StepInstances.Add(inst.Id, inst);
       

        public Type? GetIfcType(StepInstance inst)
            => Types.GetValueOrDefault(inst.EntityType);

        public IfcClass GetCreateIfcInstance(uint id)
        {
            if (IfcInstances.TryGetValue(id, out var ifcInstance))
                return ifcInstance;

            if (!StepInstances.TryGetValue(id, out var stepInstance))
                throw new Exception($"Could not find step instance {id}");

            throw new NotImplementedException();
            /*
            var type = Types.
            var vals = stepInstance.AttributeValues;
            var ctor = type
            if (vals.Count != )

            var ic = Activator.CreateInstance(type) as IfcClass;
            if (ic == null)
                throw new Exception($"Unable to create instance of {type}");
            IfcInstances.Add(inst.Id, ic);
            */
        }
    }


    public static class Tests
    {
        public static FilePath InputFile =
            PathUtil.GetCallerSourceFolder().RelativeFile("..", "..", "data", "AC20-FZK-Haus.ifc");

        public static HashSet<string> GetLocalTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Select(t => t.Name.ToUpperInvariant())
                .Where(n => n.StartsWith("IFC")).ToHashSet();
        }

        public static void OutputDetails(IfcFile file, ILogger logger)
        {
            logger.Log($"Loaded {file.FilePath.GetFileName()}");
            logger.Log($"Graph nodes: {file.Graph.Nodes.Count}");
            logger.Log($"Graph relations: {file.Graph.Relations.Count}");
            logger.Log($"Graph roots: {file.Graph.RootIds.Count}");
            logger.Log($"Property sets: {file.Graph.GetPropSets().Count()}");
            logger.Log($"Property values: {file.Graph.GetProps().Count()}");
            logger.Log($"Express ids: {file.Document.NumRawInstances}");
            logger.Log($"Geometry loaded: {file.GeometryDataLoaded}");
            logger.Log($"Num geometries loaded: {file.Model?.GetNumGeometries()}");
            logger.Log($"Num meshes loaded: {file.Model?.GetGeometries().Select(g => g.GetNumMeshes()).Sum()}");
        }
        [Test]
        public static void Test1()
        {
            var logger = Logger.Console;
            var (rd, file) = RunDetails.LoadGraph(InputFile, false, logger);
            OutputDetails(file, logger);
            Console.WriteLine(rd.Header());
            Console.WriteLine(rd.RowData());
            var localTypes = GetLocalTypes();
            var doc = file.Document;
            var numbers = new List<double>();
            var f = new IfcFactory();
            var d = new Dictionary<string, List<StepInstance>>();
            var cnt = 0;
            foreach (var rawInstance in file.Document.RawInstances)
            {
                if (rawInstance.Type.IsNull)
                    continue;

                var str = rawInstance.Type.ToString().ToUpperInvariant();
                if (!localTypes.Contains(str))
                    continue;

                var inst = doc.GetInstanceWithData(rawInstance);
                GatherNumbers(inst.AttributeValues, numbers);
                cnt++;

                f.AddValue(inst);

                if (!d.ContainsKey(str))
                    d[str] = new List<StepInstance>() { inst };
                else
                    d[str].Add(inst);
            }

            Console.WriteLine($"Found a total of {cnt} instances, and {numbers.Count} numbers");
            foreach (var kv in d.OrderBy(kv => kv.Key))
                Console.WriteLine($"{kv.Key} = {kv.Value.Count}");
        }

        public static void GatherNumbers(List<StepValue> list, List<double> numbers)
        {
            foreach (var tmp in list)
            {
                if (tmp is StepNumber n)
                    numbers.Add(n.Value);
                else if (tmp is StepList stepList)
                    GatherNumbers(stepList.Values, numbers);
            }
        }
    }
}