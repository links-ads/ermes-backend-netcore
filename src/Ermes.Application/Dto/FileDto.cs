using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto
{
    public class FileDto
    {
        public FileDto() { }
        public FileDto(string fileName, string fileType)
        {
            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
        }

        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileToken { get; set; }
    }
}
