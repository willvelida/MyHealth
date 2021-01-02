using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHealth.DBSink.Sleep;
using MyHealth.DBSink.Sleep.Repositories;
using MyHealth.Helpers;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyHealth.DBSink.Sleep
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            var cosmosClientOptions = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Direct
            };

            builder.Services.AddSingleton((s) => new CosmosClient(config["CosmosDBConnectionString"], cosmosClientOptions));
            builder.Services.AddSingleton((S) => new BlobContainerClient(config["BlobConnectionString"], config["BlobContainer"]));
            builder.Services.AddTransient<ISleepRepository, SleepRepository>();
            builder.Services.AddTransient<IAzureStorageHelper, AzureStorageHelper>();
        }
    }
}
