using System;
using System.Threading;

namespace Ara3D.Utils
{
    /// <summary>
    /// The operates similarly to "Control.Invoke" in WinForms,
    /// and "Dispatcher" in WPF, but instead uses a more generic method
    /// based on SynchronizationContexts that is not tied to a specific UI framework. 
    /// The primary purpose is to execute arbitrary code on the main UI thread. 
    /// </summary>
    public class Synchronizer 
    {
        public SynchronizationContext Context { get; }

        /// <summary>
        /// We would explicitly allow null contexts in a console application.
        /// In a WPF or Winforms application, we have to wait until the main UI thread is started
        /// and the synchronization context is created.
        ///
        /// This call is expected to happen on the main UI thread.
        /// See: https://stackoverflow.com/questions/36844382/how-do-you-get-the-current-synchronizationcontext-for-a-specific-wpf-window
        /// See: https://stackoverflow.com/questions/45054201/synchronizationcontext-current-is-null-on-main-thread
        /// See: https://stackoverflow.com/questions/21094435/alternative-to-dispatcher-class-for-net-2-0
        /// See: https://stackoverflow.com/questions/7075491/why-is-synchronizationcontext-current-null
        /// </summary>
        public Synchronizer(SynchronizationContext context, bool acceptNull = false)
        {
            Context = context;
            
            if (!acceptNull && context == null)
            {
                throw new ArgumentNullException(
                    "Synchronization context can't be null: either the application is not WinForms or WPF, or the service has been created too early.");
            }
        }

        public void Invoke(Action action)
        {
            if (Context != null)
                Context.Post(_ => action(), null);
            else
                action();
        }

        public void Invoke(Action<object> action, object state)
        {
            if (Context != null)
                Context.Post(state1 => action(state1), state);
            else
                action(state);
        }

        public static Synchronizer Create(bool acceptNull = false)
            => new Synchronizer(SynchronizationContext.Current, acceptNull);

        public void Dispose()
        {
            Disposing?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Disposing;
    }
}
