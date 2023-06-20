using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Alerts
{
    [Table("alerts_cap_info")]
    public class CapInfo : Entity, IHasCreationTime
    {
        [Required]
        [ForeignKey("AlertId")]
        public virtual Alert Alert { get; set; }
        public int AlertId { get; set; }
        public string Language { get; set; }

        [Column("Category")]
        public string CategoryString
        {
            get { return Category.ToString(); }
            private set { Category = value.ParseEnum<CapCategoryType>(); }
        }
        [NotMapped]
        public CapCategoryType Category { get; set; }
        public string Event { get; set; }

        [Column("ResponseType")]
        public string ResponseTypeString
        {
            get { return ResponseType.ToString(); }
            private set { ResponseType = value.ParseEnum<CapResponseType>(); }
        }
        [NotMapped]
        public CapResponseType ResponseType { get; set; }

        [Column("Urgency")]
        public string UrgencyString
        {
            get { return Urgency.ToString(); }
            private set { Urgency = value.ParseEnum<CapUrgencyType>(); }
        }
        [NotMapped]
        public CapUrgencyType Urgency { get; set; }

        [Column("Severity")]
        public string SeverityString
        {
            get { return Severity.ToString(); }
            private set { Severity = value.ParseEnum<CapSeverityType>(); }
        }
        [NotMapped]
        public CapSeverityType Severity { get; set; }

        [Column("Certainty")]
        public string CertaintyString
        {
            get { return Certainty.ToString(); }
            private set { Certainty = value.ParseEnum<CapCertaintyType>(); }
        }
        [NotMapped]
        public CapCertaintyType Certainty { get; set; }
        public DateTime Expires { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        [Column(TypeName = "jsonb")]
        public List<CapArea> Area { get; set; }
    }
}
