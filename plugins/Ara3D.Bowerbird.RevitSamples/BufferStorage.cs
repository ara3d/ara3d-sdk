using Ara3D.Geometry;
using Autodesk.Revit.DB.DirectContext3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PrimitiveType = Autodesk.Revit.DB.DirectContext3D.PrimitiveType;

namespace Ara3D.Bowerbird.RevitSamples
{
    public sealed class BufferStorage : IDisposable
    {
        public PrimitiveType PrimitiveType = PrimitiveType.TriangleList;

        public static readonly VertexFormatBits FormatBits = VertexFormatBits.PositionNormalColored;
        
        public VertexBuffer? VertexBuffer { get; private set; }
        public IndexBuffer? IndexBuffer { get; private set; }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }

        public const int PrimitiveSize = 3;
    
        public int PrimitiveCount => IndexCount / PrimitiveSize;

        public BufferStorage(RenderMesh mesh)
            : this(mesh.Vertices, mesh.Indices)
        { }

        public BufferStorage(ReadOnlySpan<RenderVertex> vertices, ReadOnlySpan<int> indices)
        {
            Update(vertices, indices);
        }

        public void Render()
        {
            if (VertexBuffer is null || IndexBuffer is null) return;
            Debug.Assert(VertexCount > 0);
            Debug.Assert(IndexCount > 0);
            Debug.Assert(IndexCount % PrimitiveSize == 0);

            using VertexFormat vertexFormat = new VertexFormat(FormatBits);
            using EffectInstance effectInstance = new EffectInstance(FormatBits);

            DrawContext.FlushBuffer(
                VertexBuffer,
                VertexCount,
                IndexBuffer,
                IndexCount,
                vertexFormat,
                effectInstance,
                PrimitiveType,
                0,
                PrimitiveCount);
        }

        public void Update(RenderMesh mesh)
            => Update(mesh.Vertices, mesh.Indices);

        public void Update(ReadOnlySpan<RenderVertex> vertices, ReadOnlySpan<int> indices)
        {
            SetVertexBuffer(vertices);
            SetIndexBuffer(indices);
        }

        public void Dispose()
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
            VertexBuffer = null;
            IndexBuffer = null;
            VertexCount = 0;
            IndexCount = 0;
        }
    
        private int VertexBufferSizeInFloats()
            => VertexPositionNormalColored.GetSizeInFloats() * VertexCount;

        private int IndexBufferSizeInShorts()
        {
            var primSize = IndexTriangle.GetSizeInShortInts();
            var numTriangles = IndexCount / 3;
            return numTriangles * primSize;
        }

        private void EnsureVertexBufferCapacity(int vertexCount)
        {
            // If the same size and allocated, continue
            if (VertexCount == vertexCount && VertexBuffer is not null)
                return;

            VertexBuffer?.Dispose();
            VertexBuffer = null;
            VertexCount = vertexCount;

            if (vertexCount == 0)
                return;
        
            VertexBuffer = new VertexBuffer(VertexBufferSizeInFloats());
        }

        private void EnsureIndexBufferCapacity(int indexCount)
        {
            // If the same size and allocated, continue
            if (IndexCount == indexCount && IndexBuffer is not null)
                return;

            IndexBuffer?.Dispose();
            IndexBuffer = null;
            IndexCount = indexCount;

            if (indexCount == 0)
                return;

            // NOTE: this "sizeInShortInts" is bizarre, but we just have to accept it. 
            // https://www.autodesk.com/autodesk-university/class/DirectContext3D-API-Displaying-External-Graphics-Revit-2017#downloads
            IndexBuffer = new IndexBuffer(IndexBufferSizeInShorts());
        }

        public void SetVertexBuffer(ReadOnlySpan<RenderVertex> vertices)
        {
            EnsureVertexBufferCapacity(vertices.Length);
            if (VertexBuffer is null) return;

            var size = VertexBufferSizeInFloats();
            VertexBuffer.Map(size);
            try
            {
                var stream = VertexBuffer.GetVertexStreamPositionNormalColored();
                for (var i = 0; i < vertices.Length; i++)
                    stream.AddVertex(vertices[i].ToRevit());
            }
            finally
            {
                VertexBuffer.Unmap();
            }
        }

        public static int GetPrimitiveSize(PrimitiveType primitive)
        {
            switch (primitive)
            {
                case PrimitiveType.LineList: return IndexLine.GetSizeInShortInts();
                case PrimitiveType.PointList: return IndexPoint.GetSizeInShortInts();
                case PrimitiveType.TriangleList: return IndexTriangle.GetSizeInShortInts();
                default: break;
            }
            return IndexTriangle.GetSizeInShortInts();
        }

        public void SetIndexBuffer(ReadOnlySpan<int> indices)
        {
            Debug.Assert(indices.Length % PrimitiveSize == 0);

            EnsureIndexBufferCapacity(indices.Length);
            if (IndexBuffer is null) return;

            var size = IndexBufferSizeInShorts();
            IndexBuffer.Map(size);
            try
            {
                if (PrimitiveType == PrimitiveType.TriangleList)
                {
                    var s = IndexBuffer.GetIndexStreamTriangle();
                    for (var i = 0; i < indices.Length; i += 3)
                        s.AddTriangle(new IndexTriangle(indices[i], indices[i + 1], indices[i + 2]));
                }
                else if (PrimitiveType == PrimitiveType.LineList)
                {
                    var s = IndexBuffer.GetIndexStreamLine();
                    for (var i = 0; i < indices.Length; i += 2)
                        s.AddLine(new IndexLine(indices[i], indices[i + 1]));
                }
                else if (PrimitiveType == PrimitiveType.PointList)
                {
                    var s = IndexBuffer.GetIndexStreamPoint();
                    for (var i = 0; i < indices.Length; i++)
                        s.AddPoint(new IndexPoint(indices[i])); 
                }
                else
                {
                    throw new NotSupportedException($"Unsupported {nameof(PrimitiveType)}: {PrimitiveType}");
                }
            }
            finally
            {
                IndexBuffer.Unmap();
            }
        }

        public static int[] TriangleIndicesToEdgeIndices(ReadOnlySpan<int> tri)
        {
            if (tri.Length % 3 != 0)
                throw new ArgumentException("Triangle indices must be a multiple of 3.", nameof(tri));

            // Each triangle -> 3 edges -> 6 indices
            var edges = new int[(tri.Length / 3) * 6];
            var w = 0;

            for (var i = 0; i < tri.Length; i += 3)
            {
                var a = tri[i];
                var b = tri[i + 1];
                var c = tri[i + 2];

                edges[w++] = a; edges[w++] = b;
                edges[w++] = b; edges[w++] = c;
                edges[w++] = c; edges[w++] = a;
            }

            Debug.Assert(w == edges.Length);
            return edges;
        }
    }
}