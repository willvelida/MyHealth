// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using MyHealth.DBSink.Activity.Repositories;
using MyHealth.Helpers;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using CsvHelper;
using System.Globalization;
using mdl = MyHealth.Schemas;
using MyHealth.DBSink.Activity.Models;
using System.Collections.Generic;

namespace MyHealth.DBSink.Activity.Functions
{
    public class ProcessMonthlyActivityFile
    {
        private readonly ILogger<ProcessMonthlyActivityFile> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageHelper _azureStorageHelper;
        private readonly IActivityRepository _activityRepository;
        private BlobContainerClient _blobContainerClient;

        public ProcessMonthlyActivityFile(
            ILogger<ProcessMonthlyActivityFile> logger,
            IConfiguration configuration,
            IAzureStorageHelper azureStorageHelper,
            IActivityRepository activityRepository,
            BlobContainerClient blobContainerClient)
        {
            _logger = logger;
            _configuration = configuration;
            _azureStorageHelper = azureStorageHelper;
            _activityRepository = activityRepository;
            _blobContainerClient = blobContainerClient;
        }

        [FunctionName(nameof(ProcessMonthlyActivityFile))]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            try
            {                
                _logger.LogInformation(eventGridEvent.Data.ToString());
                // Get the incoming data from event grid
                var eventData = JObject.Parse(eventGridEvent.Data.ToString());
                var fileUrlToken = eventData["url"];
                List<ActivityObject> activities = new List<ActivityObject>();

                if (fileUrlToken == null)
                {
                    throw new ApplicationException("Activity file URL is missing from the incoming event");
                }

                string fileUrl = fileUrlToken.ToString();
                var receivedBlobName = "activity/" + Path.GetFileName(fileUrl);
               
                // Download the blob from the url
                using (var inputStream = await _azureStorageHelper.DownloadBlobAsync(_blobContainerClient, receivedBlobName))
                {
                    inputStream.Seek(0, SeekOrigin.Begin);

                    using (var activityStream = new StreamReader(inputStream))
                    using (var csv = new CsvReader(activityStream, CultureInfo.InvariantCulture))
                    {
                        if (csv.Read())
                        {
                            csv.ReadHeader();

                            while (csv.Read())
                            {
                                var activityObject = new ActivityObject
                                {
                                    ActivityId = Guid.NewGuid().ToString(),
                                    Activity = new mdl.Activity
                                    {
                                        ActivityDate = csv.GetField("Date"),
                                        CaloriesBurned = int.Parse(csv.GetField("Calories Burned"), NumberStyles.AllowThousands),
                                        Steps = int.Parse(csv.GetField("Steps"), NumberStyles.AllowThousands),
                                        Distance = double.Parse(csv.GetField("Distance")),
                                        Floors = int.Parse(csv.GetField("Floors"), NumberStyles.AllowThousands),
                                        MinutesSedentary = int.Parse(csv.GetField("Minutes Sedentary"), NumberStyles.AllowThousands),
                                        MinutesLightlyActive = int.Parse(csv.GetField("Minutes Lightly Active"), NumberStyles.AllowThousands),
                                        MinutesFairlyActive = int.Parse(csv.GetField("Minutes Fairly Active"), NumberStyles.AllowThousands),
                                        MinutesVeryActive = int.Parse(csv.GetField("Minutes Very Active"), NumberStyles.AllowThousands),
                                        ActivityCalories = int.Parse(csv.GetField("Activity Calories"), NumberStyles.AllowThousands)
                                    },
                                    DocumentType = "Activity",
                                    FileName = receivedBlobName
                                };

                                activities.Add(activityObject);
                            }
                        }
                    }

                    foreach (var record in activities)
                    {
                        await _activityRepository.AddActivity(record);
                    }
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message} | {ex.InnerException} | {ex.StackTrace}");
                throw;
            }            
        }
    }
}
