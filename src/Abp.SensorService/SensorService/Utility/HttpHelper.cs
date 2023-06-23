using Abp.UI;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Abp.SensorService.Utility
{
    public static class HttpHelper
    {
        public static async Task<string> SendHttpRequestAsync(HttpClient client, HttpMethod httpMethod, Uri uri, string jsonContent = "")
        {
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, uri);
            if (httpMethod == HttpMethod.Post)
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return responseValue;
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }
    }
}
