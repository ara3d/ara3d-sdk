using System.Collections;

namespace Ara3D.Geometry;

public class VoxelizedField : IVoxels
{
    public Bounds3D Bounds { get; }
        
    public int NumColumns { get; }
    public int NumRows { get; }
    public int NumLayers { get; }

    public Vector3 VoxelSize { get; }

    public int Count => NumColumns * NumRows * NumLayers;
    public int LayerSize => NumColumns * NumRows;
    public Voxel this[int index] => this[index % LayerSize, (index / LayerSize) % NumRows, index / LayerSize / NumRows];
    public Voxel this[int column, int row, int layer] => GetVoxel(column, row, layer);

    private readonly Vector3 _size;
    private readonly Func<Point3D, Number> _function;

    public VoxelizedField(Bounds3D bounds, int numColumns, int numRows, int numLayers, Func<Point3D, Number> f)
    {
        Bounds = bounds;
        NumColumns = numColumns;
        NumRows = numRows;
        NumLayers = numLayers;
        _function = f;
        _size = bounds.Size;
        VoxelSize = _size / new Vector3(numColumns, numRows, numLayers);
    }
        
    public Point3D GetVoxelCenter(int i, int j, int k)
    {
        var offset = new Vector3(i, j, k) * VoxelSize;
        return Bounds.Min + offset + VoxelSize / 2;
    }

    public Bounds3D GetVoxelBounds(int i, int j, int k)
    {
        var voxelCenter = GetVoxelCenter(i, j, k);
        var halfSize = VoxelSize / 2;
        return new Bounds3D(voxelCenter - halfSize, voxelCenter + halfSize);
    }

    public Number GetVoxelValue(int i, int j, int k)
        => _function(GetVoxelCenter(i, j, k));

    public Voxel GetVoxel(int i, int j, int k)
    {
        var voxelPos = GetVoxelCenter(i, j, k);
        var value = _function(voxelPos);
        return new Voxel(voxelPos, value);
    }

    public IEnumerator<Voxel> GetEnumerator()
    {
        for (var i = 0; i < NumColumns; i++)
        for (var j = 0; j < NumRows; j++)
        for (var k = 0; k < NumLayers; k++)
            yield return GetVoxel(i, j, k);
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}