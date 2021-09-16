using Ermes;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Helpers
{
    public static class NotificationNameHelper
    {
        //TODO: must be updated according to the reference ERMES project
        public static string GetBusTopicName(EntityType type, EntityWriteAction action)
        {
            return "mm." + type.ToString().ToLowerInvariant();
        }

        public static string GetNotificationName(EntityType type, EntityWriteAction action)
        {
            return ErmesConsts.NotificationNames.Notification + "." + type.ToString() + "." + action.ToString();
        }
    }
}
