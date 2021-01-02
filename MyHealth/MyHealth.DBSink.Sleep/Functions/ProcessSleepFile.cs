// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MyHealth.Helpers;
using MyHealth.DBSink.Sleep.Repositories;
using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using MyHealth.DBSink.Sleep.Models;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using System.Globalization;
using mdl = MyHealth.Schemas;

namespace MyHealth.DBSink.Sleep.Functions
{
    public class ProcessSleepFile
    {
        private readonly ILogger<ProcessSleepFile> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAzureStorageHelper _azureStorageHelper;
        private readonly ISleepRepository _sleepRepository;
        private BlobContainerClient _blobContainerClient;

        public ProcessSleepFile(
            ILogger<ProcessSleepFile> logger,
            IConfiguration configuration,
            IAzureStorageHelper azureStorageHelper,
            ISleepRepository sleepRepository,
            BlobContainerClient blobContainerClient)
        {
            _logger = logger;
            _configuration = configuration;
            _azureStorageHelper = azureStorageHelper;
            _sleepRepository = sleepRepository;
            _blobContainerClient = blobContainerClient;
        }

        [FunctionName(nameof(ProcessSleepFile))]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            try
            {
                _logger.LogInformation(eventGridEvent.Data.ToString());

                var eventData = JObject.Parse(eventGridEvent.Data.ToString());
                var fileUrlToken = eventData["url"];
                List<SleepObject> sleeps = new List<SleepObject>();

                if (fileUrlToken == null)
                {
                    throw new ApplicationException("Sleep file URL is missing from the incoming event");
                }

                string fileUrl = fileUrlToken.ToString();
                var receivedBlobName = "sleep/" + Path.GetFileName(fileUrl);

                // Download the blob from the Url
                using (var inputStream = await _azureStorageHelper.DownloadBlobAsync(_blobContainerClient, receivedBlobName))
                {
                    inputStream.Seek(0, SeekOrigin.Begin);

                    using (var sleepStream = new StreamReader(inputStream))
                    using (var csv = new CsvReader(sleepStream, CultureInfo.InvariantCulture))
                    {
                        if (csv.Read())
                        {
                            csv.ReadHeader();

                            while (csv.Read())
                            {
                                var sleepObject = new SleepObject
                                {
                                    ObjectId = Guid.NewGuid().ToString(),
                                    Sleep = new mdl.Sleep
                                    {
                                        StartTime = csv.GetField("Start Time"),
                                        EndTime = csv.GetField("End Time"),
                                        MinutesAsleep = int.Parse(csv.GetField("Minutes Asleep")),
                                        MinutesAwake = int.Parse(csv.GetField("Minutes Awake")),
                                        NumberOfAwakenings = int.Parse(csv.GetField("Number of Awakenings")),
                                        TimeInBed = int.Parse(csv.GetField("Time in Bed")),
                                        MinutesREMSleep = int.Parse(csv.GetField("Minutes REM Sleep")),
                                        MinutesLightSleep = int.Parse(csv.GetField("Minutes Light Sleep")),
                                        MinutesDeepSleep = int.Parse(csv.GetField("Minutes Deep Sleep"))
                                    },
                                    DocumentType = "Sleep",
                                    FileName = receivedBlobName
                                };

                                sleeps.Add(sleepObject);
                            }
                        }
                    }

                    foreach (var record in sleeps)
                    {
                        await _sleepRepository.AddSleep(record);
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
