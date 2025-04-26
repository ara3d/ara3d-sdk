namespace Ara3D.Scene
{
    public interface IScene
    {
        IReadOnlyList<ISceneNode> Nodes { get; }
    }
}