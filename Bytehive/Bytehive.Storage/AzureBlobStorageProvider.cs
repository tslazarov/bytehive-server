using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bytehive.Storage.AppConfig;
using System.IO;
using System.Threading.Tasks;

namespace Bytehive.Storage
{
    public class AzureBlobStorageProvider : IAzureBlobStorageProvider
    {
        private IAppConfiguration appConfiguration;

        public AzureBlobStorageProvider(IAppConfiguration appConfiguration)
        {
            this.appConfiguration = appConfiguration;
        }

        public BlobContainerClient GetContainer(string containerName)
        {
            BlobContainerClient containerClient = this.Client.GetBlobContainerClient(containerName);

            return containerClient;
        }

        public async Task<BlobContainerClient> CreateContainer(string containerName)
        {
            BlobContainerClient containerClient = await this.Client.CreateBlobContainerAsync(containerName);

            return containerClient;
        }

        public async Task<bool> DeleteContainer(string containerName)
        {
            var result = await this.Client.DeleteBlobContainerAsync(containerName);

            return result.Status == 202;
        }

        public bool ContainerExists(string containerName)
        {
            BlobContainerClient containerClient = this.Client.GetBlobContainerClient(containerName);
            var result = containerClient.Exists();

            return result != null && result.Value;
        }


        public async Task<BlobContentInfo> UploadBlob(string containerName, string fileName, string fileType, Stream fileStream)
        {
            BlobContainerClient containerClient = this.GetContainer(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            BlobContentInfo response = await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = GetContentType(fileType) });

            return response;
        }

        public async Task<BlobDownloadInfo> DownloadBlob(string containerName, string fileName)
        {
            BlobContainerClient containerClient = this.GetContainer(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            BlobDownloadInfo response = await blobClient.DownloadAsync();

            return response;
        }

        public Pageable<BlobItem> GetBlobs(string containerName)
        {
            BlobContainerClient containerClient = this.GetContainer(containerName);
            return containerClient.GetBlobs();
        }

        protected BlobServiceClient Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new BlobServiceClient(this.appConfiguration.AzureBlobConnectionString);
                }

                return this.client;
            }
        }

        public BlobProperties GetBlobProperties(string containerName, string fileName)
        {
            BlobContainerClient containerClient = this.GetContainer(containerName);

            var blob = containerClient.GetBlobClient(fileName);
            if (blob.Exists())
            {
                var properties = blob.GetProperties();
                return properties.Value;
            }

            return null;
        }

        private string GetContentType(string fileType)
        {
            switch (fileType)
            {
                case ".txt": return "text/plain";
                case ".json": return "application/json";
                case ".xml": return "application/xml";
                case ".csv": return "application/csv";
                default: return "application/octet-stream";
            }
        }

        private BlobServiceClient client;
    }
}
