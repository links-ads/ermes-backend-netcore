using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Net.MimeTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ermes.Helpers
{
    public static class FileHelper
    {
        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public static string GetFileExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            extension = !string.IsNullOrWhiteSpace(extension) ? extension.ToLowerInvariant() : string.Empty;

            return extension;
        }

        public static FileDto CreateFile(string fileName, string mimeTypeName, string tempFileDownloadFolder, string content)
        {
            var file = new FileDto(fileName, mimeTypeName);
            var filePath = Path.Combine(tempFileDownloadFolder, file.FileToken);
            File.WriteAllText(filePath, content);
            return file;
        }
    }
}
