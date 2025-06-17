using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.Studio.Samples
{
    public class RoofBeams : IModelGenerator
    {
        [Range(0, 100)] public int NumHorizontal = 3;
        [Range(0, 100)] public int NumVertical = 7;

        public bool HorizontalBelow = true;

        [Range(0, 100)] public float HorizontalBeamWidth = 0.5f;
        [Range(0, 100)] public float HorizontalBeamHeight = 0.2f;
        [Range(0, 100)] public float HorizontalSpacing = 1;

        [Range(0, 100)] public float VerticalBeamWidth = 0.5f;
        [Range(0, 100)] public float VerticalBeamHeight = 0.2f;
        [Range(0, 100)] public float VerticalSpacing = 1.2f;

        [Range(0, 5)] public float ColumnRadius = 0.1f;

        [Range(0, 10)] public float Height = 1.0f;

        public TriangleMesh3D ColumnMesh(Point3D position, Number height, Number radius)
        {
            var poly = new RegularPolygon(Point2D.Zero, 32).ToPolyLine3D().Scale(radius / 2f);
            var mesh = poly.Points.Extrude(Height);
            return mesh.Triangulate();
        }

        public Model3D Eval(EvalContext context)
        {
            var coreMesh = PlatonicSolids.TriangulatedCube;
            var totalLength = HorizontalBeamWidth * NumHorizontal + HorizontalSpacing * (NumHorizontal - 1);
            var totalWidth = VerticalBeamWidth * NumVertical + VerticalSpacing * (NumVertical - 1);
            var xspacing = HorizontalBeamWidth + HorizontalSpacing;
            var yspacing = VerticalBeamWidth + VerticalSpacing;

            var hBeam = coreMesh
                .TranslateZ(HorizontalBeamHeight.Half())
                .Scale(new Vector3(totalWidth, HorizontalBeamWidth, HorizontalBeamHeight));
            var hz = Height + (HorizontalBelow ? 0 : VerticalBeamHeight);
            var xoffset = -totalWidth / 2;
            var yoffset = -totalLength / 2;
            var hzoffet = HorizontalBeamWidth + HorizontalSpacing;
            var hPositions = NumHorizontal.MapRange(i => new Point3D(0, yoffset + i * hzoffet, hz));
            var hModel = hBeam.Clone(Material.Default.WithColor((1f, 0.1f, 0.3f, 1f)), hPositions);

            var vBeam = coreMesh
                .TranslateZ(VerticalBeamHeight.Half())
                .Scale(new Vector3(VerticalBeamWidth, totalLength, VerticalBeamHeight));
            var vz = Height + (HorizontalBelow ? HorizontalBeamHeight : 0);
            var vd = VerticalBeamWidth + VerticalSpacing;
            var vPositions = NumVertical.MapRange(i => new Point3D(xoffset + i * vd, 0, vz));
            var vModel = vBeam.Clone(Material.Default.WithColor((0.1f, 1.0f, 0.3f, 1f)), vPositions);

            var xcolspace = xspacing + VerticalBeamWidth;
            var ycolspace = yspacing + HorizontalBeamWidth;

            var columnMesh = ColumnMesh(Point3D.Zero, Height, ColumnRadius);
            var columnPositions = new FunctionalReadOnlyList2D<Point3D>(
                NumVertical, NumHorizontal, (i, j) => (xoffset + i * xcolspace, yoffset + j * ycolspace, 0));
            var columns = columnMesh.Clone(Material.Default.WithColor((0.1f, 0.5f, 1.0f, 1f)), columnPositions.Data);

            var mb = new Model3DBuilder();
            mb.Add(hModel);
            mb.Add(vModel);
            mb.Add(columns);
            return mb.Build();
        }
    }
}
