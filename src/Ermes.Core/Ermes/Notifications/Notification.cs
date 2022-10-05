using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Notifications
{
    [Table("notifications")]
    public class Notification: Entity<Guid>
    {
        private const int MaxNameLength = 255;
        private const int MaxTitleLength = 100;
        public const int MaxMessageLength = 1024;
        public const int MaxFailureMessageLenght = 200;

        /// <summary>
        /// Name of the notification (i.e. MissionCreate, CommunicationUpdate,  WorkingHoursExceded, etc..)
        /// </summary>
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        [ForeignKey("ReceiverId")]
        public Person Receiver { get; set; }
        public long? ReceiverId { get; set; }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<NotificationStatus>(); }
        }
        [NotMapped]
        public NotificationStatus Status { get; set; }

        [Column("Channel")]
        public string ChannelString
        {
            get { return Channel.ToString(); }
            private set { Channel = value.ParseEnum<NotificationChannelType>(); }
        }
        [NotMapped]
        public NotificationChannelType Channel { get; set; }

        //Id of the entity the notifications refers to
        public int EntityId { get; set; }
        [Column("Entity")]
        public string EntityString
        {
            get { return Entity.ToString(); }
            private set { Entity = value.ParseEnum<EntityType>(); }
        }
        [NotMapped]
        public EntityType Entity { get; set; }

        [StringLength(MaxTitleLength)]
        public string Title { get; set; }
        [StringLength(MaxMessageLength)]
        public string Message { get; set; }

        [ForeignKey("CreatorId")]
        public Person Creator { get; set; }
        public long? CreatorId { get; set; }
        public DateTime Timestamp { get; set; }

        [StringLength(MaxFailureMessageLenght)]
        public string FailureMessage { get; set; }

    }
}
