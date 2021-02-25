using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Ermes.Enums;
using Ermes.Organizations;
using Ermes.Persons;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.ReportRequests
{
    [Table("reportrequests")]
    public class ReportRequest : AuditedEntity
    {
        public const int MaxTitleLength = 255;

        public ReportRequest()
        {
        }

        [Required]
        [StringLength(MaxTitleLength)]
        public string Title { get; set; }
        [Required]
        [Column(TypeName = "geography")]
        public Geometry AreaOfInterest { get; set; }
        [Required]
        public NpgsqlRange<DateTime> Duration { get; set; }
        [ForeignKey("CreatorUserId")]
        public Person Creator { get; set; }
        [Required]
        public List<int> SelectedCategories { get; set; } 
    }
}
