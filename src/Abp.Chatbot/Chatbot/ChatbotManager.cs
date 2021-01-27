using Abp.Chatbot.Configuration;
using Abp.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Chatbot
{
    public class ChatbotManager : IChatbotManager
    {
        private static HttpClient ChatbotClient;
        private const string ApiKeyHeaderName = "apikey";
        private readonly IChatbotConnectionProvider _connectionProvider;
        public ChatbotManager(IChatbotConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            GetChatbotClient();
        }

        private void GetChatbotClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(ApiKeyHeaderName, _connectionProvider.GetApiKey());
            client.BaseAddress = new Uri(_connectionProvider.GetBasePath());

            ChatbotClient = client;
            return;
        }

        public async Task<List<string>> SendMessageAsync(string message, string[] messageParams, List<string> userIds, int entityId, string entityType, string title = null, string[] titleParams=null)
        {
            if (ChatbotClient == null)
                GetChatbotClient();
            
            Uri uri = new Uri(ChatbotClient.BaseAddress + "bot/send-message");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                title,
                titleParams,
                message,
                messageParams,
                userIds,
                entityId,
                entityType
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await ChatbotClient.SendAsync(request);
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

                return JsonConvert.DeserializeObject<List<string>>(responseValue);
            }
            else
                throw new UserFriendlyException("ChatbotNotAvailable");
        }
    }
}
