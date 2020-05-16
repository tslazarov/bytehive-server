using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bytehive.Storage.AppConfig;
using System.IO;
using System.Threading.Tasks;

namespace Bytehive.Storage
{
    public interface IAzureBlobStorageProvider
    {
        BlobContainerClient GetContainer(string containerName);

        Task<BlobContainerClient> CreateContainer(string containerName);

        Task<bool> DeleteContainer(string containerName);

        bool ContainerExists(string containerName);

        Task<BlobContentInfo> UploadBlob(string containerName, string fileName, string fileType, Stream fileStream);

        Task<BlobDownloadInfo> DownloadBlob(string containerName, string fileName);

        Pageable<BlobItem> GetBlobs(string containerName);

        BlobProperties GetBlobProperties(string containerName, string fileName);
    }
}
