using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    /// <summary>
    /// The base class of topological elements: face, corner, half-edge, and vertex
    /// </summary>
    public class TopologicalElement
    {
        public TopologicalElement(Topology topology, int index)
            => (Topology, Index) = (topology, index);
        public Topology Topology { get; }
        public int Index { get; }
    }

    /// <summary>
    /// A topological face. 
    /// </summary>
    public class TopoFace : TopologicalElement
    {
        public TopoFace(Topology topology, int index)
            : base(topology, index)
        { }
    }

    /// <summary>
    /// A directed edge around a polygon (aka a half-edge). There is exactly one half-edge per "corner" in a mesh.
    /// A non-border edge in a manifold mesh has exactly two half-edges, and a border edge has one edge.  
    /// </summary>
    public class TopoEdge : TopologicalElement
    {
        public TopoEdge(Topology topology, int index)
            : base(topology, index)
        {}
    }

    /// <summary>
    /// A vertex in the mesh. 
    /// </summary>
    public class TopoVertex : TopologicalElement
    {
        public TopoVertex(Topology topology, int index)
            : base(topology, index)
        { }
    }

    /// <summary>
    /// Also called a "face-corner". Associated with exactly one face, and one vertex.
    /// A vertex may be associated with multiple corners 
    /// </summary>
    public class TopoCorner : TopologicalElement
    {
        public TopoCorner(Topology topology, int index)
            : base(topology, index)
        { }
    }

    /// <summary>
    /// This class is used to make efficient topological queries for a triangle mesh.
    /// Construction is a O(N) operation, so it is not created automatically. 
    /// </summary>
    public class Topology
    {
        public Topology(ITriMesh m)
        {
            if (m is TriMesh triMesh)
            {
                NumCornersPerFace = 3;

            }
            else if (m is QuadMesh quadMesh)
            {
                NumCornersPerFace = 4;
            }
            else
            {
                throw new Exception("Topological queries only supported on TriMesh or QuadMesh");
            }
            Corners = m.Indices();
            Faces = m.GetNumFaces().Range();
            Vertices = m.Points.Indices();

            // Compute the mapping from vertex indices to faces that reference them 
            VerticesToFaces = new List<int>[m.Points.Count];
            for (var c = 0; c < Corners.Count; ++c)
            {
                var v = Corners[c];
                var f = m.CornerToFace(c);

                Debug.Assert(f.Within(0, m.GetNumFaces()));
                Debug.Assert(v.Within(0, m.GetNumVertices()));
                Debug.Assert(c.Within(0, m.GetNumCorners()));

                if (VerticesToFaces[v] == null)
                    VerticesToFaces[v] = new List<int> { f };
                else
                    VerticesToFaces[v].Add(f);
            }

            // NOTE: the same edge can occur in more than two faces, only in non-manifold meshes

            // Compute the face on the other side of an edge 
            EdgeToOtherFace = (-1).Repeat(Corners.Count).ToArray();
            for (var c = 0; c < Corners.Count; ++c)
            {
                var c2 = NextCorner(c);
                var f0 = CornerToFace(c);
                foreach (var f1 in FacesFromCorner(c).ToEnumerable())
                {
                    if (f1 != f0)
                    {
                        foreach (var f2 in FacesFromCorner(c2).ToEnumerable())
                        {
                            if (f2 == f1)
                            {
                                if (EdgeToOtherFace[c] != -1)
                                    NonManifold = true;
                                EdgeToOtherFace[c] = f2;
                            }
                        }
                    }
                }
            }

            // TODO: there is some serious validation I coudl be doing doing here.
        }
        

        public List<int>[] VerticesToFaces { get; }
        public int[] EdgeToOtherFace { get; } // Assumes manifold meshes
        public bool NonManifold { get; }
        public IArray<int> Corners { get; }
        public IArray<int> Vertices { get; }
        public IArray<int> Edges => Corners;
        public IArray<int> Faces { get; }
        public int NumCornersPerFace { get; }

        public int CornerToFace(int i)
            => i % NumCornersPerFace;

        public IArray<int> FacesFromVertexIndex(int v)
            => VerticesToFaces[v]?.ToIArray() ?? 0.Repeat(0);

        public IArray<int> FacesFromCorner(int c)
            => throw new NotImplementedException();

        public int VertexIndexFromCorner(int c)
            => throw new NotImplementedException();

        /// <summary>
        /// Differs from neighbour faces in that the faces have to share an edge, not just a vertex.
        /// An alternative construction would have been to getNeighbourFaces and filter out those that don't share
        /// </summary>
        public IEnumerable<int> BorderingFacesFromFace(int f)
            => EdgesFromFace(f).Select(BorderFace).Where(bf => bf >= 0);

        public int BorderFace(int e)
            => EdgeToOtherFace[e];

        public bool IsBorderEdge(int e)
            => EdgeToOtherFace[e] < 0;

        public bool IsBorderFace(int f)
            => EdgesFromFace(f).Any(IsBorderEdge);

        public IArray<int> CornersFromFace(int f)
            => NumCornersPerFace.Range().Add(FirstCornerInFace(f));

        public IArray<int> EdgesFromFace(int f)
            => CornersFromFace(f);

        public int FirstCornerInFace(int f)
            => f * NumCornersPerFace;

        public bool FaceHasCorner(int f, int c)
            => CornersFromFace(f).Contains(c);

        public int NextCorner(int c)
        {
            var f = CornerToFace(c);
            var begin = FirstCornerInFace(f);
            var end = begin + NumCornersPerFace;
            Debug.Assert(c >= begin);
            Debug.Assert(c < end);
            var c2 = c + 1;
            if (c2 < end)
                return c2;
            Debug.Assert(c2 == end);
            return begin;
        }

        public IArray<int> CornersFromEdge(int e)
            => LinqArray.Create(e, NextCorner(e));

        public IArray<int> VertexIndicesFromEdge(int e)
            => CornersFromEdge(e).Select(VertexIndexFromCorner);

        public IArray<int> VertexIndicesFromFace(int f)
            => throw new NotImplementedException();

        public IEnumerable<int> NeighbourVertices(int v)
            => FacesFromVertexIndex(v).SelectMany(VertexIndicesFromFace).Where(v2 => v2 != v).Distinct();

        public IEnumerable<int> BorderEdges
            => Edges.Where(IsBorderEdge);

        public IEnumerable<int> BorderFaces
            => Faces.Where(IsBorderFace);

        public int EdgeFirstCorner(int e)
            => e;

        public int EdgeNextCorner(int e)
            => NextCorner(e);

        public int EdgeFirstVertex(int e)
            => VertexIndexFromCorner(EdgeFirstCorner(e));

        public int EdgeNextVertex(int e)
            => VertexIndexFromCorner(EdgeFirstCorner(e));

        public IArray<int> EdgeVertices(int e)
            => LinqArray.Create(EdgeFirstVertex(e), EdgeNextVertex(e));

        public Vector3 PointFromVertex(int v)
            => throw new NotImplementedException();

        public IArray<Vector3> EdgePoints(int e)
            => EdgeVertices(e).Select(PointFromVertex);
    }
    
}
