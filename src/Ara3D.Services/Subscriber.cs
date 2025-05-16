using System;

namespace Ara3D.Services
{
    public class Subscriber<T> : ISubscriber<T>
    {
        public Subscriber(Action<T> action)
        {
            Action = action;
        }

        public Action<T> Action { get; }

        public void OnEvent(T evt)
            => Action?.Invoke(evt);
    }
}