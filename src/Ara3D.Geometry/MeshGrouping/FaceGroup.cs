namespace Ara3D.Geometry;

public class FaceGroup
{
    public int GroupId { get; }
    public List<int> FaceIds { get; } = [];

    public int Count => FaceIds.Count;

    public FaceGroup(int groupId)
        => GroupId = groupId;

    public void Add(int faceId)
        => FaceIds.Add(faceId);
}