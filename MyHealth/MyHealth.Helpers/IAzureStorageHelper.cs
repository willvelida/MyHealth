using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHealth.Helpers
{
    /// <summary>
    /// Interface defining contracts with Azure Storage
    /// </summary>
    public interface IAzureStorageHelper
    {
        /// <summary>
        /// Connects to a Blob Client in Azure Storage
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="containerName"></param>
        /// <returns></returns>
        BlobContainerClient ConnectToBlobClient(string connectionString, string containerName);

        /// <summary>
        /// Uploads a blob to Azure Storage
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="blobName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task UploadBlobAsync(BlobContainerClient containerClient, string blobName, string fileName);
    }
}
