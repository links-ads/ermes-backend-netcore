using Ermes.Enums;
using Ermes.Notifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifiers
{
    public class FullNotificationData : BaseNotificationData
    {
        public int EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public string[] BodyParams { get; set; }
        public string[] TitleParams { get; set; }
    }
}
