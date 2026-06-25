namespace Ara3D.Graphics
{
    public interface IProgressiveRenderer : IBitmap
    {
        int MaxIterations { get; }
        IBitmap GetIteration(int iteration);
    }
}