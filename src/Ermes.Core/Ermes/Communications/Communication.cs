using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Communications
{
    [Table("communications")]
    public class Communication : AuditedEntity
    {
        public Communication() { }

        [Required]
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

        [Required]
        [Column("Restriction")]
        public string RestrictionString
        {
            get { return Restriction.ToString(); }
            private set { Restriction = value.ParseEnum<CommunicationRestrictionType>(); }
        }

        [NotMapped]
        public CommunicationRestrictionType Restriction { get; set; }

        public virtual ICollection<CommunicationReceiver> CommunicationReceivers { get; set; }
    }
}
