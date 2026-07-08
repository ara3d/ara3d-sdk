using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>Direct tessellated geometry: triangulated and polygonal face sets.</summary>
public static class Tessellated
{
    public static TriangleMesh3D BuildTriangulatedFaceSet(MeshingContext ctx, IfcEntity entity)
    {
        ctx.Diagnostics.RecordSupported("IFCTRIANGULATEDFACESET");
        var coords = ReadCartesianPointList3D(ctx, entity);
        var coordId = entity.GetId(IfcTriangulatedFaceSet.Instance.Coordinates.Index);
        var faces = ReadTriangulatedFaces(ctx.GetEntity(coordId), entity, coords);
        return new TriangleMesh3D(coords, faces);
    }

    public static TriangleMesh3D BuildPolygonalFaceSet(MeshingContext ctx, IfcEntity entity)
    {
        ctx.Diagnostics.RecordSupported("IFCPOLYGONALFACESET");
        var coords = ReadCartesianPointList3D(ctx, entity);
        var faces = new List<Integer3>();
        foreach (var faceId in MeshHelpers.ReadIds(entity, IfcPolygonalFaceSet.Instance.Faces))
        {
            var face = ctx.GetEntity(faceId);
            var name = face.GetEntityName();
            if (name == "IFCINDEXEDPOLYGONALFACEWITHVOIDS")
            {
                ctx.Diagnostics.RecordSupported("IFCINDEXEDPOLYGONALFACEWITHVOIDS");
                TriangulatePolygonFaceWithVoids(ctx, coords, face, faces);
            }
            else if (name == "IFCINDEXEDPOLYGONALFACE")
            {
                ctx.Diagnostics.RecordSupported("IFCINDEXEDPOLYGONALFACE");
                TriangulatePolygonFace(ctx, coords, ReadPositiveIndices(face, IfcIndexedPolygonalFace.Instance.CoordIndex.Index), faces);
            }
        }
        return new TriangleMesh3D(coords, faces);
    }

    static List<Point3D> ReadCartesianPointList3D(MeshingContext ctx, IfcEntity faceSet)
    {
        var coordEntity = MeshHelpers.ResolveRequired(ctx, faceSet, IfcTessellatedFaceSet.Instance.Coordinates);
        var points = new List<Point3D>();
        var token = coordEntity.GetValue(IfcCartesianPointList3D.Instance.CoordList.Index);
        if (!token.IsList)
            return points;
        foreach (var item in token.AsList(coordEntity.Document))
        {
            if (!item.IsList)
                continue;
            var nums = item.AsList(coordEntity.Document).Where(t => t.IsNumber).Select(t => t.AsNumber()).ToList();
            points.Add(new Vector3(
                ctx.ScaleLength(nums.Count > 0 ? nums[0] : 0),
                ctx.ScaleLength(nums.Count > 1 ? nums[1] : 0),
                ctx.ScaleLength(nums.Count > 2 ? nums[2] : 0)));
        }
        return points;
    }

    static List<Integer3> ReadTriangulatedFaces(IfcEntity coordEntity, IfcEntity faceSet, IReadOnlyList<Point3D> coords)
    {
        var faces = new List<Integer3>();
        var token = faceSet.GetValue(IfcTriangulatedFaceSet.Instance.CoordIndex.Index);
        var indices = FlattenPositiveIndices(faceSet.Document, token).Select(i => i - 1).ToList();
        for (var i = 0; i + 2 < indices.Count; i += 3)
            faces.Add(new Integer3(indices[i], indices[i + 1], indices[i + 2]));
        return faces;
    }

    static IEnumerable<int> FlattenPositiveIndices(StepDocument doc, StepToken token)
    {
        if (token.IsNumber)
            yield return (int)token.AsNumber();
        if (token.IsList)
        {
            foreach (var child in token.AsList(doc))
            {
                foreach (var idx in FlattenPositiveIndices(doc, child))
                    yield return idx;
            }
        }
    }

    static List<int> ReadPositiveIndices(IfcEntity entity, int index)
    {
        var token = entity.GetValue(index);
        return FlattenPositiveIndices(entity.Document, token).Select(i => i - 1).ToList();
    }

    static List<List<int>> ReadNestedPositiveIndices(IfcEntity entity, int index)
    {
        var token = entity.GetValue(index);
        if (!token.IsList)
            return [];
        var holes = new List<List<int>>();
        foreach (var child in token.AsList(entity.Document))
        {
            var ring = FlattenPositiveIndices(entity.Document, child).Select(i => i - 1).ToList();
            if (ring.Count >= 3)
                holes.Add(ring);
        }
        return holes;
    }

    static void TriangulatePolygonFace(MeshingContext ctx, IReadOnlyList<Point3D> coords, IReadOnlyList<int> indices, List<Integer3> faces)
        => TriangulatePolygonIndexFace(coords, indices.ToList(), [], faces);

    static void TriangulatePolygonFaceWithVoids(MeshingContext ctx, IReadOnlyList<Point3D> coords, IfcEntity face, List<Integer3> faces)
    {
        var outer = ReadPositiveIndices(face, IfcIndexedPolygonalFaceWithVoids.Instance.CoordIndex.Index);
        var holes = ReadNestedPositiveIndices(face, IfcIndexedPolygonalFaceWithVoids.Instance.InnerCoordIndices.Index);
        TriangulatePolygonIndexFace(coords, outer, holes, faces);
    }

    static void TriangulatePolygonIndexFace(
        IReadOnlyList<Point3D> coords,
        List<int> outerIndices,
        List<List<int>> holeIndices,
        List<Integer3> faces)
    {
        if (outerIndices.Count < 3)
            return;
        var outer2 = outerIndices.Select(i => new Vector2(coords[i].X.Value, coords[i].Y.Value)).ToList();
        var holes2 = holeIndices.Select(h => h.Select(i => new Vector2(coords[i].X.Value, coords[i].Y.Value)).ToList()).ToList();
        var tris = holes2.Count == 1 && PolygonWithHoles.TryTriangulateCongruentRing(outer2, holes2[0], out var ringTris)
            ? ringTris
            : PolygonTriangulator.GetTriangles(outer2, holes2);
        var searchIndices = outerIndices.Concat(holeIndices.SelectMany(h => h)).Distinct().ToList();
        foreach (var tri in tris)
        {
            var a = FindNearestIndex(coords, searchIndices, tri.A.Vector2);
            var b = FindNearestIndex(coords, searchIndices, tri.B.Vector2);
            var c = FindNearestIndex(coords, searchIndices, tri.C.Vector2);
            faces.Add(new Integer3(a, b, c));
        }
    }

    static int FindNearestIndex(IReadOnlyList<Point3D> coords, IReadOnlyList<int> indices, Vector2 target)
    {
        var best = indices[0];
        var bestDist = float.MaxValue;
        foreach (var idx in indices)
        {
            var p = new Vector2(coords[idx].X.Value, coords[idx].Y.Value);
            var d = p.DistanceSquared(target);
            if (d < bestDist)
            {
                bestDist = d;
                best = idx;
            }
        }
        return best;
    }
}
