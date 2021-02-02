using Abp.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class NotifierBase : INotifierBase
    {
        private readonly IPushNotificationNotifier _pushNotificationNotifier;
        private readonly IWebApiNotifier _webApiNotifier;
        private readonly IErmesBusManager _busManager;

        public NotifierBase(
            IPushNotificationNotifier pushNotificationNotifier,
            IWebApiNotifier webApiNotifier,
            IErmesBusManager busManager
            )
        {
            _pushNotificationNotifier = pushNotificationNotifier;
            _webApiNotifier = webApiNotifier;
            _busManager = busManager;
        }

        public async Task<Dictionary<string, bool>> SendPushNotificationAsync(BaseNotificationData input)
        {
            if (input.Receivers != null && input.Receivers.Count > 1)
                return await _pushNotificationNotifier.SendMultipleUserNotificationAsync(input);
            else if (input.Receivers != null && input.Receivers.Count == 1)
                return await _pushNotificationNotifier.SendUserNotificationAsync(input);
            else
                return new Dictionary<string, bool>();
        }

        public async Task<Dictionary<string, bool>> SendTestPushNotificationAsync(string regToken)
        {
            return await _pushNotificationNotifier.SendTestNotificationAsync(regToken);
        }

        public async Task<List<string>> SendWebApiNotificationAsync(FullNotificationData input)
        {
            return await _webApiNotifier.SendMessage(input);
        }

        public async Task SendBusNotificationAsync(string topic, string message)
        {
            if (!await _busManager.Publish(topic, message))
            {
                throw new Exception("Delivery Status: Not Persisted");
            }
        }
    }
}
