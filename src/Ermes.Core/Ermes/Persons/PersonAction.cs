using Abp.Domain.Entities.Auditing;
using Ermes.Activities;
using Ermes.Enums;
using Ermes.Helpers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Persons
{
    [Table("person_actions")]
    public abstract class PersonAction : AuditedEntity
    {
        public const int MaxDeviceIdLength = 100;
        public const int MaxDeviceNameLength = 255;

        [Required]
        public DateTime Timestamp { get; set; }
        public Point Location { get; set; }

        [Required]
        [Column("CurrentStatus")]
        public string CurrentStatusString
        {
            get { return CurrentStatus.ToString(); }
            private set { CurrentStatus = value.ParseEnum<ActionStatusType>(); }
        }

        [NotMapped]
        public ActionStatusType CurrentStatus { get; set; }


        [StringLength(MaxDeviceIdLength)]
        public string DeviceId { get; set; }
        [StringLength(MaxDeviceNameLength)]
        public string DeviceName { get; set; }

        [ForeignKey("PersonId")]
        public Person Person { get; set; }
        public long PersonId { get; set; }


        [ForeignKey("CurrentActivityId")]
        public virtual Activity CurrentActivity { get; set; }
        public virtual int? CurrentActivityId { get; set; }

        [Column(TypeName = "jsonb")]
        public string CurrentExtensionData { get; set; }
    }

    public class PersonActionList
    {
        public List<PersonActionSharingPosition> PersonActions { get; set; }
    }
}
