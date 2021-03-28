using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GeneratePdfHttpFunction.Configuration;
using GeneratePdfHttpFunction.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace GeneratePdfHttpFunction.Services
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public StorageService(StorageSettings settings)
        {
            _blobServiceClient = new BlobServiceClient(settings.ConnectionString);
        }

        public async Task<Response<BlobContentInfo>> UploadFromStream(string blobContainer, string fileName, Stream content, BlobHttpHeaders header = null, bool createContainerIfNotExists = true)
        {
            var container = GetContainer(blobContainer, createContainerIfNotExists);
            var blob = container.GetBlobClient(fileName);

            return await blob.UploadAsync(content, header);
        }

        private BlobContainerClient GetContainer(string blobContainerName, bool createContainerIfNotExists = false)
        {
            var container = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (createContainerIfNotExists)
                container.CreateIfNotExists();

            return container;
        }
    }
}
