using Abp.Domain.Entities;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Notifications
{
    [Table("notifications")]
    public class Notification : Entity<Guid>
    {
        private const int MAX_NAME_LENGTH = 255;
        private const int MAX_TITLE_LENGTH = 100;
        public const int MAX_MESSAGE_LENGTH = 1024;
        public const int MAX_FAILURE_MESSAGE_LENGTH = 200;

        /// <summary>
        /// Name of the notification (i.e. MissionCreate, CommunicationUpdate,  WorkingHoursExceded, etc..)
        /// </summary>
        [StringLength(MAX_NAME_LENGTH)]
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

        [StringLength(MAX_TITLE_LENGTH)]
        public string Title { get; set; }
        [StringLength(MAX_MESSAGE_LENGTH)]
        public string Message { get; set; }

        [ForeignKey("CreatorId")]
        public Person Creator { get; set; }
        public long? CreatorId { get; set; }
        public DateTime Timestamp { get; set; }

        [StringLength(MAX_FAILURE_MESSAGE_LENGTH)]
        public string FailureMessage { get; set; }

    }
}
