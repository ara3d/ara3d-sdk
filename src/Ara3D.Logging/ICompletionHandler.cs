using System;

namespace Ara3D.Logging
{
    public interface ICompletionHandler
    {
        event EventHandler<object> CompletionHandler;
    }
}