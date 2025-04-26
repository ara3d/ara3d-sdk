using System;
using System.IO;

namespace Ara3D.Utils
{
    public static class FunctionUtils
    {
        public static Action ToAction<R>(this Func<R> f)
            => () => f();

        public static Action<A0> ToAction<A0, R>(this Func<A0, R> f)
            => x => f(x);

        public static Action<A0, A1> ToAction<A0, A1, R>(Func<A0, A1, R> f)
            => (x, y) => f(x, y);

        public static Func<bool> ToFunction(this Action action)
            => () =>
            {
                action();
                return true;
            };

        /// <summary>
        /// Executes an action capturing the console output.
        /// Improved answer over:
        /// https://stackoverflow.com/questions/11911660/redirect-console-writeline-from-windows-application-to-a-string
        /// </summary>
        public static string CaptureConsoleOut(Action action)
        {
            var originalConsoleOut = Console.Out;
            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                try
                {
                    action();
                    writer.Flush();
                    return writer.GetStringBuilder().ToString();
                }
                finally
                {
                    Console.SetOut(originalConsoleOut);
                }
            }
        }


        /// <summary>
        /// Returns the result of the getValue function.
        /// If getValue throws an exception, the onExceptionAction is invoked and returned 
        /// </summary>
        public static T TryGetValue<T>(Func<T> getValue, Func<Exception, T> onException)
        {
            try
            {
                return getValue();
            }
            catch (Exception e)
            {
                return onException(e);
            }
        }

        /// <summary>
        /// Returns the result of the getValue function.
        /// If getValue throws an exception the defaultValue is returned.
        /// </summary>
        public static T TryGetValue<T>(Func<T> getValue, T defaultValue = default(T))
            => TryGetValue(getValue, _ => defaultValue);
    }
}