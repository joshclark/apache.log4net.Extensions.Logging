using System;
using System.IO;
using System.Threading;

namespace apache.log4net.Extensions.Logging
{
    class FileWatcher : IDisposable
    {
        private const int TimeoutMillis = 500;

        private readonly string _filePath;
        private readonly Action<string> _onFileChanged;
        private readonly FileSystemWatcher _watcher;
        private readonly Timer _timer;


        public FileWatcher(string filePath, Action<string> onFileChanged)
        {
            _filePath = filePath;
            _onFileChanged = onFileChanged;

            var fileInfo = new FileInfo(filePath);

            _watcher = new FileSystemWatcher
            {
                Path = fileInfo.DirectoryName,
                Filter = fileInfo.Name,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            _watcher.Changed += WatcherOnChanged;
            _watcher.Created += WatcherOnChanged;
            _watcher.Deleted += WatcherOnChanged;
            _watcher.Renamed += WatcherOnRenamed;

            _watcher.EnableRaisingEvents = true;

            _timer = new Timer(OnWatchedFileChanged, null, Timeout.Infinite, Timeout.Infinite);
        }
        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void WatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            _timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnWatchedFileChanged(object state)
        {
            _onFileChanged(_filePath);
        }


        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _timer.Dispose();
        }
    }
}
