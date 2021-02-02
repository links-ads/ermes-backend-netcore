using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public interface IPushNotificationNotifier: ITransientDependency
    {
        //Task<int> SendSingleUserNotificationAsync(PushNotificationData input);
        Task<Dictionary<string, bool>> SendMultipleUserNotificationAsync(BaseNotificationData input);
        Task<Dictionary<string, bool>> SendUserNotificationAsync(BaseNotificationData input);
        Task<Dictionary<string, bool>> SendTestNotificationAsync(string regToken);
    }
}
