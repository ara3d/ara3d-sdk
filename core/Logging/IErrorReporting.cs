using System;

namespace Ara3D.Logging
{
    public interface IErrorHandler
    {
        event EventHandler<Exception> ExceptionHandler;
        Exception Exception { get; }
    }
}