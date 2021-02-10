using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Abp.AzureCognitiveServices.CognitiveServices
{
    public interface ICognitiveServicesManager
    {
        Task<ImageAnalysis> AnalyzeImage(string imageUrl);
        Task<ImageAnalysis> AnalyzeImage(Stream stream);
        Task<byte[]> GetImageThumbnail(int width, int height, Stream stream);
        Task<byte[]> GetImageThumbnail(int width, int height, string imageUrl);
    }
}
