

using Azure.Storage.Blobs.Models;
using System;

namespace Abp.Azure.Dto
{
    public class AzureFullFileInfo : AzureFileInfoResult
    {
        public BlobType BlobType { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }
    }
}
