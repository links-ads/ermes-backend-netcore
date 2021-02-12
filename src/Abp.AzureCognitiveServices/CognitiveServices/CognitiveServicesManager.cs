using Abp.AzureCognitiveServices.CognitiveServices.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Abp.AzureCognitiveServices.CognitiveServices
{
    public class CognitiveServicesManager : ICognitiveServicesManager, IComputerVisionManager
    {
        public static ComputerVisionManager _cvManager;

        public CognitiveServicesManager(
                ComputerVisionManager cvManager
            )
        {
            _cvManager = cvManager;
        }

        public async Task<ImageAnalysis> AnalyzeImage(string imageUrl)
        {
            return await _cvManager.AnalyzeImage(imageUrl);
        }

        public async Task<ImageAnalysis> AnalyzeImage(Stream stream)
        {
            return await _cvManager.AnalyzeImage(stream);
        }

        public async Task<byte[]> GetImageThumbnail(int width, int height, Stream stream)
        {
            return await _cvManager.GetImageThumbnail(width, height, stream);
        }

        public async Task<byte[]> GetImageThumbnail(int width, int height, string imageUrl)
        {
            return await _cvManager.GetImageThumbnail(width, height, imageUrl);
        }
    }
}
