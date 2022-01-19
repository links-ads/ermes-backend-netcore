﻿using Ermes;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Helpers
{
    public static class NotificationNameHelper
    {
        public static string GetBusTopicName(EntityType type, EntityWriteAction action, string projectName)
        {
            string topicName = "mm." + type.ToString().ToLowerInvariant();
            switch (projectName)
            {
                case "FASTER":
                    break;
                case "SAFERS":
                case "SHELTER":
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
