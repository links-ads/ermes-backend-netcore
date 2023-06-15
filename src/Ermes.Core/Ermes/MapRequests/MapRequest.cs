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

namespace Ermes.MapRequests
{
    [Table("map_requests")]
    public class MapRequest: AuditedEntity
    {
        public const int MAX_CODE_LENGTH = 10;
        public const int MAX_TITLE_LENGTH = 255;
        public const int MAX_ERROR_MESSAGE_LENGTH = 2000;
        public const int MAX_DESCRIPTION_LENGTH = 1000;

        [Required]
        [StringLength(MAX_CODE_LENGTH)]
        public string Code { get; set; }

        [StringLength(MAX_TITLE_LENGTH)]
        public string Title { get; set; }

        public NpgsqlRange<DateTime> Duration { get; set; }
        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }


        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<MapRequestStatusType>(); }
        }
        [NotMapped]
        public MapRequestStatusType Status { get; set; }

        [StringLength(MAX_ERROR_MESSAGE_LENGTH)]
        public string ErrorMessage { get; set; }

        [ForeignKey("CreatorUserId")]
        public virtual Person Creator { get; set; }

        public virtual ICollection<MapRequestLayer> MapRequestLayers { get; set; }


        public int Resolution { get; set; } = 10;
        public int Frequency { get; set; }

        [NotMapped]
        public int ExpectedUpdates
        {
            get
            {
                return Frequency > 0 ? (int)(Duration.UpperBound - Duration.LowerBound).TotalDays / Frequency : 1;
            }
        }

        [Column("Type")]
        public string TypeString
        {
            get { return Type.ToString(); }
            private set { Type = value.ParseEnum<MapRequestType>(); }
        }
        [NotMapped]
        public MapRequestType Type { get; set; }

        [StringLength(MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        /// <summary>
        /// Whether to activate spotting or not
        /// </summary>
        public bool DoSpotting { get; set; }

        /// <summary>
        /// Express for each pixel the probability of the fire to reach
        /// that specific point in the given time step.
        /// </summary>
        public decimal ProbabilityRange { get; set; }

        /// <summary>
        /// Simulation time limit
        /// </summary>
        public int TimeLimit { get; set; }

        /// <summary>
        /// List of boundary conditions
        /// </summary>
        [Column(TypeName = "jsonb")]
        public List<BoundaryCondition> BoundaryConditions { get; set; }
    }
}
