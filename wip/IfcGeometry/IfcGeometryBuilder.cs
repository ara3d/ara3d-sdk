/*
using System;
using System.Collections.Generic;
using System.Linq;
using GeometryCore;    // Assumed low-level geometry primitives: Vector3, MeshBuilder, Extrude, Revolve, BooleanOp, CSG, Curve, Transform3D, etc.
using Objects.BuiltElements;
using Objects.Geometry;

namespace Ara3D.IfcGeometry
{
    /// <summary>
    /// Entry point for converting IFC products into a unified mesh.
    /// </summary>
    public static class IfcGeometryBuilder
    {
        /// <summary>
        /// Builds a single mesh by appending all product geometries.
        /// </summary>
        public static Mesh BuildModel(IEnumerable<IfcProduct> products)
        {
            var builder = new MeshBuilder();
            foreach (var product in products)
            {
                var mesh = BuildProduct(product);
                if (mesh != null)
                    builder.Append(mesh);
            }
            return builder.Build();
        }

        /// <summary>
        /// Builds geometry for one IFC product, applying its placement.
        /// </summary>
        public static Mesh BuildProduct(IfcProduct product)
        {
            // Convert the raw representation
            Mesh mesh = BuildRepresentationItem(product.Representation);

            // Apply object placement transform (local→world)
            if (mesh != null && product.ObjectPlacement != null)
            {
                var tf = PlacementBuilder.GetTransform(product.ObjectPlacement);
                mesh = mesh.Transform(tf);
            }

            return mesh;
        }

        /// <summary>
        /// Dispatches on the IFC representation item types.
        /// </summary>
        private static Mesh BuildRepresentationItem(IfcRepresentationItem item)
        {
            switch (item)
            {
                case IfcMappedItem map: return BuildMappedItem(map);
                case IfcCsgPrimitive3D csg: return BuildCsgPrimitive(csg);
                case IfcBooleanResult booleanOp: return BuildBooleanResult(booleanOp);
                case IfcExtrudedAreaSolid extrude: return BuildExtrudedAreaSolid(extrude);
                case IfcRevolvedAreaSolid revolved: return BuildRevolvedAreaSolid(revolved);
                case IfcSectionedSpine spine: return BuildSectionedSpine(spine);
                case IfcSweptDiskSolid disk: return BuildSweptDiskSolid(disk);
                case IfcSurfaceCurveSweptAreaSolid scs: return BuildSurfaceCurveSweptAreaSolid(scs);
                case IfcTriangulatedFaceSet tri: return BuildTriangulatedFaceSet(tri);
                case IfcFacetedBrep brep: return BuildClosedShell(brep.Outer);
                case IfcManifoldSolidBrep msb: return BuildClosedShell(msb.Outer);
                case IfcFaceBasedSurfaceModel fbsm: return BuildFaceBasedSurfaceModel(fbsm);
                default:
                    // unsupported or non-geometric items
                    return null;
            }
        }

        #region Primitive Constructors

        private static Mesh BuildCsgPrimitive(IfcCsgPrimitive3D prim)
        {
            switch (prim)
            {
                case IfcBlock b:
                    return Csg.Primitive.Box(b.XLength, b.YLength, b.ZLength);

                case IfcRectangularPyramid pyr:
                    return Csg.Primitive.RectangularPyramid(pyr.XLength, pyr.YLength, pyr.Height);

                case IfcRightCircularCone cone:
                    return Csg.Primitive.Cone(cone.BottomRadius, cone.Height);

                case IfcRightCircularCylinder cyl:
                    return Csg.Primitive.Cylinder(cyl.Radius, cyl.Height);

                case IfcSphere sph:
                    return Csg.Primitive.Sphere(sph.Radius);

                default:
                    throw new NotSupportedException($"Unsupported CSG primitive: {prim.GetType().Name}");
            }
        }

        private static Mesh BuildBooleanResult(IfcBooleanResult op)
        {
            var A = BuildRepresentationItem(op.FirstOperand);
            var B = BuildRepresentationItem(op.SecondOperand);
            return op.Operator switch
            {
                IfcBooleanOperator.Union => BooleanOp.Union(A, B),
                IfcBooleanOperator.Intersection => BooleanOp.Intersect(A, B),
                IfcBooleanOperator.Difference => BooleanOp.Subtract(A, B),
                _ => throw new NotSupportedException($"Boolean operator {op.Operator}")
            };
        }

        private static Mesh BuildExtrudedAreaSolid(IfcExtrudedAreaSolid ex)
        {
            var profile = ProfileBuilder.Build(ex.SweptArea);
            var direction = ToVector3(ex.ExtrudedDirection).Normalized();
            return GeometryEngine.Extrude(profile, direction, ex.Depth);
        }

        private static Mesh BuildRevolvedAreaSolid(IfcRevolvedAreaSolid rev)
        {
            var profile = ProfileBuilder.Build(rev.SweptArea);
            var axis = CurveBuilder.AxisFromPlacement(rev.Axis);
            return GeometryEngine.Revolve(profile, axis, rev.Angle);
        }

        private static Mesh BuildSectionedSpine(IfcSectionedSpine spine)
        {
            var path = CurveBuilder.Build(spine.SpineCurve);
            var sections = new List<(Profile, Placement3D)>();
            foreach (var cs in spine.CrossSections)
            {
                var prof = ProfileBuilder.Build(cs.SectionedCurveSegment);
                var pos = PlacementBuilder.AxisPlacement(cs.Position);
                sections.Add((prof, pos));
            }
            return GeometryEngine.SectionSweep(path, sections);
        }

        private static Mesh BuildSweptDiskSolid(IfcSweptDiskSolid disk)
        {
            var path = CurveBuilder.Build(disk.SweptCurve);
            return GeometryEngine.SweepDisk(path, disk.Radius, disk.InnerRadius);
        }

        private static Mesh BuildSurfaceCurveSweptAreaSolid(IfcSurfaceCurveSweptAreaSolid scs)
        {
            var profile = ProfileBuilder.Build(scs.SweptArea);
            var path = CurveBuilder.Build(scs.SweptCurve);
            return GeometryEngine.SweepProfileAlongCurve(profile, path);
        }

        private static Mesh BuildTriangulatedFaceSet(IfcTriangulatedFaceSet tfs)
        {
            var mb = new MeshBuilder();
            foreach (var tri in tfs.Coordinates.Triangles)
            {
                var pts = tri.Select(i => ToVector3(tfs.Coordinates.Points[i])).ToArray();
                mb.AddTriangle(pts[0], pts[1], pts[2]);
            }
            return mb.Build();
        }

        private static Mesh BuildFaceBasedSurfaceModel(IfcFaceBasedSurfaceModel model)
        {
            var mb = new MeshBuilder();
            foreach (var face in model.FbsmFaces)
            {
                foreach (var bound in face.Bounds)
                {
                    if (bound.Bound is IfcPolyLoop loop)
                    {
                        var verts = loop.Polygon.Select(pt => ToVector3(pt)).ToList();
                        if (!bound.Orientation) verts.Reverse();
                        mb.AddPolygon(verts);
                    }
                }
            }
            return mb.Build();
        }

        private static Mesh BuildClosedShell(IfcClosedShell shell)
        {
            var mb = new MeshBuilder();
            foreach (var face in shell.CfsFaces)
            {
                foreach (var bound in face.Bounds)
                {
                    if (bound.Bound is IfcPolyLoop loop)
                    {
                        var verts = loop.Polygon.Select(pt => ToVector3(pt)).ToList();
                        if (!bound.Orientation) verts.Reverse();
                        mb.AddPolygon(verts);
                    }
                }
            }
            return mb.Build();
        }

        private static Mesh BuildMappedItem(IfcMappedItem mi)
        {
            var sourceRep = mi.MappingSource.MappedRepresentation as IfcShapeRepresentation;
            var mb = new MeshBuilder();
            foreach (var item in sourceRep.Items)
            {
                var sub = BuildRepresentationItem(item);
                if (sub != null) mb.Append(sub);
            }
            var mesh = mb.Build();
            var tf = PlacementBuilder.GetTransform(mi.MappingTarget);
            return mesh.Transform(tf);
        }

        #endregion

        #region Geometry Converters

        private static Vector3 ToVector3(IfcCartesianPoint pt)
            => new Vector3(pt.X, pt.Y, pt.Z);

        #endregion
    }

    /// <summary>
    /// Builds 2D profiles from IFC definitions.
    /// </summary>
    internal static class ProfileBuilder
    {
        public static Profile Build(IfcProfileDef def)
        {
            switch (def)
            {
                case IfcRectangleProfileDef r: return Profile.Primitive.Rectangle(r.XDim, r.YDim);
                case IfcCircleProfileDef c: return Profile.Primitive.Circle(c.Radius);
                case IfcIShapeProfileDef i: return Profile.Primitive.IShape(i.ProfileDepth, i.ProfileWidth, i.WebThickness, i.FlangeThickness);
                case IfcTShapeProfileDef t: return Profile.Primitive.TShape(t.ProfileDepth, t.ProfileWidth, t.WebThickness, t.FlangeThickness);
                case IfcZShapeProfileDef z: return Profile.Primitive.ZShape(z.ProfileDepth, z.ProfileWidth, z.WebThickness, z.FlangeThickness);
                case IfcEllipseProfileDef e: return Profile.Primitive.Ellipse(e.XRadius, e.YRadius);
                default:
                    throw new NotSupportedException($"Profile type {def.GetType().Name} not supported");
            }
        }
    }

    /// <summary>
    /// Builds curves from IFC definitions.
    /// </summary>
    internal static class CurveBuilder
    {
        public static Curve Build(IfcCurve curve)
        {
            switch (curve)
            {
                case IfcPolyline pl:
                    var pts = pl.Points.Select(p => new Vector3(p.X, p.Y, p.Z)).ToList();
                    return Curve.Polyline(pts);
                case IfcTrimmedCurve tc:
                    var baseC = Build(tc.BasisCurve);
                    var start = tc.Trim1Parameter;
                    var end = tc.Trim2Parameter;
                    return Curve.Trim(baseC, start, end);
                case IfcCompositeCurve cc:
                    var segments = cc.Segments.Select(s => Build(s.ParentCurve)).ToList();
                    return Curve.Chain(segments);
                default:
                    throw new NotSupportedException($"Curve type {curve.GetType().Name} not supported");
            }
        }

        public static Curve.AxisBuild AxisFromPlacement(IfcAxis2Placement3D ap)
        {
            var origin = new Vector3(ap.Location.X, ap.Location.Y, ap.Location.Z);
            var dir = new Vector3(ap.Axis.X, ap.Axis.Y, ap.Axis.Z);
            var refDir = new Vector3(ap.RefDirection.X, ap.RefDirection.Y, ap.RefDirection.Z);
            return new Curve.AxisBuild(origin, dir, refDir);
        }
    }

    /// <summary>
    /// Handles IFC object placements into 3D transforms.
    /// </summary>
    internal static class PlacementBuilder
    {
        public static Transform3D GetTransform(IfcObjectPlacement placement)
        {
            if (placement is IfcLocalPlacement lp)
            {
                var rel = lp.PlacementRelTo != null
                        ? GetTransform(lp.PlacementRelTo)
                        : Transform3D.Identity;
                var own = FromAxis2Placement(lp.RelativePlacement as IfcAxis2Placement3D);
                return rel * own;
            }
            throw new NotSupportedException($"Placement type {placement.GetType().Name} not supported");
        }

        public static Placement3D AxisPlacement(IfcAxis2Placement3D ap)
            => new Placement3D(
                    new Vector3(ap.Location.X, ap.Location.Y, ap.Location.Z),
                    new Vector3(ap.Axis.X, ap.Axis.Y, ap.Axis.Z),
                    new Vector3(ap.RefDirection.X, ap.RefDirection.Y, ap.RefDirection.Z)
               );

        private static Transform3D FromAxis2Placement(IfcAxis2Placement3D ap)
        {
            var p = AxisPlacement(ap);
            return Transform3D.FromPlacement(p);
        }
    }
}
*/