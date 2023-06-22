using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Threading.Tasks;

namespace Abp.Azure.Storage
{
    public class AzureStorageManager
    {

        private readonly string containerName = string.Empty;
        private readonly string connectionString = string.Empty;
        private BlobContainerClient BlobContainer;

        #region CTors

        private AzureStorageManager(){}

        public AzureStorageManager(string connectionString, string containerName)
        {
            this.containerName = containerName.ToLower();
            this.connectionString = connectionString;
        }

        #endregion

        #region Public Members

        public async Task<Byte[]> GetFile(string blobName)
        {
            MemoryStream memstream = new MemoryStream();
            var blobClient = await GetBlobClientAsync(blobName);
            await blobClient.DownloadToAsync(memstream);
            return memstream.ToArray();
        }

        public async Task<bool> UploadFileFromLocalAsync(string fileName, string uploadFileName, string contentType)
        {
            byte[] file = File.ReadAllBytes(uploadFileName);
            return await UploadFileAsync(fileName,file,contentType);
        }

        public async Task<bool> UploadFileAsync(string blobName, byte[] file, string contentType)
        {
            var blobClient = await GetBlobClientAsync(blobName);
            using (var stream = new MemoryStream(file, writable: false))
            {
                await blobClient.UploadAsync(stream);
            }

            return true;
        }

        public bool UploadFile(string blobName, byte[] file)
        {
            var blobClient = GetBlobClient(blobName);
            using (var stream = new MemoryStream(file, writable: false))
            {
                blobClient.Upload(stream);
            }

            return true;
        }

        public async Task<BlobProperties> GetFileInfo(string blobName)
        {
            var blobClient = await GetBlobClientAsync(blobName);
            return await blobClient.GetPropertiesAsync();
        }


        public async Task<List<BlobItem>> GetFilesInFolder(string prefix, string containerName)
        {
            var container = await GetBlobContainerClientAsync(connectionString, containerName);
            var list = container.GetBlobs(traits: BlobTraits.None, states: BlobStates.None, prefix: prefix);
            var res = new List<BlobItem>();
            foreach (var item in list)
            {
                res.Add(item);
            }

            return res;
        }

        public async Task<bool> FileExist(string blobName)
        {
            var blobClient = await GetBlobClientAsync(blobName);
            return await blobClient.ExistsAsync();
        }

        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            var blobClient = await GetBlobClientAsync(blobName);
            var res = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return res;
        }

        #endregion

        #region Private Members

        private async Task<BlobClient> GetBlobClientAsync(string blobName)
        {
            var container = await GetBlobContainerClientAsync(connectionString, containerName);

            // Get a reference to a blob named "blobName" in a container named "containerName"
            return container.GetBlobClient(blobName);
        }

        private BlobClient GetBlobClient(string blobName)
        {
            var container = GetBlobContainerClient(connectionString, containerName);

            // Get a reference to a blob named "blobName" in a container named "containerName"
            return container.GetBlobClient(blobName);
        }

        private async Task<BlobContainerClient> GetBlobContainerClientAsync(string connectionString, string containerName)
        {
            // Get a reference to a container named "containerName" and then create it
            if (BlobContainer != null)
                return BlobContainer;
            BlobContainer = new BlobContainerClient(connectionString, containerName);
            await BlobContainer.CreateIfNotExistsAsync();

            return BlobContainer;
        }

        private BlobContainerClient GetBlobContainerClient(string connectionString, string containerName)
        {
            // Get a reference to a container named "containerName" and then create it
            if (BlobContainer != null)
                return BlobContainer;
            BlobContainer = new BlobContainerClient(connectionString, containerName);
            BlobContainer.CreateIfNotExists();

            return BlobContainer;
        }

        #endregion

    }
}
