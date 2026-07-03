using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using System.Threading;
using System.Collections.Concurrent;

namespace Ara3D.Bowerbird.RevitSamples
{
    /// <summary>
    /// The background processor does arbitrary work in the background while Revit is Idle.
    /// It contains a queue of work items (of any user defined type) for later processing.
    /// Good practice for choosing a work item type is to not use a Revit API type (e.g., use an integer for element ID).
    /// Work processing is performed by a delegate / action provided to the constructors. 
    /// When an OnIdle or DocumentChanged event occurs, work will be extracted out of the queue
    /// and executed using the provided action.
    /// This will be repeated until the maximum MSec per batch is reached. 
    /// You can pause processing, execute all items immediately, or do other work. 
    /// </summary>
    public class BackgroundProcessor<T> : IDisposable
    {
        public ConcurrentQueue<T> Queue { get; private set; } = new ConcurrentQueue<T>();
        public int MaxMSecPerBatch { get; set; } = 10;
        public int HeartBeatMsec { get; set; } = 1000;
        public bool ExecuteNextIdleImmediately { get; set; } = true;
        public readonly Action<T> Processor;
        public readonly UIApplication UIApp;
        public Application App => UIApp.Application;
        public bool PauseProcessing { get; set; }
        public event EventHandler<Exception> ExceptionEvent;
        public readonly Stopwatch WorkStopwatch = new Stopwatch();
        public int WorkProcessedCount = 0;
        private bool _enabled = false;
        public bool DoWorkDuringIdle { get; set; } = true;
        public bool DoWorkDuringProgress { get; set; } = true;
        public bool DoWorkDuringExternal { get; set; } = true;
        public static readonly Process RevitProcess = Process.GetCurrentProcess();
        public Thread PokeRevitThread;

        public ExternalEvent ExternalEventHeartbeat;

        public event EventHandler OnHeartbeat;
        public event EventHandler OnIdle;
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        // After the heartbeat, signals that there is 
        public AutoResetEvent HeartbeatSignal = new AutoResetEvent(false);

        public BackgroundProcessor(Action<T> processor, UIApplication uiApp)
        {
            Processor = processor;
            ExternalEventHeartbeat = RevitApiContext.CreateEvent(On_ExternalEventHeartbeat, "Heartbeat");
            UIApp = uiApp;
            Enable();
        }
        private int _processingWork = 0;
        private bool TryEnterProcessWork() => Interlocked.CompareExchange(ref _processingWork, 1, 0) == 0;
        private void ExitProcessWork() => Volatile.Write(ref _processingWork, 0);

        public bool Enabled
        {
            get => _enabled;
            set
            {

                if (value)
                    Enable();
                else
                    Disable();
            }
        }

        public void StopPokeThread()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            PokeRevitThread?.Join();
            PokeRevitThread = null;
        }

        public void StartPokeThread()
        {
            StopPokeThread();
            CancellationTokenSource = new CancellationTokenSource();

            PokeRevitThread = new Thread(() =>
            {
                var token = CancellationTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    HeartbeatSignal.Reset();
                    Thread.Sleep(HeartBeatMsec);
                    PokeRevit();
                    WaitHandle.WaitAny([HeartbeatSignal, token.WaitHandle], 10000);
                }
            });

            PokeRevitThread.IsBackground = true;
            PokeRevitThread.Priority = ThreadPriority.BelowNormal;
            PokeRevitThread.Start();
        }

        public void Disable()
        {
            try
            {
                if (!_enabled)
                    return;
                UIApp.Idling -= UiApp_Idling;
                _enabled = false;
                StopPokeThread();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public void Enable()
        {
            try
            {
                if (_enabled)
                    return;
                UIApp.Idling += UiApp_Idling;
                _enabled = true;
                StartPokeThread();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public void Dispose()
        {
            ExceptionEvent = null;
            Disable();
        }

        public void On_ExternalEventHeartbeat(UIApplication _)
        {
            try
            {
                OnHeartbeat?.Invoke(this, EventArgs.Empty);
                ProcessWork();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            finally
            {
                HeartbeatSignal.Set();
            }
        }

        private void UiApp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            try
            {
                if (!DoWorkDuringIdle || PauseProcessing || !HasWork)
                    return;
                OnIdle?.Invoke(this, EventArgs.Empty);
                ProcessWork();
                if (ExecuteNextIdleImmediately && HasWork)
                    e.SetRaiseWithoutDelay();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Note that this can be called outside of the Revit API context. 
        /// </summary>
        public void EnqueueWork(IEnumerable<T> items)
        {
            foreach (var item in items)
                EnqueueWork(item);
        }

        public void EnqueueWork(T item) 
            => Queue.Enqueue(item);

        public bool HasWork
            => !Queue.IsEmpty;

        public void ClearWork()
            => Queue = new ConcurrentQueue<T>();

        public void ResetStats()
        {
            WorkStopwatch.Reset();
            WorkProcessedCount = 0;
        }

        public void ProcessWork(bool doAllNow = false)
        {
            var startedTime = WorkStopwatch.ElapsedMilliseconds;
            WorkStopwatch.Start();
            try
            {
                if (!TryEnterProcessWork())
                    return;
                while (HasWork)
                {
                    if (!Queue.TryDequeue(out var item))
                        continue;
                    Processor(item);
                    WorkProcessedCount++;   
                    
                    var elapsedTime = WorkStopwatch.ElapsedMilliseconds - startedTime;
                    if (elapsedTime > MaxMSecPerBatch && !doAllNow)
                        break;
                } 
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
            finally
            {
                WorkStopwatch.Stop();
                ExitProcessWork();
            }
        }

        public void HandleError(Exception ex)
        {
            try
            {
                ExceptionEvent?.Invoke(this, ex);
            }
            catch (Exception)
            {
                Disable();
            }
        }

        // Technique described by 
        // https://forums.autodesk.com/t5/revit-api-forum/how-to-trigger-onidle-event-or-execution-of-an-externalevent/td-p/6645286 
        public static void PokeRevit()
        {
            if (RevitProcess?.HasExited == true || RevitProcess == null)
                return;
            RevitApiContext.PostMessage(RevitProcess.MainWindowHandle, 0, 0, 0);
        }
    }
}