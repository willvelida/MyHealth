using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using MyHealth.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace MyHealth.FileWatcher.Activity.Services
{
    public class FileWatcherService : IFileWatcherService
    {
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageHelper _azureStorageHelper;
        private readonly Timer _pollTimer;
        private readonly BlobContainerClient _blobContainerClient;

        private readonly int _secondsBetweenPolls;

        public FileWatcherService(
            IConfiguration configuration,
            IAzureStorageHelper azureStorageHelper)
        {
            _configuration = configuration;
            _azureStorageHelper = azureStorageHelper;
            _pollTimer = new Timer();
            _blobContainerClient = _azureStorageHelper.ConnectToBlobClient(_configuration["BlobConnectionString"], _configuration["ContainerName"]);
            _secondsBetweenPolls = int.Parse(_configuration["SecondsBetweenPolls"]);
        }

        public void StartListening()
        {
            try
            {
                StartFileWatcher();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StartListening Exception: {ex.Message}");
            }
        }

        public void StopListening()
        {
            _pollTimer.Dispose();
        }

        private void StartFileWatcher()
        {
            _pollTimer.AutoReset = false;
            _pollTimer.Elapsed += new ElapsedEventHandler(PollDirectoryForFileAsync);
            _pollTimer.Interval = 1;
            _pollTimer.Start();
        }

        private async void PollDirectoryForFileAsync(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Get file from local directory
                var activityFilePaths = Directory.EnumerateFiles(_configuration["LocalActivityDirectoryPath"]);

                if (!activityFilePaths.Any())
                {
                    _pollTimer.Interval = _secondsBetweenPolls * 1000;
                    _pollTimer.Start();
                    return;
                }

                foreach (var activityFilePath in activityFilePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(activityFilePath);
                    var fileExtension = Path.GetExtension(activityFilePath);
                    var fullFileName = fileName + fileExtension;

                    await _azureStorageHelper.UploadBlobAsync(_blobContainerClient, "activity/" + fullFileName, activityFilePath);
                    File.Delete(activityFilePath);
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown during PollDirectoryForFile execution: {ex.Message}");
            }
        }
    }
}
