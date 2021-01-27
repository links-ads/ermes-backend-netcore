using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Chatbot
{
    public interface IChatbotManager
    {
        Task<List<string>> SendMessageAsync(string message, string[] messageParams, List<string> userIds, int entityId, string entityType, string title = null, string[] titleParams=null);
    }
}
