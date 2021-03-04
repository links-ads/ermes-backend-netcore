using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Interfaces;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Reports
{
    [Table("reports")]
    public class Report : AuditedEntity
    {
        public const int MaxDescriptionLength = 1000;
        public const int MaxNotesLength = 1000;

        public Report()
        {
        }

        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }
        [NotMapped]
        public HazardType Hazard { get; set; }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<GeneralStatus>(); }
        }
        [NotMapped]
        public GeneralStatus Status { get; set; }
        [Required]
        public Point Location { get; set; }
        public DateTime Timestamp { get; set; }
        public string Address { get; set; }
        public List<string> MediaURIs { get; set; }
        [Column(TypeName = "jsonb")]
        public List<ReportExtensionData> ExtensionData { get; set; }
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }
        [StringLength(MaxNotesLength)]
        public string Notes { get; set; }
        [Column(TypeName = "jsonb")]
        public List<ReportTarget> Targets { get; set; }
        [ForeignKey("CreatorUserId")]
        public Person Creator { get; set; }

        [Column("Source")]
        public string SourceString
        {
            get { return Source.ToString(); }
            private set { Source = value.ParseEnum<SourceDeviceType>(); }
        }
        [ForeignKey("RelativeMissionId")]
        public Mission RelativeMission { get; set; }
        public int? RelativeMissionId { get; set; }

        [Column(TypeName = "jsonb")]
        public List<ReportTag> Tags { get; set; }

        [Column(TypeName = "jsonb")]
        public List<ReportAdultInfo> AdultInfo { get; set; }

        [NotMapped]
        public SourceDeviceType Source { get; set; }
        [NotMapped]
        public bool IsEditable { get; set; } 
    }

    public class ReportExtensionData
    {
        public int CategoryId { get; set; }
        public string Value { get; set; }
        public GeneralStatus Status { get; set; }
    }

    public class ReportTarget
    {
        public TargetType Target { get; set; }
        public GeneralStatus Status { get; set; }
    }

    public class ReportTag
    {
        public string MediaURI { get; set; }
        public double Confidence { get; set; }
        public string Name { get; set; }
    }

    public class ReportAdultInfo
    {
        public string MediaURI { get; set; }
        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public bool IsGoryContent { get; set; }
        public double AdultScore { get; set; }
        public double RacyScore { get; set; }
        public double GoreScore { get; set; }
    }
}
