using Azure;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace GeneratePdfHttpFunction.Services.Interfaces
{
    public interface IStorageService
    {
        Task<Response<BlobContentInfo>> UploadFromStream(string blobContainer, string fileName, Stream content, BlobHttpHeaders header = null, bool createContainerIfNotExists = true);
    }
}
