// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MyHealth.FileValidator.Functions
{
    public class ValidateIncomingFile
    {
        private readonly ILogger<ValidateIncomingFile> _logger;
        private readonly IConfiguration _configuration;

        public ValidateIncomingFile(
            ILogger<ValidateIncomingFile> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [FunctionName(nameof(ValidateIncomingFile))]
        public void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            try
            {
                // Read the incoming event

                // Check if the fileUrl exists, if so discard

                // If it's a activity file, send to DBSink.Activity

                // If it's a sleep file, send to DBSink Activity

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
