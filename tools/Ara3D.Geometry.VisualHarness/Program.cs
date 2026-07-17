using System.Text;
using Ara3D.Collections;
using Ara3D.Geometry;
using Ara3D.Geometry.VisualHarness;
using Ara3D.IO.GltfExporter;
using Ara3D.Models;
using Ara3D.Utils;

// Headless visual test harness (tracker: ara3d-036), proof-of-concept driven by the blue-noise demo
// (tracker: studio-041). Each case is an IModel3D → GLB (human, rotatable) + 3 orthographic PNGs
// (agent-readable, diffable) + stats, collected into one index.html gallery.
//
// Output defaults to the already-gitignored artifacts/ folder; pass an alternate dir as arg[0].

const int ImgW = 480, ImgH = 480;

var outDir = args.Length > 0
    ? args[0]
    : Path.Combine(FindArtifactsRoot(), "artifacts", "geometry-report");
Directory.CreateDirectory(outDir);

var cases = new List<Case>
{
    new("blue-vs-white",
        "BlueNoiseField — blue (left) vs white (right), equal N, min-distance violations painted red",
        "Blue noise (Poisson-disk, left) spreads evenly with no clumps; white noise (uniform random, right) "
        + "clusters and leaves holes at the SAME point count. Red = a point with a neighbour closer than the "
        + "radius. The white patch breaks out in red; the blue patch has none. Read it on the Top view.",
        BuildBlueNoiseField(12f, 12f, 0.7f, 1234, 30, 0.18f, showWhite: true, highlightDefects: true)),

    new("scatter-on-surface",
        "PoissonScatterOnSurface — even instance scatter across a wavy terrain",
        "Blue noise as a MODIFIER: the input surface's own vertices are thinned to blue-noise spacing and a "
        + "marker (green) is cloned onto each kept point, so instances conform to the surface with a guaranteed "
        + "minimum gap and no overlaps.",
        BuildScatterOnSurface(BuildWavySurface(64, 12f, 1.4f), 0.9f, 1234, 0.16f, keepInput: true)),
};

var html = new StringBuilder();
foreach (var c in cases)
{
    Console.WriteLine($"Rendering case: {c.Id}");
    var glbName = c.Id + ".glb";
    c.Model.WriteGlb(new FilePath(Path.Combine(outDir, glbName)));

    var cm = c.Model.ToColoredMesh();
    var views = new[] { ViewKind.Top, ViewKind.Front, ViewKind.Side };
    var imgNames = new string[views.Length];
    for (var i = 0; i < views.Length; i++)
    {
        var pixels = OrthoRasterizer.Render(cm, views[i], ImgW, ImgH);
        imgNames[i] = $"{c.Id}_{views[i].ToString().ToLower()}.png";
        Png.Write(Path.Combine(outDir, imgNames[i]), pixels, ImgW, ImgH);
    }

    AppendCaseHtml(html, c, glbName, imgNames, cm);
}

var page = PageShell(html.ToString());
var indexPath = Path.Combine(outDir, "index.html");
File.WriteAllText(indexPath, page);
Console.WriteLine($"\nReport written: {indexPath}");
Console.WriteLine($"Cases: {cases.Count}. Open index.html in a browser (PNGs are self-contained; the 3D view uses a CDN).");
return;

// ---- Case builders (mirror the studio-041 demo classes; share the same PoissonDiskSampling core) ----

static IModel3D BuildBlueNoiseField(float width, float height, float radius, int seed, int attempts,
    float markerSize, bool showWhite, bool highlightDefects)
{
    var marker = PlatonicSolids.TriangulatedCube.Scale(markerSize);
    var gap = width * 0.35f;
    var models = new List<IModel3D>();

    var blue = PoissonDiskSampling.Sample(width, height, radius, seed, attempts);
    AddPatch(models, marker, blue, -width - gap, height, radius, highlightDefects);

    if (showWhite)
    {
        var white = PoissonDiskSampling.UniformRandom(width, height, blue.Count, seed);
        AddPatch(models, marker, white, gap, height, radius, highlightDefects);
    }
    return models.Merge();
}

static void AddPatch(List<IModel3D> models, TriangleMesh3D marker, IReadOnlyList<Vector2> pts,
    float xOffset, float height, float radius, bool highlightDefects)
{
    var defect = highlightDefects ? Defects(pts, radius) : null;
    var good = new List<Point3D>();
    var bad = new List<Point3D>();
    for (var i = 0; i < pts.Count; i++)
    {
        var p = new Point3D(pts[i].X + xOffset, pts[i].Y - height / 2, 0);
        if (defect != null && defect[i]) bad.Add(p);
        else good.Add(p);
    }
    if (good.Count > 0) models.Add(marker.Clone(Material.Default.WithColor((0.2f, 0.5f, 1f, 1f)), good));
    if (bad.Count > 0) models.Add(marker.Clone(Material.Default.WithColor((1f, 0.15f, 0.15f, 1f)), bad));
}

static bool[] Defects(IReadOnlyList<Vector2> pts, float radius)
{
    var n = pts.Count;
    var flags = new bool[n];
    var r2 = radius * radius;
    for (var i = 0; i < n; i++)
    for (var j = i + 1; j < n; j++)
    {
        double dx = pts[i].X - pts[j].X, dy = pts[i].Y - pts[j].Y;
        if (dx * dx + dy * dy < r2) { flags[i] = true; flags[j] = true; }
    }
    return flags;
}

static IModel3D BuildWavySurface(int n, float size, float amp)
{
    var grid = new FunctionalReadOnlyList2D<Point3D>(n, n, (i, j) =>
    {
        var u = i / (float)(n - 1);
        var v = j / (float)(n - 1);
        var x = (u - 0.5f) * size;
        var y = (v - 0.5f) * size;
        var z = MathF.Sin(u * MathF.Tau * 1.5f) * MathF.Cos(v * MathF.Tau * 1.5f) * amp;
        return new Point3D(x, y, z);
    });
    return new QuadGrid3D(grid, false, false).Triangulate().ToModel3D();
}

static IModel3D BuildScatterOnSurface(IModel3D input, float radius, int seed, float instanceScale, bool keepInput)
{
    var mesh = input.ToColoredMesh();
    var points = mesh.Mesh.Points.Map(p => p.Vector3);
    var kept = PoissonDiskSampling.SelectSubset(points, radius, seed);

    var marker = PlatonicSolids.TriangulatedCube.Scale(instanceScale);
    var positions = kept.Map(v => new Point3D(v.X, v.Y, v.Z));
    var scattered = marker.Clone(Material.Default.WithColor((0.2f, 0.9f, 0.4f, 1f)), positions);
    return keepInput ? input.Combine(scattered) : scattered;
}

// ---- HTML ----

static void AppendCaseHtml(StringBuilder sb, Case c, string glb, string[] imgs, ColoredTriangleMesh3D cm)
{
    var pts = cm.Mesh.Points;
    var (min, max) = Bounds(pts);
    var stats = $"instances: {c.Model.Instances.Count} &nbsp;|&nbsp; verts: {pts.Count} &nbsp;|&nbsp; "
        + $"tris: {cm.Mesh.FaceIndices.Count} &nbsp;|&nbsp; bounds: "
        + $"({min.x:F1},{min.y:F1},{min.z:F1}) → ({max.x:F1},{max.y:F1},{max.z:F1})";

    sb.AppendLine("<section>");
    sb.AppendLine($"  <h2>{c.Title}</h2>");
    sb.AppendLine($"  <p class=\"desc\">{c.Description}</p>");
    sb.AppendLine("  <div class=\"row\">");
    sb.AppendLine($"    <div class=\"cell\"><model-viewer src=\"{glb}\" camera-controls auto-rotate " +
                  "shadow-intensity=\"1\" style=\"width:320px;height:320px;background:#eee\"></model-viewer>" +
                  "<div class=\"lbl\">3D (drag to rotate)</div></div>");
    string[] names = { "Top", "Front", "Side" };
    for (var i = 0; i < imgs.Length; i++)
        sb.AppendLine($"    <div class=\"cell\"><img src=\"{imgs[i]}\" width=\"320\" height=\"320\"/>" +
                      $"<div class=\"lbl\">{names[i]} (ortho)</div></div>");
    sb.AppendLine("  </div>");
    sb.AppendLine($"  <p class=\"stats\">{stats}</p>");
    sb.AppendLine("</section>");
}

static string PageShell(string body)
    => "<!doctype html><html><head><meta charset=\"utf-8\"><title>Geometry Visual Report</title>" +
       "<script type=\"module\" src=\"https://unpkg.com/@google/model-viewer/dist/model-viewer.min.js\"></script>" +
       "<style>" +
       "body{font-family:system-ui,sans-serif;margin:24px;color:#222;background:#fafafa}" +
       "h1{font-size:20px}h2{font-size:16px;margin-bottom:4px}" +
       "section{background:#fff;border:1px solid #ddd;border-radius:8px;padding:16px;margin-bottom:20px}" +
       ".desc{color:#444;max-width:900px;font-size:14px}" +
       ".row{display:flex;gap:12px;flex-wrap:wrap}" +
       ".cell{text-align:center}.cell img{border:1px solid #ccc;background:#fff}" +
       ".lbl{font-size:12px;color:#666;margin-top:4px}" +
       ".stats{font-size:12px;color:#555;font-family:monospace;margin-top:10px}" +
       "</style></head><body>" +
       "<h1>Geometry Visual Report <span style=\"font-weight:normal;color:#888;font-size:13px\">— " +
       "ara3d-036 harness · studio-041 blue-noise proof of concept</span></h1>" +
       body + "</body></html>";

static ((float x, float y, float z) min, (float x, float y, float z) max) Bounds(IReadOnlyList<Point3D> pts)
{
    float mnx = float.MaxValue, mny = float.MaxValue, mnz = float.MaxValue;
    float mxx = float.MinValue, mxy = float.MinValue, mxz = float.MinValue;
    for (var i = 0; i < pts.Count; i++)
    {
        float x = pts[i].X, y = pts[i].Y, z = pts[i].Z;
        if (x < mnx) mnx = x; if (y < mny) mny = y; if (z < mnz) mnz = z;
        if (x > mxx) mxx = x; if (y > mxy) mxy = y; if (z > mxz) mxz = z;
    }
    return ((mnx, mny, mnz), (mxx, mxy, mxz));
}

static string FindArtifactsRoot()
{
    // Walk up from the running exe to the ara3d-sdk root (the dir whose .gitignore already ignores artifacts/).
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Ara3D.SDK.sln"))) return dir.FullName;
        dir = dir.Parent;
    }
    return Environment.CurrentDirectory;
}

internal readonly record struct Case(string Id, string Title, string Description, IModel3D Model);
