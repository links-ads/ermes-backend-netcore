using Abp.Firebase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class PushNotificationNotifier : IPushNotificationNotifier
    {
        private readonly FirebaseManager _firebaseManager;
        public PushNotificationNotifier(FirebaseManager firebaseManager)
        {
            _firebaseManager = firebaseManager;
        }

        public async Task<Dictionary<string, bool>> SendMultipleUserNotificationAsync(BaseNotificationData input)
        {
            return await _firebaseManager.SendMessageOnMultipleDevicesAsync(input.Receivers, input.Title, input.Body);
        }

        public async Task<Dictionary<string, bool>> SendUserNotificationAsync(BaseNotificationData input)
        {
            return await _firebaseManager.SendMessageAsync(input.Receivers.First(), input.Title, input.Body);
        }

        public async Task<Dictionary<string, bool>> SendTestNotificationAsync(string regToken)
        {
           return await _firebaseManager.SendMessageAsync(regToken, "Test Title", "Test Body" );
        }

    }
}
