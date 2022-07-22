using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
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

namespace Ermes.Communications
{
    [Table("communications")]
    public class Communication: AuditedEntity
    {
        public const int MaxMessageLength = 1000;
        public Communication()
        {
        }

        [Required]
        [StringLength(MaxMessageLength)]
        public string Message { get; set; }
        public NpgsqlRange<DateTime> Duration { get; set; }
        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        [ForeignKey("CreatorUserId")]
        public Person Creator { get; set; }

        [Required]
        [Column("Scope")]
        public string ScopeString
        {
            get { return Scope.ToString(); }
            private set { Scope = value.ParseEnum<CommunicationScopeType>(); }
        }

        [NotMapped]
        public CommunicationScopeType Scope { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual Person Receiver { get; set; }
        public virtual long? ReceiverId { get; set; }

        [ForeignKey("ReceiverTeamId")]
        public virtual Team ReceiverTeam { get; set; }
        public virtual int? ReceiverTeamId { get; set; }


    }
}
