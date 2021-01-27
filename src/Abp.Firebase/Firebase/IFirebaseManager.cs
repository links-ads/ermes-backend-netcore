using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Firebase
{
    public interface IFirebaseManager 
    {
        Task<Dictionary<string, bool>> SendMessageOnMultipleDevicesAsync(List<string> registrationTokens, string title, string body);
        Task<Dictionary<string, bool>> SendMessageAsync(string registrationToken, string title, string body);
    }
}
