using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyHealth.DBSink.Sleep.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Sleep.Repositories
{
    public class SleepRepository : ISleepRepository
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;

        private readonly Container _container;

        public SleepRepository(
            IConfiguration configuration,
            CosmosClient cosmosClient)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(_configuration["DatabaseName"], _configuration["ContainerName"]);
        }

        public async Task AddSleep(SleepObject sleep)
        {
            ItemRequestOptions itemRequestOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            await _container.CreateItemAsync(sleep,
                new PartitionKey(sleep.DocumentType),
                itemRequestOptions);
        }
    }
}
