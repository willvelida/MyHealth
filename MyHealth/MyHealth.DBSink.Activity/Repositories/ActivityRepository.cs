using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.DBSink.Activity.Models;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Helpers.Models;

namespace MyHealth.DBSink.Activity.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;

        private readonly Container _container;

        public ActivityRepository(
            IConfiguration configuration,
            CosmosClient cosmosClient)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task AddActivity(ActivityObject activity)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            await _container.CreateItemAsync(activity,
                new PartitionKey(activity.ActivityId),
                itemRequestOptions);
        }
    }
}
