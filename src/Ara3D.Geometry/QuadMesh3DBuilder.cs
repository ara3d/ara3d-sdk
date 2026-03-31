namespace Ara3D.Geometry;

public class QuadMesh3DBuilder
{
    public List<Point3D> Points { get; } = new();
    public List<QuadFaceData> Faces { get; } = new();
}

public class QuadFaceData
{
    public QuadFaceData(Integer4 cornerIndices, int groupId = -1)
    {
        CornerIndices = cornerIndices;
        GroupId = groupId;
    }
    public Integer4 CornerIndices { get; set; }
    public bool IsDeleted { get; set; }
    public int GroupId { get; set; }
}

public record Quad3DFaceHandle(
    QuadMesh3DBuilder Builder,
    int Index)
{
    public int IndexA => FaceData.CornerIndices.A;
    public int IndexB => FaceData.CornerIndices.B;
    public int IndexC => FaceData.CornerIndices.C;
    public int IndexD => FaceData.CornerIndices.D;
    public Point3D PointA => Builder.Points[IndexA];
    public Point3D PointB => Builder.Points[IndexB];
    public Point3D PointC => Builder.Points[IndexC];
    public Point3D PointD => Builder.Points[IndexD];
    public QuadFaceData FaceData => Builder.Faces[Index];
    public Quad3D Quad => (PointA, PointB, PointC, PointD);
}

public static class QuadMeshBuilderExtensions
{
    public static TriangleMesh3D ToTriangleMesh3D(this QuadMesh3DBuilder self)
        => ToQuadMesh3D(self).Triangulate();

    public static QuadMesh3D ToQuadMesh3D(this QuadMesh3DBuilder self) 
        => new(self.Points, self.Faces.Where(f => !f.IsDeleted).Select(f => f.CornerIndices).ToList());

    public static Quad3DFaceHandle GetFace(this QuadMesh3DBuilder self, int faceIndex)
        => new(self, faceIndex);

    public static IReadOnlyList<Quad3DFaceHandle> GetFaces(this QuadMesh3DBuilder self)
        => self.GetNumFaces().Range().Map(i => self.GetFace(i));

    public static Quad3DFaceHandle Delete(this Quad3DFaceHandle self)
    {
        self.FaceData.IsDeleted = true;
        return self;
    }

    public static Quad3DFaceHandle Restore(this Quad3DFaceHandle self)
    {
        self.FaceData.IsDeleted = false;
        return self;
    }

    public static Quad3DFaceHandle AddFace(this QuadMesh3DBuilder self, Quad3D q, int groupId = -1)
    {
        var n = self.Points.Count;
        self.Points.AddRange([q.A, q.B, q.C, q.D]);
        return self.AddFace(n, n + 1, n + 2, n + 3, groupId);
    }

    public static Quad3DFaceHandle AddFace(this QuadMesh3DBuilder self, int a, int b, int c, int d, int groupId = -1)
        => self.AddFace((a, b, c, d), groupId);

    public static Quad3DFaceHandle AddFace(this QuadMesh3DBuilder self, Integer4 f, int groupId = -1)
    {
        var r = new QuadFaceData(f, groupId);
        var n = self.Faces.Count;
        self.Faces.Add(r);
        return new(self, n);
    }

    public static QuadMesh3DBuilder AddFaces(this QuadMesh3DBuilder self, IReadOnlyList<Integer4> faces, int groupId = -1)
    {
        foreach (var f in faces)
            self.AddFace(f, groupId);
        return self;
    }

    public static Quad3DFaceHandle Insert(this Quad3DFaceHandle self, Quad3D q, int groupId = -1)
    {
        var bldr = self.Builder;
        var n = bldr.Points.Count;
        var f = self.FaceData.CornerIndices;
        bldr.Points.AddRange([q.A, q.B, q.C, q.D]);
        bldr.AddFace(f.A, f.B, n + 1, n);
        bldr.AddFace(f.B, f.C, n + 2, n + 1);
        bldr.AddFace(f.C, f.D, n + 3, n + 2);
        bldr.AddFace(f.D, f.A, n, n + 3);
        self.Delete();
        return bldr.AddFace(n, n + 1, n + 2, n + 3, groupId >= 0 ? groupId : self.FaceData.GroupId);
    }

    public static QuadMesh3DBuilder Subdivide(this Quad3DFaceHandle f, Vector2 uv, int groupId = -1)
    {
        var bldr = f.Builder;
        var n = bldr.Points.Count;
        var q = f.Quad;

        var midBottom = q.A.Lerp(q.B, uv.X);
        var midRight = q.B.Lerp(q.C, uv.Y);
        var midTop = q.D.Lerp(q.C, uv.X);
        var midLeft = q.A.Lerp(q.D, uv.Y);
        var center = q.Bilinear(uv);

        bldr.Points.AddRange([midBottom, midRight, midTop, midLeft, center]);

        var midBottomIndex = n;
        var midRightIndex = n + 1;
        var midTopIndex = n + 2;
        var midLeftIndex = n + 3;
        var centerIndex = n + 4;

        var g = groupId >= 0 ? groupId : f.FaceData.GroupId;
        bldr.AddFace(f.IndexA, midBottomIndex, centerIndex, midLeftIndex, g);
        bldr.AddFace(midBottomIndex, f.IndexB, midRightIndex, centerIndex, g);
        bldr.AddFace(centerIndex, midRightIndex, f.IndexC, midTopIndex, g);
        bldr.AddFace(midLeftIndex, centerIndex, midTopIndex, f.IndexD, g);
        
        f.Delete();

        return bldr;
    }

    public static (Quad3DFaceHandle Bottom, Quad3DFaceHandle Top) SplitTopBottom(this Quad3DFaceHandle f, float amount = 0.5f, int groupId = -1)
    {
        var builder = f.Builder;
        var n = builder.Points.Count;
        
        var midLeft = f.PointD.Lerp(f.PointA, amount);
        var midRight = f.PointC.Lerp(f.PointB, amount);

        builder.Points.AddRange([midRight, midLeft]);

        var midRightIndex = n;
        var midLeftIndex = n + 1;

        var g = groupId >= 0 ? groupId : f.FaceData.GroupId;
        var bottom = builder.AddFace(f.IndexA, f.IndexB, midRightIndex, midLeftIndex, g);
        var top = builder.AddFace(midLeftIndex, midRightIndex, f.IndexC, f.IndexD, g);
        f.Delete();

        return (top, bottom);
    }

    public static (Quad3DFaceHandle Left, Quad3DFaceHandle Right) SplitLeftRight(this Quad3DFaceHandle f, float amount = 0.5f, int groupId = -1)
    {
        var builder = f.Builder;
        var n = builder.Points.Count;

        var midBottom = f.PointA.Lerp(f.PointB, amount);
        var midTop = f.PointD.Lerp(f.PointC, amount);

        builder.Points.AddRange([midBottom, midTop]);

        var midBottomIndex = n;
        var midTopIndex = n + 1;

        var g = groupId >= 0 ? groupId : f.FaceData.GroupId;
        var left = builder.AddFace(f.IndexA, midBottomIndex, midTopIndex, f.IndexD, g);
        var right = builder.AddFace(midBottomIndex, f.IndexB, f.IndexC, midTopIndex, g);
        f.Delete();

        return (left, right);
    }

    public static QuadMesh3DBuilder Add(this QuadMesh3DBuilder self, QuadMesh3D mesh, int groupId = -1)
    {
        var n = self.Points.Count;
        self.Points.AddRange(mesh.Points);
        foreach (var f in mesh.FaceIndices)
            self.Faces.Add(new(f.Add(n), groupId));
        return self;
    }

    public static QuadMesh3DBuilder Add(this QuadMesh3DBuilder self, QuadGrid3D grid, int groupId = -1)
    {
        var n = self.Points.Count;
        self.Points.AddRange(grid.Points);
        foreach (var f in grid.FaceIndices)
            self.Faces.Add(new(f.Add(n), groupId));
        return self;
    }

    public static QuadMesh3DBuilder Add(this QuadMesh3DBuilder self, QuadMesh3DBuilder other, int groupId = -1)
    {
        var n = self.Points.Count;
        self.Points.AddRange(other.Points); 
        foreach (var f in other.Faces)
            self.Faces.Add(new(f.CornerIndices.Add(n), groupId >= 0 ? groupId : f.GroupId));
        return self;
    }

    public static Integer4 Add(this Integer4 self, int n)
        => (self.A + n, self.B + n, self.C + n, self.D + n);

    public static QuadMesh3DBuilder ExtrudePoints(this Quad3DFaceHandle f, float amount)
        => f.Move(f.Quad.Normal * amount);

    public static QuadMesh3DBuilder Transform(this Quad3DFaceHandle f, Func<Point3D, Point3D> func)
    {
        var builder = f.Builder;
        var q = f.Quad;
        builder.Points[f.IndexA] = func(q.A);
        builder.Points[f.IndexB] = func(q.B);
        builder.Points[f.IndexC] = func(q.C);
        builder.Points[f.IndexD] = func(q.D);
        return builder;
    }

    public static QuadMesh3DBuilder Move(this Quad3DFaceHandle f, Vector3 v)
        => f.Transform(p => p.Translate(v));

    public static Quad3DFaceHandle Inset(this Quad3DFaceHandle self, float x)
        => self.Inset(x, x, x, x);

    public static Quad3DFaceHandle Inset(this Quad3DFaceHandle f, float x0, float x1, float y0, float y1)
    {
        var q = f.Quad.InsetAbs(x0, x1, y0, y1);
        return f.Insert(q);
    }

    public static Quad3DFaceHandle Extrude(this Quad3DFaceHandle f, float amount)
        => f.Insert(f.Quad.Push(amount));

    public static Quad3DFaceHandle GetLastFace(this QuadMesh3DBuilder self)
        => self.GetFace(self.GetLastFaceIndex());


    // ------------------------------------------------------------
    // Basic counts / queries
    // ------------------------------------------------------------

    public static int GetNumFaces(this QuadMesh3DBuilder self)
        => self.Faces.Count;

    public static int GetLastFaceIndex(this QuadMesh3DBuilder self)
        => self.Faces.Count - 1;

    public static int GetNumPoints(this QuadMesh3DBuilder self)
        => self.Points.Count;

    public static bool IsValidFaceIndex(this QuadMesh3DBuilder self, int index)
        => index >= 0 && index < self.Faces.Count;

    public static bool IsValidPointIndex(this QuadMesh3DBuilder self, int index)
        => index >= 0 && index < self.Points.Count;

    public static bool IsDeleted(this Quad3DFaceHandle self)
        => self.FaceData.IsDeleted;

    public static Point3D GetPoint(this QuadMesh3DBuilder self, int index)
        => self.Points[index];

    public static QuadMesh3DBuilder SetPoint(this QuadMesh3DBuilder self, int index, Point3D p)
    {
        self.Points[index] = p;
        return self;
    }
    
    public static int DuplicatePoint(this QuadMesh3DBuilder self, int index)
        => self.AddPoint(self.Points[index]);

    // ------------------------------------------------------------
    // Face-local geometry
    // ------------------------------------------------------------

    public static Point3D Center(this Quad3DFaceHandle self)
        => (self.PointA + self.PointB + self.PointC + self.PointD) / 4f;

    public static Point3D MidBottom(this Quad3DFaceHandle self)
        => self.PointA.Lerp(self.PointB, 0.5f);

    public static Point3D MidRight(this Quad3DFaceHandle self)
        => self.PointB.Lerp(self.PointC, 0.5f);

    public static Point3D MidTop(this Quad3DFaceHandle self)
        => self.PointD.Lerp(self.PointC, 0.5f);

    public static Point3D MidLeft(this Quad3DFaceHandle self)
        => self.PointA.Lerp(self.PointD, 0.5f);

    public static Vector3 BottomVector(this Quad3DFaceHandle self)
        => self.PointB - self.PointA;

    public static Vector3 RightVector(this Quad3DFaceHandle self)
        => self.PointC - self.PointB;

    public static Vector3 TopVector(this Quad3DFaceHandle self)
        => self.PointC - self.PointD;

    public static Vector3 LeftVector(this Quad3DFaceHandle self)
        => self.PointD - self.PointA;

    public static Vector3 XAxis(this Quad3DFaceHandle self)
        => (self.PointB - self.PointA).Normalize();

    public static Vector3 YAxis(this Quad3DFaceHandle self)
        => (self.PointD - self.PointA).Normalize();

    public static Vector3 Normal(this Quad3DFaceHandle self)
        => self.Quad.Normal;

    public static float BottomLength(this Quad3DFaceHandle self)
        => self.BottomVector().Length;

    public static float RightLength(this Quad3DFaceHandle self)
        => self.RightVector().Length;

    public static float TopLength(this Quad3DFaceHandle self)
        => self.TopVector().Length;

    public static float LeftLength(this Quad3DFaceHandle self)
        => self.LeftVector().Length;

    // ------------------------------------------------------------
    // Face-local sampling
    // ------------------------------------------------------------

    public static Point3D PointAt(this Quad3DFaceHandle self, float u, float v)
        => self.PointAt((u, v));

    public static Point3D PointAt(this Quad3DFaceHandle self, Vector2 uv)
        => self.Quad.Bilinear(uv);

    // ------------------------------------------------------------
    // Generic face replacement
    // ------------------------------------------------------------

    public static QuadMesh3DBuilder SetFacePoints(this Quad3DFaceHandle self, Quad3D q)
    {
        var b = self.Builder;
        b.Points[self.IndexA] = q.A;
        b.Points[self.IndexB] = q.B;
        b.Points[self.IndexC] = q.C;
        b.Points[self.IndexD] = q.D;
        return b;
    }

    public static QuadMesh3DBuilder Push(this Quad3DFaceHandle self, float amount)
        => self.Translate(self.Normal() * amount);

    public static QuadMesh3DBuilder Translate(this Quad3DFaceHandle self, Vector3 delta)
        => self.SetFacePoints(self.Quad.Translate(delta));

    public static QuadMesh3DBuilder FlipFace(this Quad3DFaceHandle self)
    {
        self.FaceData.CornerIndices = (self.IndexD, self.IndexC, self.IndexB, self.IndexA);
        return self.Builder;
    }

    public static QuadMesh3DBuilder ReverseFace(this Quad3DFaceHandle self)
        => self.FlipFace();

    public static Quad3DFaceHandle CloneFace(this Quad3DFaceHandle self, int groupId = -1)
        => self.Builder.AddFace(self.Quad, groupId < 0 ? self.FaceData.GroupId : groupId);

    public static Quad3DFaceHandle CloneFace(this Quad3DFaceHandle self, Vector3 delta, int groupId = -1)
        => self.Builder.AddFace(self.Quad.Translate(delta), groupId < 0 ? self.FaceData.GroupId : groupId);

    // ------------------------------------------------------------
    // Edge quads / bridge building blocks
    // ------------------------------------------------------------

    public static Quad3DFaceHandle AddQuad(this QuadMesh3DBuilder self, Point3D a, Point3D b, Point3D c, Point3D d, int groupId = 0)
        => self.AddFace((a, b, c, d), groupId);

    public static Quad3DFaceHandle AddQuad(this QuadMesh3DBuilder self, int a, int b, int c, int d, int groupId = 0)
        => self.AddFace(a, b, c, d, groupId);

    public static Quad3DFaceHandle AddEdgeQuad(this QuadMesh3DBuilder self, int a, int b, int c, int d, int groupId = 0)
        => self.AddFace(a, b, c, d, groupId);

    public static QuadMesh3DBuilder Bridge(this QuadMesh3DBuilder self, Integer4 a, Integer4 b, int groupId = 0)
    {
        self.AddFace(a.A, a.B, b.B, b.A, groupId);
        self.AddFace(a.B, a.C, b.C, b.B, groupId);
        self.AddFace(a.C, a.D, b.D, b.C, groupId);
        self.AddFace(a.D, a.A, b.A, b.D, groupId);
        return self;
    }

    public static QuadMesh3DBuilder Bridge(this Quad3DFaceHandle a, Quad3DFaceHandle b, int groupId = 0)
        => a.Builder.Bridge(a.FaceData.CornerIndices, b.FaceData.CornerIndices, groupId);

    public static QuadMesh3DBuilder BridgeOpen(this QuadMesh3DBuilder self, Integer4 a, Integer4 b, bool bottom = true, bool right = true, bool top = true, bool left = true, int groupId = 0)
    {
        if (bottom) self.AddFace(a.A, a.B, b.B, b.A, groupId);
        if (right) self.AddFace(a.B, a.C, b.C, b.B, groupId);
        if (top) self.AddFace(a.C, a.D, b.D, b.C, groupId);
        if (left) self.AddFace(a.D, a.A, b.A, b.D, groupId);
        return self;
    }

    // ------------------------------------------------------------
    // Generic grid creation
    // ------------------------------------------------------------

    public static int AddPoint(this QuadMesh3DBuilder self, Point3D p)
    {
        var n = self.Points.Count;
        self.Points.Add(p);
        return n;
    }

    public static QuadMesh3DBuilder GridSubdivide(this Quad3DFaceHandle self, int xCount, int yCount)
    {
        if (xCount < 1 || yCount < 1)
            return self.Builder;

        if (xCount == 1 && yCount == 1)
            return self.Builder;

        var b = self.Builder;
        var idx = new int[xCount + 1, yCount + 1];

        for (var y = 0; y <= yCount; y++)
        {
            var fy = y / (float)yCount;
            for (var x = 0; x <= xCount; x++)
            {
                var fx = x / (float)xCount;
                idx[x, y] = b.AddPoint(self.PointAt(fx, fy));
            }
        }

        for (var y = 0; y < yCount; y++)
            for (var x = 0; x < xCount; x++)
                b.AddFace(idx[x, y], idx[x + 1, y], idx[x + 1, y + 1], idx[x, y + 1], self.FaceData.GroupId);

        self.Delete();
        return b;
    }


    // ------------------------------------------------------------
    // Small but useful composition helpers
    // ------------------------------------------------------------

    public static Quad3DFaceHandle AddFaceCopy(this QuadMesh3DBuilder self, Quad3DFaceHandle face, int groupId = -1)
        => self.AddFace(face.Quad, groupId < 0 ? face.FaceData.GroupId : groupId);

    public static QuadMesh3DBuilder AddRepeated(this QuadMesh3DBuilder self, Quad3DFaceHandle face, int count, Vector3 delta, int groupId = -1)
    {
        for (var i = 0; i < count; i++)
            self.AddFace(face.Quad.Translate(delta * i), groupId < 0 ? face.FaceData.GroupId : groupId);
        return self;
    }
    
    public static QuadMesh3DBuilder ReplaceFace(
        this Quad3DFaceHandle self,
        Func<Quad3D, Quad3D> f)
    {
        return self.SetFacePoints(f(self.Quad));
    }

    public static QuadMesh3DBuilder TransformFacePoints(
        this Quad3DFaceHandle self,
        Func<Point3D, Point3D> f)
    {
        var q = self.Quad;
        return self.SetFacePoints((f(q.A), f(q.B), f(q.C), f(q.D)));
    }

    // ------------------------------------------------------------
    // Constructors
    // ------------------------------------------------------------

    public static QuadMesh3DBuilder ToBuilder(this QuadMesh3D self)
        => new QuadMesh3DBuilder().Add(self);

    public static QuadMesh3DBuilder ToBuilder(this QuadGrid3D self)
        => new QuadMesh3DBuilder().Add(self);
}