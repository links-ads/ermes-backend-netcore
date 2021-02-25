using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Interfaces;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Teams;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Missions
{
    [Table("missions")]
    public class Mission : AuditedEntity
    {
        public const int MaxTitleLength = 255;
        public const int MaxDescriptionLength = 1000;
        public const int MaxNotesLength = 1000;

        public Mission()
        {
        }
        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }
        [StringLength(MaxDescriptionLength)]
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
        public Person CoordinatorPerson { get; set; }
        public long? CoordinatorPersonId { get; set; }

        [ForeignKey("CoordinatorTeamId")]
        public Team CoordinatorTeam { get; set; }
        public int? CoordinatorTeamId { get; set; }

        [StringLength(MaxNotesLength)]
        public string Notes { get; set; }

        [ForeignKey("CreatorUserId")]
        public Person CreatorPerson { get; set; }

        [ForeignKey("OrganizationId")]
        public Organization Organization {get; set;}
        public int OrganizationId { get; set; }

    }
}
