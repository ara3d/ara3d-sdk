using System.Threading;
using System.Threading.Tasks;

namespace Ara3D.Logging
{
    public class Pauseable : IPausable
    {
        public bool IsPaused { get; private set; }
        private CancellationTokenSource _pauseTokenSource { get; set; }
        private CancellationToken _pauseToken => _pauseTokenSource.Token;

        public void Pause()
        {
            if (IsPaused)
                return;
            _pauseTokenSource = new CancellationTokenSource();
            IsPaused = true;
        }

        public void Resume()
        {
            _pauseTokenSource.Cancel();
        }

        public async Task CheckPause()
        {
            while (IsPaused)
            {
                try
                {
                    //A long delay is key here to prevent the task system from holding the thread.
                    //The cancellation token allows the work to resume with a notification 
                    //from the CancellationTokenSource.
                    await Task.Delay(10000, _pauseToken);
                }
                catch (TaskCanceledException)
                {
                    // Catch the cancellation and it turns into continuation
                    IsPaused = false;
                }
            }
        }
    }
}