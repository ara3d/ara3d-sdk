using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.Geometry;

/// <summary>
/// Isotropic explicit remeshing for triangle meshes. Splits long edges, collapses short edges,
/// and applies tangential smoothing to produce elements suitable for FEM-style analysis.
/// </summary>
public static class IsotropicRemesher
{
    public readonly record struct Settings(
        double TargetEdgeLength,
        int Iterations = 3,
        int SmoothingIterations = 2,
        float SmoothingStrength = 0.5f,
        double SplitThresholdRatio = 4.0 / 3.0,
        double CollapseThresholdRatio = 4.0 / 5.0,
        bool CollapseShortEdges = true);

    public static TriangleMesh3D Remesh(this TriangleMesh3D mesh, Settings settings)
    {
        if (mesh.FaceIndices.Count == 0)
            return mesh;

        var target = settings.TargetEdgeLength > 0
            ? settings.TargetEdgeLength
            : EstimateTargetEdgeLength(mesh);

        if (target <= 0)
            return mesh;

        var iterations = Math.Max(0, settings.Iterations);
        var smoothingIterations = Math.Max(0, settings.SmoothingIterations);
        var smoothingStrength = Math.Clamp(settings.SmoothingStrength, 0f, 1f);

        var splitRatio = Math.Max(1.0, settings.SplitThresholdRatio);
        var collapseRatio = Math.Max(0.0, settings.CollapseThresholdRatio);

        var splitLength = target * splitRatio;
        var collapseLength = target * collapseRatio;

        var result = mesh.RemoveUnreferencedPoints().WeldVertices();

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            result = SplitLongEdges(result, splitLength);

            if (settings.CollapseShortEdges && collapseLength > 0)
                result = CollapseShortEdges(result, collapseLength);

            result = result.RemoveUnreferencedPoints();

            for (var smooth = 0; smooth < smoothingIterations; smooth++)
                result = TangentialSmooth(result, smoothingStrength);
        }

        return result.RemoveUnreferencedPoints().WeldVertices();
    }

    static double EstimateTargetEdgeLength(TriangleMesh3D mesh)
    {
        var topo = new Topology(mesh);
        var attrs = new MeshAttributes(mesh, topo);

        var edgeSum = 0.0;
        var edgeCount = 0;

        foreach (var edgeId in topo.GetUndirectedEdgeIds())
        {
            edgeSum += attrs.GetEdgeLength(edgeId);
            edgeCount++;
        }

        if (edgeCount > 0)
            return edgeSum / edgeCount;

        if (mesh.Points.Count == 0)
            return 1.0;

        var bounds = mesh.Points.Bounds();
        var diagonal = bounds.DiagonalLength();

        return diagonal > 0 ? diagonal / 32.0 : 1.0;
    }

    public static TriangleMesh3D CollapseShortEdges(this TriangleMesh3D mesh, double minLength)
    {
        if (minLength <= 0)
            return mesh;

        var points = mesh.Points.ToList();
        var faces = mesh.FaceIndices.ToList();

        while (TryCollapseShortestEdge(points, faces, minLength))
        {
        }

        return new TriangleMesh3D(points, faces).RemoveUnreferencedPoints();
    }


    public static TriangleMesh3D SplitLongEdges(this TriangleMesh3D mesh, double maxLength)
    {
        if (maxLength <= 0)
            return mesh;

        var points = mesh.Points.ToList();
        var faces = mesh.FaceIndices.ToList();

        while (TrySplitLongestEdge(points, faces, maxLength))
        {
        }

        return new TriangleMesh3D(points, faces);
    }

    static bool TrySplitLongestEdge(
        List<Point3D> points,
        List<Integer3> faces,
        double maxLength)
    {
        var mesh = new TriangleMesh3D(points, faces);
        var topo = new Topology(mesh);
        var attrs = new MeshAttributes(mesh, topo);

        var bestLength = maxLength;
        var bestEdgeId = default(UndirectedEdgeId);
        var found = false;

        foreach (var edgeId in topo.GetUndirectedEdgeIds())
        {
            if (!IsEditableEdge(topo, edgeId))
                continue;

            var length = attrs.GetEdgeLength(edgeId);
            if (length <= bestLength)
                continue;

            bestLength = length;
            bestEdgeId = edgeId;
            found = true;
        }

        return found && SplitEdge(points, faces, topo, bestEdgeId);
    }

    static bool SplitEdge(
        List<Point3D> points,
        List<Integer3> faces,
        Topology topo,
        UndirectedEdgeId edgeId)
    {
        if (!topo.TryGetFirstHalfEdge(edgeId, out var halfEdge))
            return false;

        var a = (int)topo.GetStartVertex(halfEdge);
        var b = (int)topo.GetEndVertex(halfEdge);
        var midpoint = points.Count;

        points.Add(points[a].Average(points[b]));

        var facesToRemove = new HashSet<int>();
        var newFaces = new List<Integer3>();

        foreach (var he in topo.GetHalfEdgeIds(edgeId))
        {
            var faceIndex = (int)topo.GetAssociatedFaceId(he);
            facesToRemove.Add(faceIndex);

            var va = (int)topo.GetStartVertex(he);
            var vb = (int)topo.GetEndVertex(he);
            var vc = (int)topo.GetEndVertex(topo.GetNext(he));

            newFaces.Add(new Integer3(va, midpoint, vc));
            newFaces.Add(new Integer3(midpoint, vb, vc));
        }

        var keptFaces = faces
            .Where((_, index) => !facesToRemove.Contains(index))
            .ToList();

        faces.Clear();
        faces.AddRange(keptFaces);
        faces.AddRange(newFaces);

        return true;
    }

    static bool TryCollapseShortestEdge(
        List<Point3D> points,
        List<Integer3> faces,
        double minLength)
    {
        var mesh = new TriangleMesh3D(points, faces);
        var topo = new Topology(mesh);
        var attrs = new MeshAttributes(mesh, topo);

        var bestLength = minLength;
        var bestEdgeId = default(UndirectedEdgeId);
        var found = false;

        foreach (var edgeId in topo.GetUndirectedEdgeIds())
        {
            if (!IsEditableEdge(topo, edgeId))
                continue;

            var length = attrs.GetEdgeLength(edgeId);
            if (length >= bestLength)
                continue;

            if (!topo.TryGetFirstHalfEdge(edgeId, out var halfEdge))
                continue;

            var a = topo.GetStartVertex(halfEdge);
            var b = topo.GetEndVertex(halfEdge);
            var incidentFaceCount = topo.GetHalfEdgeIds(edgeId).Count;

            if (!SatisfiesLinkCondition(topo, a, b, incidentFaceCount))
                continue;

            bestLength = length;
            bestEdgeId = edgeId;
            found = true;
        }

        return found && CollapseEdge(points, faces, topo, bestEdgeId);
    }

    static bool CollapseEdge(
        List<Point3D> points,
        List<Integer3> faces,
        Topology topo,
        UndirectedEdgeId edgeId)
    {
        if (!topo.TryGetFirstHalfEdge(edgeId, out var halfEdge))
            return false;

        var keep = (int)topo.GetStartVertex(halfEdge);
        var remove = (int)topo.GetEndVertex(halfEdge);

        points[keep] = points[keep].Average(points[remove]);
        ReplaceVertexAndRemoveDegenerateFaces(faces, keep, remove);

        return true;
    }

    static bool IsEditableEdge(Topology topo, UndirectedEdgeId edgeId)
    {
        var incidentFaceCount = topo.GetHalfEdgeIds(edgeId).Count;
        return incidentFaceCount is 1 or 2 && topo.IsManifold(edgeId);
    }

    static bool SatisfiesLinkCondition(
        Topology topo,
        VertexId a,
        VertexId b, 
        int incidentFaceCount)
    {
        var neighborsA = topo.GetNeighborVertexIds(a).ToHashSet();
        var neighborsB = topo.GetNeighborVertexIds(b).ToHashSet();

        neighborsA.Remove(b);
        neighborsB.Remove(a);

        neighborsA.IntersectWith(neighborsB);

        var requiredSharedNeighborCount = incidentFaceCount == 1 ? 1 : 2;
        return neighborsA.Count == requiredSharedNeighborCount;
    }

    static void ReplaceVertexAndRemoveDegenerateFaces(
        List<Integer3> faces,
        int keep,
        int remove)
    {
        var result = new List<Integer3>(faces.Count);

        // TODO: the face key needs to be considered. 
        var seen = new HashSet<(int A, int B, int C)>();

        foreach (var face in faces)
        {
            var a = (int)(face.A == remove ? keep : face.A);
            var b = (int)(face.B == remove ? keep : face.B);
            var c = (int)(face.C == remove ? keep : face.C);

            if (a == b || b == c || c == a)
                continue;

            var key = GetUndirectedFaceKey(a, b, c);
            if (!seen.Add(key))
                continue;

            result.Add(new Integer3(a, b, c));
        }

        // TODO: this seems like there is a better way to do this. 
        faces.Clear();
        faces.AddRange(result);
    }

    // TODO: I can see how this might be useful. It should probably be lifted out to tTopology. It also needs its own record struct.
    static (int A, int B, int C) GetUndirectedFaceKey(int a, int b, int c)
    {
        if (a > b)
            (a, b) = (b, a);

        if (b > c)
            (b, c) = (c, b);

        if (a > b)
            (a, b) = (b, a);

        return (a, b, c);
    }

    public static TriangleMesh3D TangentialSmooth(
        this TriangleMesh3D mesh,
        float strength)
    {
        if (strength <= 0)
            return mesh;

        var topo = new Topology(mesh);
        var attrs = new MeshAttributes(mesh, topo);
        var normals = attrs.Vertices.AngleWeightedNormals;
        var points = mesh.Points;
        var updated = points.ToArray();

        foreach (var vertexId in topo.GetVertexIds())
        {
            var index = (int)vertexId;

            // NOTE: this doesn't need to be here. 
            if (topo.IsBoundary(vertexId))
                continue;

            var neighborSum = Vector3.Zero;
            var neighborCount = 0;

            // NOTE: this is like a neighbourhood centroid computation. It is used in a lot of computations. 

            foreach (var neighbor in topo.GetNeighborVertexIds(vertexId))
            {
                neighborSum += points[(int)neighbor].Vector3;
                neighborCount++;
            }

            if (neighborCount == 0)
                continue;

            var current = points[index].Vector3;
            var centroid = neighborSum / neighborCount;
            var normal = normals[index];
            var normalLengthSquared = normal.LengthSquared;

            if (normalLengthSquared < 1e-12f)
                continue;

            var delta = centroid - current;
            var normalComponent = Vector3.Dot(delta, normal) / normalLengthSquared;
            var tangential = delta - normal * normalComponent;

            updated[index] = current + tangential * strength;
        }

        return new TriangleMesh3D(updated, mesh.FaceIndices);
    }
}