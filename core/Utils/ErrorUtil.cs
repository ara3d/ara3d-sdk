using System;
using System.Runtime.ExceptionServices;

namespace Ara3D.Utils
{
    public static class ErrorUtil
    {
        public static void SetFirstChanceExceptionCallback(Action<object, FirstChanceExceptionEventArgs> handler)
            => AppDomain.CurrentDomain.FirstChanceException += (sender, args) => handler(sender, args);

        public static void SetUnhandledExceptionCallback(Action<object, UnhandledExceptionEventArgs> handler)
            => AppDomain.CurrentDomain.UnhandledException += (sender, args) => handler(sender, args);

        public static ErrorData ToErrorData(this Exception exception, bool caught = true)
            => new ErrorData(exception, caught);
    }
}