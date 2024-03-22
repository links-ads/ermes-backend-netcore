using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Abp.Azure.Storage
{
    public class AzureStorageManager
    {

        private readonly string containerName = string.Empty;
        private readonly string connectionString = string.Empty;
        private BlobContainerClient BlobContainer;

        #region CTors

        private AzureStorageManager() { }

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
            var blobClient = await GetBlobClient(blobName);
            await blobClient.DownloadToAsync(memstream);
            return memstream.ToArray();
        }

        public async Task<bool> UploadFile(string fileName, string uploadFileName, string contentType)
        {
            byte[] file = File.ReadAllBytes(uploadFileName);
            return await UploadFile(fileName, file, contentType);
        }

        public async Task<bool> UploadFile(string blobName, byte[] file, string contentType)
        {
            var blobClient = await GetBlobClient(blobName);
            using (var stream = new MemoryStream(file, writable: false))
            {
                await blobClient.UploadAsync(stream);
            }

            return true;
        }

        public async Task<BlobProperties> GetFileInfo(string blobName)
        {
            var blobClient = await GetBlobClient(blobName);
            return await blobClient.GetPropertiesAsync();
        }


        public async Task<List<BlobItem>> GetFilesInFolder(string prefix, string containerName)
        {
            var container = await GetBlobContainerClient(connectionString, containerName);
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
            var blobClient = await GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }

        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            var blobClient = await GetBlobClient(blobName);
            var res = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return res;
        }

        #endregion

        #region Private Members

        private async Task<BlobClient> GetBlobClient(string blobName)
        {
            var container = await GetBlobContainerClient(connectionString, containerName);

            // Get a reference to a blob named "blobName" in a container named "containerName"
            return container.GetBlobClient(blobName);
        }

        private async Task<BlobContainerClient> GetBlobContainerClient(string connectionString, string containerName)
        {
            // Get a reference to a container named "containerName" and then create it
            if (BlobContainer != null)
                return BlobContainer;
            BlobContainer = new BlobContainerClient(connectionString, containerName);
            await BlobContainer.CreateIfNotExistsAsync();
            try
            {
                await BlobContainer.SetAccessPolicyAsync(PublicAccessType.Blob);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("Cannot change access type for blob {0}: {1}", containerName, ex.Message));
            }
            return BlobContainer;
        }

        #endregion

    }
}
