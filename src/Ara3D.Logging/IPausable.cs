namespace Ara3D.Logging
{
    public interface IPausable
    {
        bool IsPaused { get; }
        void Pause();
        void Resume();
    }

    // https://stackoverflow.com/a/60221346/184528
}