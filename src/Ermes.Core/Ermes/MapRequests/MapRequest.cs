using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Persons;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.MapRequests
{
    [Table("map_requests")]
    public class MapRequest: AuditedEntity
    {
        public const int MaxCodeLength = 10;
        public const int MaxErrorMessageLength = 2000;

        [Required]
        [StringLength(MaxCodeLength)]
        public string Code { get; set; }

        public NpgsqlRange<DateTime> Duration { get; set; }
        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }

        [Column("Hazard")]
        public string HazardString
        {
            get { return Hazard.ToString(); }
            private set { Hazard = value.ParseEnum<HazardType>(); }
        }
        [NotMapped]
        public HazardType Hazard { get; set; }

        [Column("Layer")]
        public string LayerString
        {
            get { return Layer.ToString(); }
            private set { Layer = value.ParseEnum<LayerType>(); }
        }
        [NotMapped]
        public LayerType Layer { get; set; }
        public int Frequency { get; set; }
        public int DataTypeId { get; set; }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<MapRequestStatusType>(); }
        }
        [NotMapped]
        public MapRequestStatusType Status { get; set; }

        [StringLength(MaxErrorMessageLength)]
        public string ErrorMessage { get; set; }

        [ForeignKey("CreatorUserId")]
        public Person Creator { get; set; }
    }
}
