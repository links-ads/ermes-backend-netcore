using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.Alerts
{
    [Table("alerts")]
    public class Alert : Entity, IHasCreationTime
    {
        public string Identifier { get; set; }
        public string Sender { get; set; }
        public DateTime Sent { get; set; }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<CapStatusType>(); }
        }
        [NotMapped]
        public CapStatusType Status { get; set; }

        [Column("MsgType")]
        public string MsgTypeString
        {
            get { return MsgType.ToString(); }
            private set { MsgType = value.ParseEnum<CapMsgType>(); }
        }
        [NotMapped]
        public CapMsgType MsgType { get; set; }

        public string Source { get; set; }
        [Column("Scope")]
        public string ScopeString
        {
            get { return Scope.ToString(); }
            private set { Scope = value.ParseEnum<CapScopeType>(); }
        }
        [NotMapped]
        public CapScopeType Scope { get; set; }

        public string Code { get; set; }
        public string Note { get; set; }
        public string References { get; set; }
        public string Restriction { get; set; }

        // Not a CAP standard property
        public string Region { get; set; }
        // Not a CAP standard property
        public string AreaId { get; set; }


        /// <summary>
        /// Not a CAP standard property, but useful
        /// to manage suggestions coming from DSS module
        /// </summary>
        public bool IsARecommendation { get; set; }


        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        public DateTime CreationTime { get; set; }

        public virtual ICollection<CapInfo> Info { get; set; }
    }
}
