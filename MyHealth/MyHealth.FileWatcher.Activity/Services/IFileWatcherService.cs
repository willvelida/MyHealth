namespace MyHealth.FileWatcher.Activity.Services
{
    public interface IFileWatcherService
    {
        void StartListening();
        void StopListening();
    }
}
