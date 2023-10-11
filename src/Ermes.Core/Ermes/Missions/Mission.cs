using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Teams;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Missions
{
    [Table("missions")]
    public class Mission : AuditedEntity
    {
        public const int MAX_TITLE_LENGTH = 255;
        public const int MAX_DESCRIPTION_LENGTH = 1000;
        public const int MAX_NOTES_LENGTH = 1000;

        public Mission()
        {
        }
        [Required]
        [StringLength(MAX_TITLE_LENGTH)]
        public string Title { get; set; }
        [StringLength(MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        public NpgsqlRange<DateTime> Duration { get; set; }

        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        [Required]
        [Column("CurrentStatus")]
        public string CurrentStatusString
        {
            get { return CurrentStatus.ToString(); }
            private set { CurrentStatus = value.ParseEnum<MissionStatusType>(); }
        }

        [NotMapped]
        public MissionStatusType CurrentStatus { get; set; }

        [ForeignKey("CoordinatorPersonId")]
        public virtual Person CoordinatorPerson { get; set; }
        public virtual long? CoordinatorPersonId { get; set; }

        [ForeignKey("CoordinatorTeamId")]
        public virtual Team CoordinatorTeam { get; set; }
        public virtual int? CoordinatorTeamId { get; set; }

        [StringLength(MAX_NOTES_LENGTH)]
        public string Notes { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual Person CreatorPerson { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public virtual int OrganizationId { get; set; }

    }
}
