using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public interface INotifierBase : ITransientDependency
    {
        Task<Dictionary<string, bool>> SendPushNotificationAsync(BaseNotificationData data);
        Task<Dictionary<string, bool>> SendTestPushNotificationAsync(string regToken);
        Task<List<string>> SendWebApiNotificationAsync(FullNotificationData data);
        Task SendBusNotificationAsync(string topic, string message);
    }
}
