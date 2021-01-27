using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Chatbot.Configuration
{
    public interface IChatbotConnectionProvider
    {
        string GetBasePath();
        string GetApiKey();
    }
}
