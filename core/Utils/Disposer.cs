using System;

namespace Ara3D.Utils
{
    /// <summary>
    /// Used to create a class that can be used in a "using" block and that will execute
    /// an action once it goes out of scope. An example use case is profiling the time it takes
    /// to execute a block of code. 
    /// </summary>
    public sealed class Disposer : IDisposable
    {
        readonly Action OnDispose;

        public Disposer(Action onDispose)
            => OnDispose = onDispose;

        public void Dispose()
            => OnDispose();

        public static Disposer Create(Action action)
            => new Disposer(action);

        public static Disposer Create(Action beforeAction, Action afterAction)
        {
            beforeAction();
            return new Disposer(afterAction);
        }
    }
}
