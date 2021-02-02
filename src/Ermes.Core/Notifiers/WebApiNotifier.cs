using Abp.Chatbot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class WebApiNotifier : IWebApiNotifier
    {
        private readonly ChatbotManager _chatbotManager;
        public WebApiNotifier(ChatbotManager chatbotManager)
        {
            _chatbotManager = chatbotManager;
        }
        public async Task<List<string>> SendMessage(FullNotificationData input)
        {
            return await _chatbotManager.SendMessageAsync(input.Body, input.BodyParams, input.Receivers, input.EntityId, input.EntityType.ToString().ToLower(), input.Title, input.TitleParams);
        }
    }
}
