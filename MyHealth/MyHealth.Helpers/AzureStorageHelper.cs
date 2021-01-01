using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace MyHealth.Helpers
{
    public class AzureStorageHelper : IAzureStorageHelper
    {
        public BlobContainerClient ConnectToBlobClient(string connectionString, string containerName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            return containerClient;
        }

        public async Task UploadBlobAsync(BlobContainerClient containerClient, string blobName, string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileName);
        }

        public async Task<Stream> DownloadBlobAsync(BlobContainerClient containerClient, string blobName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var stream = new MemoryStream();

            await blobClient.DownloadToAsync(stream);

            return stream;
        }
    }
}
