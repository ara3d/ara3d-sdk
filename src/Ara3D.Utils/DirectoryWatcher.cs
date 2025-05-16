using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Ara3D.Utils
{
    /// <summary>
    /// A wrapper around the FileSystemWatcher class which is intended to
    /// be used for watching a directory. 
    /// Be careful!
    /// https://failingfast.io/a-robust-solution-for-filesystemwatcher-firing-events-multiple-times/
    /// This runs on a separate thread and throttles before notifying. 
    /// </summary>
    public class DirectoryWatcher : IDisposable
    {
        public FileSystemWatcher Watcher;
        public DirectoryPath Directory => Watcher.Path;

        public IEnumerable<FilePath> GetFiles()
            => Directory.GetFiles(Watcher.Filter, Watcher.IncludeSubdirectories);

        private Action OnChange { get; set; }

        public DirectoryWatcher(string dir, string filter, Action onChange)
            : this(dir, filter, false, onChange) { }

        public Thread Thread { get; }
        public TimeSpan NotificationDelay = TimeSpan.FromSeconds(0.5);
        public bool NotificationRequested { get; private set; }
        public DateTimeOffset NotificationRequestedTime { get; private set; }
        public SynchronizationContext SyncContext { get; } = SynchronizationContext.Current;

        public DirectoryWatcher(string dir, string filter, bool subDirectories, Action onChange)
        {
            Watcher = new FileSystemWatcher(dir)
            {
                NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Size,
                Filter = filter ?? "*.*",
                IncludeSubdirectories = subDirectories
            };
            Watcher.Changed += Watcher_Changed;
            Watcher.Created += Watcher_Created;
            Watcher.Deleted += Watcher_Deleted;
            Watcher.Renamed += Watcher_Renamed;
            Watcher.Error += Watcher_Error;
            Watcher.EnableRaisingEvents = true;
            OnChange = onChange;
            Thread = new Thread(StartThread);
            Thread.Start();
        }

        public void StartThread()
        {
            while (OnChange != null)
            {
                Thread.Sleep(NotificationDelay);
                if (NotificationRequested)
                {
                    if ((DateTimeOffset.Now - NotificationRequestedTime) > NotificationDelay)
                    {
                        NotificationRequested = false;
                        SyncContext.Send(NotifyOfChange, null);
                    }
                }
            }
        }

        public void RequestNotification()
        {
            NotificationRequested = true;
            NotificationRequestedTime = DateTimeOffset.Now;
        }

        public void NotifyOfChange(object args)
        {
            OnChange();
        }

        public void DisconnectEvents()
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Changed -= Watcher_Changed;
            Watcher.Created -= Watcher_Created;
            Watcher.Deleted -= Watcher_Deleted;
            Watcher.Renamed -= Watcher_Renamed;
            Watcher.Error -= Watcher_Error;
            OnChange = null;
        }

        public void Dispose()
        {
            DisconnectEvents();
            Thread.Abort();
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine($"Error occurred {e}");
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Debug.WriteLine($"File renamed {e.Name}");
            RequestNotification();
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"Deleted file {e.Name}");
            RequestNotification();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"Created file {e.Name}");
            RequestNotification();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Debug.WriteLine($"File changed {e.Name}");
            RequestNotification();
        }
    }
}
