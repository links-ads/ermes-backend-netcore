using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Notifications
{
    [Table("notifications_received")]
    public class NotificationReceived : Entity<Guid>, IHasCreationTime
    {
        private const int MAX_ROUTING_KEY_LENGTH = 255;
        public NotificationReceived()
        {
            
        }
        public NotificationReceived(string routingKey, string message)
        {
            RoutingKey = routingKey;
            Message = message;
        }
        [MaxLength(MAX_ROUTING_KEY_LENGTH)]
        public string RoutingKey { get; set; }
        public string Message { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
