using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Abp.Chatbot.Configuration
{
    public class ChatbotConnectionProvider : IChatbotConnectionProvider
    {
        private readonly IOptions<AbpChatbotSettings> _chatbotSettings;

        public ChatbotConnectionProvider(IOptions<AbpChatbotSettings> chatbotSettings)
        {
            _chatbotSettings = chatbotSettings;
        }
        public string GetApiKey()
        {
            if (_chatbotSettings == null || _chatbotSettings.Value == null)
                throw new ConfigurationErrorsException("An api key is expected for chatbot service");

            return _chatbotSettings.Value.ApiKey;
        }

        public string GetBasePath()
        {
            if (_chatbotSettings == null || _chatbotSettings.Value == null)
                throw new ConfigurationErrorsException("A base path is expected for chatbot service");

            return _chatbotSettings.Value.BasePath;
        }
    }
}
