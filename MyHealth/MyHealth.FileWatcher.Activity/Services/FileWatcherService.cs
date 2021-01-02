using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using MyHealth.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace MyHealth.FileWatcher.Services
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
                var onPremFilePaths = Directory.EnumerateFiles(_configuration["LocalActivityDirectoryPath"]);

                if (!onPremFilePaths.Any())
                {
                    _pollTimer.Interval = _secondsBetweenPolls * 1000;
                    _pollTimer.Start();
                    return;
                }

                foreach (var pickedUpFile in onPremFilePaths)
                {
                    var fileName = Path.GetFileNameWithoutExtension(pickedUpFile);
                    var fileExtension = Path.GetExtension(pickedUpFile);
                    string fullFileName;
                    // Do we apply REGEX on the file name to specify which blob to send it to?
                    if (fileName.StartsWith("activity"))
                    {
                        fullFileName = "activity/" + fileName + fileExtension;
                    }
                    else if (fileName.StartsWith("sleep"))
                    {
                        fullFileName = "sleep/" + fileName + fileExtension;
                    }
                    else
                    {
                        throw new Exception("File has invalid format");
                    }

                    await _azureStorageHelper.UploadBlobAsync(_blobContainerClient, fullFileName, pickedUpFile);
                    File.Delete(pickedUpFile);
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown during PollDirectoryForFile execution: {ex.Message}");
            }
        }
    }
}
