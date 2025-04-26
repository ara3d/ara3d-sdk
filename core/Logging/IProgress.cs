using System;

namespace Ara3D.Logging
{
    public interface IProgress
    {
        double CurrentProgress { get; }
        double TotalProgress { get; }
        event EventHandler<IProgress> ProgressChanged;
    }
}