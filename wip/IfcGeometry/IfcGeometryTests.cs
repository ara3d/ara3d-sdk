using System.Diagnostics;
using System.Reflection;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.IfcGeometry
{
    public static class IfcGeometry
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
            logger.Log($"Express ids: {file.Document.Definitions.Count}");
            logger.Log($"Geometry loaded: {file.GeometryDataLoaded}");
            logger.Log($"Num geometries loaded: {file.Model?.GetNumGeometries()}");
            logger.Log($"Num meshes loaded: {file.Model?.GetGeometries().Select(g => g.GetNumMeshes()).Sum()}");
        }

        public static string GeometryTypesMerged = @"IfcAdvancedBrep
IfcAdvancedBrepWithVoids
IfcAdvancedFace
IfcArbitraryClosedProfileDef
IfcArbitraryOpenProfileDef
IfcArbitraryProfileDefWithVoids
IfcAsymmetricIShapeProfileDef
IfcAxis1Placement
IfcAxis2Placement2D
IfcAxis2Placement3D
IfcBSplineCurve
IfcBSplineCurveWithKnots
IfcBSplineSurface
IfcBSplineSurfaceWithKnots
IfcBlock
IfcBooleanClippingResult
IfcBooleanResult
IfcBoxedHalfSpace
IfcCShapeProfileDef
IfcCartesianPoint
IfcCartesianTransformationOperator
IfcCartesianTransformationOperator2D
IfcCartesianTransformationOperator2DnonUniform
IfcCartesianTransformationOperator3D
IfcCartesianTransformationOperator3DnonUniform
IfcCircle
IfcCircleHollowProfileDef
IfcClosedShell
IfcCompositeCurve
IfcCompositeCurveSegment
IfcCompositeProfileDef
IfcConic
IfcConicalSurface
IfcConnectedFaceSet
IfcCoordinateReferenceSystem
IfcCurve
IfcCurveBoundedPlane
IfcCurveBoundedSurface
IfcCsgPrimitive3D
IfcCsgSolid
IfcDerivedProfileDef
IfcDirection
IfcEdge
IfcEdgeCurve
IfcEdgeLoop
IfcElementarySurface
IfcEllipse
IfcEllipseProfileDef
IfcExtrudedAreaSolid
IfcExtrudedDiskSolid
IfcFace
IfcFaceBasedSurfaceModel
IfcFaceBound
IfcFaceOuterBound
IfcFaceSurface
IfcFacetedBrep
IfcFacetedBrepWithVoids
IfcGeometricCurveSet
IfcGeometricRepresentationContext
IfcGeometricRepresentationItem
IfcGeometricRepresentationSubContext
IfcGeometricSet
IfcHalfSpaceSolid
IfcIShapeProfileDef
IfcIndexedPolyCurve
IfcIndexedPolygonalFace
IfcIndexedPolygonalFaceWithVoids
IfcIsoscelesTrapezoidProfileDef
IfcLine
IfcLocalPlacement
IfcLoop
IfcLShapeProfileDef
IfcMappedItem
IfcManifoldSolidBrep
IfcObjectPlacement
IfcOffsetCurve2D
IfcOffsetCurve3D
IfcOpenShell
IfcOrientedEdge
IfcParameterizedProfileDef
IfcParabola
IfcPlacement
IfcPlane
IfcPoint
IfcPointOnCurve
IfcPointOnSurface
IfcPolygonalBoundedHalfSpace
IfcPolygonalFaceSet
IfcPolyline
IfcPolyLoop
IfcProfileDef
IfcRationalBSplineCurveWithKnots
IfcRationalBSplineSurfaceWithKnots
IfcRectangleProfileDef
IfcRectangularPyramid
IfcRectangularTrimmedSurface
IfcRepresentation
IfcRepresentationContext
IfcRepresentationItem
IfcRepresentationMap
IfcRhombusProfileDef
IfcRightCircularCone
IfcRightCircularCylinder
IfcRightTriangularProfileDef
IfcRoundedRectangleProfileDef
IfcSectionedSolid
IfcSectionedSolidHorizontal
IfcSegment
IfcShapeAspect
IfcShapeRepresentation
IfcShellBasedSurfaceModel
IfcSolidModel
IfcSphere
IfcSphericalSurface
IfcSweptAreaSolid
IfcSweptDiskSolid
IfcSweptDiskSolidPolygonal
IfcSweptSurface
IfcSurface
IfcSurfaceCurveSweptAreaSolid
IfcSurfaceOfLinearExtrusion
IfcSurfaceOfRevolution
IfcTessellatedFaceSet
IfcTessellatedItem
IfcTopologicalRepresentationItem
IfcToroidalSurface
IfcTShapeProfileDef
IfcTriangulatedFaceSet
IfcTriangularPrism
IfcTrimmedCurve
IfcUShapeProfileDef
IfcVector
IfcVertex
IfcVertexPoint
IfcZShapeProfileDef
IfcZeeProfileDef";

        public static IReadOnlyList<string> GeometryTypes = GeometryTypesMerged.Split(['\n', '\r', ' '],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        [Test]
        public static void TestLoadTimes()
        {
            var logger = Logger.Console;
            var dir = new DirectoryPath(@"C:\Users\cdigg\data\Ara3D\impraria\0000100120-093 - OXAGON ADVANCED HEALTH CENTER\STAGE 3A - CONCEPT DESIGN");
            var files = dir.GetFiles("*.ifc", true).ToList();
            foreach (var filePath in files.Take(1))
            {
                var doc = StepDocument.Create(filePath);
                logger.Log($"Loaded {filePath.GetFileName()}");
            }
        }

        [Test]
        public static void TestEcho()
        {
            var logger = Logger.Console;
            //var (rd, file) = RunDetails.LoadGraph(InputFile, false, logger);
            //OutputDetails(file, logger);
            //Console.WriteLine(rd.Header());
            //Console.WriteLine(rd.RowData());
            var doc = new StepDocument(InputFile, logger);
            logger.Log($"Loaded {doc.FilePath.GetFileName()}");
            var cnt = 0;
            foreach (var kv in doc.Definitions)
            {
                var def = kv.Value;
                Debug.Assert(kv.Key == def.Id);
                //Console.WriteLine($"#{def.Id}= {def}");
            }
        }
    }
}