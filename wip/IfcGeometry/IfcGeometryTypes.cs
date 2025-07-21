// IFC 4.x Geometry & Topology – core subset for 3‑D extraction / generation
// Author: Ara 3D
// NOTE: Only entities relevant to geometry have been included.
using System.Collections.Generic;

namespace Ara3D.IfcGeometry;

/// <summary>Marker interface – any entity that is a valid IfcBooleanOperand implements this.</summary>
    public interface IIfcBooleanOperand { }

    public abstract record IfcClass;

    public abstract record IfcRepresentationItem() : IfcClass;

    public abstract record IfcGeometricRepresentationItem() : IfcRepresentationItem;

    public abstract record IfcTopologicalRepresentationItem() : IfcRepresentationItem;

    public record IfcCartesianPoint(IReadOnlyList<double> Coordinates) : IfcGeometricRepresentationItem;

    public record IfcDirection(IReadOnlyList<double> DirectionRatios) : IfcGeometricRepresentationItem;

    public record IfcVector(IfcDirection Orientation, double Magnitude) : IfcGeometricRepresentationItem;

    public abstract record IfcPlacement(IfcCartesianPoint Location) : IfcGeometricRepresentationItem;

    public record IfcAxis1Placement(IfcCartesianPoint Location, IfcDirection Axis) : IfcPlacement(Location);

    public record IfcAxis2Placement2D(IfcCartesianPoint Location, IfcDirection? RefDirection = null) : IfcPlacement(Location);

    public record IfcAxis2Placement3D(IfcCartesianPoint Location,
                                       IfcDirection? Axis = null,
                                       IfcDirection? RefDirection = null) : IfcPlacement(Location);

    public abstract record IfcCartesianTransformationOperator() : IfcGeometricRepresentationItem;

    public record IfcCartesianTransformationOperator2D(IfcDirection? Axis1,
                                                       IfcDirection? Axis2,
                                                       IfcCartesianPoint LocalOrigin,
                                                       double? Scale = null) : IfcCartesianTransformationOperator;

    public record IfcCartesianTransformationOperator3D(IfcDirection? Axis1,
                                                       IfcDirection? Axis2,
                                                       IfcDirection? Axis3,
                                                       IfcCartesianPoint LocalOrigin,
                                                       double? Scale = null) : IfcCartesianTransformationOperator;

    public record IfcRepresentationMap(IfcAxis2Placement3D MappingOrigin,
                                       IfcRepresentationItem MappedRepresentation) : IfcClass;

    public record IfcMappedItem(IfcRepresentationMap MappingSource,
                                IfcCartesianTransformationOperator MappingTarget) : IfcGeometricRepresentationItem;

    public abstract record IfcCurve() : IfcGeometricRepresentationItem;

    public abstract record IfcBoundedCurve() : IfcCurve;

    public record IfcPolyline(IReadOnlyList<IfcCartesianPoint> Points) : IfcBoundedCurve;

    public record IfcLine(IfcCartesianPoint Pnt, IfcVector Dir) : IfcCurve;

    public abstract record IfcConic(IfcAxis2Placement2D Position) : IfcCurve;
    public record IfcCircle(IfcAxis2Placement2D Position, double Radius) : IfcConic(Position);
    public record IfcEllipse(IfcAxis2Placement2D Position,
                              double SemiMajorAxis,
                              double SemiMinorAxis) : IfcConic(Position);

    public record IfcCompositeCurveSegment(IfcCurve ParentCurve, bool SameSense) : IfcClass;

    public record IfcCompositeCurve(IReadOnlyList<IfcCompositeCurveSegment> Segments,
                                    bool SelfIntersect) : IfcBoundedCurve;

    public record IfcBSplineCurveWithKnots(int Degree,
                                           IReadOnlyList<IfcCartesianPoint> ControlPointsList,
                                           string CurveForm,
                                           bool ClosedCurve,
                                           bool SelfIntersect,
                                           IReadOnlyList<int> KnotMultiplicities,
                                           IReadOnlyList<double> Knots,
                                           string KnotSpec) : IfcBoundedCurve;

    public record IfcTrimmingSelect(double? ParameterValue, IfcCartesianPoint? Point);

    public record IfcTrimmedCurve(IfcCurve BasisCurve,
                                  IReadOnlyList<IfcTrimmingSelect> Trim1,
                                  IReadOnlyList<IfcTrimmingSelect> Trim2,
                                  bool SenseAgreement,
                                  string MasterRepresentation) : IfcBoundedCurve;

    public abstract record IfcSurface() : IfcGeometricRepresentationItem;

    public abstract record IfcElementarySurface(IfcAxis2Placement3D Position) : IfcSurface;

    public record IfcPlane(IfcAxis2Placement3D Position) : IfcElementarySurface(Position);
    public record IfcCylindricalSurface(IfcAxis2Placement3D Position, double Radius) : IfcElementarySurface(Position);
    public record IfcConicalSurface(IfcAxis2Placement3D Position, double Radius, double SemiAngle) : IfcElementarySurface(Position);
    public record IfcSphericalSurface(IfcAxis2Placement3D Position, double Radius) : IfcElementarySurface(Position);
    public record IfcToroidalSurface(IfcAxis2Placement3D Position, double MajorRadius, double MinorRadius) : IfcElementarySurface(Position);

    public record IfcSurfaceOfLinearExtrusion(IfcProfileDef SweptCurve,
                                              IfcDirection ExtrudedDirection,
                                              double Depth) : IfcSurface;

    public record IfcSurfaceOfRevolution(IfcProfileDef SweptCurve,
                                         IfcAxis1Placement AxisPosition) : IfcSurface;

    public abstract record IfcSolidModel() : IfcGeometricRepresentationItem, IIfcBooleanOperand;

    public abstract record IfcSweptAreaSolid(IfcProfileDef SweptArea,
                                             IfcAxis2Placement3D? Position = null) : IfcSolidModel;

    public record IfcExtrudedAreaSolid(IfcProfileDef SweptArea,
                                       IfcAxis2Placement3D? Position,
                                       IfcDirection ExtrudedDirection,
                                       double Depth) : IfcSweptAreaSolid(SweptArea, Position);

    public record IfcRevolvedAreaSolid(IfcProfileDef SweptArea,
                                       IfcAxis2Placement3D? Position,
                                       IfcAxis1Placement Axis,
                                       double Angle) : IfcSweptAreaSolid(SweptArea, Position);

    public record IfcSweptDiskSolid(IfcCurve Directrix,
                                     double Radius,
                                     double? InnerRadius = null,
                                     double? StartParam = null,
                                     double? EndParam = null) : IfcSolidModel;

    public record IfcSweptDiskSolidPolygonal(IfcCurve Directrix,
                                             double Radius,
                                             double? InnerRadius,
                                             int NumberOfSegments,
                                             double? StartParam = null,
                                             double? EndParam = null) : IfcSolidModel;

    public record IfcSurfaceCurveSweptAreaSolid(IfcProfileDef SweptArea,
                                                IfcCurve Directrix,
                                                IfcVector? ReferenceSurfaceNormal = null) : IfcSweptAreaSolid(SweptArea, null);

    public record IfcFixedReferenceSweptAreaSolid(IfcProfileDef SweptArea,
                                                  IfcCurve Directrix,
                                                  IfcDirection FixedReference,
                                                  double? StartParam = null,
                                                  double? EndParam = null) : IfcSweptAreaSolid(SweptArea, null);

    public record IfcExtrudedAreaSolidTapered(IfcProfileDef SweptArea,
                                              IfcAxis2Placement3D? Position,
                                              IfcDirection ExtrudedDirection,
                                              double Depth,
                                              IfcProfileDef EndSweptArea) : IfcSweptAreaSolid(SweptArea, Position);

    public record IfcRevolvedAreaSolidTapered(IfcProfileDef SweptArea,
                                              IfcAxis2Placement3D? Position,
                                              IfcAxis1Placement Axis,
                                              double Angle,
                                              IfcProfileDef EndSweptArea) : IfcSweptAreaSolid(SweptArea, Position);

   public abstract record IfcCsgPrimitive3D() : IfcSolidModel;

    public record IfcBlock(double XLength, double YLength, double ZLength, IfcAxis2Placement3D Position) : IfcCsgPrimitive3D;

    public record IfcRightCircularCylinder(double Height, double Radius, IfcAxis2Placement3D Position) : IfcCsgPrimitive3D;

    public record IfcHalfSpaceSolid(IfcSurface BaseSurface, bool AgreementFlag) : IfcSolidModel;

    public record IfcCsgSelect(IfcSolidModel? SolidModel, IfcHalfSpaceSolid? HalfSpace);

    public record IfcCsgSolid(IfcCsgSelect TreeRootExpression) : IfcSolidModel;

public abstract record IfcBooleanResult(string Operation,
                                            IIfcBooleanOperand FirstOperand,
                                            IIfcBooleanOperand SecondOperand) : IfcSolidModel;

    public record IfcBooleanClippingResult(string Operation,
                                           IIfcBooleanOperand FirstOperand,
                                           IIfcBooleanOperand SecondOperand) : IfcBooleanResult(Operation, FirstOperand, SecondOperand);

public abstract record IfcProfileDef(string? ProfileName) : IfcClass;

    public abstract record IfcParameterizedProfileDef(string? ProfileName) : IfcProfileDef(ProfileName);

    public record IfcRectangleProfileDef(string? ProfileName, double XDim, double YDim) : IfcParameterizedProfileDef(ProfileName);

    public record IfcCircleProfileDef(string? ProfileName, double Radius) : IfcParameterizedProfileDef(ProfileName);

public abstract record IfcTessellatedItem() : IfcGeometricRepresentationItem, IIfcBooleanOperand;

    public abstract record IfcTessellatedFaceSet(IfcCartesianPointList3D Coordinates, bool Closed) : IfcTessellatedItem;

    public record IfcPolygonalFaceSet(IfcCartesianPointList3D Coordinates,
                                      bool Closed,
                                      IReadOnlyList<IfcIndexedPolygonalFace> Faces) : IfcTessellatedFaceSet(Coordinates, Closed);

    public record IfcTriangulatedFaceSet(IfcCartesianPointList3D Coordinates,
                                         IReadOnlyList<IReadOnlyList<int>> CoordIndex,
                                         bool Closed,
                                         IReadOnlyList<IReadOnlyList<int>>? NormalIndex = null,
                                         IReadOnlyList<double>? Normals = null) : IfcTessellatedFaceSet(Coordinates, Closed);

    public record IfcTriangulatedIrregularNetwork(IfcCartesianPointList3D Coordinates,
                                                  IReadOnlyList<IReadOnlyList<int>> CoordIndex,
                                                  IReadOnlyList<double>? PnIndex = null) : IfcTessellatedFaceSet(Coordinates, false);

    public record IfcCartesianPointList3D(IReadOnlyList<IReadOnlyList<double>> CoordList) : IfcClass;
    public record IfcIndexedPolygonalFace(IReadOnlyList<int> CoordIndex) : IfcClass;

public abstract record IfcVertex() : IfcTopologicalRepresentationItem;
    public record IfcPointOrVertexPoint(IfcCartesianPoint? Point, IfcVertex? Vertex) : IfcClass;
    public record IfcVertexPoint(IfcPointOrVertexPoint? VertexGeometry) : IfcVertex;

    public abstract record IfcEdge(IfcVertex EdgeStart, IfcVertex EdgeEnd) : IfcTopologicalRepresentationItem;

    public record IfcEdgeCurve(IfcVertex EdgeStart,
                               IfcVertex EdgeEnd,
                               IfcCurve EdgeGeometry,
                               bool SameSense) : IfcEdge(EdgeStart, EdgeEnd);

    public record IfcOrientedEdge(IfcEdge EdgeElement, bool Orientation) : IfcEdge(EdgeElement.EdgeStart, EdgeElement.EdgeEnd);

    public record IfcPath(IReadOnlyList<IfcOrientedEdge> EdgeList) : IfcTopologicalRepresentationItem;

    public abstract record IfcLoop() : IfcTopologicalRepresentationItem;
    public record IfcEdgeLoop(IReadOnlyList<IfcOrientedEdge> EdgeList) : IfcLoop;
    public record IfcVertexLoop(IfcVertex LoopVertex) : IfcLoop;

    public abstract record IfcFaceBound(IfcLoop Bound, bool Orientation) : IfcTopologicalRepresentationItem;
    public record IfcFaceOuterBound(IfcLoop Bound, bool Orientation) : IfcFaceBound(Bound, Orientation);

    public record IfcFace(IReadOnlyList<IfcFaceBound> Bounds) : IfcTopologicalRepresentationItem;

    public record IfcAdvancedFace(IReadOnlyList<IfcFaceBound> Bounds,
                                  IfcSurface FaceSurface,
                                  bool SameSense) : IfcFace(Bounds);

    public record IfcFaceSurface(IReadOnlyList<IfcFaceBound> Bounds,
                                 IfcSurface FaceSurface,
                                 bool SameSense) : IfcFace(Bounds);

    public record IfcConnectedFaceSet(IReadOnlyList<IfcFace> CfsFaces) : IfcTopologicalRepresentationItem;

    public record IfcClosedShell(IReadOnlyList<IfcFace> CfsFaces) : IfcConnectedFaceSet(CfsFaces);
    public record IfcOpenShell(IReadOnlyList<IfcFace> CfsFaces) : IfcConnectedFaceSet(CfsFaces);

    public abstract record IfcManifoldSolidBrep(IfcClosedShell Outer) : IfcSolidModel;

    public record IfcFacetedBrep(IfcClosedShell Outer) : IfcManifoldSolidBrep(Outer);
    public record IfcFacetedBrepWithVoids(IfcClosedShell Outer, IReadOnlyList<IfcClosedShell> Voids) : IfcFacetedBrep(Outer);

    public record IfcAdvancedBrep(IfcClosedShell Outer) : IfcManifoldSolidBrep(Outer);
    public record IfcAdvancedBrepWithVoids(IfcClosedShell Outer, IReadOnlyList<IfcClosedShell> Voids) : IfcAdvancedBrep(Outer);

    // Surface / shell models
    public record IfcShellBasedSurfaceModel(IReadOnlyList<IfcOpenShell> SbsmBoundary) : IfcGeometricRepresentationItem;
    public record IfcFaceBasedSurfaceModel(IReadOnlyList<IfcConnectedFaceSet> FbsmFaces) : IfcGeometricRepresentationItem;

