using Abp.UI.Inputs;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Firebase
{
    public class FirebaseManager : IFirebaseManager
    {
        public FirebaseApp FirebaseApp { get; }

        public FirebaseManager()
        {
            FirebaseApp = GetFirebaseApp();
        }

        private FirebaseApp GetFirebaseApp()
        {
            return FirebaseApp.Create(new AppOptions()
            {
                //By default, credentials are taken from firebasekey.json file in Web project
                //Env variable is set in web.config file, key GOOGLE_APPLICATION_CREDENTIALS
                //and in docker-compose files
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }

        public async Task<Dictionary<string, bool>> SendMessageOnMultipleDevicesAsync(List<string> registrationTokens, string title, string body)
        {
            if (registrationTokens == null || registrationTokens.Count == 0)
                return null;

            var message = new MulticastMessage()
            {
                Tokens = registrationTokens,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
            };

            BatchResponse response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            var res = new Dictionary<string,bool>();
            if (response != null && response.Responses != null)
            {
                if (registrationTokens.Count != response.Responses.Count)
                    return null;
                for (var i = 0; i < response.Responses.Count; i++)
                {
                    // The order of responses corresponds to the order of the registration tokens
                    res.Add(registrationTokens[i], response.Responses[i].IsSuccess);
                }
            }

            return res;
        }

        
        public async Task<Dictionary<string, bool>> SendMessageAsync(string registrationToken, string title, string body)
        {
            var respose = await FirebaseMessaging.DefaultInstance.SendAsync(new Message()
            {
                Token = registrationToken,
                Notification = new Notification()
                {
                    Body = body,
                    Title = title
                }
            });
            //TBD
            Dictionary<string, bool> result = new Dictionary<string, bool>
            {
                { registrationToken, true }
            };
            return result;
        }
    }
}
