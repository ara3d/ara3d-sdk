namespace Ara3D.Geometry;

public class CellGridBuilder3D
{
    private readonly bool[,,] _occupied;

    public int SizeX { get; }
    public int SizeY { get; }
    public int SizeZ { get; }

    public CellGridBuilder3D(int sx, int sy, int sz)
    {
        if (sx <= 0) throw new ArgumentOutOfRangeException(nameof(sx));
        if (sy <= 0) throw new ArgumentOutOfRangeException(nameof(sy));
        if (sz <= 0) throw new ArgumentOutOfRangeException(nameof(sz));

        SizeX = sx;
        SizeY = sy;
        SizeZ = sz;

        _occupied = new bool[sx, sy, sz];
        for (var x = 0; x < sx; x++)
        for (var y = 0; y < sy; y++)
        for (var z = 0; z < sz; z++)
            _occupied[x, y, z] = true;
    }

    public CellGridBuilder3D Remove(int x, int y, int z)
    {
        _occupied[x, y, z] = false;
        return this;
    }

    public CellGridBuilder3D Add(int x, int y, int z)
    {
        _occupied[x, y, z] = true;
        return this;
    }

    public IReadOnlyList<Point3D> GetLatticeVertices()
    {
        var vertices = new List<Point3D>();
        for (var x = 0; x <= SizeX; x++)
        for (var y = 0; y <= SizeY; y++)
        for (var z = 0; z <= SizeZ; z++)
            vertices.Add((
                x / (float)SizeX - 0.5f, 
                y / (float)SizeY - 0.5f, 
                z / (float)SizeZ - 0.5f));
        return vertices;
    }

    public int GetVertex(int x, int y, int z)
        => x * (SizeY + 1) 
            * (SizeZ + 1) + y 
            * (SizeZ + 1) + z;

    bool IsOccupied(int x, int y, int z)
        => x > 0 && x < SizeX &&
           y > 0 && y < SizeY &&
           z > 0 && z < SizeZ && 
           _occupied[x, y, z];

    public IReadOnlyList<Integer4> GetQuadFaces()
    {
        var faces = new List<Integer4>();

        for (var x = 0; x < SizeX; x++)
        {
            for (var y = 0; y < SizeY; y++)
            {
                for (var z = 0; z < SizeZ; z++)
                {
                    if (!_occupied[x, y, z])
                        continue;

                    // Emit a face only if the neighbor in that direction is empty/outside.

                    if (!IsOccupied(x - 1, y, z))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x, y, z),
                            GetVertex(x, y, z + 1),
                            GetVertex(x, y + 1, z + 1),
                            GetVertex(x, y + 1, z)));
                    }

                    if (!IsOccupied(x + 1, y, z))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x + 1, y, z),
                            GetVertex(x + 1, y + 1, z),
                            GetVertex(x + 1, y + 1, z + 1),
                            GetVertex(x + 1, y, z + 1)));
                    }

                    if (!IsOccupied(x, y - 1, z))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x, y, z),
                            GetVertex(x + 1, y, z),
                            GetVertex(x + 1, y, z + 1),
                            GetVertex(x, y, z + 1)));
                    }

                    if (!IsOccupied(x, y + 1, z))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x, y + 1, z),
                            GetVertex(x, y + 1, z + 1),
                            GetVertex(x + 1, y + 1, z + 1),
                            GetVertex(x + 1, y + 1, z)));
                    }

                    if (!IsOccupied(x, y, z - 1))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x, y, z),
                            GetVertex(x, y + 1, z),
                            GetVertex(x + 1, y + 1, z),
                            GetVertex(x + 1, y, z)));
                    }

                    if (!IsOccupied(x, y, z + 1))
                    {
                        faces.Add(new Integer4(
                            GetVertex(x, y, z + 1),
                            GetVertex(x + 1, y, z + 1),
                            GetVertex(x + 1, y + 1, z + 1),
                            GetVertex(x, y + 1, z + 1)));
                    }
                }
            }
        }

        return faces;
    }  
        
    public QuadMesh3D ToMesh()
    {
        var vertices = GetLatticeVertices();
        var faces = GetQuadFaces();
        return new QuadMesh3D(vertices, faces);
    }
}