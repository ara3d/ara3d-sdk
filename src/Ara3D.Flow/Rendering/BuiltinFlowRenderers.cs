using Ara3D.Collections;
using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Studio.API;

/// <summary>
/// The built-in "instruction cards" (tracker studio-166/168): how each non-mesh flow type turns
/// itself into geometry the renderer already draws. Every card is a few lines and produces one of
/// the primitives the GL sink handles (LineMesh3D / TriangleMesh3D / Model3D) — or another flow
/// type whose card finishes the job (Sdf3D → VoxelizedField → Model3D). This is the whole cost of
/// making a type renderable; adding the next type means adding one entry here (or calling
/// <see cref="FlowRenderRegistry.Register"/> from a plug-in).
/// </summary>
public static class BuiltinFlowRenderers
{
    /// <summary>The marker instanced at each transform in a <see cref="TransformList"/>.</summary>
    private static readonly TriangleMesh3D Marker = PlatonicSolids.TriangulatedCube;

    public static void RegisterAll()
    {
        // Parametric curve -> polyline (lines).
        FlowRenderRegistry.Register<Curve3D>((curve, ctx) => CurveToLines(curve, ctx.Resolution));

        // Parametric surface -> triangle mesh.
        FlowRenderRegistry.Register<ParametricSurface>((surface, ctx) =>
            surface.Triangulate(ctx.Resolution, ctx.Resolution));

        // Transform list -> a marker instanced at each transform.
        FlowRenderRegistry.Register<TransformList>((list, _) =>
            Model3D.Create(Marker, Material.Default, ScaledMarkers(list)));

        // SDF (possibly infinite) -> voxelize over the context's bounds, then the voxel card draws it.
        FlowRenderRegistry.Register<Sdf3D>((sdf, ctx) =>
            sdf.Voxelize(ctx.DefaultBounds, new Integer3(ctx.Resolution, ctx.Resolution, ctx.Resolution)));

        // Voxels -> a box instanced at every occupied cell (value <= 0 is inside the surface).
        FlowRenderRegistry.Register<VoxelizedField>((voxels, _) => VoxelsToBoxes(voxels));

        // Closed 2D profile -> a closed polyline lifted onto the XY plane.
        FlowRenderRegistry.Register<Profile2D>((profile, _) => ProfileToLines(profile));

        // Polyline -> line segments (a closing segment is added when Closed).
        FlowRenderRegistry.Register<Polyline3D>((polyline, _) => PolylineToLines(polyline));

        // Scalar grid -> a box at every cell at/below the iso value (reuses the voxel box path).
        FlowRenderRegistry.Register<ScalarGrid3D>((grid, _) => GridToBoxes(grid));

        // Bitmap -> a flat colored quad grid on the XY plane (one colored vertex per pixel).
        FlowRenderRegistry.Register<Bitmap2D>((bitmap, _) => BitmapToMesh(bitmap));
    }

    private static LineMesh3D ProfileToLines(Profile2D profile)
    {
        var n = profile.Count;
        if (n < 2)
            return new LineMesh3D([], System.Array.Empty<Integer2>());
        var points = new Point3D[n];
        for (var i = 0; i < n; i++)
            points[i] = new Point3D(profile.Points[i].X, profile.Points[i].Y, 0f);
        var segments = new Integer2[n]; // closed loop
        for (var i = 0; i < n; i++)
            segments[i] = (i, (i + 1) % n);
        return new LineMesh3D(points, segments);
    }

    private static LineMesh3D PolylineToLines(Polyline3D polyline)
    {
        var n = polyline.Count;
        var segCount = polyline.Closed ? n : n - 1;
        if (segCount < 1)
            return new LineMesh3D(polyline.Points, System.Array.Empty<Integer2>());
        var segments = new Integer2[segCount];
        for (var i = 0; i < segCount; i++)
            segments[i] = (i, (i + 1) % n);
        return new LineMesh3D(polyline.Points, segments);
    }

    private static Model3D GridToBoxes(ScalarGrid3D grid)
    {
        // Reuse VoxelizedField purely for its cell-centre / cell-size geometry.
        var cells = new VoxelizedField(grid.Bounds, grid.Dims, _ => 0f);
        var s = cells.VoxelSize;
        var scale = Matrix4x4.CreateScale(s.X, s.Y, s.Z);
        var matrices = new List<Matrix4x4>();
        for (var k = 0; k < grid.NumLayers; k++)
        for (var j = 0; j < grid.NumRows; j++)
        for (var i = 0; i < grid.NumColumns; i++)
        {
            if (grid.Get(i, j, k) > grid.Iso)
                continue;
            matrices.Add(scale * Matrix4x4.CreateTranslation(cells.GetVoxelCenter(i, j, k).Vector3));
        }
        return Model3D.Create(Marker, Material.Default, matrices);
    }

    private static ColoredTriangleMesh3D BitmapToMesh(Bitmap2D bitmap)
    {
        var w = Math.Max(2, bitmap.Width);
        var h = Math.Max(2, bitmap.Height);
        var points = new FunctionalReadOnlyList2D<Point3D>(w, h, (i, j) => new Point3D(i, j, 0f));
        var mesh = new QuadGrid3D(points, false, false).Triangulate();
        var colors = new Vector3[mesh.Points.Count];
        for (var idx = 0; idx < mesh.Points.Count; idx++)
        {
            var p = mesh.Points[idx];
            colors[idx] = bitmap.SampleUV(p.X / (w - 1), p.Y / (h - 1));
        }
        return mesh.ToColored(colors);
    }

    private static LineMesh3D CurveToLines(Curve3D curve, int resolution)
    {
        var n = Math.Max(2, resolution);
        var points = curve.Sample(n);
        var segments = new Integer2[n - 1];
        for (var i = 0; i < n - 1; i++)
            segments[i] = (i, i + 1);
        return new LineMesh3D(points, segments);
    }

    private static IReadOnlyList<Matrix4x4> ScaledMarkers(TransformList list)
    {
        // Shrink the unit marker so instances read as points/frames, not a solid blob.
        var scale = Matrix4x4.CreateScale(0.2f);
        var result = new Matrix4x4[list.Count];
        for (var i = 0; i < list.Count; i++)
            result[i] = scale * list.Transforms[i];
        return result;
    }

    private static Model3D VoxelsToBoxes(VoxelizedField voxels)
    {
        var s = voxels.VoxelSize;
        var scale = Matrix4x4.CreateScale(s.X, s.Y, s.Z);
        var matrices = new List<Matrix4x4>();
        foreach (var v in voxels)
        {
            if (v.Value > 0)
                continue; // outside the surface
            matrices.Add(scale * Matrix4x4.CreateTranslation(v.Position.Vector3));
        }
        return Model3D.Create(Marker, Material.Default, matrices);
    }
}
