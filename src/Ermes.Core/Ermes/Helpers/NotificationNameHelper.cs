using Ermes;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Helpers
{
    public static class NotificationNameHelper
    {
        public static string GetBusTopicName(EntityType type, EntityWriteAction action, string busType, string entityIdentifier = "", string dataTypeId = "")
        {
            string topicName = "mm." + type.ToString().ToLowerInvariant();
            switch (busType)
            {
                case ErmesConsts.BusType.KAFKA:
                    break;
                case ErmesConsts.BusType.RABBITMQ:
                    if (type == EntityType.MapRequest)
                        topicName = string.Format("request.{0}.links.{1}", dataTypeId, entityIdentifier);
                    else
                        topicName += "." + action.ToString().ToLowerInvariant();
                    break;
                default:
                    break;
            }
            return  topicName;
        }

        public static string GetNotificationName(EntityType type, EntityWriteAction action)
        {
            return ErmesConsts.NotificationNames.Notification + "." + type.ToString() + "." + action.ToString();
        }
    }
}
