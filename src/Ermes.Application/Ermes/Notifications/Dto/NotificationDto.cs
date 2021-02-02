using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifications.Dto
{
    public class NotificationDto
    {
        public string Name { get; set; }
        public NotificationStatus Status { get; set; }
        public NotificationChannelType Channel { get; set; }
        //Id of the entity the notifications refers to
        public int EntityId { get; set; }
        public EntityType Entity { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
