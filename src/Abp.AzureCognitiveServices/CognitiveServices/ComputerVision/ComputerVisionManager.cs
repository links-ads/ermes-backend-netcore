using Abp.AzureCognitiveServices.CognitiveServices.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.AzureCognitiveServices.CognitiveServices.ComputerVision
{
    public class ComputerVisionManager : IComputerVisionManager
    {
        private static ComputerVisionClient client;
        private readonly IComputerVisionConnectionProvider _connectionProvider;
        private readonly ILogger Logger;

        public ComputerVisionManager(IComputerVisionConnectionProvider connectionProvider, ILogger<ComputerVisionManager> logger)
        {
            _connectionProvider = connectionProvider;
            client = GetComputerVisionClient();
            Logger = logger;
        }
        private ComputerVisionClient GetComputerVisionClient()
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(_connectionProvider.GetSubscriptionKey()))
              { Endpoint = _connectionProvider.GetEndpoint() };
            return client;
        }
        public async Task<ImageAnalysis> AnalyzeImage(string imageUrl)
        {
            try
            {
                Logger.LogInformation("Ermes Cognitive Service: Analyze Image");
                if (client == null)
                    client = GetComputerVisionClient();

                // Creating a list that defines the features to be extracted from the image. 
                List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
                };

                // Analyze the URL image 
                var response = await client.AnalyzeImageAsync(imageUrl, features);
                Logger.LogInformation("Ermes Cognitive Service: successfully analyzed image");
                return response;
            }
            catch (Exception e)
            {
                Logger.LogError("Ermes Cognitive Service: exception while analyzing the image: " + e.Message);
                return null;
            }
        }

        public async Task<ImageAnalysis> AnalyzeImage(Stream stream)
        {
            try
            {
                Logger.LogInformation("Ermes Cognitive Service: Analyze Image");
                if (client == null)
                    client = GetComputerVisionClient();

                // Creating a list that defines the features to be extracted from the image. 
                List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
                {
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult
                };

                // Analyze the URL image 
                var response = await client.AnalyzeImageInStreamAsync(stream, features);
                Logger.LogInformation("Ermes Cognitive Service: successfully analyzed image");
                return response;
            }
            catch(Exception e)
            {
                Logger.LogError("Ermes Cognitive Service: exception while analyzing the image: " + e.Message);
                return null;
            }
        }

        public async Task<byte[]> GetImageThumbnail(int width, int height, Stream stream)
        {
            try
            {
                if (client == null)
                    client = GetComputerVisionClient();

                var thumbnail = await client.GenerateThumbnailInStreamAsync(width, height, stream);
                using (MemoryStream ms = new MemoryStream())
                {
                    thumbnail.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<byte[]> GetImageThumbnail(int width, int height, string imageUrl)
        {
            try
            {
                if (client == null)
                    client = GetComputerVisionClient();

                var thumbnail = await client.GenerateThumbnailAsync(width, height, imageUrl);
                using (MemoryStream ms = new MemoryStream())
                {
                    thumbnail.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
