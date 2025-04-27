using System;
using System.Windows.Threading;

namespace Ara3D.Utils.Wpf
{
    public static class DispatcherUtils
    {
        public static Func<T?> Dispatch<T>(this Dispatcher dispatcher, Func<T?> func)
            => () =>
            {
                T? result = default;
                dispatcher.Invoke(() => result = func());
                return result;
            };

        public static Action Dispatch(this Dispatcher dispatcher, Action action)
            => () => dispatcher.Invoke(action);
    }
}
