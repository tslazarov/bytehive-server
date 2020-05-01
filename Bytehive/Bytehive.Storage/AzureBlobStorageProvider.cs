﻿using Azure;
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


        public async Task<BlobContentInfo> UploadBlob(string containerName, string fileName, Stream fileStream)
        {
            BlobContainerClient containerClient = this.GetContainer(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            BlobContentInfo response = await blobClient.UploadAsync(fileStream, true);

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

        private BlobServiceClient client;
    }
}
