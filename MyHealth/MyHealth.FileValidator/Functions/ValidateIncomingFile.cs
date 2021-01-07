// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using MyHealth.Helpers;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;

namespace MyHealth.FileValidator.Functions
{
    public class ValidateIncomingFile
    {
        private readonly ILogger<ValidateIncomingFile> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageHelper _azureStorageHelper;
        private BlobContainerClient _blobContainerClient;

        public ValidateIncomingFile(
            ILogger<ValidateIncomingFile> logger,
            IConfiguration configuration,
            IAzureStorageHelper azureStorageHelper,
            BlobContainerClient blobContainerClient)
        {
            _logger = logger;
            _configuration = configuration;
            _azureStorageHelper = azureStorageHelper;
            _blobContainerClient = blobContainerClient;
        }

        [FunctionName(nameof(ValidateIncomingFile))]
        public void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            string receivedBlobName;

            try
            {
                // Read the incoming event
                var eventData = JObject.Parse(eventGridEvent.Data.ToString());
                var fileUrlToken = eventData["url"];

                if (fileUrlToken == null)
                {
                    throw new ApplicationException("Activity file URL is missing from the incoming event");
                }

                string fileUrl = fileUrlToken.ToString();
                // Check if the fileUrl exists, if so discard. If not add to cache

                // If it's a activity file, send to DBSink.Activity
                if (Path.GetFileName(fileUrl).Contains("activity"))
                {
                    // Create a parser class that seperates the file to class mapping
                }

                // If it's a sleep file, send to DBSink Sleep
                if (Path.GetFileName(fileUrl).Contains("sleep"))
                {

                }

                // Once the event has been sent, delete the file?
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(ValidateIncomingFile)}: {ex.Message}");
                // Send event to service bus
            }          
        }
    }
}
