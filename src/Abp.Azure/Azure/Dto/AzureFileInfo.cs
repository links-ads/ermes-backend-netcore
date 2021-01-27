using System;

namespace Abp.Azure.Dto
{
    public class AzureFileInfoResult
    {
        public bool Success { get; set; }

        public long FileSize { get; set; }

        public string CheckSum { get; set; }

        public string ContentType { get; set; }

        public DateTimeOffset? LastModified { get; set; }
    }
}