using Ara3D.Geometry;
using Ara3D.IO.PLY;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.DirectContext3D;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ara3D.Collections;
using View = Autodesk.Revit.DB.View;

namespace Ara3D.Bowerbird.RevitSamples;

public sealed class RenderMesh
{
    private readonly RenderVertex[] _vertices;
    private readonly int[] _indices;

    public readonly Outline Bounds;

    public Span<RenderVertex> Vertices => _vertices;
    public Span<int> Indices => _indices;

    public int VertexCount => _vertices.Length;
    public int IndexCount => _indices.Length;

    public RenderMesh(RenderVertex[] vertices, int[] indices)
    {
        _vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        _indices = indices ?? throw new ArgumentNullException(nameof(indices));
        Bounds = _computeBoundingBox();
        ApplyBoundingBoxGradient();
        Debug.Assert(_vertices.Length > 0, "Mesh must contain at least one vertex.");
    }

    public static RenderMesh Create(
        IReadOnlyList<Point3D> positions,
        IReadOnlyList<int>? indices,
        IReadOnlyList<Vector3>? normals = null,
        IReadOnlyList<Vector2>? uvs = null,
        IReadOnlyList<Color32>? colors = null)
    {
        if (positions is null) throw new ArgumentNullException(nameof(positions));
        if (positions.Count == 0) throw new ArgumentException("Empty meshes are not supported.", nameof(positions));
        if (indices is null) throw new ArgumentNullException(nameof(indices), "Triangle meshes require indices.");
        if (indices.Count == 0) throw new ArgumentException("Mesh indices cannot be empty.", nameof(indices));
        if (indices.Count % 3 != 0) throw new ArgumentException("Triangle indices must be a multiple of 3.", nameof(indices));

        var n = positions.Count;

        normals ??= Vector3.Default.Repeat(n);
        uvs ??= Vector2.Default.Repeat(n);
        colors ??= new Color32(128, 128, 128).Repeat(n);

        if (normals.Count != n) throw new InvalidOperationException($"Normals count {normals.Count} must match vertices count {n}.");
        if (uvs.Count != n) throw new InvalidOperationException($"UVs count {uvs.Count} must match vertices count {n}.");
        if (colors.Count != n) throw new InvalidOperationException($"Colors count {colors.Count} must match vertices count {n}.");

        var rv = new RenderVertex[n];
        for (var i = 0; i < n; i++)
            rv[i] = new RenderVertex(positions[i], normals[i], uvs[i], colors[i]);

        var idx = new int[indices.Count];
        for (var i = 0; i < idx.Length; i++)
            idx[i] = indices[i];

        var rm = new RenderMesh(rv, idx);
        rm.ComputeSmoothedNormalsInPlace();

        

        return rm;
    }

    private Outline _computeBoundingBox()
    {
        // Revit XYZ uses doubles; RenderVertex stores floats.
        var v = Vertices;
        Debug.Assert(v.Length > 0);

        var minX = (double)v[0].PX; var minY = (double)v[0].PY; var minZ = (double)v[0].PZ;
        var maxX = minX; var maxY = minY; var maxZ = minZ;

        for (var i = 1; i < v.Length; i++)
        {
            var x = (double)v[i].PX; var y = (double)v[i].PY; var z = (double)v[i].PZ;
            if (x < minX) minX = x; if (y < minY) minY = y; if (z < minZ) minZ = z;
            if (x > maxX) maxX = x; if (y > maxY) maxY = y; if (z > maxZ) maxZ = z;
        }

        // Avoid zero-volume outlines (can cause culling surprises).
        const double eps = 1e-6;
        if (maxX - minX < eps) { maxX += eps; minX -= eps; }
        if (maxY - minY < eps) { maxY += eps; minY -= eps; }
        if (maxZ - minZ < eps) { maxZ += eps; minZ -= eps; }

        return new Outline(new XYZ(minX, minY, minZ), new XYZ(maxX, maxY, maxZ));
    }

    public void ComputeSmoothedNormalsInPlace(bool areaWeighted = true, float degenerateEps = 1e-10f)
    {
        Debug.Assert(_vertices.Length > 0);
        Debug.Assert(_indices.Length % 3 == 0);

        var vertexCount = _vertices.Length;
        var acc = new Vector3[vertexCount];

        for (int i = 0; i < _indices.Length; i += 3)
        {
            var i0 = _indices[i];
            var i1 = _indices[i + 1];
            var i2 = _indices[i + 2];

            Debug.Assert((uint)i0 < (uint)vertexCount);
            Debug.Assert((uint)i1 < (uint)vertexCount);
            Debug.Assert((uint)i2 < (uint)vertexCount);

            if (i0 == i1 || i1 == i2 || i2 == i0)
                continue;

            var p0 = _vertices[i0].Position;
            var p1 = _vertices[i1].Position;
            var p2 = _vertices[i2].Position;

            var n = Vector3.Cross(p1 - p0, p2 - p0);
            var lenSq = n.LengthSquared();

            if (lenSq <= degenerateEps)
                continue;

            if (!areaWeighted)
                n *= 1.0f / MathF.Sqrt(lenSq);

            acc[i0] += n;
            acc[i1] += n;
            acc[i2] += n;
        }

        for (int i = 0; i < vertexCount; i++)
        {
            var n = acc[i];
            var lenSq = n.LengthSquared();

            n = lenSq > degenerateEps
                ? n * (1.0f / MathF.Sqrt(lenSq))
                : new Vector3(0, 0, 1);

            var v = _vertices[i];

            // Reconstruct struct with new normal
            _vertices[i] = new RenderVertex(
                position: v.Position,
                normal: n,
                uv: v.UV,
                color: v.RGBA
            );
        }
    }

    public void ApplyBoundingBoxGradient(bool enhanceContrast = true)
    {
        Debug.Assert(_vertices.Length > 0);

        // Compute bounds
        float minX = _vertices[0].PX, maxX = _vertices[0].PX;
        float minY = _vertices[0].PY, maxY = _vertices[0].PY;
        float minZ = _vertices[0].PZ, maxZ = _vertices[0].PZ;

        for (int i = 1; i < _vertices.Length; i++)
        {
            var v = _vertices[i];

            if (v.PX < minX) minX = v.PX;
            if (v.PX > maxX) maxX = v.PX;

            if (v.PY < minY) minY = v.PY;
            if (v.PY > maxY) maxY = v.PY;

            if (v.PZ < minZ) minZ = v.PZ;
            if (v.PZ > maxZ) maxZ = v.PZ;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float dz = maxZ - minZ;

        const float eps = 1e-12f;

        // Avoid divide-by-zero for flat meshes
        if (dx < eps) dx = 1;
        if (dy < eps) dy = 1;
        if (dz < eps) dz = 1;

        for (int i = 0; i < _vertices.Length; i++)
        {
            var v = _vertices[i];

            // Normalize to [0,1]
            float nx = (v.PX - minX) / dx;
            float ny = (v.PY - minY) / dy;
            float nz = (v.PZ - minZ) / dz;

            Debug.Assert(nx >= -0.01f && nx <= 1.01f);
            Debug.Assert(ny >= -0.01f && ny <= 1.01f);
            Debug.Assert(nz >= -0.01f && nz <= 1.01f);

            if (enhanceContrast)
            {
                // Slight gamma curve to make gradient more vivid
                nx = MathF.Pow(nx, 0.8f);
                ny = MathF.Pow(ny, 0.8f);
                nz = MathF.Pow(nz, 0.8f);
            }

            byte r = (byte)(Math.Clamp(nx, 0f, 1f) * 255f);
            byte g = (byte)(Math.Clamp(ny, 0f, 1f) * 255f);
            byte b = (byte)(Math.Clamp(nz, 0f, 1f) * 255f);

            var newColor = new Color32(r, g, b, 255);

            _vertices[i] = new RenderVertex(
                position: v.Position,
                normal: v.Normal,
                uv: v.UV,
                color: newColor
            );
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RenderVertex
{
    public RenderVertex(Vector3 position, Vector3 normal, Vector2 uv, Color32 color)
    {
        PX = position.X; PY = position.Y; PZ = position.Z;
        NX = normal.X; NY = normal.Y; NZ = normal.Z;
        U = uv.X; V = uv.Y;
        RGBA = color;
    }

    public Vector3 Position => new(PX, PY, PZ);
    public Vector3 Normal => new(NX, NY, NZ);
    public Vector2 UV => new(U, V);

    public readonly float PX, PY, PZ;     // 12 bytes
    public readonly float NX, NY, NZ;     // 12 bytes
    public readonly float U, V;           // 8 bytes
    public readonly Color32 RGBA;         // 4 bytes

    public VertexPositionNormalColored ToRevit()
        => new VertexPositionNormalColored(
            new XYZ(PX, PY, PZ),
            new XYZ(NX, NY, NZ),
            new ColorWithTransparency(RGBA.R, RGBA.G, RGBA.B, (byte)(255 - RGBA.A))
        );
}

public sealed class CommandDirectContextPlyLoader : NamedCommand, IDirectContext3DServer
{
    public override string Name => "Direct Context Demo - Ply Loader";

    public Guid Guid { get; } = Guid.NewGuid();

    private Outline _boundingBox = new(new XYZ(0, 0, 0), new XYZ(1, 1, 1));

    public override void Execute(object argument)
    {
        var app = (UIApplication)argument;

        LoadPlyFile();
        if (Mesh is null)
            return;

        _boundingBox = Mesh.Bounds;
        
        var svc = ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
        svc.AddServer(this);

        if (svc is not MultiServerService ms)
            throw new InvalidOperationException("Expected DirectContext3DService to be a MultiServerService.");

        var ids = ms.GetActiveServerIds();
        if (!ids.Contains(GetServerId()))
            ids.Add(GetServerId());

        ms.SetActiveServers(ids);

        app.ActiveUIDocument?.UpdateAllOpenViews();
    }

    public Guid GetServerId() => Guid;
    public ExternalServiceId GetServiceId() => ExternalServices.BuiltInExternalServices.DirectContext3DService;
    public string GetName() => Name;
    public string GetVendorId() => "Ara 3D Inc.";
    public string GetDescription() => "Demonstrates using the DirectContext3D API";
    public bool CanExecute(View dBView) => dBView.ViewType == ViewType.ThreeD;
    public string GetApplicationId() => "Bowerbird";
    public string GetSourceId() => "";
    public bool UsesHandles() => false;
    public Outline GetBoundingBox(View dBView) => _boundingBox;
    public bool UseInTransparentPass(View dBView) => true;

    public RenderMesh? Mesh { get; private set; }
    private BufferStorage? _faceBuffers;

    public void RenderScene(View dBView, DisplayStyle displayStyle)
    {
        if (Mesh is null) return;

        _faceBuffers ??= new BufferStorage(Mesh);
        _faceBuffers.Render();
    }

    private OpenFileDialog? _plyOpenFileDialog;

    private void LoadPlyFile()
    {
        _plyOpenFileDialog ??= new OpenFileDialog
        {
            DefaultExt = ".ply",
            Filter = "PLY Files (*.ply)|*.ply|All Files (*.*)|*.*",
            Title = "Open PLY File"
        };

        if (_plyOpenFileDialog.ShowDialog() != DialogResult.OK)
            return;

        var plyFile = _plyOpenFileDialog.FileName;
        //var plyFile = @"C:\Users\cdigg\git\draco\testdata\bun_zipper.ply";
        
        var mesh = PlyImporter.LoadMesh(plyFile);

        // Assuming your extension exists:
        Mesh = mesh.ToRenderMesh(new Color32());
        

        Debug.Assert(Mesh.VertexCount > 0);
        Debug.Assert(Mesh.IndexCount > 0);
        Debug.Assert(Mesh.IndexCount % 3 == 0);
    }
}